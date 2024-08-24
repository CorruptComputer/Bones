namespace Bones.Shared.Consts;

// TODO: Make this not terrible
public static class ClaimTypes
{
    public static class User
    {

    }

    public static class Role
    {
        public static class System
        {
            // Gives ALL system permissions
            public const string SYSTEM_ADMINISTRATOR = "SystemAdministrator";
        }

        public static class Organization
        {
            public static class Project
            {
                public const string VIEW_PROJECT_SETTINGS = "ViewProjectSettings";
                public const string EDIT_PROJECT_SETTINGS = "EditProjectSettings";

                public const string CREATE_INITIATIVE = "CreateInitive";
                public const string DELETE_INITIATIVE = "DeleteInitive";

                public static string GetProjectClaimTypes(Guid projectId, string permissionName)
                {
                    return $"{projectId}|{permissionName}";
                }
            }

            public static class Initiative
            {
                public const string VIEW_INITIATIVE_SETTINGS = "ViewInitiativeSettings";
                public const string EDIT_INITIATIVE_SETTINGS = "EditInitiativeSettings";

                public const string CREATE_QUEUE = "CreateQueue";
                public const string DELETE_QUEUE = "DeleteQueue";

                public static string GetInitiativeClaimTypes(Guid projectId, Guid initiativeId, string permissionName)
                {
                    return $"{projectId}|{initiativeId}|{permissionName}";
                }
            }
        }
    }
}