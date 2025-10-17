using System;
using System.Collections.Generic;
using EVCenterService.Models;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Data;

public partial class EVServiceCenterContext : DbContext
{
    public EVServiceCenterContext()
    {
    }

    public EVServiceCenterContext(DbContextOptions<EVServiceCenterContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Invoice> Invoices { get; set; }

    public virtual DbSet<MaintenanceCenter> MaintenanceCenters { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<OrderService> OrderServices { get; set; }

    public virtual DbSet<Part> Parts { get; set; }

    public virtual DbSet<PartsUsed> PartsUseds { get; set; }

    public virtual DbSet<ServiceCatalog> ServiceCatalogs { get; set; }

    public virtual DbSet<Slot> Slots { get; set; }

    public virtual DbSet<Storage> Storages { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<SubscriptionPlan> SubscriptionPlans { get; set; }

    public virtual DbSet<Vehicle> Vehicles { get; set; }

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=EVServiceCenter;User ID=sa;Password=12345;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Account__1788CCAC376F95F8");

            entity.Property(e => e.UserId).HasDefaultValueSql("(newid())");
        });

        modelBuilder.Entity<Feedback>(entity =>
        {
            entity.HasKey(e => e.FeedbackId).HasName("PK__Feedback__6A4BEDF608B7A8D8");

            entity.Property(e => e.FeedbackId).ValueGeneratedNever();
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Order).WithMany(p => p.Feedbacks).HasConstraintName("FK__Feedback__OrderI__6FE99F9F");

            entity.HasOne(d => d.User).WithMany(p => p.Feedbacks).HasConstraintName("FK__Feedback__UserID__70DDC3D8");
        });

        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoice__D796AAD5F01DE521");

            entity.Property(e => e.InvoiceId).ValueGeneratedNever();

            entity.HasOne(d => d.Subscription).WithMany(p => p.Invoices).HasConstraintName("FK__Invoice__Subscri__68487DD7");
        });

        modelBuilder.Entity<MaintenanceCenter>(entity =>
        {
            entity.HasKey(e => e.CenterId).HasName("PK__Maintena__398FC7D7AF460D18");

            entity.Property(e => e.CenterId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32499BFD16");

            entity.Property(e => e.NotificationId).ValueGeneratedNever();
            entity.Property(e => e.IsRead).HasDefaultValue(false);

            entity.HasOne(d => d.Receiver).WithMany(p => p.Notifications).HasConstraintName("FK__Notificat__Recei__6C190EBB");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C5D8A0C85");

            entity.Property(e => e.OrderDetailId).ValueGeneratedNever();
            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails).HasConstraintName("FK__OrderDeta__Order__5070F446");

            entity.HasOne(d => d.Service).WithMany(p => p.OrderDetails).HasConstraintName("FK__OrderDeta__Servi__5165187F");
        });

        modelBuilder.Entity<OrderService>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK__OrderSer__C3905BAFC93F21DA");

            entity.Property(e => e.OrderId).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.OrderServices).HasConstraintName("FK__OrderServ__UserI__4CA06362");

            entity.HasOne(d => d.Vehicle).WithMany(p => p.OrderServices).HasConstraintName("FK__OrderServ__Vehic__4BAC3F29");
        });

        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(e => e.PartId).HasName("PK__Part__7C3F0D3032B58819");

            entity.Property(e => e.PartId).ValueGeneratedNever();
        });

        modelBuilder.Entity<PartsUsed>(entity =>
        {
            entity.HasKey(e => e.UsageId).HasName("PK__PartsUse__29B197C0E363B8D3");

            entity.Property(e => e.UsageId).ValueGeneratedNever();

            entity.HasOne(d => d.Order).WithMany(p => p.PartsUseds).HasConstraintName("FK__PartsUsed__Order__5441852A");

            entity.HasOne(d => d.Part).WithMany(p => p.PartsUseds).HasConstraintName("FK__PartsUsed__PartI__5535A963");
        });

        modelBuilder.Entity<ServiceCatalog>(entity =>
        {
            entity.HasKey(e => e.ServiceId).HasName("PK__ServiceC__C51BB0EAA34714D3");

            entity.Property(e => e.ServiceId).ValueGeneratedNever();
        });

        modelBuilder.Entity<Slot>(entity =>
        {
            entity.HasKey(e => e.SlotId).HasName("PK__Slot__0A124A4F392B4E0A");

            entity.Property(e => e.SlotId).ValueGeneratedNever();

            entity.HasOne(d => d.Center).WithMany(p => p.Slots).HasConstraintName("FK__Slot__CenterID__59063A47");

            entity.HasOne(d => d.Order).WithOne(p => p.Slot).HasConstraintName("FK__Slot__OrderID__5AEE82B9");

            entity.HasOne(d => d.Technician).WithMany(p => p.Slots).HasConstraintName("FK__Slot__Technician__59FA5E80");
        });

        modelBuilder.Entity<Storage>(entity =>
        {
            entity.HasKey(e => e.StorageId).HasName("PK__Storage__8A247E37D28F7A41");

            entity.Property(e => e.StorageId).ValueGeneratedNever();

            entity.HasOne(d => d.Center).WithMany(p => p.Storages).HasConstraintName("FK__Storage__CenterI__45F365D3");

            entity.HasOne(d => d.Part).WithMany(p => p.Storages).HasConstraintName("FK__Storage__PartID__46E78A0C");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => e.SubscriptionId).HasName("PK__Subscrip__9A2B24BD05B92A78");

            entity.Property(e => e.SubscriptionId).ValueGeneratedNever();
            entity.Property(e => e.AutoRenew).HasDefaultValue(false);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Status).HasDefaultValue("active");

            entity.HasOne(d => d.Plan).WithMany(p => p.Subscriptions).HasConstraintName("FK__Subscript__PlanI__656C112C");

            entity.HasOne(d => d.User).WithMany(p => p.Subscriptions).HasConstraintName("FK__Subscript__UserI__6477ECF3");
        });

        modelBuilder.Entity<SubscriptionPlan>(entity =>
        {
            entity.HasKey(e => e.PlanId).HasName("PK__Subscrip__755C22D706673C48");

            entity.Property(e => e.PlanId).ValueGeneratedNever();
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(e => e.VehicleId).HasName("PK__Vehicle__476B54B29147CA38");

            entity.Property(e => e.VehicleId).ValueGeneratedNever();

            entity.HasOne(d => d.User).WithMany(p => p.Vehicles).HasConstraintName("FK__Vehicle__UserID__3D5E1FD2");
        });


        #region Data Seeding

        // --- KHAI BÁO CÁC ID ĐỂ SỬ DỤNG LẠI (GIỐNG NHƯ BIẾN TRONG SQL) ---
        var adminId = Guid.NewGuid();
        var tech1Id = Guid.NewGuid();
        var tech2Id = Guid.NewGuid();
        var staffId = Guid.NewGuid();
        var customer1Id = Guid.NewGuid();
        var customer2Id = Guid.NewGuid();

        var planBasicId = Guid.NewGuid();
        var planPremiumId = Guid.NewGuid();
        var subscription1Id = Guid.NewGuid();

        // --- BẢNG: Account ---
        // LƯU Ý: Trong thực tế, bạn PHẢI HASH mật khẩu này.
        // Ví dụ: Password = new PasswordHasherService().HashPassword("123456")
        modelBuilder.Entity<Account>().HasData(
            new Account { UserId = adminId, FullName = "Admin", Email = "admin@gmail.com", Phone = "0901000001", Password = "hashed_password_placeholder", Role = "Admin" },
            new Account { UserId = tech1Id, FullName = "Tran Van B", Email = "tech1@gmail.com", Phone = "0902000002", Password = "hashed_password_placeholder", Role = "Technician", Certification = "EV Maintenance Level 1" },
            new Account { UserId = tech2Id, FullName = "Le Thi C", Email = "tech2@gmail.com", Phone = "0903000003", Password = "hashed_password_placeholder", Role = "Technician", Certification = "EV Battery Specialist" },
            new Account { UserId = staffId, FullName = "Phan Anh C", Email = "staff@gmail.com", Phone = "0906000006", Password = "hashed_password_placeholder", Role = "Staff" },
            new Account { UserId = customer1Id, FullName = "Pham Van D", Email = "user1@gmail.com", Phone = "0904000004", Password = "hashed_password_placeholder", Role = "Customer" },
            new Account { UserId = customer2Id, FullName = "Do Thi E", Email = "user2@gmail.com", Phone = "0905000005", Password = "hashed_password_placeholder", Role = "Customer" }
        );

        // --- BẢNG: MaintenanceCenter ---
        modelBuilder.Entity<MaintenanceCenter>().HasData(
            new MaintenanceCenter { CenterId = 1, Name = "EV Center - District 1", Address = "12 Le Loi, Q1, HCM", Phone = "0281111111", Email = "center1@evcenter.vn", OpenTime = new TimeOnly(8, 0, 0), CloseTime = new TimeOnly(18, 0, 0) },
            new MaintenanceCenter { CenterId = 2, Name = "EV Center - Thu Duc", Address = "22 Vo Van Ngan, Thu Duc, HCM", Phone = "0282222222", Email = "center2@evcenter.vn", OpenTime = new TimeOnly(8, 0, 0), CloseTime = new TimeOnly(17, 30, 0) }
        );

        // --- BẢNG: Part ---
        modelBuilder.Entity<Part>().HasData(
            new Part { PartId = 1, Name = "EV Battery Pack", Type = "Battery", Brand = "Tesla", UnitPrice = 15000000m },
            new Part { PartId = 2, Name = "Charging Port", Type = "Electrical", Brand = "VinFast", UnitPrice = 3000000m },
            new Part { PartId = 3, Name = "Brake Pad", Type = "Mechanical", Brand = "Brembo", UnitPrice = 1000000m },
            new Part { PartId = 4, Name = "Coolant Fluid", Type = "Chemical", Brand = "Shell", UnitPrice = 500000m }
        );

        // --- BẢNG: Storage ---
        modelBuilder.Entity<Storage>().HasData(
            new Storage { StorageId = 1, CenterId = 1, PartId = 1, Quantity = 10, MinThreshold = 3 },
            new Storage { StorageId = 2, CenterId = 1, PartId = 2, Quantity = 5, MinThreshold = 2 },
            new Storage { StorageId = 3, CenterId = 2, PartId = 3, Quantity = 8, MinThreshold = 3 },
            new Storage { StorageId = 4, CenterId = 2, PartId = 4, Quantity = 15, MinThreshold = 5 }
        );

        // --- BẢNG: ServiceCatalog ---
        modelBuilder.Entity<ServiceCatalog>().HasData(
            new ServiceCatalog { ServiceId = 1, Name = "Battery Replacement", Description = "Replace entire battery pack", BasePrice = 20000000m, DurationMinutes = 180 },
            new ServiceCatalog { ServiceId = 2, Name = "Brake Check", Description = "Inspect and replace brake pads if needed", BasePrice = 1500000m, DurationMinutes = 60 },
            new ServiceCatalog { ServiceId = 3, Name = "Cooling System Check", Description = "Check coolant and thermal management", BasePrice = 800000m, DurationMinutes = 45 },
            new ServiceCatalog { ServiceId = 4, Name = "General Inspection", Description = "Full vehicle health check", BasePrice = 1000000m, DurationMinutes = 90 }
        );

        // --- BẢNG: Vehicle ---
        modelBuilder.Entity<Vehicle>().HasData(
            new Vehicle { VehicleId = 1, UserId = customer1Id, Vin = "VN123456789ABCDEFG", Model = "VinFast VF8", BatteryCapacity = 82.0m, Mileage = 15000m, LastMaintenanceDate = new DateOnly(2025, 8, 15) },
            new Vehicle { VehicleId = 2, UserId = customer2Id, Vin = "VN999999999ABCDEFG", Model = "Tesla Model Y", BatteryCapacity = 75.5m, Mileage = 22000m, LastMaintenanceDate = new DateOnly(2025, 9, 20) }
        );

        // --- BẢNG: OrderService ---
        modelBuilder.Entity<OrderService>().HasData(
            new OrderService { OrderId = 1, VehicleId = 1, UserId = customer1Id, AppointmentDate = new DateTime(2025, 10, 5), Status = "Completed", ChecklistNote = "Replaced brake pads, coolant check", TotalCost = 2500000m },
            new OrderService { OrderId = 2, VehicleId = 2, UserId = customer2Id, AppointmentDate = new DateTime(2025, 10, 7), Status = "Pending", ChecklistNote = "General checkup", TotalCost = 1000000m }
        );

        // --- BẢNG: OrderDetail ---
        modelBuilder.Entity<OrderDetail>().HasData(
            new OrderDetail { OrderDetailId = 1, OrderId = 1, ServiceId = 2, Quantity = 1, UnitPrice = 1500000m },
            new OrderDetail { OrderDetailId = 2, OrderId = 1, ServiceId = 3, Quantity = 1, UnitPrice = 1000000m },
            new OrderDetail { OrderDetailId = 3, OrderId = 2, ServiceId = 4, Quantity = 1, UnitPrice = 1000000m }
        );

        // --- BẢNG: PartsUsed ---
        modelBuilder.Entity<PartsUsed>().HasData(
            new PartsUsed { UsageId = 1, OrderId = 1, PartId = 3, Quantity = 4, Note = "Brake pads replaced" },
            new PartsUsed { UsageId = 2, OrderId = 1, PartId = 4, Quantity = 1, Note = "Coolant refilled" }
        );

        // --- BẢNG: Slot ---
        // Lưu ý: EF Core không hỗ trợ seeding với kiểu dữ liệu TIME,
        // nên chúng ta sẽ dùng DateTime và bỏ qua phần ngày.
        // Hoặc tốt hơn là đổi kiểu dữ liệu cột trong Model thành DateTime.
        // Giả định bạn đã đổi cột StartTime/EndTime thành DateTime trong Model.
        modelBuilder.Entity<Slot>().HasData(
            new Slot { SlotId = 1, CenterId = 1, TechnicianId = tech1Id, OrderId = 1, StartTime = new DateTime(2025, 10, 5, 8, 0, 0), EndTime = new DateTime(2025, 10, 5, 12, 0, 0) },
            new Slot { SlotId = 2, CenterId = 2, TechnicianId = tech2Id, OrderId = null, StartTime = new DateTime(2025, 10, 9, 13, 0, 0), EndTime = new DateTime(2025, 10, 9, 17, 0, 0) }
        );

        // --- BẢNG: SubscriptionPlan ---
        modelBuilder.Entity<SubscriptionPlan>().HasData(
            new SubscriptionPlan { PlanId = planBasicId, Code = "BASIC", Name = "Basic Care", PriceVnd = 499000m, DurationDays = 30, Benefits = "1 free inspection/month", IsActive = true },
            new SubscriptionPlan { PlanId = planPremiumId, Code = "PREMIUM", Name = "Premium Care", PriceVnd = 999000m, DurationDays = 90, Benefits = "Priority booking, 3 free inspections", IsActive = true }
        );

        // --- BẢNG: Subscription ---
        modelBuilder.Entity<Subscription>().HasData(
            new Subscription { SubscriptionId = subscription1Id, UserId = customer1Id, PlanId = planPremiumId, StartDate = new DateTime(2025, 9, 1), EndDate = new DateTime(2025, 12, 1), AutoRenew = true, Status = "active", CreatedAt = DateTime.Now }
        );

        // --- BẢNG: Invoice ---
        modelBuilder.Entity<Invoice>().HasData(
            new Invoice { InvoiceId = 1, SubscriptionId = subscription1Id, Amount = 999000m, Status = "Paid", IssueDate = new DateTime(2025, 9, 1), DueDate = new DateTime(2025, 9, 7) }
        );

        // --- BẢNG: Notification ---
        modelBuilder.Entity<Notification>().HasData(
            new Notification { NotificationId = 1, ReceiverId = customer1Id, Content = "Your vehicle maintenance is completed.", Type = "StatusUpdate", TriggerDate = DateTime.Now, IsRead = false },
            new Notification { NotificationId = 2, ReceiverId = customer2Id, Content = "Your appointment is scheduled for tomorrow.", Type = "MaintenanceReminder", TriggerDate = DateTime.Now, IsRead = false }
        );

        // --- BẢNG: Feedback ---
        modelBuilder.Entity<Feedback>().HasData(
            new Feedback { FeedbackId = 1, OrderId = 1, UserId = customer1Id, Rating = 5, Comment = "Excellent service! Technician was professional.", CreatedAt = DateTime.Now }
        );

        #endregion

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
