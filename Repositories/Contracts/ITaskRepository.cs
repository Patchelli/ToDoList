namespace ToDo.API.Repositories.Contracts
{
    public interface ITaskRepository
    {
        void AddTask(Models.Task task);
        void DeleteTask(Models.Task task);
        void UpdateTask(Models.Task task);
        Models.Task? GetTask(int id , string userId);
        List<Models.Task> GetTasks(string email);
    }
}
