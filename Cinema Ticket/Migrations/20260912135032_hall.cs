using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinema_Ticket.Migrations
{
    /// <inheritdoc />
    public partial class hall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CinemaId",
                table: "Hall",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Hall_CinemaId",
                table: "Hall",
                column: "CinemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Hall_Cinemas_CinemaId",
                table: "Hall",
                column: "CinemaId",
                principalTable: "Cinemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hall_Cinemas_CinemaId",
                table: "Hall");

            migrationBuilder.DropIndex(
                name: "IX_Hall_CinemaId",
                table: "Hall");

            migrationBuilder.DropColumn(
                name: "CinemaId",
                table: "Hall");
        }
    }
}