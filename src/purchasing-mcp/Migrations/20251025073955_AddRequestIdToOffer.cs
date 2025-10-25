﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PurchasingService.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestIdToOffer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestId",
                table: "Offers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestId",
                table: "Offers");
        }
    }
}
