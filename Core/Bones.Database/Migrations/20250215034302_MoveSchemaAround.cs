using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class MoveSchemaAround : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AssetManagement");

            migrationBuilder.EnsureSchema(
                name: "OrganizationManagement");

            migrationBuilder.EnsureSchema(
                name: "AccountManagement");

            migrationBuilder.EnsureSchema(
                name: "System");

            migrationBuilder.EnsureSchema(
                name: "GenericItem");

            migrationBuilder.EnsureSchema(
                name: "MappingManagement");

            migrationBuilder.EnsureSchema(
                name: "ProjectManagement");

            migrationBuilder.EnsureSchema(
                name: "WorkItemManagement");

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
                    EmailConfirmedDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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
                name: "GenericItemFields",
                schema: "GenericItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemFields", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenericItemLayouts",
                schema: "GenericItem",
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
                    table.PrimaryKey("PK_GenericItemLayouts", x => x.Id);
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
                    OsmGeometry = table.Column<string>(type: "text", nullable: true),
                    DeletedOnOsmFlag = table.Column<bool>(type: "boolean", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OsmObjects", x => x.Id);
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
                name: "GenericItemLayoutVersions",
                schema: "GenericItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    EnabledFor = table.Column<int>(type: "integer", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ItemLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    GenericItemLayoutId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemLayoutVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericItemLayoutVersions_GenericItemLayouts_GenericItemLay~",
                        column: x => x.GenericItemLayoutId,
                        principalSchema: "GenericItem",
                        principalTable: "GenericItemLayouts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GenericItems",
                schema: "GenericItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    GenericItemLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentVersion = table.Column<int>(type: "integer", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericItems_GenericItemLayouts_GenericItemLayoutId",
                        column: x => x.GenericItemLayoutId,
                        principalSchema: "GenericItem",
                        principalTable: "GenericItemLayouts",
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
                    Geometry = table.Column<string>(type: "text", nullable: true),
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
                name: "GenericItemFieldVersions",
                schema: "GenericItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    GenericItemFieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    CanBeNegative = table.Column<bool>(type: "boolean", nullable: true),
                    GeoLocationType = table.Column<int>(type: "integer", nullable: true),
                    RequiredAddressFields = table.Column<int>(type: "integer", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    GenericItemLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemFieldVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericItemFieldVersions_GenericItemFields_GenericItemField~",
                        column: x => x.GenericItemFieldId,
                        principalSchema: "GenericItem",
                        principalTable: "GenericItemFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenericItemFieldVersions_GenericItemLayoutVersions_GenericI~",
                        column: x => x.GenericItemLayoutVersionId,
                        principalSchema: "GenericItem",
                        principalTable: "GenericItemLayoutVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                schema: "AssetManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_GenericItems_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "GenericItem",
                        principalTable: "GenericItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericItemVersions",
                schema: "GenericItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    GenericItemLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    GenericItemId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericItemVersions_GenericItemLayoutVersions_GenericItemLa~",
                        column: x => x.GenericItemLayoutVersionId,
                        principalSchema: "GenericItem",
                        principalTable: "GenericItemLayoutVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenericItemVersions_GenericItems_GenericItemId",
                        column: x => x.GenericItemId,
                        principalSchema: "GenericItem",
                        principalTable: "GenericItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkItemQueues",
                schema: "WorkItemManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InitiativeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemQueues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItemQueues_Initiatives_InitiativeId",
                        column: x => x.InitiativeId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericItemFieldListEntries",
                schema: "GenericItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    GenericItemFieldVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingType = table.Column<int>(type: "integer", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemFieldListEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericItemFieldListEntries_GenericItemFieldVersions_Generi~",
                        column: x => x.GenericItemFieldVersionId,
                        principalSchema: "GenericItem",
                        principalTable: "GenericItemFieldVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericItemValues",
                schema: "GenericItem",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationId = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    GenericItemVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericItemValues_GenericItemFieldVersions_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "GenericItem",
                        principalTable: "GenericItemFieldVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenericItemValues_GenericItemVersions_GenericItemVersionId",
                        column: x => x.GenericItemVersionId,
                        principalSchema: "GenericItem",
                        principalTable: "GenericItemVersions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GenericItemValues_GeoLocations_LocationId",
                        column: x => x.LocationId,
                        principalSchema: "MappingManagement",
                        principalTable: "GeoLocations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkItems",
                schema: "WorkItemManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkItemQueueId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedToQueueDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItems_GenericItems_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "GenericItem",
                        principalTable: "GenericItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkItems_WorkItemQueues_WorkItemQueueId",
                        column: x => x.WorkItemQueueId,
                        principalSchema: "WorkItemManagement",
                        principalTable: "WorkItemQueues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_ItemId",
                schema: "AssetManagement",
                table: "Assets",
                column: "ItemId");

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
                name: "IX_GenericItemFieldListEntries_GenericItemFieldVersionId",
                schema: "GenericItem",
                table: "GenericItemFieldListEntries",
                column: "GenericItemFieldVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemFieldVersions_GenericItemFieldId",
                schema: "GenericItem",
                table: "GenericItemFieldVersions",
                column: "GenericItemFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemFieldVersions_GenericItemLayoutVersionId",
                schema: "GenericItem",
                table: "GenericItemFieldVersions",
                column: "GenericItemLayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemLayoutVersions_GenericItemLayoutId",
                schema: "GenericItem",
                table: "GenericItemLayoutVersions",
                column: "GenericItemLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItems_GenericItemLayoutId",
                schema: "GenericItem",
                table: "GenericItems",
                column: "GenericItemLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemValues_FieldId",
                schema: "GenericItem",
                table: "GenericItemValues",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemValues_GenericItemVersionId",
                schema: "GenericItem",
                table: "GenericItemValues",
                column: "GenericItemVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemValues_LocationId",
                schema: "GenericItem",
                table: "GenericItemValues",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemVersions_GenericItemId",
                schema: "GenericItem",
                table: "GenericItemVersions",
                column: "GenericItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemVersions_GenericItemLayoutVersionId",
                schema: "GenericItem",
                table: "GenericItemVersions",
                column: "GenericItemLayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_GeoLocations_OsmObjectId",
                schema: "MappingManagement",
                table: "GeoLocations",
                column: "OsmObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Initiatives_ProjectId",
                schema: "ProjectManagement",
                table: "Initiatives",
                column: "ProjectId");

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
                name: "IX_WorkItemQueues_InitiativeId",
                schema: "WorkItemManagement",
                table: "WorkItemQueues",
                column: "InitiativeId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItems_ItemId",
                schema: "WorkItemManagement",
                table: "WorkItems",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItems_WorkItemQueueId",
                schema: "WorkItemManagement",
                table: "WorkItems",
                column: "WorkItemQueueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "GenericItemFieldListEntries",
                schema: "GenericItem");

            migrationBuilder.DropTable(
                name: "GenericItemValues",
                schema: "GenericItem");

            migrationBuilder.DropTable(
                name: "TaskErrors",
                schema: "System");

            migrationBuilder.DropTable(
                name: "WorkItems",
                schema: "WorkItemManagement");

            migrationBuilder.DropTable(
                name: "BonesRoles",
                schema: "AccountManagement");

            migrationBuilder.DropTable(
                name: "GenericItemFieldVersions",
                schema: "GenericItem");

            migrationBuilder.DropTable(
                name: "GenericItemVersions",
                schema: "GenericItem");

            migrationBuilder.DropTable(
                name: "GeoLocations",
                schema: "MappingManagement");

            migrationBuilder.DropTable(
                name: "WorkItemQueues",
                schema: "WorkItemManagement");

            migrationBuilder.DropTable(
                name: "GenericItemFields",
                schema: "GenericItem");

            migrationBuilder.DropTable(
                name: "GenericItemLayoutVersions",
                schema: "GenericItem");

            migrationBuilder.DropTable(
                name: "GenericItems",
                schema: "GenericItem");

            migrationBuilder.DropTable(
                name: "OsmObjects",
                schema: "MappingManagement");

            migrationBuilder.DropTable(
                name: "Initiatives",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "GenericItemLayouts",
                schema: "GenericItem");

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
