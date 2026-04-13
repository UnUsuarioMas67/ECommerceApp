using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Api.EF.Migrations
{
    /// <inheritdoc />
    public partial class MoveStripeSessionIdToShopOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_StripeSessionId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                table: "Payments");

            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                table: "ShopOrders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_ShopOrders_StripeSessionId",
                table: "ShopOrders",
                column: "StripeSessionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ShopOrders_StripeSessionId",
                table: "ShopOrders");

            migrationBuilder.DropColumn(
                name: "StripeSessionId",
                table: "ShopOrders");

            migrationBuilder.AddColumn<string>(
                name: "StripeSessionId",
                table: "Payments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StripeSessionId",
                table: "Payments",
                column: "StripeSessionId",
                unique: true);
        }
    }
}
