namespace EVCenterService.ViewModels
{
    public class AppointmentHistoryViewModel
    {
        public int OrderId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? VehicleModel { get; set; }
        public string? ServiceNames { get; set; }
        public string? Status { get; set; }
        public decimal? TotalCost { get; set; }
        public string? TechnicianName { get; set; }
        public string? ChecklistNote { get; set; }
        public bool HasFeedback { get; set; }
        public int? InvoiceId { get; set; }
    }
}
