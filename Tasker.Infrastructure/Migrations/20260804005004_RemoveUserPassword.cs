using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tasker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUserPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "User"
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
         
        }
    }
}
