using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnCSharp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateTimeOrderSnapshotFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "shipping_address",
                table: "order");

            migrationBuilder.AddColumn<DateTime>(
                name: "scheduled_time",
                table: "order",
                type: "TIMESTAMP",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "scheduled_time",
                table: "order");

            migrationBuilder.AddColumn<string>(
                name: "shipping_address",
                table: "order",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);
        }
    }
}
