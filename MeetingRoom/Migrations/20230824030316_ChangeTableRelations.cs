using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MeetingRoom.Migrations
{
    /// <inheritdoc />
    public partial class ChangeTableRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookedRooms_Rooms_RoomId",
                table: "BookedRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedRooms_Users_UserId",
                table: "BookedRooms");

            migrationBuilder.AddForeignKey(
                name: "FK_BookedRooms_Rooms_UserId",
                table: "BookedRooms",
                column: "UserId",
                principalTable: "Rooms",
                principalColumn: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookedRooms_Users_RoomId",
                table: "BookedRooms",
                column: "RoomId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookedRooms_Rooms_UserId",
                table: "BookedRooms");

            migrationBuilder.DropForeignKey(
                name: "FK_BookedRooms_Users_RoomId",
                table: "BookedRooms");

            migrationBuilder.AddForeignKey(
                name: "FK_BookedRooms_Rooms_RoomId",
                table: "BookedRooms",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "RoomId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookedRooms_Users_UserId",
                table: "BookedRooms",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
