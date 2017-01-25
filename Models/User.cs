using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace wall.Models
{
    public class User
    {
        public User(){
            messages = new List<Message>();
        }
        [Key]
        public int id { get; set; }
        [Required]
        [MinLength(2)]
        public string first { get; set; }

        [Required]
        [MinLength(2)]
        public string last { get; set; }
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        [MinLength(8)]        
        [DataType(DataType.Password)]
        public string password { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public ICollection<Message> messages { get; set; }
    }
}