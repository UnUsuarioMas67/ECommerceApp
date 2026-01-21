using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerce.Api.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class FixClientsNaming : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_ClientId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Users_ClientId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_Users_ClientId",
                table: "ShopOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReviews_OrderLines_OrderLineId",
                table: "UserReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_UserReviews_Users_ClientId",
                table: "UserReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Users",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserReviews",
                table: "UserReviews");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Clients");

            migrationBuilder.RenameTable(
                name: "UserReviews",
                newName: "ClientReviews");

            migrationBuilder.RenameIndex(
                name: "IX_UserReviews_OrderLineId",
                table: "ClientReviews",
                newName: "IX_ClientReviews_OrderLineId");

            migrationBuilder.RenameIndex(
                name: "IX_UserReviews_ClientId",
                table: "ClientReviews",
                newName: "IX_ClientReviews_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Clients",
                table: "Clients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientReviews",
                table: "ClientReviews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Clients_ClientId",
                table: "Addresses",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Clients_ClientId",
                table: "Carts",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientReviews_Clients_ClientId",
                table: "ClientReviews",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClientReviews_OrderLines_OrderLineId",
                table: "ClientReviews",
                column: "OrderLineId",
                principalTable: "OrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_Clients_ClientId",
                table: "ShopOrders",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Clients_ClientId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Clients_ClientId",
                table: "Carts");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientReviews_Clients_ClientId",
                table: "ClientReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientReviews_OrderLines_OrderLineId",
                table: "ClientReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_ShopOrders_Clients_ClientId",
                table: "ShopOrders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Clients",
                table: "Clients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientReviews",
                table: "ClientReviews");

            migrationBuilder.RenameTable(
                name: "Clients",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "ClientReviews",
                newName: "UserReviews");

            migrationBuilder.RenameIndex(
                name: "IX_ClientReviews_OrderLineId",
                table: "UserReviews",
                newName: "IX_UserReviews_OrderLineId");

            migrationBuilder.RenameIndex(
                name: "IX_ClientReviews_ClientId",
                table: "UserReviews",
                newName: "IX_UserReviews_ClientId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Users",
                table: "Users",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserReviews",
                table: "UserReviews",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_ClientId",
                table: "Addresses",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Users_ClientId",
                table: "Carts",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ShopOrders_Users_ClientId",
                table: "ShopOrders",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserReviews_OrderLines_OrderLineId",
                table: "UserReviews",
                column: "OrderLineId",
                principalTable: "OrderLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserReviews_Users_ClientId",
                table: "UserReviews",
                column: "ClientId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
