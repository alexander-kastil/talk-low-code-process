using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingService.Migrations
{
    /// <inheritdoc />
    public partial class AddOfferStatusAndOrderFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Offers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Offers");
        }
    }
}
