using Bones.Backend.Features.AccountManagement.RegisterUser;
using Bones.Database.DbSets.AccountManagement;
using Bones.Database.DbSets.SystemQueues;
using Bones.Shared.Backend.Models;
using Bones.Shared.Consts;
using Bones.Testing.Shared.Backend;
using Bones.Testing.Shared.Backend.TestOperations.AccountManagement;
using FluentValidation;
using FluentValidation.TestHelper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Bones.Backend.UnitTests.Features.AccountManagement;

public class RegisterUserTests : TestBase
{
    private readonly RegisterUserQueryValidator _validator = new();
    
    /// <summary>
    ///     Checks that the validator and handler both validate the email address.
    /// </summary>
    /// <param name="email">The email address to test.</param>
    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    [InlineData(" @ ")]
    [InlineData("InvalidEmail@example,com")]
    [InlineData("InvalidEmail@example.")]
    [InlineData("InvalidEmail@example")]
    [InlineData("InvalidEmail@")]
    [InlineData("InvalidEmail")]
    public async Task InvalidEmail_ShouldFail(string email)
    {
        RegisterUserQuery request = new(email, "abcdEFGH1!");
        
        TestValidationResult<RegisterUserQuery> validationResult = await _validator.TestValidateAsync(request);
        validationResult.ShouldHaveValidationErrorFor(x => x.Email);
        
        QueryResponse<IdentityResult> result = await Sender.Send(request);
        result.Success.Should().BeFalse();
        result.Result?.Succeeded.Should().BeFalse();
    }
    
    /// <summary>
    ///     Checks that the validator and handler both validate the password.
    /// </summary>
    /// <param name="password">The password to test.</param>
    [Theory]
    [InlineData("")]
    [InlineData("aA1!")] // Not long enough
    [InlineData("abcdefgh1!")] // No caps
    [InlineData("ABCDEFGH1!")] // No lowercase
    [InlineData("abcdEFGH!")] // No number
    [InlineData("abcdEFGH1")] // No special char
    public async Task InvalidPassword_ShouldFail(string password)
    {
        RegisterUserQuery request = new("InvalidPassword@example.com", password);
        TestValidationResult<RegisterUserQuery> validationResult = await _validator.TestValidateAsync(request);
        validationResult.ShouldHaveValidationErrorFor(x => x.Password);
        
        QueryResponse<IdentityResult> result = await Sender.Send(request);
        result.Success.Should().BeFalse();
        result.Result?.Succeeded.Should().BeFalse();
    }
    
    /// <summary>
    ///     Checks that the validator and handler both pass this.
    /// </summary>
    [Fact]
    public async Task ValidEmailAndPassword_ShouldSucceed()
    {
        RegisterUserQuery request = new("ValidEmailAndPassword@example.com", "abcdEFGH1!");
        
        TestValidationResult<RegisterUserQuery> validationResult = await _validator.TestValidateAsync(request);
        validationResult.ShouldNotHaveAnyValidationErrors();
        
        QueryResponse<IdentityResult> result = await Sender.Send(request);
        result.Success.Should().BeTrue();
        result.Result?.Succeeded.Should().BeTrue();
    }
    
    /// <summary>
    ///     Checks that the handler actually queues a confirmation email for the new account
    /// </summary>
    [Fact]
    public async Task RegisteringUser_ShouldQueueConfirmationEmail()
    {
        RegisterUserQuery request = new("RegisteringUserQueueConfirmationEmail@example.com", "abcdEFGH1!");
        TestValidationResult<RegisterUserQuery> validationResult = await _validator.TestValidateAsync(request);
        validationResult.ShouldNotHaveAnyValidationErrors();
        
        QueryResponse<IdentityResult> result = await Sender.Send(request);
        result.Success.Should().BeTrue();
        result.Result?.Succeeded.Should().BeTrue();

        List<BonesUser>? allUsers = await Sender.Send(new GetAllUsersQuery());
        BonesUser? createdUser = allUsers?.Find(u => u.Email == request.Email);
        createdUser.Should().NotBeNull();
        
        ConfirmationEmailQueue? confirmation = await Sender.Send(new GetEmailConfirmationByUserEmailQuery(request.Email));
        confirmation.Should().NotBeNull();
        confirmation?.ConfirmationLink.Should().NotBeNullOrEmpty();
    }
    
    /// <summary>
    ///     Checks that the validator passes this and the handler fails it.
    /// </summary>
    [Fact]
    public async Task DuplicateEmail_ShouldFail()
    {
        RegisterUserQuery request = new("DuplicateEmail@example.com", "abcdEFGH1!");
        
        // Do it
        TestValidationResult<RegisterUserQuery> validationResult = await _validator.TestValidateAsync(request);
        validationResult.ShouldNotHaveAnyValidationErrors();
        
        QueryResponse<IdentityResult> result = await Sender.Send(request);
        result.Success.Should().BeTrue();
        result.Result?.Succeeded.Should().BeTrue();
        
        // Do it again
        TestValidationResult<RegisterUserQuery> validationResult2 = await _validator.TestValidateAsync(request);
        validationResult2.ShouldNotHaveAnyValidationErrors();
        
        QueryResponse<IdentityResult> result2 = await Sender.Send(request);
        result2.Success.Should().BeFalse();
        result2.Result?.Succeeded.Should().BeFalse();
    }
}