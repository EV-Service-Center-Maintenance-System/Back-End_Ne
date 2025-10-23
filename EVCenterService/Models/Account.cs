using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("Account")]
[Index("Phone", Name = "UQ__Account__5C7E359E226AF5C9", IsUnique = true)]
[Index("Email", Name = "UQ__Account__A9D105341D624674", IsUnique = true)]
public partial class Account
{
    [Key]
    [Column("UserID")]
    public Guid UserId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? FullName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Password { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string Role { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? Certification { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Status { get; set; } = "Active";

    [InverseProperty("User")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("Receiver")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual ICollection<OrderService> OrderServices { get; set; } = new List<OrderService>();

    [InverseProperty("Technician")]
    public virtual ICollection<OrderService> AssignedOrders { get; set; } = new List<OrderService>();

    [InverseProperty("Technician")]
    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    [InverseProperty("User")]
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();

    [InverseProperty("User")]
    public virtual ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
