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
            ///  item Field Page
            /// </summary>
            public const string ITEM_FIELD = $"{_projectWithId}/item-field";

            /// <summary>
            ///   The placeholder for the item layout ID in URLs
            /// </summary>
            public const string ITEM_FIELD_ID_PLACEHOLDER = "{ItemFieldId:guid}";

            /// <summary>
            ///  item Layout Page with item layout ID provided
            /// </summary>
            public const string ITEM_FIELD_WITH_ID = $"{ITEM_FIELD}?itemFieldId={ITEM_FIELD_ID_PLACEHOLDER}";
        }

        /// <summary>
        ///   Project Item Layout pages
        /// </summary>
        public static class ItemLayout
        {
            /// <summary>
            ///  item Layout Page
            /// </summary>
            public const string ITEM_LAYOUT = $"{_projectWithId}/item-layout";

            /// <summary>
            ///   The placeholder for the item layout ID in URLs
            /// </summary>
            public const string ITEM_LAYOUT_ID_PLACEHOLDER = "{ItemLayoutId:guid}";

            /// <summary>
            ///  item Layout Page with item layout ID provided
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
            ///   Page to create a task queue within an initiative
            /// </summary>
            public const string INITIATIVE_CREATE_TASK_QUEUE = $"{_initiativeWithId}/create-task-queue";
        }
    }

    /// <summary>
    ///   Task pages
    /// </summary>
    public static class Task
    {
        /// <summary>
        ///   Placeholder for the task ID in URLs
        /// </summary>
        public const string TASK_ID_PLACEHOLDER = "{TaskId:guid}";

        /// <summary>
        ///   Placeholder for the task queue ID in URLs
        /// </summary>
        public const string TASK_QUEUE_ID_PLACEHOLDER = "{TaskQueueId:guid}";

        private const string _task = "/Task";

        private const string _taskQueue = "/TaskQueue";

        private const string _taskQueueWithId = $"{_taskQueue}/{TASK_QUEUE_ID_PLACEHOLDER}";

        /// <summary>
        ///   Task queue dashboard page
        /// </summary>
        public const string TASK_QUEUE_DASHBOARD = $"{_taskQueueWithId}/dashboard";

        /// <summary>
        ///   Create task page
        /// </summary>
        public const string CREATE_TASK = $"{_task}/create";

        /// <summary>
        ///   Create task page
        /// </summary>
        public const string CREATE_TASK_IN_QUEUE = $"{_task}/create?task-queue-id={TASK_QUEUE_ID_PLACEHOLDER}";

        /// <summary>
        ///   View task page
        /// </summary>
        public const string VIEW_TASK = $"{_task}/{TASK_ID_PLACEHOLDER}";

    }

    /// <summary>
    ///   Asset pages
    /// </summary>
    public static class Asset
    {
        /// <summary>
        ///   Placeholder for the asset ID in URLs
        /// </summary>
        public const string ASSET_ID_PLACEHOLDER = "{AssetId:guid}";

        /// <summary>
        ///   Placeholder for the asset layout ID in URLs
        /// </summary>
        public const string ASSET_LAYOUT_ID_PLACEHOLDER = "{AssetLayoutId:guid}";

        private const string _asset = "/Asset";

        private const string _assetWithId = $"{_asset}/{ASSET_ID_PLACEHOLDER}";

        private const string _assetLayoutWithId = $"{_asset}/Layout/{ASSET_LAYOUT_ID_PLACEHOLDER}";

        /// <summary>
        ///   Asset layout dashboard page
        /// </summary>
        public const string ASSET_LAYOUT_DASHBOARD = $"{_assetLayoutWithId}/dashboard";

        /// <summary>
        ///   Create asset page
        /// </summary>
        public const string CREATE_ASSET = $"{_asset}/create";

        /// <summary>
        ///   Create asset with layout page
        /// </summary>
        public const string CREATE_ASSET_WITH_LAYOUT = $"{_asset}/create?layout-id={ASSET_LAYOUT_ID_PLACEHOLDER}";

        /// <summary>
        ///   View asset page
        /// </summary>
        public const string VIEW_ASSET = _assetWithId;

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