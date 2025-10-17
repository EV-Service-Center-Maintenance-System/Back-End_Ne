using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("Storage")]
[Index("CenterId", "PartId", Name = "UQ__Storage__2E4C37056410AAE2", IsUnique = true)]
public partial class Storage
{
    [Key]
    [Column("StorageID")]
    public int StorageId { get; set; }

    [Column("CenterID")]
    public int? CenterId { get; set; }

    [Column("PartID")]
    public int? PartId { get; set; }

    public int Quantity { get; set; }

    public int? MinThreshold { get; set; }

    [ForeignKey("CenterId")]
    [InverseProperty("Storages")]
    public virtual MaintenanceCenter? Center { get; set; }

    [ForeignKey("PartId")]
    [InverseProperty("Storages")]
    public virtual Part? Part { get; set; }
}
