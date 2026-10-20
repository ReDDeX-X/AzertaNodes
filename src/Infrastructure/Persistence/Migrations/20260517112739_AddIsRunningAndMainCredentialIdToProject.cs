using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzertaNodes.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddIsRunningAndMainCredentialIdToProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRunning",
                table: "ProjectSet",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "MainCredentialId",
                table: "ProjectSet",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsRunning",
                table: "ProjectSet");

            migrationBuilder.DropColumn(
                name: "MainCredentialId",
                table: "ProjectSet");
        }
    }
}
