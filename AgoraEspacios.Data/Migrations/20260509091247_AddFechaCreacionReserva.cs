using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgoraEspacios.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFechaCreacionReserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<System.DateTime>(
                name: "FechaCreacion",
                table: "Reservas",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaCreacion",
                table: "Reservas");
        }
    }
}
