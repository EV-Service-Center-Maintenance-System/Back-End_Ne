using System.Threading.Tasks;

namespace EVCenterService.Service.Interfaces
{
    public interface IEmailSender
    {
        // Thay đổi nhỏ: Trả về bool để biết thành công hay thất bại (tùy chọn)
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlMessage);
    }
}
