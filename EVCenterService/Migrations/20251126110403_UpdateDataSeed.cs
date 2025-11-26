using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCenterService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 5);

            migrationBuilder.AddColumn<string>(
                name: "RequiredCertification",
                table: "ServiceCatalog",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a1a1a1a1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEASmDqVKMiEHmAacXNJNxBFRLBssar4em2uMiOKRF1t2hQ4esv0mqUarJ4mvrS+QQA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a3a3a3a3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEMQXFBUVgzQKGnyeNL8K4nG3cNk/Y5MHZfEuJ6sW3/mcVhiAhSn+FhSYRn644gj1eQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEMh+fofojY+dkqRkGg2avFbndbkLHi7f16flDci9Ajq4W+COizf4Z+FagaR2LgZxiw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b4b4b4b4-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAELlqbolKCVrMoG4VbHaiiKGSdkYO2erwLO7U227ASjP+5+4mvX0KL13598BfgY44BQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEH6DNVasJ5pamSjkFqDj3vtPt1oI006A3Co/MX3PmnU4c8AfSVPIry/yiteeHPPmDA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c5c5c5c5-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEJY0cyexEckFX6yvTaic9QPt8xXjzGr2c6/v1cbbYdDC12hzncO9A7EGSuJwzSub2g==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("d4d4d4d4-dddd-dddd-dddd-dddddddddddd"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEC3WpxdudpKldmXN4XVBzlrOWNcR3RQa8pN51PG553caO93myq7kBv31LxSGJSDmWA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEL8TJ7CwDBZudXfBZJVfWIjBPQj/4aQZ9tYopeyFtpR2g7CwTroISZKfGFi41EgtGw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEJ12gu23SxR8amQ6NV2wvolPB7ldWv/30Z54nyZpraD5lRwr1Ko0/ikJ9AQ4e/49Iw==");

            migrationBuilder.UpdateData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 18, 4, 3, 215, DateTimeKind.Local).AddTicks(6321));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 1,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 26, 18, 4, 3, 215, DateTimeKind.Local).AddTicks(6299));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 26, 18, 4, 3, 215, DateTimeKind.Local).AddTicks(6301));

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 1,
                column: "RequiredCertification",
                value: "Battery System Certified");

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 2,
                column: "RequiredCertification",
                value: "Brake System Certified");

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 3,
                column: "RequiredCertification",
                value: "Thermal & Cooling System Certified");

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 4,
                column: "RequiredCertification",
                value: "General Inspection Certified");

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 18, 4, 3, 215, DateTimeKind.Local).AddTicks(6226));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiredCertification",
                table: "ServiceCatalog");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a1a1a1a1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAELV1us4OlzpO4+HG0PjsxC0GXZdHf5ArQ/L5zBvh9rPdls/qtZ61qah7QSXNTLPvpQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a3a3a3a3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEONdZPrQFtrrSaF2r6jDD0R2GA4FgwSAST5KIPAWzyVpB/2G/VCyAd6VVlaz+Huh0w==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHQf6azJWUsRxxLuzz6/KWXbfqO7oD9qmSpXBXGg5j9a233dy5hi+RoBAGBXHqo+cQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b4b4b4b4-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHaM9LLrBe1/xXYxhJV7fNrEMSC9dRvDAC97vx+0fLllXo9LZjw5FHJJTKrqSmQ0Gw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEIu5TINK2/xmopLvfIIiJWkUunzrq2zbGawZ9MQTHR7EXZyb0aKBmAoX30LeaMSOUg==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c5c5c5c5-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEBb6IkpggZSvLBQYQ5n53CKZhzfcWhrD4xJcioyKvM8eRFp2DZbhEMUlYAChNQDGOA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("d4d4d4d4-dddd-dddd-dddd-dddddddddddd"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEEi5Ym6/tnanmbIBHzaH8/9EJFXGeuJHV3wMr2zi9t3PjZhXWa8LuAuLbOF3uCcwjQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEEw9C8ulp31TmdfsDNTsL+k2iPovA5opXoEywP05JATA50vUxHSmPdMgxkAtnKFB7Q==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEA9zCWQxYYKsYYg6drVdxtw2KwEinyrCpTHbG/l4TfsT2j8daSiAclqMuMckTQG/Rw==");

            migrationBuilder.UpdateData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 17, 15, 22, 602, DateTimeKind.Local).AddTicks(1835));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 1,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 26, 17, 15, 22, 602, DateTimeKind.Local).AddTicks(1792));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 26, 17, 15, 22, 602, DateTimeKind.Local).AddTicks(1794));

            migrationBuilder.InsertData(
                table: "ServiceCatalog",
                columns: new[] { "ServiceID", "BasePrice", "Description", "DurationMinutes", "IncludeInChecklist", "Name" },
                values: new object[] { 5, 200000m, "Kiểm tra áp suất và độ mòn", 30, true, "Kiểm tra Lốp xe" });

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 17, 15, 22, 602, DateTimeKind.Local).AddTicks(1724));
        }
    }
}
