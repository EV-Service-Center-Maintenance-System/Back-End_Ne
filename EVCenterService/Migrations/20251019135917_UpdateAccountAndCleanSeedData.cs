using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EVCenterService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountAndCleanSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("85b2a152-e189-42bc-bf35-6bc61fdd2295"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("cbafbbfa-4088-40fc-b38f-86d8ebe3b15e"));

            migrationBuilder.DeleteData(
                table: "Feedback",
                keyColumn: "FeedbackID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Invoice",
                keyColumn: "InvoiceID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Notification",
                keyColumn: "NotificationID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderDetail",
                keyColumn: "OrderDetailID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderDetail",
                keyColumn: "OrderDetailID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "OrderDetail",
                keyColumn: "OrderDetailID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "PartsUsed",
                keyColumn: "UsageID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "PartsUsed",
                keyColumn: "UsageID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Slot",
                keyColumn: "SlotID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Slot",
                keyColumn: "SlotID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Storage",
                keyColumn: "StorageID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Storage",
                keyColumn: "StorageID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Storage",
                keyColumn: "StorageID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Storage",
                keyColumn: "StorageID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SubscriptionPlan",
                keyColumn: "PlanID",
                keyValue: new Guid("b4d0b3bd-dda4-4032-9d6a-da64745aa38e"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("4cad7706-115f-4b35-a0c5-23e3ac3005f5"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("d2cc8dfd-b5fd-43a1-9c9d-707f9beda2cd"));

            migrationBuilder.DeleteData(
                table: "OrderService",
                keyColumn: "OrderID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "OrderService",
                keyColumn: "OrderID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Subscription",
                keyColumn: "SubscriptionID",
                keyValue: new Guid("4ca95637-b9e9-48fd-8019-845e35a79543"));

            migrationBuilder.DeleteData(
                table: "SubscriptionPlan",
                keyColumn: "PlanID",
                keyValue: new Guid("fd490afc-90cc-4bf4-90ae-adfacacd2fcd"));

            migrationBuilder.DeleteData(
                table: "Vehicle",
                keyColumn: "VehicleID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Vehicle",
                keyColumn: "VehicleID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("56755807-5aff-4e3e-9e69-7108020cb8cf"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("901e7662-d51b-42f0-8826-53f100caf68a"));

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Account",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "UserID", "Certification", "Email", "FullName", "Password", "Phone", "Role", "Status" },
                values: new object[,]
                {
                    { new Guid("1a34911e-98d4-45f1-a2f9-a6074134b16c"), null, "staff@gmail.com", "Phan Anh C", "hashed_password_placeholder", "0906000006", "Staff", "Active" },
                    { new Guid("4efbf8b8-8679-405f-904e-d5f24545accb"), null, "user2@gmail.com", "Do Thi E", "hashed_password_placeholder", "0905000005", "Customer", "Active" },
                    { new Guid("65ca8a9a-515e-4d15-a5f8-d51a4d9bcc96"), "EV Maintenance Level 1", "tech1@gmail.com", "Tran Van B", "hashed_password_placeholder", "0902000002", "Technician", "Active" },
                    { new Guid("8a46d62f-b68c-4b28-9ac3-ce30cb1c367e"), null, "admin@gmail.com", "Admin", "hashed_password_placeholder", "0901000001", "Admin", "Active" },
                    { new Guid("a97493ff-192d-4ba1-9c37-4d909fbc0cee"), "EV Battery Specialist", "tech2@gmail.com", "Le Thi C", "hashed_password_placeholder", "0903000003", "Technician", "Active" },
                    { new Guid("ddc47ff3-e23e-4e23-b064-d7803ae67014"), null, "user1@gmail.com", "Pham Van D", "hashed_password_placeholder", "0904000004", "Customer", "Active" }
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlan",
                columns: new[] { "PlanID", "Benefits", "Code", "DurationDays", "IsActive", "Name", "PriceVND" },
                values: new object[,]
                {
                    { new Guid("5f797d50-4dca-4d58-abd6-d6ace23c923a"), "1 free inspection/month", "BASIC", 30, true, "Basic Care", 499000m },
                    { new Guid("62fc25c4-c7c8-4561-a6a5-4bceaa309cd6"), "Priority booking, 3 free inspections", "PREMIUM", 90, true, "Premium Care", 999000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("1a34911e-98d4-45f1-a2f9-a6074134b16c"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("4efbf8b8-8679-405f-904e-d5f24545accb"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("65ca8a9a-515e-4d15-a5f8-d51a4d9bcc96"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("8a46d62f-b68c-4b28-9ac3-ce30cb1c367e"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("a97493ff-192d-4ba1-9c37-4d909fbc0cee"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("ddc47ff3-e23e-4e23-b064-d7803ae67014"));

            migrationBuilder.DeleteData(
                table: "SubscriptionPlan",
                keyColumn: "PlanID",
                keyValue: new Guid("5f797d50-4dca-4d58-abd6-d6ace23c923a"));

            migrationBuilder.DeleteData(
                table: "SubscriptionPlan",
                keyColumn: "PlanID",
                keyValue: new Guid("62fc25c4-c7c8-4561-a6a5-4bceaa309cd6"));

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Account");

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "UserID", "Certification", "Email", "FullName", "Password", "Phone", "Role" },
                values: new object[,]
                {
                    { new Guid("4cad7706-115f-4b35-a0c5-23e3ac3005f5"), "EV Maintenance Level 1", "tech1@gmail.com", "Tran Van B", "hashed_password_placeholder", "0902000002", "Technician" },
                    { new Guid("56755807-5aff-4e3e-9e69-7108020cb8cf"), null, "user2@gmail.com", "Do Thi E", "hashed_password_placeholder", "0905000005", "Customer" },
                    { new Guid("85b2a152-e189-42bc-bf35-6bc61fdd2295"), null, "admin@gmail.com", "Admin", "hashed_password_placeholder", "0901000001", "Admin" },
                    { new Guid("901e7662-d51b-42f0-8826-53f100caf68a"), null, "user1@gmail.com", "Pham Van D", "hashed_password_placeholder", "0904000004", "Customer" },
                    { new Guid("cbafbbfa-4088-40fc-b38f-86d8ebe3b15e"), null, "staff@gmail.com", "Phan Anh C", "hashed_password_placeholder", "0906000006", "Staff" },
                    { new Guid("d2cc8dfd-b5fd-43a1-9c9d-707f9beda2cd"), "EV Battery Specialist", "tech2@gmail.com", "Le Thi C", "hashed_password_placeholder", "0903000003", "Technician" }
                });

            migrationBuilder.InsertData(
                table: "Storage",
                columns: new[] { "StorageID", "CenterID", "MinThreshold", "PartID", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 3, 1, 10 },
                    { 2, 1, 2, 2, 5 },
                    { 3, 2, 3, 3, 8 },
                    { 4, 2, 5, 4, 15 }
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlan",
                columns: new[] { "PlanID", "Benefits", "Code", "DurationDays", "IsActive", "Name", "PriceVND" },
                values: new object[,]
                {
                    { new Guid("b4d0b3bd-dda4-4032-9d6a-da64745aa38e"), "1 free inspection/month", "BASIC", 30, true, "Basic Care", 499000m },
                    { new Guid("fd490afc-90cc-4bf4-90ae-adfacacd2fcd"), "Priority booking, 3 free inspections", "PREMIUM", 90, true, "Premium Care", 999000m }
                });

            migrationBuilder.InsertData(
                table: "Notification",
                columns: new[] { "NotificationID", "Content", "IsRead", "ReceiverID", "TriggerDate", "Type" },
                values: new object[,]
                {
                    { 1, "Your vehicle maintenance is completed.", false, new Guid("901e7662-d51b-42f0-8826-53f100caf68a"), new DateTime(2025, 10, 17, 13, 28, 31, 959, DateTimeKind.Local).AddTicks(3493), "StatusUpdate" },
                    { 2, "Your appointment is scheduled for tomorrow.", false, new Guid("56755807-5aff-4e3e-9e69-7108020cb8cf"), new DateTime(2025, 10, 17, 13, 28, 31, 959, DateTimeKind.Local).AddTicks(3495), "MaintenanceReminder" }
                });

            migrationBuilder.InsertData(
                table: "Slot",
                columns: new[] { "SlotID", "CenterID", "EndTime", "OrderID", "StartTime", "TechnicianID" },
                values: new object[] { 2, 2, new DateTime(2025, 10, 9, 17, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2025, 10, 9, 13, 0, 0, 0, DateTimeKind.Unspecified), new Guid("d2cc8dfd-b5fd-43a1-9c9d-707f9beda2cd") });

            migrationBuilder.InsertData(
                table: "Subscription",
                columns: new[] { "SubscriptionID", "AutoRenew", "CreatedAt", "EndDate", "PlanID", "StartDate", "Status", "UserID" },
                values: new object[] { new Guid("4ca95637-b9e9-48fd-8019-845e35a79543"), true, new DateTime(2025, 10, 17, 13, 28, 31, 959, DateTimeKind.Local).AddTicks(3447), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("fd490afc-90cc-4bf4-90ae-adfacacd2fcd"), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "active", new Guid("901e7662-d51b-42f0-8826-53f100caf68a") });

            migrationBuilder.InsertData(
                table: "Vehicle",
                columns: new[] { "VehicleID", "BatteryCapacity", "LastMaintenanceDate", "Mileage", "Model", "UserID", "VIN" },
                values: new object[,]
                {
                    { 1, 82.0m, new DateOnly(2025, 8, 15), 15000m, "VinFast VF8", new Guid("901e7662-d51b-42f0-8826-53f100caf68a"), "VN123456789ABCDEFG" },
                    { 2, 75.5m, new DateOnly(2025, 9, 20), 22000m, "Tesla Model Y", new Guid("56755807-5aff-4e3e-9e69-7108020cb8cf"), "VN999999999ABCDEFG" }
                });

            migrationBuilder.InsertData(
                table: "Invoice",
                columns: new[] { "InvoiceID", "Amount", "DueDate", "IssueDate", "Status", "SubscriptionID" },
                values: new object[] { 1, 999000m, new DateTime(2025, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Paid", new Guid("4ca95637-b9e9-48fd-8019-845e35a79543") });

            migrationBuilder.InsertData(
                table: "OrderService",
                columns: new[] { "OrderID", "AppointmentDate", "ChecklistNote", "Status", "TotalCost", "UserID", "VehicleID" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Replaced brake pads, coolant check", "Completed", 2500000m, new Guid("901e7662-d51b-42f0-8826-53f100caf68a"), 1 },
                    { 2, new DateTime(2025, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "General checkup", "Pending", 1000000m, new Guid("56755807-5aff-4e3e-9e69-7108020cb8cf"), 2 }
                });

            migrationBuilder.InsertData(
                table: "Feedback",
                columns: new[] { "FeedbackID", "Comment", "CreatedAt", "OrderID", "Rating", "UserID" },
                values: new object[] { 1, "Excellent service! Technician was professional.", new DateTime(2025, 10, 17, 13, 28, 31, 959, DateTimeKind.Local).AddTicks(3512), 1, 5, new Guid("901e7662-d51b-42f0-8826-53f100caf68a") });

            migrationBuilder.InsertData(
                table: "OrderDetail",
                columns: new[] { "OrderDetailID", "OrderID", "Quantity", "ServiceID", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, 2, 1500000m },
                    { 2, 1, 1, 3, 1000000m },
                    { 3, 2, 1, 4, 1000000m }
                });

            migrationBuilder.InsertData(
                table: "PartsUsed",
                columns: new[] { "UsageID", "Note", "OrderID", "PartID", "Quantity" },
                values: new object[,]
                {
                    { 1, "Brake pads replaced", 1, 3, 4 },
                    { 2, "Coolant refilled", 1, 4, 1 }
                });

            migrationBuilder.InsertData(
                table: "Slot",
                columns: new[] { "SlotID", "CenterID", "EndTime", "OrderID", "StartTime", "TechnicianID" },
                values: new object[] { 1, 1, new DateTime(2025, 10, 5, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2025, 10, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), new Guid("4cad7706-115f-4b35-a0c5-23e3ac3005f5") });
        }
    }
}
