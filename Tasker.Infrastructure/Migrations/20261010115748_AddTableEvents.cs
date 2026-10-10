using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTableEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(name: "Events", columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                GoogleEventId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                DateStart = table.Column<DateTime>(type: "datetime2", nullable: false ),
                DateEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                GoogleHtmlLink = table.Column<string>(type: "nvarchar(max)", nullable: true),
                SyncStatus = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type : "datetime2", nullable: false)
            },
                constraints : table =>
                {
                    table.PrimaryKey("PK_Events_UserId", x => x.Id);
                        table.ForeignKey( name: "FK_Events_User_UserId",
                            column: x => x.UserId,
                            principalTable: "User",
                            principalColumn: "Id",
                            onDelete: ReferentialAction.Cascade);
                }
                
            );
            
            migrationBuilder.CreateIndex(
                name: "IX_Events_UserId",
                table: "Events",
                column: "UserId");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
