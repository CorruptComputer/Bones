namespace Bones.Shared.Consts;

/// <summary>
///   URLs for the pages in the front-end, makes it easy to link to pages.
/// </summary>
public static class FrontEndUrls
{
    /// <summary>
    ///   The home page
    /// </summary>
    public const string HOME = "/";

    /// <summary>
    ///   Account pages
    /// </summary>
    public static class Account
    {
        private const string _account = "/Account";

        /// <summary>
        ///   Confirm email page
        /// </summary>
        public const string CONFIRM_EMAIL = $"{_account}/ConfirmEmail";

        /// <summary>
        ///   Login page
        /// </summary>
        public const string LOGIN = $"{_account}/Login";

        /// <summary>
        ///   Logout page
        /// </summary>
        public const string LOGOUT = $"{_account}/Logout";

        /// <summary>
        ///   My profile page
        /// </summary>
        public const string MY_PROFILE = $"{_account}/MyProfile";

        /// <summary>
        ///   Register page
        /// </summary>
        public const string REGISTER = $"{_account}/Register";
    }
}