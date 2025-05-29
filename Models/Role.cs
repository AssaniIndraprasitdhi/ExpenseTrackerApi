using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExpenseTrackerApi.Models
{
    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = null!;

        // Navigation back to users
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}