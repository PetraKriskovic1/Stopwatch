using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stopwatch.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpgradingTimedSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "TimedSessions");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ElapsedTime",
                table: "TimedSessions",
                type: "time",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsReset",
                table: "TimedSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRunning",
                table: "TimedSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ElapsedTime",
                table: "TimedSessions");

            migrationBuilder.DropColumn(
                name: "IsReset",
                table: "TimedSessions");

            migrationBuilder.DropColumn(
                name: "IsRunning",
                table: "TimedSessions");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "TimedSessions",
                type: "datetimeoffset",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
