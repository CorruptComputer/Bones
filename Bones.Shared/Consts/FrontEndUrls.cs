namespace Bones.Shared.Consts;

public static class FrontEndUrls
{
    public const string HOME = "/";

    public static class Account
    {
        private const string _account = "/Account";

        public const string CONFIRM_EMAIL = $"{_account}/ConfirmEmail";
        public const string LOGIN = $"{_account}/Login";
        public const string LOGOUT = $"{_account}/Logout";
        public const string MY_PROFILE = $"{_account}/MyProfile";
        public const string REGISTER = $"{_account}/Register";
    }
}