using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class Rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_Items_ItemId",
                schema: "AssetManagement",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_Items_ItemId",
                schema: "WorkItemManagement",
                table: "WorkItems");

            migrationBuilder.DropTable(
                name: "ItemFieldListEntries",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "ItemValues",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "ItemFields",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "ItemVersions",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "ItemLayoutVersions",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "Items",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "ItemLayouts",
                schema: "GenericItems");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                schema: "System",
                table: "TaskErrors",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordResetLink",
                schema: "SystemQueues",
                table: "ForgotPasswordEmailQueue",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "EmailTo",
                schema: "SystemQueues",
                table: "ForgotPasswordEmailQueue",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "PasswordResetLink",
                schema: "SystemQueues",
                table: "ForgotPasswordEmailDeadQueue",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "EmailTo",
                schema: "SystemQueues",
                table: "ForgotPasswordEmailDeadQueue",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "EmailTo",
                schema: "SystemQueues",
                table: "ConfirmationEmailQueue",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ConfirmationLink",
                schema: "SystemQueues",
                table: "ConfirmationEmailQueue",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "EmailTo",
                schema: "SystemQueues",
                table: "ConfirmationEmailDeadQueue",
                type: "character varying(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "ConfirmationLink",
                schema: "SystemQueues",
                table: "ConfirmationEmailDeadQueue",
                type: "character varying(4096)",
                maxLength: 4096,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "GenericItemLayouts",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    EnabledFor = table.Column<int[]>(type: "integer[]", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemLayouts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GenericItemLayoutVersions",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    GenericItemLayoutId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemLayoutVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericItemLayoutVersions_GenericItemLayouts_GenericItemLay~",
                        column: x => x.GenericItemLayoutId,
                        principalSchema: "GenericItems",
                        principalTable: "GenericItemLayouts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GenericItems",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
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
                        principalSchema: "GenericItems",
                        principalTable: "GenericItemLayouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericItemFields",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    GenericItemLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericItemFields_GenericItemLayoutVersions_GenericItemLayo~",
                        column: x => x.GenericItemLayoutVersionId,
                        principalSchema: "GenericItems",
                        principalTable: "GenericItemLayoutVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GenericItemVersions",
                schema: "GenericItems",
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
                        principalSchema: "GenericItems",
                        principalTable: "GenericItemLayoutVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenericItemVersions_GenericItems_GenericItemId",
                        column: x => x.GenericItemId,
                        principalSchema: "GenericItems",
                        principalTable: "GenericItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GenericItemFieldListEntries",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false),
                    ParentFieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemFieldListEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericItemFieldListEntries_GenericItemFields_ParentFieldId",
                        column: x => x.ParentFieldId,
                        principalSchema: "GenericItems",
                        principalTable: "GenericItemFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GenericItemValues",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    LocationType = table.Column<int>(type: "integer", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    GenericItemVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GenericItemValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GenericItemValues_GenericItemFields_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "GenericItems",
                        principalTable: "GenericItemFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GenericItemValues_GenericItemVersions_GenericItemVersionId",
                        column: x => x.GenericItemVersionId,
                        principalSchema: "GenericItems",
                        principalTable: "GenericItemVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemFieldListEntries_ParentFieldId",
                schema: "GenericItems",
                table: "GenericItemFieldListEntries",
                column: "ParentFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemFields_GenericItemLayoutVersionId",
                schema: "GenericItems",
                table: "GenericItemFields",
                column: "GenericItemLayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemLayoutVersions_GenericItemLayoutId",
                schema: "GenericItems",
                table: "GenericItemLayoutVersions",
                column: "GenericItemLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItems_GenericItemLayoutId",
                schema: "GenericItems",
                table: "GenericItems",
                column: "GenericItemLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemValues_FieldId",
                schema: "GenericItems",
                table: "GenericItemValues",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemValues_GenericItemVersionId",
                schema: "GenericItems",
                table: "GenericItemValues",
                column: "GenericItemVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemVersions_GenericItemId",
                schema: "GenericItems",
                table: "GenericItemVersions",
                column: "GenericItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GenericItemVersions_GenericItemLayoutVersionId",
                schema: "GenericItems",
                table: "GenericItemVersions",
                column: "GenericItemLayoutVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_GenericItems_ItemId",
                schema: "AssetManagement",
                table: "Assets",
                column: "ItemId",
                principalSchema: "GenericItems",
                principalTable: "GenericItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_GenericItems_ItemId",
                schema: "WorkItemManagement",
                table: "WorkItems",
                column: "ItemId",
                principalSchema: "GenericItems",
                principalTable: "GenericItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assets_GenericItems_ItemId",
                schema: "AssetManagement",
                table: "Assets");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkItems_GenericItems_ItemId",
                schema: "WorkItemManagement",
                table: "WorkItems");

            migrationBuilder.DropTable(
                name: "GenericItemFieldListEntries",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "GenericItemValues",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "GenericItemFields",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "GenericItemVersions",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "GenericItemLayoutVersions",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "GenericItems",
                schema: "GenericItems");

            migrationBuilder.DropTable(
                name: "GenericItemLayouts",
                schema: "GenericItems");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                schema: "System",
                table: "TaskErrors",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordResetLink",
                schema: "SystemQueues",
                table: "ForgotPasswordEmailQueue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4096)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "EmailTo",
                schema: "SystemQueues",
                table: "ForgotPasswordEmailQueue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "PasswordResetLink",
                schema: "SystemQueues",
                table: "ForgotPasswordEmailDeadQueue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4096)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "EmailTo",
                schema: "SystemQueues",
                table: "ForgotPasswordEmailDeadQueue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "EmailTo",
                schema: "SystemQueues",
                table: "ConfirmationEmailQueue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "ConfirmationLink",
                schema: "SystemQueues",
                table: "ConfirmationEmailQueue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4096)",
                oldMaxLength: 4096);

            migrationBuilder.AlterColumn<string>(
                name: "EmailTo",
                schema: "SystemQueues",
                table: "ConfirmationEmailDeadQueue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "ConfirmationLink",
                schema: "SystemQueues",
                table: "ConfirmationEmailDeadQueue",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(4096)",
                oldMaxLength: 4096);

            migrationBuilder.CreateTable(
                name: "ItemLayouts",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    EnabledFor = table.Column<int[]>(type: "integer[]", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemLayouts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ItemLayoutVersions",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    ItemLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemLayoutVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemLayoutVersions_ItemLayouts_ItemLayoutId",
                        column: x => x.ItemLayoutId,
                        principalSchema: "GenericItems",
                        principalTable: "ItemLayouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemLayoutId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentVersion = table.Column<int>(type: "integer", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_ItemLayouts_ItemLayoutId",
                        column: x => x.ItemLayoutId,
                        principalSchema: "GenericItems",
                        principalTable: "ItemLayouts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemFields",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    ItemLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemFields", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemFields_ItemLayoutVersions_ItemLayoutVersionId",
                        column: x => x.ItemLayoutVersionId,
                        principalSchema: "GenericItems",
                        principalTable: "ItemLayoutVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ItemVersions",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ItemLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateDateTime = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    ItemId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemVersions_ItemLayoutVersions_ItemLayoutVersionId",
                        column: x => x.ItemLayoutVersionId,
                        principalSchema: "GenericItems",
                        principalTable: "ItemLayoutVersions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemVersions_Items_ItemId",
                        column: x => x.ItemId,
                        principalSchema: "GenericItems",
                        principalTable: "Items",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemFieldListEntries",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParentFieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchingType = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemFieldListEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemFieldListEntries_ItemFields_ParentFieldId",
                        column: x => x.ParentFieldId,
                        principalSchema: "GenericItems",
                        principalTable: "ItemFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ItemValues",
                schema: "GenericItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    ItemVersionId = table.Column<Guid>(type: "uuid", nullable: true),
                    LocationType = table.Column<int>(type: "integer", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemValues_ItemFields_FieldId",
                        column: x => x.FieldId,
                        principalSchema: "GenericItems",
                        principalTable: "ItemFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ItemValues_ItemVersions_ItemVersionId",
                        column: x => x.ItemVersionId,
                        principalSchema: "GenericItems",
                        principalTable: "ItemVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemFieldListEntries_ParentFieldId",
                schema: "GenericItems",
                table: "ItemFieldListEntries",
                column: "ParentFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemFields_ItemLayoutVersionId",
                schema: "GenericItems",
                table: "ItemFields",
                column: "ItemLayoutVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayoutVersions_ItemLayoutId",
                schema: "GenericItems",
                table: "ItemLayoutVersions",
                column: "ItemLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemLayoutId",
                schema: "GenericItems",
                table: "Items",
                column: "ItemLayoutId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemValues_FieldId",
                schema: "GenericItems",
                table: "ItemValues",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemValues_ItemVersionId",
                schema: "GenericItems",
                table: "ItemValues",
                column: "ItemVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVersions_ItemId",
                schema: "GenericItems",
                table: "ItemVersions",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVersions_ItemLayoutVersionId",
                schema: "GenericItems",
                table: "ItemVersions",
                column: "ItemLayoutVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assets_Items_ItemId",
                schema: "AssetManagement",
                table: "Assets",
                column: "ItemId",
                principalSchema: "GenericItems",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkItems_Items_ItemId",
                schema: "WorkItemManagement",
                table: "WorkItems",
                column: "ItemId",
                principalSchema: "GenericItems",
                principalTable: "Items",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
