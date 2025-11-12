using EVCenterService.Models; 
using EVCenterService.ViewModels;
using Google.Cloud.Firestore; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims; 
using System.Threading.Tasks; 

namespace EVCenterService.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly FirestoreDb _firestoreDb; 

        // SỬA HÀM KHỞI TẠO (Constructor)
        public IndexModel(ILogger<IndexModel> logger, FirestoreDb firestoreDb)
        {
            _logger = logger;
            _firestoreDb = firestoreDb; 
        }

        public void OnGet()
        {
   
        }

        public async Task<IActionResult> OnGetMyMessagesAsync()
        {
            // Lấy ID của Customer đang đăng nhập
            var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(customerId) || !User.IsInRole("Customer"))
            {
                return BadRequest();
            }

            // Truy vấn Firestore
            var messagesCol = _firestoreDb.Collection("conversations").Document(customerId).Collection("messages");
            var query = messagesCol.OrderBy("Timestamp");
            var snapshot = await query.GetSnapshotAsync();

            var messages = snapshot.Documents.Select(doc => doc.ConvertTo<ChatMessage>()).ToList();

            return new JsonResult(messages);
        }
    }
}