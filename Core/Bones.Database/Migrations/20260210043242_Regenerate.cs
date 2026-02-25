using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class Regenerate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Audits");

            migrationBuilder.EnsureSchema(
                name: "Items.Types");

            migrationBuilder.EnsureSchema(
                name: "Organizations");

            migrationBuilder.EnsureSchema(
                name: "Accounts");

            migrationBuilder.EnsureSchema(
                name: "System.Queues");

            migrationBuilder.EnsureSchema(
                name: "Projects");

            migrationBuilder.EnsureSchema(
                name: "Items.Assignments");

            migrationBuilder.EnsureSchema(
                name: "Items.Fields");

            migrationBuilder.EnsureSchema(
                name: "Items.Layouts");

            migrationBuilder.EnsureSchema(
                name: "Items");

            migrationBuilder.EnsureSchema(
                name: "System");

            migrationBuilder.CreateTable(
                name: "BonesOrganizations",
                schema: "Organizations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonesOrganizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BonesUser",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DisplayName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EmailConfirmedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    PasswordLastSetDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    PasswordExpired = table.Column<bool>(type: "boolean", nullable: false),
                    UserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: true),
                    SecurityStamp = table.Column<string>(type: "text", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true),
                    PhoneNumber = table.Column<string>(type: "text", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonesUser", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmationEmailDeadQueue",
                schema: "System.Queues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeadQueueCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OriginalCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    LastTry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FailureReasons = table.Column<List<string>>(type: "text[]", nullable: false),
                    EmailTo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ConfirmationLink = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationEmailDeadQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmationEmailQueue",
                schema: "System.Queues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    LastTry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReasons = table.Column<List<string>>(type: "text[]", nullable: false),
                    EmailTo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ConfirmationLink = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationEmailQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForgotPasswordEmailDeadQueue",
                schema: "System.Queues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeadQueueCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OriginalCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    LastTry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReasons = table.Column<List<string>>(type: "text[]", nullable: false),
                    EmailTo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    PasswordResetLink = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForgotPasswordEmailDeadQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForgotPasswordEmailQueue",
                schema: "System.Queues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    LastTry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReasons = table.Column<List<string>>(type: "text[]", nullable: false),
                    EmailTo = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    PasswordResetLink = table.Column<string>(type: "character varying(4096)", maxLength: 4096, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForgotPasswordEmailQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionAttemptAudits",
                schema: "Audits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", nullable: false),
                    SessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Successful = table.Column<bool>(type: "boolean", nullable: false),
                    AttemptDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessionAttemptAudits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                schema: "System",
                columns: table => new
                {
                    Setting = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Setting);
                });

            migrationBuilder.CreateTable(
                name: "TaskErrors",
                schema: "System",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ErrorTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ErrorMessage = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    StackTrace = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskErrors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BonesRoles",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSystemRole = table.Column<bool>(type: "boolean", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReadOnlyRole = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonesRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonesRoles_BonesOrganizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Organizations",
                        principalTable: "BonesOrganizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountAudits",
                schema: "Audits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountBonesUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ActionTaken = table.Column<int>(type: "integer", nullable: false),
                    ActionTakenByBonesUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountAudits_BonesUser_AccountBonesUserId",
                        column: x => x.AccountBonesUserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountAudits_BonesUser_ActionTakenByBonesUserId",
                        column: x => x.ActionTakenByBonesUserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesUserClaims",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonesUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonesUserClaims_BonesUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesUserLogins",
                schema: "Accounts",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    ProviderKey = table.Column<string>(type: "text", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "text", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonesUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_BonesUserLogins_BonesUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesUserSessions",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    IpAddress = table.Column<string>(type: "character varying(45)", nullable: false),
                    Base64LocalStorageKey = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastAccessedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsInvalidated = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonesUserSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonesUserSessions_BonesUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesUserTokens",
                schema: "Accounts",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    LoginProvider = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonesUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_BonesUserTokens_BonesUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoginAudits",
                schema: "Audits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BonesUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UnknownEmail = table.Column<string>(type: "text", nullable: true),
                    LoginDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Successful = table.Column<bool>(type: "boolean", nullable: false),
                    RequestingIpAddress = table.Column<string>(type: "character varying(45)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginAudits_BonesUser_BonesUserId",
                        column: x => x.BonesUserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    OwnerType = table.Column<int>(type: "integer", nullable: false),
                    OwningUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwningOrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    BonesOrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    BonesUserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_BonesOrganizations_BonesOrganizationId",
                        column: x => x.BonesOrganizationId,
                        principalSchema: "Organizations",
                        principalTable: "BonesOrganizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_BonesOrganizations_OwningOrganizationId",
                        column: x => x.OwningOrganizationId,
                        principalSchema: "Organizations",
                        principalTable: "BonesOrganizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_BonesUser_BonesUserId",
                        column: x => x.BonesUserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_BonesUser_OwningUserId",
                        column: x => x.OwningUserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SystemAudits",
                schema: "Audits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ActionTaken = table.Column<int>(type: "integer", nullable: false),
                    SettingChanged = table.Column<int>(type: "integer", nullable: true),
                    ActionTakenByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemAudits_BonesUser_ActionTakenByUserId",
                        column: x => x.ActionTakenByUserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesRoleClaims",
                schema: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClaimType = table.Column<string>(type: "text", nullable: true),
                    ClaimValue = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonesRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BonesRoleClaims_BonesRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Accounts",
                        principalTable: "BonesRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesUserRoles",
                schema: "Accounts",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BonesUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_BonesUserRoles_BonesRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Accounts",
                        principalTable: "BonesRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BonesUserRoles_BonesUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Initiatives",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Initiatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Initiatives_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemFields",
                schema: "Items.Fields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemFields_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemLayouts",
                schema: "Items.Layouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendlyIdPrefix = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    FriendlyIdNonce = table.Column<long>(type: "bigint", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemLayouts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemLayouts_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskQueues",
                schema: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InitiativeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskQueues_Initiatives_InitiativeId",
                        column: x => x.InitiativeId,
                        principalSchema: "Projects",
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemFieldVersions",
                schema: "Items.Fields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ItemFieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    CanBeNegative = table.Column<bool>(type: "boolean", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemFieldVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemFieldVersions_ItemFields_ItemFieldId",
                        column: x => x.ItemFieldId,
                        principalSchema: "Items.Fields",
                        principalTable: "ItemFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemLayoutVersions",
                schema: "Items.Layouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    LayoutUse = table.Column<int>(type: "integer", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ItemLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemLayoutVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemLayoutVersions_ItemLayouts_ItemLayoutId",
                        column: x => x.ItemLayoutId,
                        principalSchema: "Items.Layouts",
                        principalTable: "ItemLayouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                schema: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendlyId = table.Column<string>(type: "text", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentVersion = table.Column<long>(type: "bigint", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_ItemLayouts_ItemLayoutId",
                        column: x => x.ItemLayoutId,
                        principalSchema: "Items.Layouts",
                        principalTable: "ItemLayouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Items_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemFieldListEntries",
                schema: "Items.Fields",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    ItemFieldVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingType = table.Column<int>(type: "integer", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemFieldListEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemFieldListEntries_ItemFieldVersions_ItemFieldVersionId",
                        column: x => x.ItemFieldVersionId,
                        principalSchema: "Items.Fields",
                        principalTable: "ItemFieldVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemAssignmentSlots",
                schema: "Items.Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentType = table.Column<int>(type: "integer", nullable: false),
                    SelectionType = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    AssignmentStates = table.Column<List<string>>(type: "text[]", nullable: false),
                    ItemLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAssignmentSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemAssignmentSlots_ItemLayoutVersions_ItemLayoutVersionId",
                        column: x => x.ItemLayoutVersionId,
                        principalSchema: "Items.Layouts",
                        principalTable: "ItemLayoutVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemLayoutFieldVersionLinks",
                schema: "Items.Layouts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<int>(type: "integer", nullable: false),
                    ItemLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemFieldVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemLayoutFieldVersionLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemLayoutFieldVersionLinks_ItemFieldVersions_ItemFieldVers~",
                        column: x => x.ItemFieldVersionId,
                        principalSchema: "Items.Fields",
                        principalTable: "ItemFieldVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemLayoutFieldVersionLinks_ItemLayoutVersions_ItemLayoutVe~",
                        column: x => x.ItemLayoutVersionId,
                        principalSchema: "Items.Layouts",
                        principalTable: "ItemLayoutVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                schema: "Items.Types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Items",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assets_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "Projects",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemVersions",
                schema: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ItemLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemVersions_ItemLayoutVersions_ItemLayoutVersionId",
                        column: x => x.ItemLayoutVersionId,
                        principalSchema: "Items.Layouts",
                        principalTable: "ItemLayoutVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemVersions_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Items",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                schema: "Items.Types",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskQueueId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedToQueueDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Items",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Tasks_TaskQueues_TaskQueueId",
                        column: x => x.TaskQueueId,
                        principalSchema: "Projects",
                        principalTable: "TaskQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemAssignees",
                schema: "Items.Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemAssignmentSlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedRoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    State = table.Column<string>(type: "text", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAssignees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemAssignees_BonesRoles_AssignedRoleId",
                        column: x => x.AssignedRoleId,
                        principalSchema: "Accounts",
                        principalTable: "BonesRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemAssignees_BonesUser_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalSchema: "Accounts",
                        principalTable: "BonesUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemAssignees_ItemAssignmentSlots_ItemAssignmentSlotId",
                        column: x => x.ItemAssignmentSlotId,
                        principalSchema: "Items.Assignments",
                        principalTable: "ItemAssignmentSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemAssignees_ItemVersions_ItemVersionId",
                        column: x => x.ItemVersionId,
                        principalSchema: "Items",
                        principalTable: "ItemVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemValues",
                schema: "Items",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ItemVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemFieldVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemValues_ItemFieldVersions_ItemFieldVersionId",
                        column: x => x.ItemFieldVersionId,
                        principalSchema: "Items.Fields",
                        principalTable: "ItemFieldVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemValues_ItemVersions_ItemVersionId",
                        column: x => x.ItemVersionId,
                        principalSchema: "Items",
                        principalTable: "ItemVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountAudits_AccountBonesUserId",
                schema: "Audits",
                table: "AccountAudits",
                column: "AccountBonesUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAudits_ActionTakenByBonesUserId",
                schema: "Audits",
                table: "AccountAudits",
                column: "ActionTakenByBonesUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ItemId",
                schema: "Items.Types",
                table: "Assets",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ProjectId",
                schema: "Items.Types",
                table: "Assets",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BonesRoleClaims_RoleId",
                schema: "Accounts",
                table: "BonesRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BonesRoles_OrganizationId",
                schema: "Accounts",
                table: "BonesRoles",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Accounts",
                table: "BonesRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Accounts",
                table: "BonesUser",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Accounts",
                table: "BonesUser",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonesUserClaims_UserId",
                schema: "Accounts",
                table: "BonesUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BonesUserLogins_UserId",
                schema: "Accounts",
                table: "BonesUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BonesUserRoles_RoleId",
                schema: "Accounts",
                table: "BonesUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BonesUserSessions_UserId",
                schema: "Accounts",
                table: "BonesUserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Initiatives_ProjectId",
                schema: "Projects",
                table: "Initiatives",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAssignees_AssignedRoleId",
                schema: "Items.Assignments",
                table: "ItemAssignees",
                column: "AssignedRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAssignees_AssignedUserId",
                schema: "Items.Assignments",
                table: "ItemAssignees",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAssignees_ItemAssignmentSlotId",
                schema: "Items.Assignments",
                table: "ItemAssignees",
                column: "ItemAssignmentSlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAssignees_ItemVersionId",
                schema: "Items.Assignments",
                table: "ItemAssignees",
                column: "ItemVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAssignmentSlots_ItemLayoutVersionId",
                schema: "Items.Assignments",
                table: "ItemAssignmentSlots",
                column: "ItemLayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFieldListEntries_ItemFieldVersionId",
                schema: "Items.Fields",
                table: "ItemFieldListEntries",
                column: "ItemFieldVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFields_ProjectId",
                schema: "Items.Fields",
                table: "ItemFields",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFieldVersions_ItemFieldId",
                schema: "Items.Fields",
                table: "ItemFieldVersions",
                column: "ItemFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayoutFieldVersionLinks_ItemFieldVersionId",
                schema: "Items.Layouts",
                table: "ItemLayoutFieldVersionLinks",
                column: "ItemFieldVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayoutFieldVersionLinks_ItemLayoutVersionId",
                schema: "Items.Layouts",
                table: "ItemLayoutFieldVersionLinks",
                column: "ItemLayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayouts_FriendlyIdPrefix",
                schema: "Items.Layouts",
                table: "ItemLayouts",
                column: "FriendlyIdPrefix");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayouts_ProjectId",
                schema: "Items.Layouts",
                table: "ItemLayouts",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayoutVersions_ItemLayoutId",
                schema: "Items.Layouts",
                table: "ItemLayoutVersions",
                column: "ItemLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_FriendlyId",
                schema: "Items",
                table: "Items",
                column: "FriendlyId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemLayoutId",
                schema: "Items",
                table: "Items",
                column: "ItemLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ProjectId",
                schema: "Items",
                table: "Items",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemValues_ItemFieldVersionId",
                schema: "Items",
                table: "ItemValues",
                column: "ItemFieldVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemValues_ItemVersionId",
                schema: "Items",
                table: "ItemValues",
                column: "ItemVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVersions_ItemId",
                schema: "Items",
                table: "ItemVersions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVersions_ItemLayoutVersionId",
                schema: "Items",
                table: "ItemVersions",
                column: "ItemLayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVersions_Version",
                schema: "Items",
                table: "ItemVersions",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAudits_BonesUserId",
                schema: "Audits",
                table: "LoginAudits",
                column: "BonesUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BonesOrganizationId",
                schema: "Projects",
                table: "Projects",
                column: "BonesOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BonesUserId",
                schema: "Projects",
                table: "Projects",
                column: "BonesUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwningOrganizationId",
                schema: "Projects",
                table: "Projects",
                column: "OwningOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwningUserId",
                schema: "Projects",
                table: "Projects",
                column: "OwningUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAudits_ActionTakenByUserId",
                schema: "Audits",
                table: "SystemAudits",
                column: "ActionTakenByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Setting",
                schema: "System",
                table: "SystemSettings",
                column: "Setting",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskQueues_InitiativeId",
                schema: "Projects",
                table: "TaskQueues",
                column: "InitiativeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ItemId",
                schema: "Items.Types",
                table: "Tasks",
                column: "ItemId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskQueueId",
                schema: "Items.Types",
                table: "Tasks",
                column: "TaskQueueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountAudits",
                schema: "Audits");

            migrationBuilder.DropTable(
                name: "Assets",
                schema: "Items.Types");

            migrationBuilder.DropTable(
                name: "BonesRoleClaims",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "BonesUserClaims",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "BonesUserLogins",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "BonesUserRoles",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "BonesUserSessions",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "BonesUserTokens",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "ConfirmationEmailDeadQueue",
                schema: "System.Queues");

            migrationBuilder.DropTable(
                name: "ConfirmationEmailQueue",
                schema: "System.Queues");

            migrationBuilder.DropTable(
                name: "ForgotPasswordEmailDeadQueue",
                schema: "System.Queues");

            migrationBuilder.DropTable(
                name: "ForgotPasswordEmailQueue",
                schema: "System.Queues");

            migrationBuilder.DropTable(
                name: "ItemAssignees",
                schema: "Items.Assignments");

            migrationBuilder.DropTable(
                name: "ItemFieldListEntries",
                schema: "Items.Fields");

            migrationBuilder.DropTable(
                name: "ItemLayoutFieldVersionLinks",
                schema: "Items.Layouts");

            migrationBuilder.DropTable(
                name: "ItemValues",
                schema: "Items");

            migrationBuilder.DropTable(
                name: "LoginAudits",
                schema: "Audits");

            migrationBuilder.DropTable(
                name: "SessionAttemptAudits",
                schema: "Audits");

            migrationBuilder.DropTable(
                name: "SystemAudits",
                schema: "Audits");

            migrationBuilder.DropTable(
                name: "SystemSettings",
                schema: "System");

            migrationBuilder.DropTable(
                name: "TaskErrors",
                schema: "System");

            migrationBuilder.DropTable(
                name: "Tasks",
                schema: "Items.Types");

            migrationBuilder.DropTable(
                name: "BonesRoles",
                schema: "Accounts");

            migrationBuilder.DropTable(
                name: "ItemAssignmentSlots",
                schema: "Items.Assignments");

            migrationBuilder.DropTable(
                name: "ItemFieldVersions",
                schema: "Items.Fields");

            migrationBuilder.DropTable(
                name: "ItemVersions",
                schema: "Items");

            migrationBuilder.DropTable(
                name: "TaskQueues",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "ItemFields",
                schema: "Items.Fields");

            migrationBuilder.DropTable(
                name: "ItemLayoutVersions",
                schema: "Items.Layouts");

            migrationBuilder.DropTable(
                name: "Items",
                schema: "Items");

            migrationBuilder.DropTable(
                name: "Initiatives",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "ItemLayouts",
                schema: "Items.Layouts");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "Projects");

            migrationBuilder.DropTable(
                name: "BonesOrganizations",
                schema: "Organizations");

            migrationBuilder.DropTable(
                name: "BonesUser",
                schema: "Accounts");
        }
    }
}
