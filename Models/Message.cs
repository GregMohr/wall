using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;


namespace wall.Models
{
    public class Message
    {
        public Message()
        {
            comments = new List<Comment>();
        }
        [Key]
        public int id { get; set; }
        [Required]
        [MinLength(2)]
        public string message { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int userid { get; set; }
        public User user { get; set; }
        public ICollection<Comment> comments { get; set; }
    }
}