using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCenterService.Migrations
{
    /// <inheritdoc />
    public partial class entitiesToVehicles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a1a1a1a1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEKY2dgF4VNPd4Qd8a2SDsFH3bivFwAGSgBAkpTsdyEmWeZNZxzdM/kRKvsP74fSSiA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a3a3a3a3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEB3rEFrLRUYpOz1iDvWfNP9y/d7kz9nSEwL/UFRRlhdWyh33icD/4Vjk//X0Lnm/DA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEGgARbnEgK18cvGa5lrcfc6furObiDwXNVp9c3E0ZLf7Rn1Hjb5io5/RAyaXSQQnEw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b4b4b4b4-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEC/TMqEdwKlc9XeTCR+5QafD6pKa4NWSjegvCKpEEfF1Og0GxM7+/61zwNlceUooRw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEGZz42eKVHAeWVeXjAmd1iO5ZexxgrYDWNmDI2bae79kAzAA/gijsW+Em+Yf3gu8iA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c5c5c5c5-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHnqBk1gH74ZefwhR/I6LOQniw4P2aM0fOJUTlVlcY2w5cCra0BOBFb9K/ao7SuHvQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("d4d4d4d4-dddd-dddd-dddd-dddddddddddd"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEOwBwJy6oCr174135i8X67kl3Hq8MwcsvsNR0qmRdMRy91wBQ6GUnRlVstNByaruag==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEKPdqU5sfT1RddHYQEKZqUoQjaJDTzChJg7zP8N8ulvGNxDRqSfBH1qxRf7MXxr2Wg==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAECxa6gQ1j5Ap2TEnPUYg61CO22yloMgOKFHfjQNYKXEipwPiLntLk1V0AKdFdOBh0Q==");

            migrationBuilder.UpdateData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 25, 15, 55, 9, 208, DateTimeKind.Local).AddTicks(8139));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 1,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 25, 15, 55, 9, 208, DateTimeKind.Local).AddTicks(8118));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 25, 15, 55, 9, 208, DateTimeKind.Local).AddTicks(8120));

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 25, 15, 55, 9, 208, DateTimeKind.Local).AddTicks(8056));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
