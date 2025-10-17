using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("Slot")]
[Index("OrderId", Name = "UQ__Slot__C3905BAEB2FB822B", IsUnique = true)]
public partial class Slot
{
    [Key]
    [Column("SlotID")]
    public int SlotId { get; set; }

    [Column("CenterID")]
    public int? CenterId { get; set; }

    [Column("TechnicianID")]
    public Guid? TechnicianId { get; set; }

    [Column("OrderID")]
    public int? OrderId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndTime { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("Slots")]
    public virtual MaintenanceCenter? Center { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Slot")]
    public virtual OrderService? Order { get; set; }

    [ForeignKey("TechnicianId")]
    [InverseProperty("Slots")]
    public virtual Account? Technician { get; set; }
}
