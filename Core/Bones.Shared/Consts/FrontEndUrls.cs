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
        private const string _account = "/account";

        /// <summary>
        ///   Confirm email page
        /// </summary>
        public const string CHANGE_EMAIL = $"{_account}/change-email";

        /// <summary>
        ///   Confirm email page
        /// </summary>
        public const string CHANGE_PASSWORD = $"{_account}/change-password";

        /// <summary>
        ///   My profile page
        /// </summary>
        public const string MY_PROFILE = $"{_account}/my-profile";

        /// <summary>
        ///   Confirm email page
        /// </summary>
        public const string FORGOT_PASSWORD = $"{_account}/forgot-password";

        /// <summary>
        ///   Confirm email page
        /// </summary>
        public const string RESET_PASSWORD = $"{_account}/reset-password";
    }

    /// <summary>
    ///   Project pages
    /// </summary>
    public static class Project
    {
        /// <summary>
        ///   The placeholder for the project ID in URLs
        /// </summary>
        public const string PROJECT_ID_PLACEHOLDER = "{ProjectId:guid}";

        private const string _project = "/project";
        private const string _projectWithId = $"{_project}/{PROJECT_ID_PLACEHOLDER}";

        /// <summary>
        ///   Create project page
        /// </summary>
        public const string CREATE = $"{_project}/create";

        /// <summary>
        ///   Project dashboard page
        /// </summary>
        public const string PROJECT_DASHBOARD = $"{_projectWithId}/dashboard";

        /// <summary>
        ///   Project dashboard page
        /// </summary>
        public const string MODIFY_PROJECT = $"{_projectWithId}/edit";

        /// <summary>
        ///   Create initiative page
        /// </summary>
        public const string CREATE_INITIATIVE = $"{_projectWithId}/create-initiative";

        /// <summary>
        ///   Project Item Field pages
        /// </summary>
        public static class ItemField
        {
            /// <summary>
            ///   Item Field Page
            /// </summary>
            public const string ITEM_FIELD = $"{_projectWithId}/item-field";

            /// <summary>
            ///   The placeholder for the item layout ID in URLs
            /// </summary>
            public const string ITEM_FIELD_ID_PLACEHOLDER = "{ItemFieldId:guid}";

            /// <summary>
            ///   Item Layout Page with item layout ID provided
            /// </summary>
            public const string ITEM_FIELD_WITH_ID = $"{ITEM_FIELD}?itemFieldId={ITEM_FIELD_ID_PLACEHOLDER}";
        }

        /// <summary>
        ///   Project Item Layout pages
        /// </summary>
        public static class ItemLayout
        {
            /// <summary>
            ///   Item Layout Page
            /// </summary>
            public const string ITEM_LAYOUT = $"{_projectWithId}/item-layout";

            /// <summary>
            ///   The placeholder for the item layout ID in URLs
            /// </summary>
            public const string ITEM_LAYOUT_ID_PLACEHOLDER = "{ItemLayoutId:guid}";

            /// <summary>
            ///   Item Layout Page with item layout ID provided
            /// </summary>
            public const string ITEM_LAYOUT_WITH_ID = $"{ITEM_LAYOUT}?itemLayoutId={ITEM_LAYOUT_ID_PLACEHOLDER}";
        }

        /// <summary>
        ///   Project Initiative pages
        /// </summary>
        public static class Initiative
        {
            /// <summary>
            ///   The placeholder for the initiative ID in URLs
            /// </summary>
            public const string INITIATIVE_ID_PLACEHOLDER = "{InitiativeId:guid}";

            private const string _initiative = $"Initiative";
            private const string _initiativeWithId = $"{_initiative}/{INITIATIVE_ID_PLACEHOLDER}";



            /// <summary>
            ///   Initiative dashboard page
            /// </summary>
            public const string INITIATIVE_DASHBOARD = $"{_initiativeWithId}/dashboard";

            /// <summary>
            ///   Page to create a work item queue within an initiative
            /// </summary>
            public const string INITIATIVE_CREATE_WORKITEM_QUEUE = $"{_initiativeWithId}/create-work-item-queue";
        }
    }

    /// <summary>
    ///   Work item pages
    /// </summary>
    public static class WorkItem
    {
        /// <summary>
        ///   Placeholder for the work item ID in URLs
        /// </summary>
        public const string WORKITEM_ID_PLACEHOLDER = "{WorkItemId:guid}";

        /// <summary>
        ///   Placeholder for the work item queue ID in URLs
        /// </summary>
        public const string WORKITEM_QUEUE_ID_PLACEHOLDER = "{WorkItemQueueId:guid}";

        private const string _workItem = "/WorkItem";

        private const string _workItemQueue = "/WorkItemQueue";

        private const string _workItemQueueWithId = $"{_workItemQueue}/{WORKITEM_QUEUE_ID_PLACEHOLDER}";

        /// <summary>
        ///   Work item queue dashboard page
        /// </summary>
        public const string WORKITEM_QUEUE_DASHBOARD = $"{_workItemQueueWithId}/dashboard";

        /// <summary>
        ///   Create work item page
        /// </summary>
        public const string CREATE_WORK_ITEM = $"{_workItem}/create";

        /// <summary>
        ///   Create work item page
        /// </summary>
        public const string CREATE_WORK_ITEM_IN_QUEUE = $"{_workItem}/create?work-item-queue-id={WORKITEM_QUEUE_ID_PLACEHOLDER}";

        /// <summary>
        ///   View work item page
        /// </summary>
        public const string VIEW_WORK_ITEM = $"{_workItem}/{WORKITEM_ID_PLACEHOLDER}";

    }

    /// <summary>
    ///   System Admin pages
    /// </summary>
    public static class SystemAdmin
    {
        private const string _systemAdmin = "/SystemAdmin";

        /// <summary>
        ///   System admin dashboard page
        /// </summary>
        public const string DASHBOARD = $"{_systemAdmin}/Dashboard";

        /// <summary>
        ///   Modify system settings page
        /// </summary>
        public const string MODIFY_SYSTEM_SETTINGS = $"{_systemAdmin}/ModifySystemSettings";
    }
}