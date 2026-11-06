using System;

namespace NoteApp.Models
{
    public class NoteFavorite
    {
        public int Id { get; set; }
        public int NoteId { get; set; }
        public string UserIdentifier { get; set; } = "guest"; // kullanıcı sistemi yoksa varsayılan
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsFavorite { get; set; } // favorilenen notları listeleme.

        // Navigation
        public Note Note { get; set; }
    }

    }
