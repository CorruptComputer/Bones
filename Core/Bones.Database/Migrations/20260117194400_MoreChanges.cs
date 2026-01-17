using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class MoreChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemVersions_Items_ItemId1",
                schema: "Item",
                table: "ItemVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskQueues_Initiatives_InitiativeId1",
                schema: "TaskManagement",
                table: "TaskQueues");

            migrationBuilder.DropIndex(
                name: "IX_TaskQueues_InitiativeId1",
                schema: "TaskManagement",
                table: "TaskQueues");

            migrationBuilder.DropIndex(
                name: "IX_ItemVersions_ItemId1",
                schema: "Item",
                table: "ItemVersions");

            migrationBuilder.DropColumn(
                name: "InitiativeId1",
                schema: "TaskManagement",
                table: "TaskQueues");

            migrationBuilder.DropColumn(
                name: "ItemId1",
                schema: "Item",
                table: "ItemVersions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InitiativeId1",
                schema: "TaskManagement",
                table: "TaskQueues",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ItemId1",
                schema: "Item",
                table: "ItemVersions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskQueues_InitiativeId1",
                schema: "TaskManagement",
                table: "TaskQueues",
                column: "InitiativeId1");

            migrationBuilder.CreateIndex(
                name: "IX_ItemVersions_ItemId1",
                schema: "Item",
                table: "ItemVersions",
                column: "ItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemVersions_Items_ItemId1",
                schema: "Item",
                table: "ItemVersions",
                column: "ItemId1",
                principalSchema: "Item",
                principalTable: "Items",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskQueues_Initiatives_InitiativeId1",
                schema: "TaskManagement",
                table: "TaskQueues",
                column: "InitiativeId1",
                principalSchema: "ProjectManagement",
                principalTable: "Initiatives",
                principalColumn: "Id");
        }
    }
}
