using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgoraEspacios.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNifToUsuario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Primero creo el campo NIF permitiendo null para poder rellenar usuarios antiguos
            migrationBuilder.AddColumn<string>(
                name: "Nif",
                table: "Usuarios",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: true);

            // Pongo los NIF de los usuarios que ya existen en mi base de datos local
            migrationBuilder.Sql("""
                UPDATE Usuarios SET Nif = '73018330T' WHERE Email = 'admin@agoraespacios.com';
                UPDATE Usuarios SET Nif = '93971827Z' WHERE Email = 'paula@agoraespacios.com';

                IF EXISTS (SELECT 1 FROM Usuarios WHERE Nif IS NULL OR LTRIM(RTRIM(Nif)) = '')
                    THROW 50001, 'No se puede completar la migracion: todos los usuarios existentes deben tener NIF.', 1;
                """);

            // Cuando ya todos tienen NIF, hago que sea obligatorio
            migrationBuilder.AlterColumn<string>(
                name: "Nif",
                table: "Usuarios",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9,
                oldNullable: true);

            // Este indice evita NIF repetidos
            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Nif",
                table: "Usuarios",
                column: "Nif",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Nif",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Nif",
                table: "Usuarios");
        }
    }
}
