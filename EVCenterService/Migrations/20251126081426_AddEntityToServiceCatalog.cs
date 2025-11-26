using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCenterService.Migrations
{
    /// <inheritdoc />
    public partial class AddEntityToServiceCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IncludeInChecklist",
                table: "ServiceCatalog",
                type: "bit",
                nullable: false,
                defaultValue: false);

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
                column: "IncludeInChecklist",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 2,
                column: "IncludeInChecklist",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 3,
                column: "IncludeInChecklist",
                value: false);

            migrationBuilder.UpdateData(
                table: "ServiceCatalog",
                keyColumn: "ServiceID",
                keyValue: 4,
                column: "IncludeInChecklist",
                value: false);

            migrationBuilder.UpdateData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 26, 15, 14, 25, 7, DateTimeKind.Local).AddTicks(9117));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncludeInChecklist",
                table: "ServiceCatalog");

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
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2025, 11, 25, 16, 56, 9, 81, DateTimeKind.Local).AddTicks(6181));
        }
    }
}
