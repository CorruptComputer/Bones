using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddQueues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "SystemQueues");

            migrationBuilder.CreateTable(
                name: "ConfirmationEmailDeadQueue",
                schema: "SystemQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeadQueueCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OriginalCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    LastTry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    FailureReasons = table.Column<List<string>>(type: "text[]", nullable: false),
                    EmailTo = table.Column<string>(type: "text", nullable: false),
                    ConfirmationLink = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationEmailDeadQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmationEmailQueue",
                schema: "SystemQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    LastTry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReasons = table.Column<List<string>>(type: "text[]", nullable: false),
                    EmailTo = table.Column<string>(type: "text", nullable: false),
                    ConfirmationLink = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationEmailQueue", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfirmationEmailDeadQueue",
                schema: "SystemQueues");

            migrationBuilder.DropTable(
                name: "ConfirmationEmailQueue",
                schema: "SystemQueues");
        }
    }
}
