using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OnlineMarket.DataAccess.Migrations
{
    public partial class AddFeedBack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CallBacks",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Contact = table.Column<string>(nullable: true),
                    CallTime = table.Column<DateTime>(nullable: false),
                    Marked = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallBacks", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CallBacks");
        }
    }
}
