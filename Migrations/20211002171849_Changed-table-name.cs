using Microsoft.EntityFrameworkCore.Migrations;

namespace HotelListing.Migrations
{
    public partial class Changedtablename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Contries_CountryId",
                table: "Hotels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Contries",
                table: "Contries");

            migrationBuilder.RenameTable(
                name: "Contries",
                newName: "Countries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Countries",
                table: "Countries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Countries_CountryId",
                table: "Hotels",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Hotels_Countries_CountryId",
                table: "Hotels");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Countries",
                table: "Countries");

            migrationBuilder.RenameTable(
                name: "Countries",
                newName: "Contries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Contries",
                table: "Contries",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Contries_CountryId",
                table: "Hotels",
                column: "CountryId",
                principalTable: "Contries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
