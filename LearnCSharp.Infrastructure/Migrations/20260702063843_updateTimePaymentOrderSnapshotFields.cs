using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnCSharp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateTimePaymentOrderSnapshotFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "payment_date",
                table: "order",
                type: "TIMESTAMP",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "payment_transaction_id",
                table: "order",
                type: "character varying(150)",
                maxLength: 150,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "payment_date",
                table: "order");

            migrationBuilder.DropColumn(
                name: "payment_transaction_id",
                table: "order");
        }
    }
}
