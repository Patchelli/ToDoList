using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDo.API.Models;

namespace ToDo.API.Database
{
    public class TaskContext : IdentityDbContext<ApplicationUser>
    {
        public TaskContext(DbContextOptions<TaskContext> options) : base(options)
        {
        }

        public DbSet<ToDo.API.Models.Task> Tasks { get; set; }
    }
}
