using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("Vehicle")]
[Index("Vin", Name = "UQ__Vehicle__C5DF234C30687499", IsUnique = true)]
public partial class Vehicle
{
    [Key]
    [Column("VehicleID")]
    public int VehicleId { get; set; }

    [Column("UserID")]
    public Guid? UserId { get; set; }

    [Column("VIN")]
    [StringLength(50)]
    [Unicode(false)]
    public string Vin { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? Model { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? BatteryCapacity { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? Mileage { get; set; }

    public DateOnly? LastMaintenanceDate { get; set; }

    [InverseProperty("Vehicle")]
    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();

    [ForeignKey("UserId")]
    [InverseProperty("Vehicles")]
    public virtual Account? User { get; set; }
}
