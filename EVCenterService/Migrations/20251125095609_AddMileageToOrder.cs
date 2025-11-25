using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCenterService.Migrations
{
    /// <inheritdoc />
    public partial class AddMileageToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MileageAtService",
                table: "OrderService",
                type: "decimal(10,2)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a1a1a1a1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEFDNQCvkUHDnVjcOrVdocoD58XwlUlRnEw6hMLHNPG4JXJiWdLZoAfgjoX7I21i0NA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a3a3a3a3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAENRHnTY+RYBaQ1GeIjBd2ZhgXb1RmyWN3gzNEi1/FHQhLq+p+ruQtmyA3TH9HtvUSw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAENWpGIHQlp/YUvwf6jcjjr1SiB0Vp6t7kiqOpq7xHZxggzgU0ZqAhb9mlAFtEL3EAg==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b4b4b4b4-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAELBBCrKuH+dnKjDL85W5N8iGLXruCwHELA44gDh0GiY90kq67VuZshjewkE00gDs+Q==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEGdEralKeZENTsnO6ZFujLv9MABo22jqbBK6dkl2aMB17/mG2ZYte+mJHXDM39f0QA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c5c5c5c5-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEIKZUKfAQYewj0hnnovx21j3+po5BMq3CUQkqzzW0pwbDqQBKq5kbCJ7NpGTj22E8g==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("d4d4d4d4-dddd-dddd-dddd-dddddddddddd"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEGLTzvoDZOsF3pFd7dlNsKYSg97f3QmkYxYxwl3FDBi91n6k19cFihLsq2KVppizHw==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEA9h8kwT4veXfQvBEZtLlN2q0B7re16LDU9CWhASvXfdpb9jLxGIaTcJzkUywefoVg==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEFB2V4SRs44EJuNXTIsbxzKPy59NHQAW4weQ8f0BHH4dYGtCvnUFbT1uW2ihX/70XQ==");

            migrationBuilder.UpdateData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 25, 16, 56, 9, 81, DateTimeKind.Local).AddTicks(6283));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 1,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 25, 16, 56, 9, 81, DateTimeKind.Local).AddTicks(6257));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 25, 16, 56, 9, 81, DateTimeKind.Local).AddTicks(6259));

            migrationBuilder.UpdateData(
                table: "OrderService",
                keyColumn: "OrderID",
                keyValue: 1,
                column: "MileageAtService",
                value: null);

            migrationBuilder.UpdateData(
                table: "OrderService",
                keyColumn: "OrderID",
                keyValue: 2,
                column: "MileageAtService",
                value: null);

            migrationBuilder.UpdateData(
                table: "OrderService",
                keyColumn: "OrderID",
                keyValue: 3,
                column: "MileageAtService",
                value: null);

            migrationBuilder.UpdateData(
                table: "OrderService",
                keyColumn: "OrderID",
                keyValue: 4,
                column: "MileageAtService",
                value: null);

            migrationBuilder.UpdateData(
                table: "OrderService",
                keyColumn: "OrderID",
                keyValue: 5,
                column: "MileageAtService",
                value: null);

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 25, 16, 56, 9, 81, DateTimeKind.Local).AddTicks(6181));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MileageAtService",
                table: "OrderService");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a1a1a1a1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAENVTY9XIFMULlbjq6Rx2m438Xx2wvigSzkZz+O2N6l+6VVpSuy/f9qma2eOMKCc2ag==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a3a3a3a3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEF5bFk//W4foyYKyntWTMiiXPbhkeLnq2BumzPxSYE7cZL3bxeq9IxBNPGyylElkQA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEIEENCziyirrIscNQGSOF21tnkTGK8mcXakV1RxNwo+zq3UeDdDjEWI6F00AiIV+CA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b4b4b4b4-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEDbUKJ4i/6dczVrJICImmk+IYV3vEiFt9swHq4TuyQfSvDILtLXKm/SMgKVJgEnwYg==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEHX/SWTE2WvJuIlVIiUsflswyWQWcsrVo1wh//n9zQqxjgvU4OiVLOt3zun4KediXQ==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("c5c5c5c5-cccc-cccc-cccc-cccccccccccc"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEDaOXHu7Ivyd1NLMwOtDfhJoNHwwvdzi1PfhV76M4n4hkcPycojfmD+bW7X7q3+QpA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("d4d4d4d4-dddd-dddd-dddd-dddddddddddd"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEMR3B70O8Dwp7O0+Cf5Z51EzIwVZH1ccTeXsbasYYuc4IhO7awEE0p3HOs6z9dzYYA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEGRcDEU1FStF/F8orZ7SfFZJd22//LyDJkqtbi1ke0TNMGJ0KTVuF/p18jJMhd5hTA==");

            migrationBuilder.UpdateData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"),
                column: "Password",
                value: "AQAAAAIAAYagAAAAEBa6ExtuJuRFdwFJED0VImm4Js5a3EvpRdcIU/ktouXW6U03Fdq3l9+oomoZg6R2ow==");

            migrationBuilder.UpdateData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 11, 25, 16, 25, 48, 398, DateTimeKind.Local).AddTicks(9775));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 1,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 25, 16, 25, 48, 398, DateTimeKind.Local).AddTicks(9751));

            migrationBuilder.UpdateData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 2,
                column: "TriggerDate",
                value: new DateTime(2025, 11, 25, 16, 25, 48, 398, DateTimeKind.Local).AddTicks(9753));

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 25, 16, 25, 48, 398, DateTimeKind.Local).AddTicks(9655));
        }
    }
}
