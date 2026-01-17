using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemLayoutVersions_ItemLayouts_ItemLayoutId1",
                schema: "Item",
                table: "ItemLayoutVersions");

            migrationBuilder.DropIndex(
                name: "IX_ItemLayoutVersions_ItemLayoutId1",
                schema: "Item",
                table: "ItemLayoutVersions");

            migrationBuilder.DropColumn(
                name: "ItemLayoutId1",
                schema: "Item",
                table: "ItemLayoutVersions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ItemLayoutId1",
                schema: "Item",
                table: "ItemLayoutVersions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ItemLayoutVersions_ItemLayoutId1",
                schema: "Item",
                table: "ItemLayoutVersions",
                column: "ItemLayoutId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ItemLayoutVersions_ItemLayouts_ItemLayoutId1",
                schema: "Item",
                table: "ItemLayoutVersions",
                column: "ItemLayoutId1",
                principalSchema: "Item",
                principalTable: "ItemLayouts",
                principalColumn: "Id");
        }
    }
}
