using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.API.Models
{
    public class Token
    {
        public int Id { get; set; }
        public string RefreshToken { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool IsExpired { get; set; }
        public DateTime ExpirationRefreshToken { get; set; }
        public DateTime ExpirationToken { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime? DateUpdate { get; set; }

    }
}
