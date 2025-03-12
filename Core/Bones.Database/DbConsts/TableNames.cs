namespace Bones.Database.DbConsts;

internal static class TableNames
{
    internal static class AccountManagement
    {
        internal const string BonesRoles = "BonesRoles";
        internal const string BonesRoleClaims = "BonesRoleClaims";

        internal const string BonesUsers = "BonesUser";
        internal const string BonesUserClaims = "BonesUserClaims";
        internal const string BonesUserLogins = "BonesUserLogins";
        internal const string BonesUserRoles = "BonesUserRoles";
        internal const string BonesUserTokens = "BonesUserTokens";
    }

    internal static class AssetManagement
    {
        internal const string Assets = "Assets";
    }

    internal static class Audit
    {
        internal const string AccountAudits = "AccountAudits";
        internal const string LoginAudits = "LoginAudits";
        internal const string ProjectAudits = "ProjectAudits";
        internal const string SystemAdminAudits = "SystemAdminAudits";
        internal const string SystemAudits = "SystemAudits";
    }

    internal static class GenericItem
    {
        internal const string GenericItemFields = "GenericItemFields";
        internal const string GenericItemFieldListEntries = "GenericItemFieldListEntries";
        internal const string GenericItemFieldVersions = "GenericItemFieldVersions";

        internal const string GenericItemLayouts = "GenericItemLayouts";
        internal const string GenericItemLayoutFieldVersionLinks = "GenericItemLayoutFieldVersionLinks";
        internal const string GenericItemLayoutVersions = "GenericItemLayoutVersions";

        internal const string GenericItems = "GenericItems";
        internal const string GenericItemValues = "GenericItemValues";
        internal const string GenericItemVersions = "GenericItemVersions";
    }

    internal static class MappingManagement
    {
        internal const string GeoLocations = "GeoLocations";
        internal const string OsmObjects = "OsmObjects";
    }

    internal static class OrganizationManagement
    {
        internal const string BonesOrganizations = "BonesOrganizations";
    }

    internal static class ProjectManagement
    {
        internal const string Initiatives = "Initiatives";
        internal const string Projects = "Projects";
    }

    internal static class System
    {
        internal const string ConfirmationEmailDeadQueue = "ConfirmationEmailDeadQueue";
        internal const string ConfirmationEmailQueue = "ConfirmationEmailQueue";

        internal const string ForgotPasswordEmailDeadQueue = "ForgotPasswordEmailDeadQueue";
        internal const string ForgotPasswordEmailQueue = "ForgotPasswordEmailQueue";

        internal const string SystemSettings = "SystemSettings";

        internal const string TaskErrors = "TaskErrors";
    }

    internal static class WorkItemManagement
    {
        internal const string WorkItems = "WorkItems";

        internal const string WorkItemQueues = "WorkItemQueues";
    }
}
