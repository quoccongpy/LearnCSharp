using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearnCSharp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addUniqueProductVariant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_details_product_variant_ProductVariantId",
                table: "order_details");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_crust_crust_id",
                table: "product_variant");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_size_size_id",
                table: "product_variant");

            migrationBuilder.DropIndex(
                name: "IX_product_variant_product_id",
                table: "product_variant");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "product_variant",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ProductVariantId",
                table: "order_details",
                newName: "product_variant_id");

            migrationBuilder.RenameIndex(
                name: "IX_order_details_ProductVariantId",
                table: "order_details",
                newName: "IX_order_details_product_variant_id");

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_product_id_size_id_crust_id",
                table: "product_variant",
                columns: new[] { "product_id", "size_id", "crust_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_order_details_product_variant_product_variant_id",
                table: "order_details",
                column: "product_variant_id",
                principalTable: "product_variant",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_crust_crust_id",
                table: "product_variant",
                column: "crust_id",
                principalTable: "crust",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_size_size_id",
                table: "product_variant",
                column: "size_id",
                principalTable: "size",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_order_details_product_variant_product_variant_id",
                table: "order_details");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_crust_crust_id",
                table: "product_variant");

            migrationBuilder.DropForeignKey(
                name: "FK_product_variant_size_size_id",
                table: "product_variant");

            migrationBuilder.DropIndex(
                name: "IX_product_variant_product_id_size_id_crust_id",
                table: "product_variant");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "product_variant",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "product_variant_id",
                table: "order_details",
                newName: "ProductVariantId");

            migrationBuilder.RenameIndex(
                name: "IX_order_details_product_variant_id",
                table: "order_details",
                newName: "IX_order_details_ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_product_variant_product_id",
                table: "product_variant",
                column: "product_id");

            migrationBuilder.AddForeignKey(
                name: "FK_order_details_product_variant_ProductVariantId",
                table: "order_details",
                column: "ProductVariantId",
                principalTable: "product_variant",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_crust_crust_id",
                table: "product_variant",
                column: "crust_id",
                principalTable: "crust",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_product_variant_size_size_id",
                table: "product_variant",
                column: "size_id",
                principalTable: "size",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
