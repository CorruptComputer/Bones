using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                name: "ProjectManagement");

            migrationBuilder.CreateTable(
                name: "AssetLayouts",
                schema: "AssetManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetLayouts", x => x.Id);
                });

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
                name: "BonesUsers",
                schema: "AccountManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("PK_BonesUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkItemLayouts",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemLayouts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetLayoutVersions",
                schema: "AssetManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetLayoutVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetLayoutVersions_AssetLayouts_AssetLayoutId",
                        column: x => x.AssetLayoutId,
                        principalSchema: "AssetManagement",
                        principalTable: "AssetLayouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BonesRoles",
                schema: "AccountManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsSystemRole = table.Column<bool>(type: "boolean", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uuid", nullable: true),
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
                name: "Assets",
                schema: "AssetManagement",
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
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_BonesOrganizations_OwningOrganizationId",
                        column: x => x.OwningOrganizationId,
                        principalSchema: "OrganizationManagement",
                        principalTable: "BonesOrganizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Assets_BonesUsers_OwningUserId",
                        column: x => x.OwningUserId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUsers",
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
                        name: "FK_BonesUserClaims_BonesUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUsers",
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
                        name: "FK_BonesUserLogins_BonesUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUsers",
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
                        name: "FK_BonesUserTokens_BonesUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUsers",
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
                        name: "FK_Projects_BonesUsers_OwningUserId",
                        column: x => x.OwningUserId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkItemLayoutVersions",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkItemLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemLayoutVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItemLayoutVersions_WorkItemLayouts_WorkItemLayoutId",
                        column: x => x.WorkItemLayoutId,
                        principalSchema: "ProjectManagement",
                        principalTable: "WorkItemLayouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetFields",
                schema: "AssetManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    AssetLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetFields_AssetLayoutVersions_AssetLayoutVersionId",
                        column: x => x.AssetLayoutVersionId,
                        principalSchema: "AssetManagement",
                        principalTable: "AssetLayoutVersions",
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
                        name: "FK_BonesUserRoles_BonesUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "AccountManagement",
                        principalTable: "BonesUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetVersions",
                schema: "AssetManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetVersions_AssetLayoutVersions_AssetLayoutId",
                        column: x => x.AssetLayoutId,
                        principalSchema: "AssetManagement",
                        principalTable: "AssetLayoutVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetVersions_Assets_AssetId",
                        column: x => x.AssetId,
                        principalSchema: "AssetManagement",
                        principalTable: "Assets",
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
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
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
                name: "WorkItemFields",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    WorkItemLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItemFields_WorkItemLayoutVersions_WorkItemLayoutVersion~",
                        column: x => x.WorkItemLayoutVersionId,
                        principalSchema: "ProjectManagement",
                        principalTable: "WorkItemLayoutVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AssetFieldListEntries",
                schema: "AssetManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    ParentFieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetFieldListEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetFieldListEntries_AssetFields_ParentFieldId",
                        column: x => x.ParentFieldId,
                        principalSchema: "AssetManagement",
                        principalTable: "AssetFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetValues",
                schema: "AssetManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationType = table.Column<int>(type: "integer", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    AssetVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetValues_AssetFields_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "AssetManagement",
                        principalTable: "AssetFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AssetValues_AssetVersions_AssetVersionId",
                        column: x => x.AssetVersionId,
                        principalSchema: "AssetManagement",
                        principalTable: "AssetVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Queues",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InitiativeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Queues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Queues_Initiatives_InitiativeId",
                        column: x => x.InitiativeId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkItemFieldListEntries",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    ParentFieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemFieldListEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItemFieldListEntries_WorkItemFields_ParentFieldId",
                        column: x => x.ParentFieldId,
                        principalSchema: "ProjectManagement",
                        principalTable: "WorkItemFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkItems",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    QueueId = table.Column<Guid>(type: "uuid", nullable: false),
                    AddedToQueueDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItems_Queues_QueueId",
                        column: x => x.QueueId,
                        principalSchema: "ProjectManagement",
                        principalTable: "Queues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    WorkItemId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_WorkItems_WorkItemId",
                        column: x => x.WorkItemId,
                        principalSchema: "ProjectManagement",
                        principalTable: "WorkItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WorkItemVersions",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkItemLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    WorkItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItemVersions_WorkItemLayoutVersions_WorkItemLayoutId",
                        column: x => x.WorkItemLayoutId,
                        principalSchema: "ProjectManagement",
                        principalTable: "WorkItemLayoutVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkItemVersions_WorkItems_WorkItemId",
                        column: x => x.WorkItemId,
                        principalSchema: "ProjectManagement",
                        principalTable: "WorkItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkItemValues",
                schema: "ProjectManagement",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationType = table.Column<int>(type: "integer", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    WorkItemVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkItemValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkItemValues_WorkItemFields_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "ProjectManagement",
                        principalTable: "WorkItemFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkItemValues_WorkItemVersions_WorkItemVersionId",
                        column: x => x.WorkItemVersionId,
                        principalSchema: "ProjectManagement",
                        principalTable: "WorkItemVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetFieldListEntries_ParentFieldId",
                schema: "AssetManagement",
                table: "AssetFieldListEntries",
                column: "ParentFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFields_AssetLayoutVersionId",
                schema: "AssetManagement",
                table: "AssetFields",
                column: "AssetLayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetLayoutVersions_AssetLayoutId",
                schema: "AssetManagement",
                table: "AssetLayoutVersions",
                column: "AssetLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_OwningOrganizationId",
                schema: "AssetManagement",
                table: "Assets",
                column: "OwningOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_OwningUserId",
                schema: "AssetManagement",
                table: "Assets",
                column: "OwningUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetValues_AssetVersionId",
                schema: "AssetManagement",
                table: "AssetValues",
                column: "AssetVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetValues_FieldId",
                schema: "AssetManagement",
                table: "AssetValues",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetVersions_AssetId",
                schema: "AssetManagement",
                table: "AssetVersions",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetVersions_AssetLayoutId",
                schema: "AssetManagement",
                table: "AssetVersions",
                column: "AssetLayoutId");

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
                name: "EmailIndex",
                schema: "AccountManagement",
                table: "BonesUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "AccountManagement",
                table: "BonesUsers",
                column: "NormalizedUserName",
                unique: true);

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
                name: "IX_Queues_InitiativeId",
                schema: "ProjectManagement",
                table: "Queues",
                column: "InitiativeId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_WorkItemId",
                schema: "ProjectManagement",
                table: "Tags",
                column: "WorkItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemFieldListEntries_ParentFieldId",
                schema: "ProjectManagement",
                table: "WorkItemFieldListEntries",
                column: "ParentFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemFields_WorkItemLayoutVersionId",
                schema: "ProjectManagement",
                table: "WorkItemFields",
                column: "WorkItemLayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemLayoutVersions_WorkItemLayoutId",
                schema: "ProjectManagement",
                table: "WorkItemLayoutVersions",
                column: "WorkItemLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItems_QueueId",
                schema: "ProjectManagement",
                table: "WorkItems",
                column: "QueueId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemValues_FieldId",
                schema: "ProjectManagement",
                table: "WorkItemValues",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemValues_WorkItemVersionId",
                schema: "ProjectManagement",
                table: "WorkItemValues",
                column: "WorkItemVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemVersions_WorkItemId",
                schema: "ProjectManagement",
                table: "WorkItemVersions",
                column: "WorkItemId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemVersions_WorkItemLayoutId",
                schema: "ProjectManagement",
                table: "WorkItemVersions",
                column: "WorkItemLayoutId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetFieldListEntries",
                schema: "AssetManagement");

            migrationBuilder.DropTable(
                name: "AssetValues",
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
                name: "Tags",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "WorkItemFieldListEntries",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "WorkItemValues",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "AssetFields",
                schema: "AssetManagement");

            migrationBuilder.DropTable(
                name: "AssetVersions",
                schema: "AssetManagement");

            migrationBuilder.DropTable(
                name: "BonesRoles",
                schema: "AccountManagement");

            migrationBuilder.DropTable(
                name: "WorkItemFields",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "WorkItemVersions",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "AssetLayoutVersions",
                schema: "AssetManagement");

            migrationBuilder.DropTable(
                name: "Assets",
                schema: "AssetManagement");

            migrationBuilder.DropTable(
                name: "WorkItemLayoutVersions",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "WorkItems",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "AssetLayouts",
                schema: "AssetManagement");

            migrationBuilder.DropTable(
                name: "WorkItemLayouts",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "Queues",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "Initiatives",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "ProjectManagement");

            migrationBuilder.DropTable(
                name: "BonesOrganizations",
                schema: "OrganizationManagement");

            migrationBuilder.DropTable(
                name: "BonesUsers",
                schema: "AccountManagement");
        }
    }
}
