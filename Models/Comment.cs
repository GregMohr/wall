using System.ComponentModel.DataAnnotations;
using System;

namespace wall.Models
{
    public class Comment
    {
        [Key]
        public int id { get; set; }
        [Required]
        [MinLength(2)]
        public string comment { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int userid { get; set; }
        public int messageid { get; set; }
        public User user { get; set; }
    }
}