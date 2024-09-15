using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bones.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddForgotPasswordQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ForgotPasswordEmailDeadQueue",
                schema: "SystemQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeadQueueCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    OriginalCreated = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    LastTry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReasons = table.Column<List<string>>(type: "text[]", nullable: false),
                    EmailTo = table.Column<string>(type: "text", nullable: false),
                    PasswordResetLink = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForgotPasswordEmailDeadQueue", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ForgotPasswordEmailQueue",
                schema: "SystemQueues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    RetryCount = table.Column<int>(type: "integer", nullable: false),
                    LastTry = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FailureReasons = table.Column<List<string>>(type: "text[]", nullable: false),
                    EmailTo = table.Column<string>(type: "text", nullable: false),
                    PasswordResetLink = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForgotPasswordEmailQueue", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ForgotPasswordEmailDeadQueue",
                schema: "SystemQueues");

            migrationBuilder.DropTable(
                name: "ForgotPasswordEmailQueue",
                schema: "SystemQueues");
        }
    }
}
