using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApplication_GameStoreIL.Migrations
{
    public partial class ADDInCategoryModelImgPathproperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "img_path",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "img_path",
                table: "Categories");
        }
    }
}
