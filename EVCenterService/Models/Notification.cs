using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("Notification")]
public partial class Notification
{
    [Key]
    [Column("NotificationID")]
    public int NotificationId { get; set; }

    [Column("ReceiverID")]
    public Guid? ReceiverId { get; set; }

    [Column(TypeName = "text")]
    public string Content { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string? Type { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? TriggerDate { get; set; }

    public bool? IsRead { get; set; }

    [ForeignKey("ReceiverId")]
    [InverseProperty("Notifications")]
    public virtual Account? Receiver { get; set; }
}
