using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Threading.Tasks; // Đảm bảo bạn đã using

namespace EVCenterService.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            if (Context.User.IsInRole("Staff") || Context.User.IsInRole("Admin"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "StaffSupport");
            }
            await base.OnConnectedAsync();
        }

        // 1. SỬA HÀM NÀY: Customer gửi tin nhắn
        public async Task CustomerSendMessage(string message)
        {
            var customerName = Context.User.FindFirst(ClaimTypes.GivenName)?.Value ?? Context.User.Identity.Name;

            // THAY ĐỔI: Thêm "(Customer)" vào tên
            var displayName = $"{customerName} (Customer)";
            var customerConnectionId = Context.ConnectionId;

            // Gửi "displayName" mới cho Staff
            await Clients.Group("StaffSupport").SendAsync(
                "ReceiveCustomerMessage",
                customerConnectionId,
                displayName, // Gửi tên đã có (Customer)
                message
            );
        }

        // 2. SỬA HÀM NÀY: Staff trả lời
        public async Task StaffReplyMessage(string customerConnectionId, string message)
        {
            // THAY ĐỔI: Luôn gửi tên là "Nhân viên hỗ trợ"
            var displayName = "Nhân viên hỗ trợ";

            // Gửi "displayName" mới cho Customer
            await Clients.Client(customerConnectionId).SendAsync(
                "ReceiveStaffReply",
                displayName, // Gửi tên chung "Nhân viên hỗ trợ"
                message
            );
        }
    }
}