using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCenterService.Migrations
{
    /// <inheritdoc />
    public partial class FixDataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a1a1a1a1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEOj5Ctg3FfJcCEEro5nCDqgR3mj8f+kk6uxn2ScTJ6udrHMoX3q9Ctd40lA5DANMAA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a3a3a3a3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEDbjE27BhA0uI/QgBEUUtVBspzUdbifGsZ1QLzyq8JY2wOGfUsWeqW3CK4h2CxHH7g==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAENruk/YpnXy4Z1GSrXEpJdWhoh9+51wPTOvPYn05RxNDxxPebQu3Fp2NNnJ+o2KW6w==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b4b4b4b4-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEMtaTzTjFs+k3k5Ruid7DWFbmrbFajbfHM06pyqUKcxG/+Ww6k7TUHaJSJNlk9DjUw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEOACUTdkL1zgWmo4EK7aG/B7e8oMpdCk76OW2hrp38UIeVjO/qCJub6SKDNKwYg42w==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c5c5c5c5-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHY5/xtdThI3q5hQ1Dza+lTUXNZmeDKE8a7NfnsKrlLihzZ7S2Wn3/Q2PbPGubz39g==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("d4d4d4d4-dddd-dddd-dddd-dddddddddddd"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEJ8uDsiBljbN/7wDILGwA2Cal9aHv5zZO4X3qjWRfORtRBAJgQuPN4odVvSJCC57EQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEMknEXpgSsbEq2NIEFQXhE4yMtX1ThPbDCPC28woWDyQHaV1z55tgxeikpOJ9/GGvg==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEIsbyGgBv+C0ReDRnjKf99ad1Mhc9Yc2UplRuEClSjqsA1ehO0voO/pHG3powg0vNA==");

            migrationBuilder.UpdateData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 15, 57, 42, 266, DateTimeKind.Local).AddTicks(1526));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 1,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 26, 15, 57, 42, 266, DateTimeKind.Local).AddTicks(1503));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 26, 15, 57, 42, 266, DateTimeKind.Local).AddTicks(1506));

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 1,
                columns: new[] { "Description", "IncludeInChecklist", "Name" },
                values: new object[] { "Thay thế toàn bộ gói pin", true, "Thay thế Pin" });

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 2,
                columns: new[] { "Description", "IncludeInChecklist", "Name" },
                values: new object[] { "Kiểm tra và thay má phanh", true, "Kiểm tra Phanh" });

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 3,
                columns: new[] { "Description", "IncludeInChecklist", "Name" },
                values: new object[] { "Kiểm tra dung dịch và tản nhiệt", true, "Kiểm tra Hệ thống Làm mát" });

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Gói kiểm tra toàn diện xe", "Bảo dưỡng Tổng quát" });

            migrationBuilder.InsertData(
                table: "ServiceCatalog",
                columns: new[] { "ServiceID", "BasePrice", "Description", "DurationMinutes", "IncludeInChecklist", "Name" },
                values: new object[] { 5, 200000m, "Kiểm tra áp suất và độ mòn", 30, true, "Kiểm tra Lốp xe" });

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 15, 57, 42, 266, DateTimeKind.Local).AddTicks(1417));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a1a1a1a1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEMKPI0R1UkV3oTSyHOYpGk/1K6QpmdG5Ari6KjcGRTCmIiZknDm9OKJEoqXh7kgnMg==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a3a3a3a3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEJ1K10KHnHIL6mqsMCUBXenXWfR6oeHkayt5QT5YV5GQGFGCl/Ip00ArBnp3tRM/qA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEIXctqyf33DPgS60cXddSrsO6xl49ZFhUpv7mNcNgBINeZz8F1pGMnCFpSeUBpjMVw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b4b4b4b4-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAENOt7Vcw3wGEh/Bk11/m8xwxOmLnDBiqombDHfm0Lr3a0SGxepNjf5+88JnwDOcP7Q==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAECmemNSxqT4w/nuCVTLgngEXHORWZYkIrJGISGJMYA6dfkn9LJNQxS3LeFf5komLmg==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c5c5c5c5-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHr3wmB7DqLtrDrFbJMgemM5j2kC29F91cHstfHom6UhJZH7db38fM4w3gvNsyYp0A==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("d4d4d4d4-dddd-dddd-dddd-dddddddddddd"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEL3OkQuQtFLvMaZaMEPG1AYfrPkfHDuW2ga1hHDJlH7TPM/dBIgLskFIiK4cgHYyyw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEL3GXpXGvfcLb6CrW4LC5jUCeTsG34sm36KB9lsRyJmkvOMRTfWumr/cRadbXxe52A==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEEkRsQN2ooSd49rPmJeRfXWBoH0+HrZ5TgLS1h7yY7kd06P9lRvts7qGcsLmdlUgnw==");

            migrationBuilder.UpdateData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 15, 14, 25, 7, DateTimeKind.Local).AddTicks(9219));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 1,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 26, 15, 14, 25, 7, DateTimeKind.Local).AddTicks(9195));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 26, 15, 14, 25, 7, DateTimeKind.Local).AddTicks(9197));

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 1,
                columns: new[] { "Description", "IncludeInChecklist", "Name" },
                values: new object[] { "Replace entire battery pack", false, "Battery Replacement" });

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 2,
                columns: new[] { "Description", "IncludeInChecklist", "Name" },
                values: new object[] { "Inspect and replace brake pads if needed", false, "Brake Check" });

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 3,
                columns: new[] { "Description", "IncludeInChecklist", "Name" },
                values: new object[] { "Check coolant and thermal management", false, "Cooling System Check" });

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 4,
                columns: new[] { "Description", "Name" },
                values: new object[] { "Full vehicle health check", "General Inspection" });

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 15, 14, 25, 7, DateTimeKind.Local).AddTicks(9117));
        }
    }
}
