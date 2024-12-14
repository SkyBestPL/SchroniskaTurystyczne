using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SchroniskaTurystyczne.Models;

namespace SchroniskaTurystyczne.ViewModels
{
    public class MessageViewModel
    {
        // Informacje o obecnym użytkowniku (tylko dane niepoufne)
        public string CurrentUserId { get; set; }
        public string CurrentUserName { get; set; }

        // Lista konwersacji
        public List<ConversationViewModel> Conversations { get; set; }

        // Wybrana aktualnie konwersacja
        public ConversationViewModel CurrentConversation { get; set; }

        // Treść nowej wiadomości
        [Required]
        [StringLength(1000, ErrorMessage = "Wiadomość nie może być dłuższa niż 1000 znaków.")]
        public string NewMessageContent { get; set; }

        // ID schroniska, jeśli rozpoczynamy nową konwersację
        public int? InitialShelterId { get; set; }

        // Informacje o odbiorcy (w przypadku nowej konwersacji)
        public ReceiverViewModel Receiver { get; set; }
    }

    public class ConversationViewModel
    {
        // ID użytkownika, z którym trwa konwersacja
        public string OtherUserId { get; set; }
        public string OtherUserName { get; set; }

        // Informacje o powiązanym schronisku
        public ShelterInfoViewModel RelatedShelter { get; set; }

        // Lista wiadomości w tej konwersacji
        public List<MessageInfoViewModel> Messages { get; set; }

        // Czy jest to nowa konwersacja
        public bool IsNewConversation { get; set; }
    }

    public class MessageInfoViewModel
    {
        // ID wiadomości
        public int Id { get; set; }

        // Treść wiadomości
        public string Contents { get; set; }

        // Data i godzina wysłania wiadomości
        public DateTime SentAt { get; set; }

        // Czy wiadomość została wysłana przez obecnego użytkownika
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