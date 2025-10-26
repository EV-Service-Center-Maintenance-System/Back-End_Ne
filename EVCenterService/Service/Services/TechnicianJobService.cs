using EVCenterService.Models;
using EVCenterService.Repository.Interfaces;
using EVCenterService.Service.Interfaces;

namespace EVCenterService.Service.Services
{
    public class TechnicianJobService : ITechnicianJobService
    {
        private readonly ITechnicianJobRepository _repo;

        public TechnicianJobService(ITechnicianJobRepository repo) => _repo = repo;
        public async Task CancelJobAsync(int orderId, string? note)
        {
            var job = await _repo.GetJobByIdAsync(orderId)
                ?? throw new Exception("Không tìm thấy công việc.");

            job.Status = "Cancelled";
            if (!string.IsNullOrWhiteSpace(note))
            {
                job.ChecklistNote += $"\nCancelled by Technician: {note}";
            }

            await _repo.UpdateJobAsync(job);

        }

        public async Task CompleteJobAsync(int orderId, string? note)
        {
            var job = await _repo.GetJobByIdAsync(orderId)
                ?? throw new Exception("Không tìm thấy công việc.");

            job.Status = "TechnicianCompleted";
            if (!string.IsNullOrWhiteSpace(note))
            {
                job.ChecklistNote += $"\nTechnician note: {note}";
            }

            await _repo.UpdateJobAsync(job);
        }

        public async Task<List<OrderService>> GetAssignedJobsAsync(Guid technicianId)
        {
            return await _repo.GetJobsByTechnicianIdAsync(technicianId);
        }

        public async Task<OrderService?> GetJobDetailAsync(int orderId)
        {
            return await _repo.GetJobByIdAsync(orderId);
        }

        public async Task UpdateJobAsync(OrderService job)
        {
            await _repo.UpdateJobAsync(job);
        }
    }
}