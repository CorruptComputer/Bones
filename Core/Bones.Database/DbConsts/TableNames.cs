namespace Bones.Database.DbConsts;

internal static class TableNames
{
    internal static class Accounts
    {
        internal const string BonesRoles = "BonesRoles";
        internal const string BonesRoleClaims = "BonesRoleClaims";

        internal const string BonesUsers = "BonesUser";
        internal const string BonesUserClaims = "BonesUserClaims";
        internal const string BonesUserLogins = "BonesUserLogins";
        internal const string BonesUserRoles = "BonesUserRoles";
        internal const string BonesUserSessions = "BonesUserSessions";
        internal const string BonesUserTokens = "BonesUserTokens";
    }

    internal static class Audits
    {
        internal const string AccountAudits = "AccountAudits";
        internal const string LoginAudits = "LoginAudits";
        internal const string ProjectAudits = "ProjectAudits";
        internal const string SystemAdminAudits = "SystemAdminAudits";
        internal const string SystemAudits = "SystemAudits";
        internal const string SessionAttemptAudits = "SessionAttemptAudits";
    }

    internal static class Item
    {
        internal static class Assignments
        {
            internal const string ItemAssignmentSlots = "ItemAssignmentSlots";
            internal const string ItemAssignees = "ItemAssignees";
        }

        internal static class Fields
        {
            internal const string ItemFields = "ItemFields";
            internal const string ItemFieldListEntries = "ItemFieldListEntries";
            internal const string ItemFieldVersions = "ItemFieldVersions";
        }

        internal static class Layouts
        {
            internal const string ItemLayouts = "ItemLayouts";
            internal const string ItemLayoutFieldVersionLinks = "ItemLayoutFieldVersionLinks";
            internal const string ItemLayoutVersions = "ItemLayoutVersions";
        }

        internal static class Types
        {
            internal const string Assets = "Assets";

            internal const string Tasks = "Tasks";
        }

        internal const string Items = "Items";
        internal const string ItemValues = "ItemValues";
        internal const string ItemVersions = "ItemVersions";
    }

    internal static class OrganizationManagement
    {
        internal const string BonesOrganizations = "BonesOrganizations";
    }

    internal static class ProjectManagement
    {
        internal const string Initiatives = "Initiatives";
        internal const string Projects = "Projects";
        internal const string TaskQueues = "TaskQueues";
    }

    internal static class System
    {
        internal static class Queues
        {
            internal const string ConfirmationEmailDeadQueue = "ConfirmationEmailDeadQueue";
            internal const string ConfirmationEmailQueue = "ConfirmationEmailQueue";

            internal const string ForgotPasswordEmailDeadQueue = "ForgotPasswordEmailDeadQueue";
            internal const string ForgotPasswordEmailQueue = "ForgotPasswordEmailQueue";
        }

        internal const string SystemSettings = "SystemSettings";

        internal const string TaskErrors = "TaskErrors";
    }
}
