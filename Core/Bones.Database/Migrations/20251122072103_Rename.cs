using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Audit");

            migrationBuilder.EnsureSchema(
                name: "AssetManagement");

            migrationBuilder.EnsureSchema(
                name: "OrganizationManagement");

            migrationBuilder.EnsureSchema(
                name: "AccountManagement");

            migrationBuilder.EnsureSchema(
                name: "System");

            migrationBuilder.EnsureSchema(
                name: "MappingManagement");

            migrationBuilder.EnsureSchema(
                name: "ProjectManagement");

            migrationBuilder.EnsureSchema(
                name: "Item");

            migrationBuilder.EnsureSchema(
                name: "TaskManagement");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "BonesOrganizations",
                schema: "OrganizationManagement",
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
                schema: "AccountManagement",
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
                schema: "System",
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
                schema: "System",
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
                schema: "System",
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
                schema: "System",
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
                name: "OsmObjects",
                schema: "MappingManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OsmType = table.Column<int>(type: "integer", nullable: false),
                    OsmId = table.Column<long>(type: "bigint", nullable: false),
                    LastOsmVersion = table.Column<long>(type: "bigint", nullable: true),
                    LastOsmGeometryUpdate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    OsmGeometry = table.Column<Geometry>(type: "geometry", nullable: true),
                    DeletedOnOsmFlag = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsmObjects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SessionAttemptAudits",
                schema: "Audit",
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
                schema: "AccountManagement",
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
                        principalSchema: "OrganizationManagement",
                        principalTable: "BonesOrganizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AccountAudits",
                schema: "Audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ActionTaken = table.Column<int>(type: "integer", nullable: false),
                    ActionTakenById = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccountAudits_BonesUser_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccountAudits_BonesUser_ActionTakenById",
                        column: x => x.ActionTakenById,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesUserClaims",
                schema: "AccountManagement",
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
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesUserLogins",
                schema: "AccountManagement",
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
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesUserSessions",
                schema: "AccountManagement",
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
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesUserTokens",
                schema: "AccountManagement",
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
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LoginAudits",
                schema: "Audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccountId = table.Column<Guid>(type: "uuid", nullable: true),
                    UnknownEmail = table.Column<string>(type: "text", nullable: true),
                    LoginDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Successful = table.Column<bool>(type: "boolean", nullable: false),
                    RequestingIpAddress = table.Column<string>(type: "character varying(45)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoginAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoginAudits_BonesUser_AccountId",
                        column: x => x.AccountId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    OwnerType = table.Column<int>(type: "integer", nullable: false),
                    OwningUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwningOrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_BonesOrganizations_OwningOrganizationId",
                        column: x => x.OwningOrganizationId,
                        principalSchema: "OrganizationManagement",
                        principalTable: "BonesOrganizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_BonesUser_OwningUserId",
                        column: x => x.OwningUserId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUser",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SystemAudits",
                schema: "Audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ActionTaken = table.Column<int>(type: "integer", nullable: false),
                    SettingChanged = table.Column<int>(type: "integer", nullable: true),
                    ActionTakenById = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAudits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemAudits_BonesUser_ActionTakenById",
                        column: x => x.ActionTakenById,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesRoleClaims",
                schema: "AccountManagement",
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
                        principalSchema: "AccountManagement",
                        principalTable: "BonesRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesUserRoles",
                schema: "AccountManagement",
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
                        principalSchema: "AccountManagement",
                        principalTable: "BonesRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BonesUserRoles_BonesUser_UserId",
                        column: x => x.UserId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUser",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeoLocations",
                schema: "MappingManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Latitude = table.Column<double>(type: "double precision", nullable: true),
                    Longitude = table.Column<double>(type: "double precision", nullable: true),
                    Geometry = table.Column<Geometry>(type: "geometry", nullable: true),
                    OsmObjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastAcknowledgedOsmVersion = table.Column<long>(type: "bigint", nullable: true),
                    StreetNumber = table.Column<string>(type: "text", nullable: true),
                    StreetName = table.Column<string>(type: "text", nullable: true),
                    CityOrPlace = table.Column<string>(type: "text", nullable: true),
                    StateOrProvince = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    County = table.Column<string>(type: "text", nullable: true),
                    Country = table.Column<string>(type: "text", nullable: true),
                    GeoLocated = table.Column<bool>(type: "boolean", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeoLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeoLocations_OsmObjects_OsmObjectId",
                        column: x => x.OsmObjectId,
                        principalSchema: "MappingManagement",
                        principalTable: "OsmObjects",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GeoLocations_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Initiatives",
                schema: "ProjectManagement",
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
                        principalSchema: "ProjectManagement",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemFields",
                schema: "Item",
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
                        principalSchema: "ProjectManagement",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemLayouts",
                schema: "Item",
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
                        principalSchema: "ProjectManagement",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskQueues",
                schema: "TaskManagement",
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
                        principalSchema: "ProjectManagement",
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemFieldVersions",
                schema: "Item",
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
                    GeoLocationType = table.Column<int>(type: "integer", nullable: true),
                    RequiredAddressFields = table.Column<int>(type: "integer", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemFieldVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemFieldVersions_ItemFields_ItemFieldId",
                        column: x => x.ItemFieldId,
                        principalSchema: "Item",
                        principalTable: "ItemFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemLayoutVersions",
                schema: "Item",
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
                        principalSchema: "Item",
                        principalTable: "ItemLayouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                schema: "Item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FriendlyId = table.Column<string>(type: "text", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentVersion = table.Column<int>(type: "integer", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_ItemLayouts_ItemLayoutId",
                        column: x => x.ItemLayoutId,
                        principalSchema: "Item",
                        principalTable: "ItemLayouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Items_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemFieldListEntries",
                schema: "Item",
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
                        principalSchema: "Item",
                        principalTable: "ItemFieldVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemAssignmentSlots",
                schema: "Item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentType = table.Column<int>(type: "integer", nullable: false),
                    SelectionType = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    AssignmentStates = table.Column<List<string>>(type: "text[]", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    ItemLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAssignmentSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemAssignmentSlots_ItemLayoutVersions_ItemLayoutVersionId",
                        column: x => x.ItemLayoutVersionId,
                        principalSchema: "Item",
                        principalTable: "ItemLayoutVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemLayoutFieldVersionLinks",
                schema: "Item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OrderNumber = table.Column<int>(type: "integer", nullable: false),
                    LayoutVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemLayoutFieldVersionLinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemLayoutFieldVersionLinks_ItemFieldVersions_FieldVersionId",
                        column: x => x.FieldVersionId,
                        principalSchema: "Item",
                        principalTable: "ItemFieldVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemLayoutFieldVersionLinks_ItemLayoutVersions_LayoutVersio~",
                        column: x => x.LayoutVersionId,
                        principalSchema: "Item",
                        principalTable: "ItemLayoutVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                schema: "AssetManagement",
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
                        principalSchema: "Item",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assets_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemVersions",
                schema: "Item",
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
                        principalSchema: "Item",
                        principalTable: "ItemLayoutVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemVersions_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Item",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskItems",
                schema: "TaskManagement",
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
                    table.PrimaryKey("PK_TaskItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskItems_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "Item",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskItems_TaskQueues_TaskQueueId",
                        column: x => x.TaskQueueId,
                        principalSchema: "TaskManagement",
                        principalTable: "TaskQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemAssignees",
                schema: "Item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedRoleId = table.Column<Guid>(type: "uuid", nullable: true),
                    State = table.Column<string>(type: "text", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    ItemVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAssignees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemAssignees_BonesRoles_AssignedRoleId",
                        column: x => x.AssignedRoleId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemAssignees_BonesUser_AssignedUserId",
                        column: x => x.AssignedUserId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemAssignees_ItemAssignmentSlots_SlotId",
                        column: x => x.SlotId,
                        principalSchema: "Item",
                        principalTable: "ItemAssignmentSlots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemAssignees_ItemVersions_ItemVersionId",
                        column: x => x.ItemVersionId,
                        principalSchema: "Item",
                        principalTable: "ItemVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemValues",
                schema: "Item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    ItemVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemValues_GeoLocations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "MappingManagement",
                        principalTable: "GeoLocations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ItemValues_ItemFieldVersions_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "Item",
                        principalTable: "ItemFieldVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemValues_ItemVersions_ItemVersionId",
                        column: x => x.ItemVersionId,
                        principalSchema: "Item",
                        principalTable: "ItemVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountAudits_AccountId",
                schema: "Audit",
                table: "AccountAudits",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountAudits_ActionTakenById",
                schema: "Audit",
                table: "AccountAudits",
                column: "ActionTakenById");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ItemId",
                schema: "AssetManagement",
                table: "Assets",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ProjectId",
                schema: "AssetManagement",
                table: "Assets",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_BonesRoleClaims_RoleId",
                schema: "AccountManagement",
                table: "BonesRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BonesRoles_OrganizationId",
                schema: "AccountManagement",
                table: "BonesRoles",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "AccountManagement",
                table: "BonesRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "AccountManagement",
                table: "BonesUser",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "AccountManagement",
                table: "BonesUser",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BonesUserClaims_UserId",
                schema: "AccountManagement",
                table: "BonesUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BonesUserLogins_UserId",
                schema: "AccountManagement",
                table: "BonesUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_BonesUserRoles_RoleId",
                schema: "AccountManagement",
                table: "BonesUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_BonesUserSessions_UserId",
                schema: "AccountManagement",
                table: "BonesUserSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GeoLocations_OsmObjectId",
                schema: "MappingManagement",
                table: "GeoLocations",
                column: "OsmObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GeoLocations_ProjectId",
                schema: "MappingManagement",
                table: "GeoLocations",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Initiatives_ProjectId",
                schema: "ProjectManagement",
                table: "Initiatives",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAssignees_AssignedRoleId",
                schema: "Item",
                table: "ItemAssignees",
                column: "AssignedRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAssignees_AssignedUserId",
                schema: "Item",
                table: "ItemAssignees",
                column: "AssignedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAssignees_ItemVersionId",
                schema: "Item",
                table: "ItemAssignees",
                column: "ItemVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAssignees_SlotId",
                schema: "Item",
                table: "ItemAssignees",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAssignmentSlots_ItemLayoutVersionId",
                schema: "Item",
                table: "ItemAssignmentSlots",
                column: "ItemLayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFieldListEntries_ItemFieldVersionId",
                schema: "Item",
                table: "ItemFieldListEntries",
                column: "ItemFieldVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFields_ProjectId",
                schema: "Item",
                table: "ItemFields",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFieldVersions_ItemFieldId",
                schema: "Item",
                table: "ItemFieldVersions",
                column: "ItemFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayoutFieldVersionLinks_FieldVersionId",
                schema: "Item",
                table: "ItemLayoutFieldVersionLinks",
                column: "FieldVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayoutFieldVersionLinks_LayoutVersionId",
                schema: "Item",
                table: "ItemLayoutFieldVersionLinks",
                column: "LayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayouts_FriendlyIdPrefix",
                schema: "Item",
                table: "ItemLayouts",
                column: "FriendlyIdPrefix");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayouts_ProjectId",
                schema: "Item",
                table: "ItemLayouts",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayoutVersions_ItemLayoutId",
                schema: "Item",
                table: "ItemLayoutVersions",
                column: "ItemLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_FriendlyId",
                schema: "Item",
                table: "Items",
                column: "FriendlyId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemLayoutId",
                schema: "Item",
                table: "Items",
                column: "ItemLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ProjectId",
                schema: "Item",
                table: "Items",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemValues_FieldId",
                schema: "Item",
                table: "ItemValues",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemValues_ItemVersionId",
                schema: "Item",
                table: "ItemValues",
                column: "ItemVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemValues_LocationId",
                schema: "Item",
                table: "ItemValues",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVersions_ItemId",
                schema: "Item",
                table: "ItemVersions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVersions_ItemLayoutVersionId",
                schema: "Item",
                table: "ItemVersions",
                column: "ItemLayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVersions_Version",
                schema: "Item",
                table: "ItemVersions",
                column: "Version");

            migrationBuilder.CreateIndex(
                name: "IX_LoginAudits_AccountId",
                schema: "Audit",
                table: "LoginAudits",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwningOrganizationId",
                schema: "ProjectManagement",
                table: "Projects",
                column: "OwningOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_OwningUserId",
                schema: "ProjectManagement",
                table: "Projects",
                column: "OwningUserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAudits_ActionTakenById",
                schema: "Audit",
                table: "SystemAudits",
                column: "ActionTakenById");

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_Setting",
                schema: "System",
                table: "SystemSettings",
                column: "Setting",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_ItemId",
                schema: "TaskManagement",
                table: "TaskItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskItems_TaskQueueId",
                schema: "TaskManagement",
                table: "TaskItems",
                column: "TaskQueueId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskQueues_InitiativeId",
                schema: "TaskManagement",
                table: "TaskQueues",
                column: "InitiativeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountAudits",
                schema: "Audit");

            migrationBuilder.DropTable(
                name: "Assets",
                schema: "AssetManagement");

            migrationBuilder.DropTable(
                name: "BonesRoleClaims",
                schema: "AccountManagement");

            migrationBuilder.DropTable(
                name: "BonesUserClaims",
                schema: "AccountManagement");

            migrationBuilder.DropTable(
                name: "BonesUserLogins",
                schema: "AccountManagement");

            migrationBuilder.DropTable(
                name: "BonesUserRoles",
                schema: "AccountManagement");

            migrationBuilder.DropTable(
                name: "BonesUserSessions",
                schema: "AccountManagement");

            migrationBuilder.DropTable(
                name: "BonesUserTokens",
                schema: "AccountManagement");

            migrationBuilder.DropTable(
                name: "ConfirmationEmailDeadQueue",
                schema: "System");

            migrationBuilder.DropTable(
                name: "ConfirmationEmailQueue",
                schema: "System");

            migrationBuilder.DropTable(
                name: "ForgotPasswordEmailDeadQueue",
                schema: "System");

            migrationBuilder.DropTable(
                name: "ForgotPasswordEmailQueue",
                schema: "System");

            migrationBuilder.DropTable(
                name: "ItemAssignees",
                schema: "Item");

            migrationBuilder.DropTable(
                name: "ItemFieldListEntries",
                schema: "Item");

            migrationBuilder.DropTable(
                name: "ItemLayoutFieldVersionLinks",
                schema: "Item");

            migrationBuilder.DropTable(
                name: "ItemValues",
                schema: "Item");

            migrationBuilder.DropTable(
                name: "LoginAudits",
                schema: "Audit");

            migrationBuilder.DropTable(
                name: "SessionAttemptAudits",
                schema: "Audit");

            migrationBuilder.DropTable(
                name: "SystemAudits",
                schema: "Audit");

            migrationBuilder.DropTable(
                name: "SystemSettings",
                schema: "System");

            migrationBuilder.DropTable(
                name: "TaskErrors",
                schema: "System");

            migrationBuilder.DropTable(
                name: "TaskItems",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "BonesRoles",
                schema: "AccountManagement");

            migrationBuilder.DropTable(
                name: "ItemAssignmentSlots",
                schema: "Item");

            migrationBuilder.DropTable(
                name: "GeoLocations",
                schema: "MappingManagement");

            migrationBuilder.DropTable(
                name: "ItemFieldVersions",
                schema: "Item");

            migrationBuilder.DropTable(
                name: "ItemVersions",
                schema: "Item");

            migrationBuilder.DropTable(
                name: "TaskQueues",
                schema: "TaskManagement");

            migrationBuilder.DropTable(
                name: "OsmObjects",
                schema: "MappingManagement");

            migrationBuilder.DropTable(
                name: "ItemFields",
                schema: "Item");

            migrationBuilder.DropTable(
                name: "ItemLayoutVersions",
                schema: "Item");

            migrationBuilder.DropTable(
                name: "Items",
                schema: "Item");

            migrationBuilder.DropTable(
                name: "Initiatives",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "ItemLayouts",
                schema: "Item");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "BonesOrganizations",
                schema: "OrganizationManagement");

            migrationBuilder.DropTable(
                name: "BonesUser",
                schema: "AccountManagement");
        }
    }
}
