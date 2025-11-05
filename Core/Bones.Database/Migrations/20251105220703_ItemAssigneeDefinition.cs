using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class ItemAssigneeDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ItemAssigneeDefinitions",
                schema: "Item",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignmentType = table.Column<int>(type: "integer", nullable: false),
                    SelectionType = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    OrderIndex = table.Column<int>(type: "integer", nullable: false),
                    DeleteFlag = table.Column<bool>(type: "boolean", nullable: false),
                    ItemLayoutVersionId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAssigneeDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemAssigneeDefinitions_ItemLayoutVersions_ItemLayoutVersio~",
                        column: x => x.ItemLayoutVersionId,
                        principalSchema: "Item",
                        principalTable: "ItemLayoutVersions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ItemAssigneeDefinitions_ItemLayoutVersionId",
                schema: "Item",
                table: "ItemAssigneeDefinitions",
                column: "ItemLayoutVersionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ItemAssigneeDefinitions",
                schema: "Item");
        }
    }
}
