using Microsoft.EntityFrameworkCore.Migrations;
using Tasker.Domain.Entities;

#nullable disable

namespace Tasker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AlterColumnCreatedAtUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "User",
                newName: "CreatedAt"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
