using EVCenterService.Models;  
using EVCenterService.ViewModels;
using Google.Cloud.Firestore; 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EVCenterService.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly FirestoreDb _firestoreDb;

        // Inject FirestoreDb
        public ChatHub(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        // Collection (bảng) chính trong Firestore
        private CollectionReference GetConversationsCol()
        {
            return _firestoreDb.Collection("conversations");
        }

        public override async Task OnConnectedAsync()
        {
            var customerId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customerName = Context.User.FindFirst(ClaimTypes.GivenName)?.Value ?? Context.User.Identity.Name;

            if (Context.User.IsInRole("Staff") || Context.User.IsInRole("Admin"))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, "StaffSupport");
            }
            else // Nếu là Customer
            {
                // Cập nhật ConnectionId mới nhất
                var docRef = GetConversationsCol().Document(customerId);
                await docRef.SetAsync(new { CustomerConnectionId = Context.ConnectionId, CustomerName = customerName }, SetOptions.MergeAll);
            }
            await base.OnConnectedAsync();
        }

        // 1. Customer gửi tin nhắn 
        public async Task CustomerSendMessage(string message)
        {
            var customerId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customerName = Context.User.FindFirst(ClaimTypes.GivenName)?.Value ?? Context.User.Identity.Name;
            var displayName = $"{customerName} (Customer)";
            var timestamp = Timestamp.FromDateTime(DateTime.UtcNow);

            // 1. Chuẩn bị tin nhắn
            var chatMessage = new ChatMessage
            {
                SenderName = displayName,
                Message = message,
                Timestamp = timestamp,
                IsStaffMessage = false
            };

            // 2. Lưu tin nhắn vào Firestore (trong sub-collection)
            var messageRef = GetConversationsCol().Document(customerId).Collection("messages").Document();
            await messageRef.SetAsync(chatMessage);

            // 3. Cập nhật "tiêu đề" cuộc trò chuyện (để Staff thấy)
            var conversationData = new ChatConversation
            {
                CustomerName = customerName,
                LastMessage = message,
                LastMessageTimestamp = timestamp,
                IsReadByStaff = false,
                CustomerConnectionId = Context.ConnectionId
            };
            await GetConversationsCol().Document(customerId).SetAsync(conversationData, SetOptions.MergeAll);

            // 4. Gửi real-time cho Staff 
            await Clients.Group("StaffSupport").SendAsync(
                "ReceiveCustomerMessage",
                Context.ConnectionId, // ConnectionId của khách
                customerId,         // ID (Guid) của khách
                displayName,
                message
            );
        }

        // 2. Staff trả lời 
        // (Lưu ý: JS phải gửi customerId (Guid), không phải connectionId)
        public async Task StaffReplyMessage(string customerId, string message)
        {
            var displayName = "Nhân viên hỗ trợ";
            var timestamp = Timestamp.FromDateTime(DateTime.UtcNow);

            // 1. Chuẩn bị tin nhắn
            var chatMessage = new ChatMessage
            {
                SenderName = displayName,
                Message = message,
                Timestamp = timestamp,
                IsStaffMessage = true
            };

            // 2. Lưu tin nhắn vào Firestore
            var messageRef = GetConversationsCol().Document(customerId).Collection("messages").Document();
            await messageRef.SetAsync(chatMessage);

            // 3. Cập nhật "tiêu đề" (đánh dấu đã đọc)
            await GetConversationsCol().Document(customerId).SetAsync(new { LastMessage = message, IsReadByStaff = true, LastMessageTimestamp = timestamp }, SetOptions.MergeAll);

            // 4. Gửi real-time cho Customer
            // Lấy connectionId mới nhất của khách từ Firestore
            var docSnap = await GetConversationsCol().Document(customerId).GetSnapshotAsync();
            if (docSnap.Exists && docSnap.TryGetValue<string>("CustomerConnectionId", out var customerConnectionId))
            {
                await Clients.Client(customerConnectionId).SendAsync(
                    "ReceiveStaffReply",
                    displayName,
                    message
                );
            }
        }
    }
}