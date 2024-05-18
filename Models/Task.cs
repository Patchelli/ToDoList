using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.API.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DateHour { get; set; }
        public DateTime DateCreation { get; set; }
        public DateTime DateUpdated { get; set; }
        public string Location { get; set; }
        public string Type { get; set; }
        public bool IsComplete { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }

    }
}
