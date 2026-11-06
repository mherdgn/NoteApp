
using System;

namespace NoteApp.Models
{
    public class Note
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public bool IsFavorite { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        // Notun sahibi olan kullanıcı
        public int? UserId { get; set; }

        // Kullanıcı ilişkisi
        public User? User { get; set; }
    }
}
