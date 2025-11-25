using Google.Cloud.Firestore;

namespace EVCenterService.ViewModels
{
    [FirestoreData] 
    public class ChatMessage
    {
        [FirestoreProperty]
        public string SenderName { get; set; }
        [FirestoreProperty]
        public string Message { get; set; }
        [FirestoreProperty]
        public Timestamp Timestamp { get; set; }
        [FirestoreProperty]
        public bool IsStaffMessage { get; set; }
    }
}
