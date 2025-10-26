using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("OrderService")]
public partial class OrderService
{
    [Key]
    [Column("OrderID")]
    public int OrderId { get; set; }

    [Column("VehicleID")]
    public int? VehicleId { get; set; }

    [Column("UserID")]
    public Guid? UserId { get; set; }

    [Column("TechnicianID")]
    public Guid? TechnicianId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime AppointmentDate { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column(TypeName = "text")]
    public string? ChecklistNote { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? TotalCost { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [InverseProperty("Order")]
    public virtual ICollection<PartsUsed> PartsUseds { get; set; } = new List<PartsUsed>();

    [InverseProperty("Order")]
    public virtual Slot? Slot { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("OrderServices")]
    public virtual Account? User { get; set; }

    [ForeignKey("VehicleId")]
    [InverseProperty("OrderServices")]
    public virtual Vehicle? Vehicle { get; set; }

    [ForeignKey("TechnicianId")]
    [InverseProperty("AssignedOrders")]
    public virtual Account? Technician { get; set; }
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
