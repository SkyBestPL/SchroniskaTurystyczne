using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SchroniskaTurystyczne.Models;

namespace SchroniskaTurystyczne.ViewModels
{
    public class MessageViewModel
    {
        public string CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }
        public List<ConversationViewModel> Conversations { get; set; }
        public ConversationViewModel CurrentConversation { get; set; }
        [Required]
        [StringLength(1000, ErrorMessage = "Wiadomość nie może być dłuższa niż 1000 znaków.")]
        public string NewMessageContent { get; set; }
        public int? InitialShelterId { get; set; }
        public ReceiverViewModel Receiver { get; set; }
    }

    public class ConversationViewModel
    {
        public string OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public bool IsExhibitor { get; set; }
        public bool IsAdmin { get; set; }
        public ShelterInfoViewModel? RelatedShelter { get; set; }
        public List<MessageInfoViewModel>? Messages { get; set; }
        public bool IsNewConversation { get; set; }
        public int UnreadMessagesCount { get; set; }
    }

    public class MessageInfoViewModel
    {
        public int Id { get; set; }
        public string Contents { get; set; }
        public DateTime SentAt { get; set; }
        public bool IsRead { get; set; }
        public bool IsSentByCurrentUser { get; set; }
    }

    public class ShelterInfoViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ReceiverViewModel
    {
        [Required]
        public string Id { get; set; }
        public string DisplayName { get; set; }
    }
}