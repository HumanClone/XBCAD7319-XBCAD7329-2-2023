using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcapp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTicketResponseTicketDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "TicketResponses");
            migrationBuilder.AddColumn<DateTime>(
                name: "date",
                table: "TicketResponses",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sender",
                table: "TicketResponses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CategoryName",
                table: "TicketDetails",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date",
                table: "TicketResponses");

            migrationBuilder.DropColumn(
                name: "sender",
                table: "TicketResponses");

            migrationBuilder.DropColumn(
                name: "CategoryName",
                table: "TicketDetails");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "TicketResponses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "CategoryName",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
