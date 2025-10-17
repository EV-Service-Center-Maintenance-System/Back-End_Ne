using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("Part")]
public partial class Part
{
    [Key]
    [Column("PartID")]
    public int PartId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? Type { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Brand { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? UnitPrice { get; set; }

    [InverseProperty("Part")]
    public virtual ICollection<PartsUsed> PartsUseds { get; set; } = new List<PartsUsed>();

    [InverseProperty("Part")]
    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
