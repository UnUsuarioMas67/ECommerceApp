using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ECommerce.Api.Infrastructure.EF.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderStatuses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: "Pending");

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: "Success");

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Status",
                value: "Failed");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Status",
                value: "Awaiting Payment");

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Status",
                value: "In Progress");

            migrationBuilder.UpdateData(
                table: "OrderStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Status",
                value: "Awaiting Shipping");

            migrationBuilder.InsertData(
                table: "OrderStatuses",
                columns: new[] { "Id", "Status" },
                values: new object[,]
                {
                    { 4, "Shipped" },
                    { 5, "Out for Delivery" },
                    { 6, "Delivered" }
                });
        }
    }
}
