using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<Task> Tasks { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<Token> Tokens { get; set; }

    }
}
