using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("Subscription")]
public partial class Subscription
{
    [Key]
    [Column("SubscriptionID")]
    public Guid SubscriptionId { get; set; }

    [Column("UserID")]
    public Guid? UserId { get; set; }

    [Column("PlanID")]
    public Guid? PlanId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime EndDate { get; set; }

    public bool? AutoRenew { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string Status { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [InverseProperty("Subscription")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    [ForeignKey("PlanId")]
    [InverseProperty("Subscriptions")]
    public virtual SubscriptionPlan? Plan { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Subscriptions")]
    public virtual Account? User { get; set; }
}
