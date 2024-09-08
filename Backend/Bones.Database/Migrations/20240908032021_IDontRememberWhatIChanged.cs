using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class IDontRememberWhatIChanged : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "System");

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                schema: "ProjectManagement",
                table: "WorkItemLayouts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "DeleteFlag",
                schema: "ProjectManagement",
                table: "Initiatives",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                schema: "AccountManagement",
                table: "BonesUsers",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ReadOnlyRole",
                schema: "AccountManagement",
                table: "BonesRoles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "OwnerType",
                schema: "AssetManagement",
                table: "AssetLayouts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetLayouts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwningUserId",
                schema: "AssetManagement",
                table: "AssetLayouts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerType",
                schema: "AssetManagement",
                table: "AssetFields",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetFields",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwningUserId",
                schema: "AssetManagement",
                table: "AssetFields",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaskErrors",
                schema: "System",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ErrorTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ErrorMessage = table.Column<string>(type: "text", nullable: false),
                    StackTrace = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskErrors", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkItemLayouts_ProjectId",
                schema: "ProjectManagement",
                table: "WorkItemLayouts",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetLayouts_OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetLayouts",
                column: "OwningOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetLayouts_OwningUserId",
                schema: "AssetManagement",
                table: "AssetLayouts",
                column: "OwningUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFields_OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetFields",
                column: "OwningOrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetFields_OwningUserId",
                schema: "AssetManagement",
                table: "AssetFields",
                column: "OwningUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetFields_BonesOrganizations_OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetFields",
                column: "OwningOrganizationId",
                principalSchema: "OrganizationManagement",
                principalTable: "BonesOrganizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetFields_BonesUsers_OwningUserId",
                schema: "AssetManagement",
                table: "AssetFields",
                column: "OwningUserId",
                principalSchema: "AccountManagement",
                principalTable: "BonesUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetLayouts_BonesOrganizations_OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetLayouts",
                column: "OwningOrganizationId",
                principalSchema: "OrganizationManagement",
                principalTable: "BonesOrganizations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetLayouts_BonesUsers_OwningUserId",
                schema: "AssetManagement",
                table: "AssetLayouts",
                column: "OwningUserId",
                principalSchema: "AccountManagement",
                principalTable: "BonesUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItemLayouts_Projects_ProjectId",
                schema: "ProjectManagement",
                table: "WorkItemLayouts",
                column: "ProjectId",
                principalSchema: "ProjectManagement",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetFields_BonesOrganizations_OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetFields");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetFields_BonesUsers_OwningUserId",
                schema: "AssetManagement",
                table: "AssetFields");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetLayouts_BonesOrganizations_OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetLayouts");

            migrationBuilder.DropForeignKey(
                name: "FK_AssetLayouts_BonesUsers_OwningUserId",
                schema: "AssetManagement",
                table: "AssetLayouts");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItemLayouts_Projects_ProjectId",
                schema: "ProjectManagement",
                table: "WorkItemLayouts");

            migrationBuilder.DropTable(
                name: "TaskErrors",
                schema: "System");

            migrationBuilder.DropIndex(
                name: "IX_WorkItemLayouts_ProjectId",
                schema: "ProjectManagement",
                table: "WorkItemLayouts");

            migrationBuilder.DropIndex(
                name: "IX_AssetLayouts_OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetLayouts");

            migrationBuilder.DropIndex(
                name: "IX_AssetLayouts_OwningUserId",
                schema: "AssetManagement",
                table: "AssetLayouts");

            migrationBuilder.DropIndex(
                name: "IX_AssetFields_OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetFields");

            migrationBuilder.DropIndex(
                name: "IX_AssetFields_OwningUserId",
                schema: "AssetManagement",
                table: "AssetFields");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                schema: "ProjectManagement",
                table: "WorkItemLayouts");

            migrationBuilder.DropColumn(
                name: "DeleteFlag",
                schema: "ProjectManagement",
                table: "Initiatives");

            migrationBuilder.DropColumn(
                name: "ReadOnlyRole",
                schema: "AccountManagement",
                table: "BonesRoles");

            migrationBuilder.DropColumn(
                name: "OwnerType",
                schema: "AssetManagement",
                table: "AssetLayouts");

            migrationBuilder.DropColumn(
                name: "OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetLayouts");

            migrationBuilder.DropColumn(
                name: "OwningUserId",
                schema: "AssetManagement",
                table: "AssetLayouts");

            migrationBuilder.DropColumn(
                name: "OwnerType",
                schema: "AssetManagement",
                table: "AssetFields");

            migrationBuilder.DropColumn(
                name: "OwningOrganizationId",
                schema: "AssetManagement",
                table: "AssetFields");

            migrationBuilder.DropColumn(
                name: "OwningUserId",
                schema: "AssetManagement",
                table: "AssetFields");

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                schema: "AccountManagement",
                table: "BonesUsers",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(256)",
                oldMaxLength: 256,
                oldNullable: true);
        }
    }
}
