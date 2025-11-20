using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCenterService.Migrations
{
    /// <inheritdoc />
    public partial class FixVietnameseNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ChecklistNote",
                table: "OrderService",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a1a1a1a1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAENda676cCdAIrREsYP9pg+Y2Tn2P4Jv697CpBdeBI+jaDQXwyfgLve/9Qqkt67ICDw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a3a3a3a3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAECEXhvlq4l7qjjKOz8bCeRhV3KCMtE9agsaM6GH6SSjlmG2VFIZGGqjB9/VdjDnmUw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAELkys18XpIqp1eT4iRsF58tPAWJ1F0Yqr0cCqixm8iUEjRm6IHGNDgBKzgFMZE648g==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b4b4b4b4-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEC41C7DX3ktzhkZHNP+jiYhKzv+/vPPLIimtmocJmd65duwaNca8mjblwuTp1uJa3g==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAED3B4yxSYMf1efAaM4EU9vuoiQ/CpzYhKxtbUa8rd8y8eR/z2w6Oa06eJVu5Sbrtrw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c5c5c5c5-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEClBQUb/DjsiLalS6SxXS7WMW2EilEwemeDWs3o+2XwkuMt3BtptbwthSI63ohC0aw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("d4d4d4d4-dddd-dddd-dddd-dddddddddddd"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEKBroz6YbH60VtVU8JhP4FK7UzGzJROkfQZyMbqCOopPMALFScOBIyN0ECa/snFPVA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEFbNXy8LNqzxpOk/kAy3D4ZEB3IFjxNmQQwA6PAlab+nvZpbo1q6Hk0PKYa1bUvn0A==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHt34B/AqEVks2iVddgYpCsNSHxtw3EHwTgNwIjbiHbzxFbDX6AdM9j7L9l2hvedbQ==");

            migrationBuilder.UpdateData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 21, 2, 16, 1, 950, DateTimeKind.Local).AddTicks(1857));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 1,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 21, 2, 16, 1, 950, DateTimeKind.Local).AddTicks(1836));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 21, 2, 16, 1, 950, DateTimeKind.Local).AddTicks(1838));

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 21, 2, 16, 1, 950, DateTimeKind.Local).AddTicks(1763));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ChecklistNote",
                table: "OrderService",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a1a1a1a1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEEEUCDN+AXBeOIqlMT2MH3UyH8qQZ2ZFwlYa0+pJKPx5R2EUDrFwMyk8SrZDjr2pDw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a3a3a3a3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEGjR2SOZnV1J6lQZ2lsmIByyn54ousmXvL0YRIvdJzhUptgvMMwQNPMWSKvuU49YYQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEK/vdI9MVUAbpqawPirvb3dZlffN1EwQCv/b0zpKQAKFcfuqHR5poGBRyW7Qd14TRQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b4b4b4b4-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAELvxFaYS1EV87pH/1YuL3i9GT21ZMnm3RNKpIokrx4iDFTXym0j38oObCplpLNEXHQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEJYBvgqC0GkMKpYl0RH/+A969e9bQD6kJ1W6OzlF7JeKwR9qdJ9CrvPkk/3JPqr/EQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c5c5c5c5-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEEcN9YOzwbLav8ifunLU3X+9uUu2tnFMai32lrbWKw5OJfKzhLpnTAsS4K/bztlnMA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("d4d4d4d4-dddd-dddd-dddd-dddddddddddd"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEJKmFRz5/rQduWd7ZCBBk7umkD3DUDzUVqtVhwLGn9mHXrU0QUE9whx0AQ32KXW06w==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEN+/vuaejn+tES3v8hs3Qn+5/udLA4Su3YyIbsCikQXLOG91qYWvQPN8QFAun2FB3g==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEEsjR3NG+DvOrrcR9q4zEo9FgvCL9CMTfza7yG3xCo6XMR7VtrhpeLFeRs0Yn6UVTw==");

            migrationBuilder.UpdateData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 12, 16, 2, 34, 814, DateTimeKind.Local).AddTicks(6781));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 1,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 12, 16, 2, 34, 814, DateTimeKind.Local).AddTicks(6730));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 12, 16, 2, 34, 814, DateTimeKind.Local).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 12, 16, 2, 34, 814, DateTimeKind.Local).AddTicks(6652));
        }
    }
}
