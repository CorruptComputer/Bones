using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddItemVersionTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                schema: "GenericItem",
                table: "GenericItemVersions",
                type: "character varying(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                schema: "GenericItem",
                table: "GenericItemVersions");
        }
    }
}
