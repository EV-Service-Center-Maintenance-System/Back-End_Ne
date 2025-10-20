using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EVCenterService.Migrations
{
    /// <inheritdoc />
    public partial class UpdateHashedSeedPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "UserID", "Certification", "Email", "FullName", "Password", "Phone", "Role", "Status" },
                values: new object[,]
                {
                    { new Guid("48a51c51-1554-4b95-bb59-065d6f4b7986"), null, "user1@gmail.com", "Pham Van D", "AQAAAAIAAYagAAAAEDbkAoEE55WBH4eLFVkXSAWK+22/g6EUCj+vSnhVDboaq4YZW+ccdWp5vKPeoXBIxQ==", "0904000004", "Customer", "Active" },
                    { new Guid("6610800c-aead-4970-b0ea-926cc58776a9"), null, "admin@gmail.com", "Admin", "AQAAAAIAAYagAAAAEN9zKU98dBy3r85eJODQgI6iqlI4qAFJT9Bp2YQqv4XXs2JJ5sJtjSMLoQZZM3V2UA==", "0901000001", "Admin", "Active" },
                    { new Guid("8f944328-a6d8-4fba-a7cb-9075be9eff4e"), "EV Battery Specialist", "tech2@gmail.com", "Le Thi C", "AQAAAAIAAYagAAAAELJ/w0DU7mENqF2DG/QXxzRgPvUSCp6SuXF2tVxFXcMFIQO1nh/SxWJcALkFLEqudg==", "0903000003", "Technician", "Active" },
                    { new Guid("b3a43285-8bf8-42d5-a336-b95bb787f569"), "EV Maintenance Level 1", "tech1@gmail.com", "Tran Van B", "AQAAAAIAAYagAAAAEJ3p13Lq6gpNs02ZSY1gl3YkyofTVlpYLR01PRxj9Etk1N5gpVT+5JgBm8ayCFXGFw==", "0902000002", "Technician", "Active" },
                    { new Guid("ea4d369a-fdb2-40b9-8631-bd569fbfec89"), null, "user2@gmail.com", "Do Thi E", "AQAAAAIAAYagAAAAEPtnb6s+cKwTZMr+GAeAmVw06IemRxHuQpS2PYejht/32IdtoqqRsiab0RB3ASLtsQ==", "0905000005", "Customer", "Active" },
                    { new Guid("f2b76daa-988f-4f93-ab69-f1e8ba18ce64"), null, "staff@gmail.com", "Phan Anh C", "AQAAAAIAAYagAAAAEH8DBId+fa1M7g/AOepV/P3QXR3Rvdo7h+CDGBd7FbyWCBYHl+vto48OBuusBaJFzA==", "0906000006", "Staff", "Active" }
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlan",
                columns: new[] { "PlanID", "Benefits", "Code", "DurationDays", "IsActive", "Name", "PriceVND" },
                values: new object[,]
                {
                    { new Guid("17a8716d-f043-4c35-9443-19751173872a"), "Priority booking, 3 free inspections", "PREMIUM", 90, true, "Premium Care", 999000m },
                    { new Guid("6c0abe88-8d97-42b5-b4ce-14edb10dd4ad"), "1 free inspection/month", "BASIC", 30, true, "Basic Care", 499000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("48a51c51-1554-4b95-bb59-065d6f4b7986"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("6610800c-aead-4970-b0ea-926cc58776a9"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("8f944328-a6d8-4fba-a7cb-9075be9eff4e"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("b3a43285-8bf8-42d5-a336-b95bb787f569"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("ea4d369a-fdb2-40b9-8631-bd569fbfec89"));

            migrationBuilder.DeleteData(
                table: "Account",
                keyColumn: "UserID",
                keyValue: new Guid("f2b76daa-988f-4f93-ab69-f1e8ba18ce64"));

            migrationBuilder.DeleteData(
                table: "SubscriptionPlan",
                keyColumn: "PlanID",
                keyValue: new Guid("17a8716d-f043-4c35-9443-19751173872a"));

            migrationBuilder.DeleteData(
                table: "SubscriptionPlan",
                keyColumn: "PlanID",
                keyValue: new Guid("6c0abe88-8d97-42b5-b4ce-14edb10dd4ad"));

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
    }
}
