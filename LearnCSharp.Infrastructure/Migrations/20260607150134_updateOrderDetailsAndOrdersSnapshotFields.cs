using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnCSharp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateOrderDetailsAndOrdersSnapshotFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_active",
                table: "order");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "order_details",
                newName: "unit_price");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "order",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "order",
                newName: "address");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "order",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "order",
                newName: "full_name");

            migrationBuilder.AddColumn<string>(
                name: "crust_name",
                table: "order_details",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "order_details",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "product_name",
                table: "order_details",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "size_name",
                table: "order_details",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "crust_name",
                table: "order_details");

            migrationBuilder.DropColumn(
                name: "note",
                table: "order_details");

            migrationBuilder.DropColumn(
                name: "product_name",
                table: "order_details");

            migrationBuilder.DropColumn(
                name: "size_name",
                table: "order_details");

            migrationBuilder.RenameColumn(
                name: "unit_price",
                table: "order_details",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "order",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "address",
                table: "order",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "order",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "order",
                newName: "FullName");

            migrationBuilder.AddColumn<bool>(
                name: "is_active",
                table: "order",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
