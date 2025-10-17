using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("MaintenanceCenter")]
public partial class MaintenanceCenter
{
    [Key]
    [Column("CenterID")]
    public int CenterId { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Name { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Address { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Phone { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? Email { get; set; }

    public TimeOnly? OpenTime { get; set; }

    public TimeOnly? CloseTime { get; set; }

    [InverseProperty("Center")]
    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();

    [InverseProperty("Center")]
    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
