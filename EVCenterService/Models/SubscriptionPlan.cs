using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("SubscriptionPlan")]
[Index("Code", Name = "UQ__Subscrip__A25C5AA731620A2C", IsUnique = true)]
public partial class SubscriptionPlan
{
    [Key]
    [Column("PlanID")]
    public Guid PlanId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string Code { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column("PriceVND", TypeName = "decimal(12, 2)")]
    public decimal PriceVnd { get; set; }

    public int DurationDays { get; set; }

    [Column(TypeName = "text")]
    public string? Benefits { get; set; }

    public bool? IsActive { get; set; }

    [InverseProperty("Plan")]
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
}
