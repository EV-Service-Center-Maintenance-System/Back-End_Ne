using EVCenterService.Service.Interfaces;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace EVCenterService.Service.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IMailjetClient _mailjetClient;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiKey = _configuration["Mailjet:ApiKey"];
            var secretKey = _configuration["Mailjet:SecretKey"];
            _senderEmail = _configuration["Mailjet:SenderEmail"];
            _senderName = _configuration["Mailjet:SenderName"];

            if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(_senderEmail))
            {
                // Xử lý lỗi thiếu cấu hình
                Console.WriteLine("WARN: Mailjet settings are missing in appsettings.json. Email sending disabled.");
                // Hoặc throw exception nếu muốn bắt buộc phải có cấu hình
                // throw new InvalidOperationException("Mailjet settings are missing.");

                // Tạo client giả để tránh lỗi NullReference nếu cấu hình thiếu
                _mailjetClient = new MailjetClient("dummy_key", "dummy_secret");
            }
            else
            {
                _mailjetClient = new MailjetClient(apiKey, secretKey);
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            // Kiểm tra lại cấu hình trước khi gửi
            if (string.IsNullOrEmpty(_configuration["Mailjet:ApiKey"]) || string.IsNullOrEmpty(_senderEmail))
            {
                Console.WriteLine($"WARN: Cannot send email to {toEmail} due to missing Mailjet configuration.");
                return false;
            }


            MailjetRequest request = new MailjetRequest
            {
                Resource = SendV31.Resource,
            }
            .Property(Send.Messages, new JArray {
                new JObject {
                 {"From", new JObject {
                  {"Email", _senderEmail},
                  {"Name", _senderName}
                  }},
                 {"To", new JArray {
                  new JObject {
                   {"Email", toEmail},
                   // {"Name", "Recipient Name"} // Tên người nhận (tùy chọn)
                   }
                  }},
                 {"Subject", subject},
                 // {"TextPart", "Plain text content (optional)"}, // Nội dung dạng text (tùy chọn)
                 {"HTMLPart", htmlMessage} // Nội dung dạng HTML
                 }
                });

            try
            {
                MailjetResponse response = await _mailjetClient.PostAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Successfully sent email to {toEmail}. Status: {response.StatusCode}");
                    // Có thể log thêm response.GetData() nếu cần debug
                    return true;
                }
                else
                {
                    Console.WriteLine($"Failed to send email to {toEmail}. Status: {response.StatusCode}, Error: {response.GetErrorMessage()}, Data: {response.GetData()}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception sending email to {toEmail}: {ex.Message}");
                return false;
            }
        }
    }
}
