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
        public const string CHANGE_EMAIL = $"{_account}/ChangeEmail";

        /// <summary>
        ///   Confirm email page
        /// </summary>
        public const string CHANGE_PASSWORD = $"{_account}/ChangePassword";

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

        /// <summary>
        ///   Confirm email page
        /// </summary>
        public const string FORGOT_PASSWORD = $"{_account}/ForgotPassword";

        /// <summary>
        ///   Confirm email page
        /// </summary>
        public const string RESET_PASSWORD = $"{_account}/ResetPassword";
    }

    /// <summary>
    ///   Project pages
    /// </summary>
    public static class Project
    {
        private const string _project = "/Project";
        private const string _projectWithId = $"{_project}/{{ProjectId:guid}}";

        /// <summary>
        ///   Create project page
        /// </summary>
        public const string CREATE = $"{_project}/Create";

        /// <summary>
        ///   Project dashboard page
        /// </summary>
        public const string PROJECT_DASHBOARD = $"{_projectWithId}/Dashboard";

        /// <summary>
        ///   Project dashboard page
        /// </summary>
        public const string MODIFY_PROJECT = $"{_projectWithId}/edit";

        /// <summary>
        ///   Project Item Field pages
        /// </summary>
        public static class ItemField
        {
            private const string _itemField = $"{_projectWithId}/ItemField";
            private const string _itemFieldWithId = $"{_itemField}/{{ItemFieldId:guid}}";

            /// <summary>
            ///   Create item field page
            /// </summary>
            public const string CREATE_FIELD = $"{_itemField}/create";

            /// <summary>
            ///   Edit item field page
            /// </summary>
            public const string EDIT_FIELD = $"{_itemFieldWithId}/edit";
        }

        /// <summary>
        ///   Project Item Layout pages
        /// </summary>
        public static class ItemLayout
        {
            private const string _itemLayout = $"{_projectWithId}/ItemLayout";
            private const string _itemLayoutWithId = $"{_itemLayout}/{{ItemLayoutId:guid}}";

            /// <summary>
            ///   Create layout page
            /// </summary>
            public const string CREATE_LAYOUT = $"{_itemLayout}/create";

            /// <summary>
            ///   Edit layout page
            /// </summary>
            public const string EDIT_LAYOUT = $"{_itemLayoutWithId}/edit";
        }

        /// <summary>
        ///   Project Initiative pages
        /// </summary>
        public static class Initiative
        {
            private const string _initiative = $"{_projectWithId}/Initiative";
            private const string _initiativeWithId = $"{_initiative}/{{InitiativeId:guid}}";

            /// <summary>
            ///   Create initiative page
            /// </summary>
            public const string CREATE_INITIATIVE = $"{_initiative}/Create";

            /// <summary>
            ///   Initiative dashboard page
            /// </summary>
            public const string INITIATIVE_DASHBOARD = $"{_initiativeWithId}/Dashboard";

            /// <summary>
            ///   Page to create a work item queue within an initiative
            /// </summary>
            public const string INITIATIVE_CREATE_WORKITEM_QUEUE = $"{_initiativeWithId}/CreateWorkItemQueue";
        }
    }

    /// <summary>
    ///   Work item pages
    /// </summary>
    public static class WorkItem
    {
        private const string _workItem = "/WorkItem";

        private const string _workItemQueueWithId = $"{_workItem}/Q{{WorkItemQueueId:guid}}";

        /// <summary>
        ///   Work item queue dashboard page
        /// </summary>
        public const string WORKITEM_QUEUE_DASHBOARD = $"{_workItemQueueWithId}/Dashboard";

    }

    /// <summary>
    ///   System Admin pages
    /// </summary>
    public static class SystemAdmin
    {
        private const string _systemAdmin = "/SystemAdmin";

        /// <summary>
        ///   Confirm email page
        /// </summary>
        public const string DASHBOARD = $"{_systemAdmin}/Dashboard";
    }
}