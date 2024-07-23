using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace game_tools.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Equipments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Place = table.Column<string>(type: "TEXT", nullable: true),
                    Strength = table.Column<string>(type: "TEXT", nullable: true),
                    Armor = table.Column<string>(type: "TEXT", nullable: true),
                    MaximumHealth = table.Column<string>(type: "TEXT", nullable: true),
                    CriticalChance = table.Column<string>(type: "TEXT", nullable: true),
                    CriticalDamage = table.Column<string>(type: "TEXT", nullable: true),
                    SpecialEffectName = table.Column<string>(type: "TEXT", nullable: true),
                    SpecialEffectDescription = table.Column<string>(type: "TEXT", nullable: true),
                    AmazingPower = table.Column<string>(type: "TEXT", nullable: true),
                    Needlevel = table.Column<string>(type: "TEXT", nullable: true),
                    Traded = table.Column<string>(type: "TEXT", nullable: true),
                    ImagePath = table.Column<string>(type: "TEXT", nullable: false),
                    ItemIndex = table.Column<short>(type: "INTEGER", nullable: true),
                    AllElementResistance = table.Column<string>(type: "TEXT", nullable: true),
                    AttackSpeed = table.Column<string>(type: "TEXT", nullable: true),
                    ResourceReduction = table.Column<string>(type: "TEXT", nullable: true),
                    Intelligence = table.Column<string>(type: "TEXT", nullable: true),
                    CriticalHitChance = table.Column<string>(type: "TEXT", nullable: true),
                    IncreasedDamageTaken = table.Column<string>(type: "TEXT", nullable: true),
                    DamageSuppression = table.Column<string>(type: "TEXT", nullable: true),
                    DamageOverTime = table.Column<string>(type: "TEXT", nullable: true),
                    CDR = table.Column<string>(type: "TEXT", nullable: true),
                    Damage = table.Column<string>(type: "TEXT", nullable: true),
                    MovementSpeed = table.Column<string>(type: "TEXT", nullable: true),
                    TaskBatch = table.Column<int>(type: "INTEGER", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipments", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Equipments");
        }
    }
}
