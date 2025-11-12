using Google.Cloud.Firestore;

namespace EVCenterService.ViewModels
{
    [FirestoreData]
    public class ChatConversation
    {

        [FirestoreProperty]
        public string CustomerName { get; set; }
        [FirestoreProperty]
        public string LastMessage { get; set; }
        [FirestoreProperty]
        public Timestamp LastMessageTimestamp { get; set; }
        [FirestoreProperty]
        public bool IsReadByStaff { get; set; }
        [FirestoreProperty]
        public string CustomerConnectionId { get; set; } // ConnectionId cuối cùng
    }
}
