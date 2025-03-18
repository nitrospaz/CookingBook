using CookingBook.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CookingBook.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public string Ingredients { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
        public string Categories { get; set; } = string.Empty;
        public string Tags { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string SourceUrl { get; set; } = string.Empty;

        public string Text { get; set; }

        public DateTime DateCreated { get; set; }
        public DateTime DateModified { get; set; }

        public Note()
        {
            DateModified = DateTime.Now;
            DateCreated = DateTime.Now;
            Text = "";
        }
    }
}
