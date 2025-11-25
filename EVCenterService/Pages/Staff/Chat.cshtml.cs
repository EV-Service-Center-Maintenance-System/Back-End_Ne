using EVCenterService.Models;
using EVCenterService.ViewModels;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EVCenterService.Pages.Staff
{
    [Authorize(Roles = "Staff,Admin")]
    public class ChatModel : PageModel
    {
        private readonly FirestoreDb _firestoreDb;

        // 1. Inject FirestoreDb (đã đăng ký trong Program.cs)
        public ChatModel(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // 2. Model này sẽ được gửi sang giao diện
        public List<ChatConversationModel> WaitingList { get; set; } = new();

        // 3. Class con để hiển thị
        public class ChatConversationModel
        {
            public string CustomerId { get; set; } // ID (Guid)
            public string CustomerName { get; set; }
            public string LastMessage { get; set; }
            public bool IsReadByStaff { get; set; }
        }

        // 4. Tải danh sách chờ khi Staff mở trang
        public async Task OnGetAsync()
        {
            var conversationsCol = _firestoreDb.Collection("conversations");
            // Lấy tất cả cuộc hội thoại, sắp xếp theo thời gian
            var query = conversationsCol.OrderByDescending("LastMessageTimestamp");
            var snapshot = await query.GetSnapshotAsync();

            foreach (var doc in snapshot.Documents)
            {
                var conversation = doc.ConvertTo<ChatConversation>();
                WaitingList.Add(new ChatConversationModel
                {
                    CustomerId = doc.Id, // ID của Document chính là CustomerId
                    CustomerName = conversation.CustomerName,
                    LastMessage = conversation.LastMessage,
                    IsReadByStaff = conversation.IsReadByStaff
                });
            }
        }

        // 5. Handler mới: Lấy lịch sử tin nhắn (để JS gọi)
        public async Task<IActionResult> OnGetMessagesAsync(string customerId)
        {
            if (string.IsNullOrEmpty(customerId)) return BadRequest();

            var messagesCol = _firestoreDb.Collection("conversations").Document(customerId).Collection("messages");
            var query = messagesCol.OrderBy("Timestamp");
            var snapshot = await query.GetSnapshotAsync();

            var messages = snapshot.Documents.Select(doc => doc.ConvertTo<ChatMessage>()).ToList();

            // Đánh dấu đã đọc
            await _firestoreDb.Collection("conversations").Document(customerId).SetAsync(new { IsReadByStaff = true }, SetOptions.MergeAll);

            return new JsonResult(messages);
        }
    }
}