using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCenterService.Migrations
{
    /// <inheritdoc />
    public partial class FixServiceDataValues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 17, 15, 22, 602, DateTimeKind.Local).AddTicks(1724));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a1a1a1a1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEBrD+2NJdBr2FH6Cwj3pOeBOcvfe/rIs+9A2TudGXvpxGHdSWAmLL46aSbn/EPhjRw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a3a3a3a3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHg+CF8XhyTGyffn0V9nj1M09eo97k7i2kggdQDn1mcMVVvFG4wClnyr2NpseZpinw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEEuzXA0bPC8h72SrAd74UVUTZS+0AiJXLBZzpy+MH8QvP8CpNmiD3/c2WRUcjRBhNQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b4b4b4b4-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEI9BfiAG1elYpjpivBOCPjgKNtwGJKB8rfkn7Eumhyzei8dVR8KvQ/NAfmcHZPYRJw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAENRPel+ud4W11+lXaKKfOlnHtbYgqw+xRPRvIhebYmDU645rv7arU3Vvu2C+bRsvIA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c5c5c5c5-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEOvX8KvBjXYB9nq3kGniwOCYIBS+nrDynMX3OBJ8hyN6NzkAMgWoBXGvM6TVEQc3qA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("d4d4d4d4-dddd-dddd-dddd-dddddddddddd"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAENPaUc63Std0wpcxEphWWnvhJOAVsVQquS+PCHOcOW/7EMyYL4zc/Mc182rhAqaC2g==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEIUMVDVH/FhX9uQIqEjlokXWOibVUN6HYWEf8Q8gP2Vp5VrLxpHh9olGyoOlJGIWkw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEI/DpRTOnLOtUzSXKEqHQtu8u9p2JyJvywqf2pxWgtEp+PggX2yB1FLt3N0RLcF4sg==");

            migrationBuilder.UpdateData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 16, 19, 10, 629, DateTimeKind.Local).AddTicks(1579));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 1,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 26, 16, 19, 10, 629, DateTimeKind.Local).AddTicks(1555));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 26, 16, 19, 10, 629, DateTimeKind.Local).AddTicks(1558));

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 16, 19, 10, 629, DateTimeKind.Local).AddTicks(1465));
        }
    }
}
