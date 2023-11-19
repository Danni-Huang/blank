using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace QuotesApp.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    QuoteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModified = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.QuoteId);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    TagId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.TagId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Like",
                columns: table => new
                {
                    LikeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    QuoteId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Like", x => x.LikeId);
                    table.ForeignKey(
                        name: "FK_Like_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "QuoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TagAssignments",
                columns: table => new
                {
                    QuoteId = table.Column<int>(type: "int", nullable: false),
                    TagId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagAssignments", x => new { x.QuoteId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TagAssignments_Quotes_QuoteId",
                        column: x => x.QuoteId,
                        principalTable: "Quotes",
                        principalColumn: "QuoteId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagAssignments_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "TagId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Quotes",
                columns: new[] { "QuoteId", "Author", "Content", "LastModified" },
                values: new object[,]
                {
                    { 1, "John Wooden", "Things work out best for those who make the best of how things work out.", new DateTime(2023, 11, 18, 20, 52, 34, 383, DateTimeKind.Local).AddTicks(3486) },
                    { 2, "Jim Rohn", "If you are not willing to risk the usual you will have to settle for the ordinary.", new DateTime(2023, 11, 18, 20, 52, 34, 383, DateTimeKind.Local).AddTicks(3607) },
                    { 3, "Walt Disney", "All our dreams can come true if we have the courage to pursue them.", new DateTime(2023, 11, 18, 20, 52, 34, 383, DateTimeKind.Local).AddTicks(3610) },
                    { 4, "Winston Churchill", "Success is walking from failure to failure with no loss of enthusiasm.", new DateTime(2023, 11, 18, 20, 52, 34, 383, DateTimeKind.Local).AddTicks(3613) },
                    { 5, "Proverb", "Just when the caterpillar thought the world was ending, he turned into a butterfly.", new DateTime(2023, 11, 18, 20, 52, 34, 383, DateTimeKind.Local).AddTicks(3615) },
                    { 6, "Chris Grosser", "Opportunities don't happen, you create them.", new DateTime(2023, 11, 18, 20, 52, 34, 383, DateTimeKind.Local).AddTicks(3617) },
                    { 7, "Winston Churchill", "If you're going through hell keep going.", new DateTime(2023, 11, 18, 20, 52, 34, 383, DateTimeKind.Local).AddTicks(3620) },
                    { 8, "Anonymous", "Don't raise your voice, improve your argument.", new DateTime(2023, 11, 18, 20, 52, 34, 383, DateTimeKind.Local).AddTicks(3622) },
                    { 9, "Anonymous", "Do one thing every day that scares you.", new DateTime(2023, 11, 18, 20, 52, 34, 383, DateTimeKind.Local).AddTicks(3625) },
                    { 10, "Lolly Daskal", "Life is not about finding yourself. Life is about creating yourself.", new DateTime(2023, 11, 18, 20, 52, 34, 383, DateTimeKind.Local).AddTicks(3627) }
                });

            migrationBuilder.InsertData(
                table: "Tags",
                columns: new[] { "TagId", "Name" },
                values: new object[,]
                {
                    { 1, "Funny" },
                    { 2, "Philosophical" },
                    { 3, "Serious" },
                    { 4, "Educational" },
                    { 5, "Motivational" }
                });

            migrationBuilder.InsertData(
                table: "Like",
                columns: new[] { "LikeId", "QuoteId", "UserId" },
                values: new object[,]
                {
                    { 1, 8, 123 },
                    { 2, 8, 123 },
                    { 3, 8, 123 },
                    { 4, 8, 123 },
                    { 5, 2, 123 },
                    { 6, 2, 123 },
                    { 7, 4, 123 }
                });

            migrationBuilder.InsertData(
                table: "TagAssignments",
                columns: new[] { "QuoteId", "TagId" },
                values: new object[,]
                {
                    { 2, 2 },
                    { 3, 5 },
                    { 4, 3 },
                    { 5, 4 },
                    { 8, 1 },
                    { 9, 3 },
                    { 10, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Like_QuoteId",
                table: "Like",
                column: "QuoteId");

            migrationBuilder.CreateIndex(
                name: "IX_TagAssignments_TagId",
                table: "TagAssignments",
                column: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Like");

            migrationBuilder.DropTable(
                name: "TagAssignments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "Tags");
        }
    }
}
