using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mvcapp.Migrations
{
    /// <inheritdoc />
    public partial class LatestTicketDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TicketDetails",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TicketDetails");
        }
    }
}
