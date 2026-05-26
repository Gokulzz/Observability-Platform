using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelemetryCollector.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLatencyBucket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EndpointLatencyBuckets",
                columns: table => new
                {
                    ServiceName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    EndPoint = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    WindowStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BucketUpperBoundMs = table.Column<int>(type: "int", nullable: false),
                    RequestCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndpointLatencyBuckets", x => new { x.ServiceName, x.EndPoint, x.WindowStart, x.BucketUpperBoundMs });
                });

            migrationBuilder.CreateIndex(
                name: "IX_EndpointLatencyBuckets_ServiceName_EndPoint_WindowStart",
                table: "EndpointLatencyBuckets",
                columns: new[] { "ServiceName", "EndPoint", "WindowStart" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EndpointLatencyBuckets");
        }
    }
}
