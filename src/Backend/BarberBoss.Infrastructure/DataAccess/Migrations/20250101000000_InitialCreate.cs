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
                UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Billings", x => x.Id);
            })
            .Annotation("MySql:CharSet", "utf8mb4");

        migrationBuilder.CreateIndex(
            name: "IX_Billings_Date",
            table: "Billings",
            column: "Date");

        migrationBuilder.CreateIndex(
            name: "IX_Billings_Status",
            table: "Billings",
            column: "Status");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Billings");
    }
}
