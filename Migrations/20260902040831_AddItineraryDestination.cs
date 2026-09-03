using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KerlaVlogs.Migrations
{
    /// <inheritdoc />
    public partial class AddItineraryDestination : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DestinationId",
                table: "Itineraries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Itineraries_DestinationId",
                table: "Itineraries",
                column: "DestinationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Itineraries_Destinations_DestinationId",
                table: "Itineraries",
                column: "DestinationId",
                principalTable: "Destinations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Itineraries_Destinations_DestinationId",
                table: "Itineraries");

            migrationBuilder.DropIndex(
                name: "IX_Itineraries_DestinationId",
                table: "Itineraries");

            migrationBuilder.DropColumn(
                name: "DestinationId",
                table: "Itineraries");
        }
    }
}
