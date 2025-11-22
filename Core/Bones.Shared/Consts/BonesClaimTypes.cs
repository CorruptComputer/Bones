namespace Bones.Shared.Consts;

/// <summary>
///   All the claim types used in the app
/// </summary>
public static class BonesClaimTypes
{
    /// <summary>
    ///   Claim types for users
    /// </summary>
    public static class User
    {
        /// <summary>
        ///   Claim type for their email address
        /// </summary>
        public const string EMAIL = "UserEmail";

        /// <summary>
        ///   Claim type for their display name
        /// </summary>
        public const string DISPLAY_NAME = "UserDisplayName";
    }

    /// <summary>
    ///   Claim types for roles
    /// </summary>
    public static class Role
    {
        /// <summary>
        ///   Claim types for system roles
        /// </summary>
        public static class System
        {
            /// <summary>
            ///   Claim type for the system administrator flag
            /// </summary>
            public const string SYSTEM_ADMINISTRATOR = "SystemAdministrator";
        }

        /// <summary>
        ///   Claim types for organization roles
        /// </summary>
        public static class Organization
        {
            /// <summary>
            ///   Full administrator privileges over the organization, they can access everything within.
            /// </summary>
            public const string ORGANIZATION_ADMINISTRATOR = "OrganizationAdministrator";

            /// <summary>
            ///   Gets the claim type for permissions that should apply organization wide
            /// </summary>
            /// <param name="organizationId"></param>
            /// <param name="permissionName"></param>
            /// <returns></returns>
            public static string GetOrganizationWideClaimType(Guid organizationId, string permissionName)
            {
                return $"O{organizationId}|{permissionName}";
            }

        }

        /// <summary>
        ///   Project level claim types for roles
        /// </summary>
        public static class Project
        {
            /// <summary>
            ///   Claim type for if a user can view a project
            /// </summary>
            public const string VIEW_PROJECT = "ViewProject";

            /// <summary>
            ///   Claim type for if a user can create a project
            /// </summary>
            public const string CREATE_PROJECT = "CreateProject";

            /// <summary>
            ///   Claim type for if a user can delete a project
            /// </summary>
            public const string DELETE_PROJECT = "DeleteProject";

            /// <summary>
            ///   Claim type for if a user can view the project settings
            /// </summary>
            public const string VIEW_PROJECT_SETTINGS = "ViewProjectSettings";

            /// <summary>
            ///   Claim type for if a user can edit the project settings
            /// </summary>
            public const string EDIT_PROJECT_SETTINGS = "EditProjectSettings";

            /// <summary>
            ///   Gets the claim type for permissions that should apply to this project
            /// </summary>
            /// <param name="projectId"></param>
            /// <param name="permissionName"></param>
            /// <returns></returns>
            public static string GetProjectClaimType(Guid projectId, string permissionName)
            {
                return $"P{projectId}|{permissionName}";
            }
        }

        /// <summary>
        ///   Initiative level claim types for organization roles
        /// </summary>
        public static class Initiative
        {
            /// <summary>
            ///   Claim type for if a user can view an initiative
            /// </summary>
            public const string VIEW_INITIATIVE = "ViewInitiative";

            /// <summary>
            ///   Claim type for if a user can create an initiative
            /// </summary>
            public const string CREATE_INITIATIVE = "CreateInitive";

            /// <summary>
            ///   Claim type for if a user can delete an initiative
            /// </summary>
            public const string DELETE_INITIATIVE = "DeleteInitive";

            /// <summary>
            ///   Claim type for if a user can view the initiatives settings
            /// </summary>
            public const string VIEW_INITIATIVE_SETTINGS = "ViewInitiativeSettings";

            /// <summary>
            ///   Claim type for if a user can edit the initiatives settings
            /// </summary>
            public const string EDIT_INITIATIVE_SETTINGS = "EditInitiativeSettings";


            /// <summary>
            ///   Gets the claim type for permissions that should apply to this initiative
            /// </summary>
            /// <param name="initiativeId"></param>
            /// <param name="permissionName"></param>
            /// <returns></returns>
            public static string GetInitiativeClaimType(Guid initiativeId, string permissionName)
            {
                return $"I{initiativeId}|{permissionName}";
            }
        }

        /// <summary>
        ///   TaskQueue level claim types for organization roles
        /// </summary>
        public static class TaskQueue
        {
            /// <summary>
            ///   Claim type for if a user can view a queue
            /// </summary>
            public const string VIEW_QUEUE = "ViewTaskQueue";

            /// <summary>
            ///   Claim type for if a user can create a queue
            /// </summary>
            public const string CREATE_QUEUE = "CreateTaskQueue";

            /// <summary>
            ///   Claim type for if a user can delete a queue
            /// </summary>
            public const string DELETE_QUEUE = "DeleteTaskQueue";

            /// <summary>
            ///   Claim type for if a user can view the queues settings
            /// </summary>
            public const string VIEW_QUEUE_SETTINGS = "ViewTaskQueueSettings";

            /// <summary>
            ///   Claim type for if a user can edit the queues settings
            /// </summary>
            public const string EDIT_QUEUE_SETTINGS = "EditTaskQueueSettings";

            /// <summary>
            ///   Gets the claim type for permissions that should apply to this queue
            /// </summary>
            /// <param name="queueId"></param>
            /// <param name="permissionName"></param>
            /// <returns></returns>
            public static string GetTaskQueueClaimType(Guid queueId, string permissionName)
            {
                return $"Q{queueId}|{permissionName}";
            }
        }

        /// <summary>
        ///   Task level claim types for organization roles
        /// </summary>
        public static class Task
        {
            /// <summary>
            ///   Claim type for if a user can view a task
            /// </summary>
            public const string VIEW_TASK = "ViewTask";

            /// <summary>
            ///   Claim type for if a user can create a task
            /// </summary>
            public const string CREATE_TASK = "CreateTask";

            /// <summary>
            ///   Claim type for if a user can create a new task version
            /// </summary>
            public const string CREATE_TASK_VERSION = "CreateTaskVersion";

            /// <summary>
            ///   Claim type for if a user can delete a task
            /// </summary>
            public const string DELETE_TASK = "DeleteTask";

            /// <summary>
            ///   Claim type for if a user can edit a task
            /// </summary>
            public const string EDIT_TASK = "EditTask";
        }

        /// <summary>
        ///   Asset level claim types for organization roles
        /// </summary>
        public static class Asset
        {
            /// <summary>
            ///   Claim type for if a user can view an asset
            /// </summary>
            public const string VIEW_ASSET = "ViewAsset";

            /// <summary>
            ///   Claim type for if a user can create an asset
            /// </summary>
            public const string CREATE_ASSET = "CreateAsset";

            /// <summary>
            ///   Claim type for if a user can create an asset version
            /// </summary>
            public const string CREATE_ASSET_VERSION = "CreateAssetVersion";

            /// <summary>
            ///   Claim type for if a user can delete an asset
            /// </summary>
            public const string DELETE_ASSET = "DeleteAsset";

            /// <summary>
            ///   Claim type for if a user can edit an asset
            /// </summary>
            public const string EDIT_ASSET = "EditAsset";
        }

    }
}