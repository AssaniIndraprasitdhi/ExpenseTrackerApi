using System;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        public byte[] PasswordHash { get; set; } = null!;

        [Required]
        public byte[] PasswordSalt { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Role relationship
        [Required]
        public int RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
