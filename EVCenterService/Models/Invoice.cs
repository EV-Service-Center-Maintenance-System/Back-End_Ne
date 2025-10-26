using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EVCenterService.Models;

[Table("Invoice")]
public partial class Invoice
{
    [Key]
    [Column("InvoiceID")]
    public int InvoiceId { get; set; }

    [Column("SubscriptionID")]
    public Guid? SubscriptionId { get; set; }

    [Column("OrderID")]
    public int? OrderId { get; set; }

    [Column(TypeName = "decimal(12, 2)")]
    public decimal? Amount { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Status { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? IssueDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DueDate { get; set; }

    [ForeignKey("OrderId")]
    [InverseProperty("Invoices")]
    public virtual OrderService? Order { get; set; }

    [ForeignKey("SubscriptionId")]
    [InverseProperty("Invoices")]
    public virtual Subscription? Subscription { get; set; }
}