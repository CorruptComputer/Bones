using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class AuditsAndSystemSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Audit");

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
                name: "IX_LoginAudits_AccountId",
                schema: "Audit",
                table: "LoginAudits",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAudits_ActionTakenById",
                schema: "Audit",
                table: "SystemAudits",
                column: "ActionTakenById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountAudits",
                schema: "Audit");

            migrationBuilder.DropTable(
                name: "LoginAudits",
                schema: "Audit");

            migrationBuilder.DropTable(
                name: "SystemAudits",
                schema: "Audit");

            migrationBuilder.DropTable(
                name: "SystemSettings",
                schema: "System");
        }
    }
}
