using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("PartsUsed")]
public partial class PartsUsed
{
    [Key]
    [Column("UsageID")]
    public int UsageId { get; set; }

    [Column("OrderID")]
    public int? OrderId { get; set; }

    [Column("PartID")]
    public int? PartId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "text")]
    public string? Note { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("PartsUseds")]
    public virtual OrderService? Order { get; set; }

    [ForeignKey("PartId")]
    [InverseProperty("PartsUseds")]
    public virtual Part? Part { get; set; }
}
