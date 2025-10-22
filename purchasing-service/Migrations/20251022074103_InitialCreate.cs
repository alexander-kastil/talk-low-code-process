using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PurchasingService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactTitle = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Region = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EMail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HomePage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupplierProducts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierProducts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupplierProducts_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "SupplierId", "Address", "City", "CompanyName", "ContactName", "ContactTitle", "Country", "EMail", "HomePage", "Phone", "PostalCode", "Region" },
                values: new object[,]
                {
                    { 1, "Graben 21", "Vienna", "Wiener Feinkost GmbH", "Anna Schmid", "Einkaufsleiterin", "Austria", "anna.schmid@wiener-feinkost.example", "https://wiener-feinkost.example", "+43 1 234 5678", "1010", "Wien" },
                    { 2, "Marienplatz 1", "Muenchen", "Muenchner Gewuerze GmbH", "Juergen Mueller", "Verkaufsleiter", "Germany", "info@muenchner-gewuerze.example", "https://muenchner-gewuerze.example", "+49 89 123456", "80331", "Bayern" },
                    { 3, "Via Dante 34", "Milano", "Antica Cucina S.r.l.", "Lucia Romano", "Responsabile Acquisti", "Italy", "info@anticacucina.example", "https://anticacucina.example", "+39 02 3456789", "20121", "Lombardia" },
                    { 4, "Sukhumvit Rd. 45", "Bangkok", "Bangkok Foods Co., Ltd.", "Somsak Chaiyawan", "Operations Manager", "Thailand", "somsak.chaiyawan@bangkokfoods.example", "https://bangkokfoods.example", "+66 2 123 4567", "10100", "" }
                });

            migrationBuilder.InsertData(
                table: "SupplierProducts",
                columns: new[] { "Id", "ProductName", "SupplierId" },
                values: new object[,]
                {
                    { 1, "Butter Chicken", 1 },
                    { 2, "Blini with Salmon", 1 },
                    { 3, "Wiener Schnitzel", 1 },
                    { 4, "Cevapcici", 1 },
                    { 5, "Germknoedel", 1 },
                    { 6, "Greek Salad", 1 },
                    { 7, "Spare Ribs", 1 },
                    { 8, "Falaffel with Humus.", 1 },
                    { 9, "Butter Chicken", 2 },
                    { 10, "Blini with Salmon", 2 },
                    { 11, "Wiener Schnitzel", 2 },
                    { 12, "Cevapcici", 2 },
                    { 13, "Germknoedel", 2 },
                    { 14, "Greek Salad", 2 },
                    { 15, "Spare Ribs", 2 },
                    { 16, "Falaffel with Humus.", 2 },
                    { 17, "Butter Chicken", 3 },
                    { 18, "Blini with Salmon", 3 },
                    { 19, "Wiener Schnitzel", 3 },
                    { 20, "Cevapcici", 3 },
                    { 21, "Germknoedel", 3 },
                    { 22, "Greek Salad", 3 },
                    { 23, "Spare Ribs", 3 },
                    { 24, "Falaffel with Humus.", 3 },
                    { 25, "Pad Ka Prao", 4 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_SupplierId",
                table: "Orders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierProducts_SupplierId",
                table: "SupplierProducts",
                column: "SupplierId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "SupplierProducts");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Suppliers");
        }
    }
}
