using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LOI.Domain.NFXFileSystemEventWatcherManager.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NFXFileSystemWorkers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Machine = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsConnected = table.Column<bool>(type: "bit", nullable: false),
                    Connection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdateUTC = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NFXFileSystemWorkers", x => x.Id);
                    table.UniqueConstraint("AK_NFXFileSystemWorkers_Machine", x => x.Machine);
                });

            migrationBuilder.CreateTable(
                name: "WatchFolders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Machine = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Filter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastFileEventUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateModifiedUTC = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DateAddedUTC = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchFolders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchFolders_NFXFileSystemWorkers_Machine",
                        column: x => x.Machine,
                        principalTable: "NFXFileSystemWorkers",
                        principalColumn: "Machine",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WatchFolders_Machine",
                table: "WatchFolders",
                column: "Machine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WatchFolders");

            migrationBuilder.DropTable(
                name: "NFXFileSystemWorkers");
        }
    }
}
