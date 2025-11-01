using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EVCenterService.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Account",
                columns: table => new
                {
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false, defaultValueSql: "(newid())"),
                    FullName = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Certification = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Account__1788CCAC376F95F8", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "MaintenanceCenter",
                columns: table => new
                {
                    CenterID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Address = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    Phone = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    OpenTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    CloseTime = table.Column<TimeOnly>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Maintena__398FC7D7AF460D18", x => x.CenterID);
                });

            migrationBuilder.CreateTable(
                name: "Part",
                columns: table => new
                {
                    PartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Type = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Brand = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Part__7C3F0D3032B58819", x => x.PartID);
                });

            migrationBuilder.CreateTable(
                name: "ServiceCatalog",
                columns: table => new
                {
                    ServiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BasePrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    DurationMinutes = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ServiceC__C51BB0EAA34714D3", x => x.ServiceID);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionPlan",
                columns: table => new
                {
                    PlanID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    PriceVND = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    DurationDays = table.Column<int>(type: "int", nullable: false),
                    Benefits = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subscrip__755C22D706673C48", x => x.PlanID);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReceiverID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    TriggerDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    IsRead = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20CF2E32499BFD16", x => x.NotificationID);
                    table.ForeignKey(
                        name: "FK__Notificat__Recei__6C190EBB",
                        column: x => x.ReceiverID,
                        principalTable: "Account",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Vehicle",
                columns: table => new
                {
                    VehicleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VIN = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    BatteryCapacity = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    Mileage = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    LastMaintenanceDate = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Vehicle__476B54B29147CA38", x => x.VehicleID);
                    table.ForeignKey(
                        name: "FK__Vehicle__UserID__3D5E1FD2",
                        column: x => x.UserID,
                        principalTable: "Account",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Storage",
                columns: table => new
                {
                    StorageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CenterID = table.Column<int>(type: "int", nullable: true),
                    PartID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    MinThreshold = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Storage__8A247E37D28F7A41", x => x.StorageID);
                    table.ForeignKey(
                        name: "FK__Storage__CenterI__45F365D3",
                        column: x => x.CenterID,
                        principalTable: "MaintenanceCenter",
                        principalColumn: "CenterID");
                    table.ForeignKey(
                        name: "FK__Storage__PartID__46E78A0C",
                        column: x => x.PartID,
                        principalTable: "Part",
                        principalColumn: "PartID");
                });

            migrationBuilder.CreateTable(
                name: "Subscription",
                columns: table => new
                {
                    SubscriptionID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlanID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    AutoRenew = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false, defaultValue: "active"),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Subscrip__9A2B24BD05B92A78", x => x.SubscriptionID);
                    table.ForeignKey(
                        name: "FK__Subscript__PlanI__656C112C",
                        column: x => x.PlanID,
                        principalTable: "SubscriptionPlan",
                        principalColumn: "PlanID");
                    table.ForeignKey(
                        name: "FK__Subscript__UserI__6477ECF3",
                        column: x => x.UserID,
                        principalTable: "Account",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "OrderService",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VehicleID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TechnicianID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AppointmentDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    ChecklistNote = table.Column<string>(type: "text", nullable: true),
                    TotalCost = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderSer__C3905BAFC93F21DA", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_OrderService_Technician",
                        column: x => x.TechnicianID,
                        principalTable: "Account",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__OrderServ__UserI__4CA06362",
                        column: x => x.UserID,
                        principalTable: "Account",
                        principalColumn: "UserID");
                    table.ForeignKey(
                        name: "FK__OrderServ__Vehic__4BAC3F29",
                        column: x => x.VehicleID,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleID");
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    FeedbackID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Rating = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Feedback__6A4BEDF608B7A8D8", x => x.FeedbackID);
                    table.ForeignKey(
                        name: "FK__Feedback__OrderI__6FE99F9F",
                        column: x => x.OrderID,
                        principalTable: "OrderService",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK__Feedback__UserID__70DDC3D8",
                        column: x => x.UserID,
                        principalTable: "Account",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    InvoiceID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SubscriptionID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    IssueDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    DueDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Invoice__D796AAD5F01DE521", x => x.InvoiceID);
                    table.ForeignKey(
                        name: "FK_Invoice_OrderService",
                        column: x => x.OrderID,
                        principalTable: "OrderService",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK_Invoice_Subscription_SubscriptionID",
                        column: x => x.SubscriptionID,
                        principalTable: "Subscription",
                        principalColumn: "SubscriptionID");
                });

            migrationBuilder.CreateTable(
                name: "OrderDetail",
                columns: table => new
                {
                    OrderDetailID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    ServiceID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__OrderDet__D3B9D30C5D8A0C85", x => x.OrderDetailID);
                    table.ForeignKey(
                        name: "FK__OrderDeta__Order__5070F446",
                        column: x => x.OrderID,
                        principalTable: "OrderService",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK__OrderDeta__Servi__5165187F",
                        column: x => x.ServiceID,
                        principalTable: "ServiceCatalog",
                        principalColumn: "ServiceID");
                });

            migrationBuilder.CreateTable(
                name: "PartsUsed",
                columns: table => new
                {
                    UsageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    PartID = table.Column<int>(type: "int", nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PartsUse__29B197C0E363B8D3", x => x.UsageID);
                    table.ForeignKey(
                        name: "FK__PartsUsed__Order__5441852A",
                        column: x => x.OrderID,
                        principalTable: "OrderService",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK__PartsUsed__PartI__5535A963",
                        column: x => x.PartID,
                        principalTable: "Part",
                        principalColumn: "PartID");
                });

            migrationBuilder.CreateTable(
                name: "Slot",
                columns: table => new
                {
                    SlotID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CenterID = table.Column<int>(type: "int", nullable: true),
                    TechnicianID = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrderID = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime", nullable: true),
                    EndTime = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Slot__0A124A4F392B4E0A", x => x.SlotID);
                    table.ForeignKey(
                        name: "FK__Slot__CenterID__59063A47",
                        column: x => x.CenterID,
                        principalTable: "MaintenanceCenter",
                        principalColumn: "CenterID");
                    table.ForeignKey(
                        name: "FK__Slot__OrderID__5AEE82B9",
                        column: x => x.OrderID,
                        principalTable: "OrderService",
                        principalColumn: "OrderID");
                    table.ForeignKey(
                        name: "FK__Slot__Technician__59FA5E80",
                        column: x => x.TechnicianID,
                        principalTable: "Account",
                        principalColumn: "UserID");
                });

            migrationBuilder.InsertData(
                table: "Account",
                columns: new[] { "UserID", "Certification", "Email", "FullName", "Password", "Phone", "Role", "Status" },
                values: new object[,]
                {
                    { new Guid("a1a1a1a1-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, "admin@gmail.com", "Admin", "AQAAAAIAAYagAAAAEFvpoKLN92r91qDzs8wTh8ZCSRQboJv4H74ErWT9/qR5r7NtOOHbnKbwYoY/nNvg6w==", "0901000001", "Admin", "Active" },
                    { new Guid("a3a3a3a3-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), "Thermal & Cooling System Certified", "tech3@gmail.com", "Nguyen Van F", "AQAAAAIAAYagAAAAEA6R7q/kHhUKp7PSdO1sauGRBHZowvi31ofRiNzvgnpi5B6Bn+DqUwPMhgaKAdO4nw==", "0908000008", "Technician", "Active" },
                    { new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), "Brake System Certified", "tech1@gmail.com", "Tran Van B", "AQAAAAIAAYagAAAAEDTPGB4Gv3iCxJe9XtlRwauoUTrAYYU9G6ia7WmLQ52xvj8O9XLcbMS9Ov1Il9LDVA==", "0902000002", "Technician", "Active" },
                    { new Guid("b4b4b4b4-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), null, "staff2@gmail.com", "Hoang Thi G", "AQAAAAIAAYagAAAAEGt0Mi3a/VTQ57InxniOeS1zsiPKOe8XrYus5MkXUfF3ta1axAXLDV3sQf0TUSV5EA==", "0909000009", "Staff", "Active" },
                    { new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc"), "Battery System Certified", "tech2@gmail.com", "Le Thi C", "AQAAAAIAAYagAAAAEJVT5qzAJS+kOnHiY8exN2awkyH1v5NC6JIrY0qVsgJJvucIfAxIUXLqsG9huQF/+Q==", "0903000003", "Technician", "Active" },
                    { new Guid("c5c5c5c5-cccc-cccc-cccc-cccccccccccc"), "General Inspection Certified", "tech4@gmail.com", "Vu Dinh H", "AQAAAAIAAYagAAAAEMQdKS4QXm0Wc1GC41JAZfnSniN/yYIpHczjb6YTTVocTrlMDO9bQTr9Qj1kd6mS5w==", "0910000010", "Technician", "Active" },
                    { new Guid("d4d4d4d4-dddd-dddd-dddd-dddddddddddd"), null, "staff@gmail.com", "Phan Anh C", "AQAAAAIAAYagAAAAELDjQIdNYlWbr+9sFXzIbmrkVmnbvEx+JlzPAObis4bnvcHp0VPB7i+u+oXETzO6Cw==", "0906000006", "Staff", "Active" },
                    { new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"), null, "user1@gmail.com", "Pham Van D", "AQAAAAIAAYagAAAAEDu7LTIDxK4MmeiJb0q5LAsXhQiSws6y7teF5AY+GctTGHuQac7ewAhwU+rsr7M8pA==", "0904000004", "Customer", "Active" },
                    { new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"), null, "user2@gmail.com", "Do Thi E", "AQAAAAIAAYagAAAAEBBgoglhZoioZFKM1WqdQzkBiWrLcbM9b2YyXxhHCeVj8R8RYZEWT7vqltIwrU82LQ==", "0905000005", "Customer", "Active" }
                });

            migrationBuilder.InsertData(
                table: "MaintenanceCenter",
                columns: new[] { "CenterID", "Address", "CloseTime", "Email", "Name", "OpenTime", "Phone" },
                values: new object[] { 1, "12 Le Loi, Q1, HCM", new TimeOnly(18, 0, 0), "center1@evcenter.vn", "EV Center - District 1", new TimeOnly(8, 0, 0), "0281111111" });

            migrationBuilder.InsertData(
                table: "Part",
                columns: new[] { "PartID", "Brand", "Name", "Type", "UnitPrice" },
                values: new object[,]
                {
                    { 1, "Tesla", "EV Battery Pack", "Battery", 15000000m },
                    { 2, "VinFast", "Charging Port", "Electrical", 3000000m },
                    { 3, "Brembo", "Brake Pad", "Mechanical", 1000000m },
                    { 4, "Shell", "Coolant Fluid", "Chemical", 500000m }
                });

            migrationBuilder.InsertData(
                table: "ServiceCatalog",
                columns: new[] { "ServiceID", "BasePrice", "Description", "DurationMinutes", "Name" },
                values: new object[,]
                {
                    { 1, 20000000m, "Replace entire battery pack", 180, "Battery Replacement" },
                    { 2, 1500000m, "Inspect and replace brake pads if needed", 60, "Brake Check" },
                    { 3, 800000m, "Check coolant and thermal management", 45, "Cooling System Check" },
                    { 4, 1000000m, "Full vehicle health check", 90, "General Inspection" }
                });

            migrationBuilder.InsertData(
                table: "SubscriptionPlan",
                columns: new[] { "PlanID", "Benefits", "Code", "DurationDays", "IsActive", "Name", "PriceVND" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "1 free inspection/month", "BASIC", 30, true, "Basic Care", 499000m },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Priority booking, 3 free inspections", "PREMIUM", 90, true, "Premium Care", 999000m }
                });

            migrationBuilder.InsertData(
                table: "Notification",
                columns: new[] { "NotificationID", "Content", "IsRead", "ReceiverID", "TriggerDate", "Type" },
                values: new object[,]
                {
                    { 1, "Your vehicle maintenance is completed.", false, new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"), new DateTime(2025, 11, 2, 3, 13, 34, 922, DateTimeKind.Local).AddTicks(7108), "StatusUpdate" },
                    { 2, "Your appointment is scheduled for tomorrow.", false, new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"), new DateTime(2025, 11, 2, 3, 13, 34, 922, DateTimeKind.Local).AddTicks(7110), "MaintenanceReminder" }
                });

            migrationBuilder.InsertData(
                table: "Slot",
                columns: new[] { "SlotID", "CenterID", "EndTime", "OrderID", "StartTime", "TechnicianID" },
                values: new object[] { 2, 1, new DateTime(2025, 10, 9, 17, 0, 0, 0, DateTimeKind.Unspecified), null, new DateTime(2025, 10, 9, 13, 0, 0, 0, DateTimeKind.Unspecified), new Guid("c3c3c3c3-cccc-cccc-cccc-cccccccccccc") });

            migrationBuilder.InsertData(
                table: "Storage",
                columns: new[] { "StorageID", "CenterID", "MinThreshold", "PartID", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 3, 1, 10 },
                    { 2, 1, 2, 2, 5 },
                    { 3, 1, 3, 3, 8 },
                    { 4, 1, 5, 4, 15 }
                });

            migrationBuilder.InsertData(
                table: "Subscription",
                columns: new[] { "SubscriptionID", "AutoRenew", "CreatedAt", "EndDate", "PlanID", "StartDate", "Status", "UserID" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), true, new DateTime(2025, 11, 2, 3, 13, 34, 922, DateTimeKind.Local).AddTicks(7048), new DateTime(2025, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "active", new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee") });

            migrationBuilder.InsertData(
                table: "Vehicle",
                columns: new[] { "VehicleID", "BatteryCapacity", "LastMaintenanceDate", "Mileage", "Model", "UserID", "VIN" },
                values: new object[,]
                {
                    { 1, 82.0m, new DateOnly(2025, 8, 15), 15000m, "VinFast VF8", new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"), "VN123456789ABCDEFG" },
                    { 2, 75.5m, new DateOnly(2025, 9, 20), 22000m, "Tesla Model Y", new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"), "VN999999999ABCDEFG" }
                });

            migrationBuilder.InsertData(
                table: "Invoice",
                columns: new[] { "InvoiceID", "Amount", "DueDate", "IssueDate", "OrderID", "Status", "SubscriptionID" },
                values: new object[] { 1, 999000m, new DateTime(2025, 9, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "Paid", new Guid("33333333-3333-3333-3333-333333333333") });

            migrationBuilder.InsertData(
                table: "OrderService",
                columns: new[] { "OrderID", "AppointmentDate", "ChecklistNote", "Status", "TechnicianID", "TotalCost", "UserID", "VehicleID" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Replaced brake pads, coolant check", "Completed", new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), 2500000m, new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee"), 1 },
                    { 2, new DateTime(2025, 10, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "General checkup", "Pending", null, 1000000m, new Guid("f6f6f6f6-ffff-ffff-ffff-ffffffffffff"), 2 }
                });

            migrationBuilder.InsertData(
                table: "Feedback",
                columns: new[] { "FeedbackID", "Comment", "CreatedAt", "OrderID", "Rating", "UserID" },
                values: new object[] { 1, "Excellent service! Technician was professional.", new DateTime(2025, 11, 2, 3, 13, 34, 922, DateTimeKind.Local).AddTicks(7132), 1, 5, new Guid("e5e5e5e5-eeee-eeee-eeee-eeeeeeeeeeee") });

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
                values: new object[] { 1, 1, new DateTime(2025, 10, 5, 12, 0, 0, 0, DateTimeKind.Unspecified), 1, new DateTime(2025, 10, 5, 8, 0, 0, 0, DateTimeKind.Unspecified), new Guid("b2b2b2b2-bbbb-bbbb-bbbb-bbbbbbbbbbbb") });

            migrationBuilder.CreateIndex(
                name: "UQ__Account__5C7E359E226AF5C9",
                table: "Account",
                column: "Phone",
                unique: true,
                filter: "[Phone] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "UQ__Account__A9D105341D624674",
                table: "Account",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_OrderID",
                table: "Feedback",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_UserID",
                table: "Feedback",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_OrderID",
                table: "Invoice",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Invoice_SubscriptionID",
                table: "Invoice",
                column: "SubscriptionID");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ReceiverID",
                table: "Notification",
                column: "ReceiverID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_OrderID",
                table: "OrderDetail",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetail_ServiceID",
                table: "OrderDetail",
                column: "ServiceID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderService_TechnicianID",
                table: "OrderService",
                column: "TechnicianID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderService_UserID",
                table: "OrderService",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderService_VehicleID",
                table: "OrderService",
                column: "VehicleID");

            migrationBuilder.CreateIndex(
                name: "IX_PartsUsed_OrderID",
                table: "PartsUsed",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_PartsUsed_PartID",
                table: "PartsUsed",
                column: "PartID");

            migrationBuilder.CreateIndex(
                name: "IX_Slot_CenterID",
                table: "Slot",
                column: "CenterID");

            migrationBuilder.CreateIndex(
                name: "IX_Slot_TechnicianID",
                table: "Slot",
                column: "TechnicianID");

            migrationBuilder.CreateIndex(
                name: "UQ__Slot__C3905BAEB2FB822B",
                table: "Slot",
                column: "OrderID",
                unique: true,
                filter: "[OrderID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Storage_PartID",
                table: "Storage",
                column: "PartID");

            migrationBuilder.CreateIndex(
                name: "UQ__Storage__2E4C37056410AAE2",
                table: "Storage",
                columns: new[] { "CenterID", "PartID" },
                unique: true,
                filter: "[CenterID] IS NOT NULL AND [PartID] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_PlanID",
                table: "Subscription",
                column: "PlanID");

            migrationBuilder.CreateIndex(
                name: "IX_Subscription_UserID",
                table: "Subscription",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ__Subscrip__A25C5AA731620A2C",
                table: "SubscriptionPlan",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_UserID",
                table: "Vehicle",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "UQ__Vehicle__C5DF234C30687499",
                table: "Vehicle",
                column: "VIN",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedback");

            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Notification");

            migrationBuilder.DropTable(
                name: "OrderDetail");

            migrationBuilder.DropTable(
                name: "PartsUsed");

            migrationBuilder.DropTable(
                name: "Slot");

            migrationBuilder.DropTable(
                name: "Storage");

            migrationBuilder.DropTable(
                name: "Subscription");

            migrationBuilder.DropTable(
                name: "ServiceCatalog");

            migrationBuilder.DropTable(
                name: "OrderService");

            migrationBuilder.DropTable(
                name: "MaintenanceCenter");

            migrationBuilder.DropTable(
                name: "Part");

            migrationBuilder.DropTable(
                name: "SubscriptionPlan");

            migrationBuilder.DropTable(
                name: "Vehicle");

            migrationBuilder.DropTable(
                name: "Account");
        }
    }
}
