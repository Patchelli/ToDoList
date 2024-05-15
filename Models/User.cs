using System.ComponentModel.DataAnnotations;

namespace ToDo.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
    }
}
