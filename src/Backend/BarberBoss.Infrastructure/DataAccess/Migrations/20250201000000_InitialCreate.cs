using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarberBoss.Infrastructure.DataAccess.Migrations;

/// <inheritdoc />
public partial class InitialCreate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterDatabase()
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Users",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false),
                Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                Email = table.Column<string>(type: "varchar(180)", maxLength: 180, nullable: false),
                PasswordHash = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                Role = table.Column<int>(type: "int", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Users", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateTable(
            name: "Billings",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "char(36)", nullable: false),
                Date = table.Column<DateOnly>(type: "date", nullable: false),
                BarberName = table.Column<string>(type: "varchar(80)", maxLength: 80, nullable: false),
                ClientName = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                ServiceName = table.Column<string>(type: "varchar(120)", maxLength: 120, nullable: false),
                Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                PaymentMethod = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<int>(type: "int", nullable: false),
                Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                UserId = table.Column<Guid>(type: "char(36)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Billings", x => x.Id);
                table.ForeignKey(
                    name: "FK_Billings_Users_UserId",
                    column: x => x.UserId,
                    principalTable: "Users",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_Users_Email",
            table: "Users",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Billings_Date",
            table: "Billings",
            column: "Date");

        migrationBuilder.CreateIndex(
            name: "IX_Billings_Status",
            table: "Billings",
            column: "Status");

        migrationBuilder.CreateIndex(
            name: "IX_Billings_UserId",
            table: "Billings",
            column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Billings");
        migrationBuilder.DropTable(name: "Users");
    }
}
