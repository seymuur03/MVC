using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PartialView.pustok.Migrations
{
    /// <inheritdoc />
    public partial class mig_FeaturesTblUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IconName",
                table: "Features",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IconName",
                table: "Features");
        }
    }
}
