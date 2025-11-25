using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EVCenterService.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleMaintenanceFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaintenanceCount",
                table: "Vehicle",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "NextMaintenanceDate",
                table: "Vehicle",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NextMaintenanceMileage",
                table: "Vehicle",
                type: "decimal(10,2)",
                nullable: true);

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

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "VehicleID",
                keyValue: 1,
                columns: new[] { "MaintenanceCount", "NextMaintenanceDate", "NextMaintenanceMileage" },
                values: new object[] { 0, null, null });

            migrationBuilder.UpdateData(
                table: "Vehicle",
                keyColumn: "VehicleID",
                keyValue: 2,
                columns: new[] { "MaintenanceCount", "NextMaintenanceDate", "NextMaintenanceMileage" },
                values: new object[] { 0, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaintenanceCount",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "NextMaintenanceDate",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "NextMaintenanceMileage",
                table: "Vehicle");

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
    }
}
