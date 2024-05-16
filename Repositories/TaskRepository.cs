using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using ToDo.API.Database;
using ToDo.API.Models;
using ToDo.API.Repositories.Contracts;

namespace ToDo.API.Repositories
{
    public class TaskRepository : ITaskRepository
    {
        private readonly TaskContext _taskcontext;

        public TaskRepository(TaskContext taskContext)
        {
            _taskcontext = taskContext;
        }

        public void AddTask(ToDo.API.Models.Task task)
        {
            try
            {
                _taskcontext.Tasks.Add(task);
                _taskcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao adicionar tarefa: " + ex.Message);
            }
        }

        public void DeleteTask(ToDo.API.Models.Task task)
        {
            try
            {
                _taskcontext.Tasks.Remove(task);
                _taskcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                // Aqui você pode tratar a exceção, logá-la ou relançá-la conforme necessário
                throw new Exception("Erro ao excluir tarefa: " + ex.Message);
            }
        }


        public ToDo.API.Models.Task? GetTask(int id, string userId)
        {
            try
            {
                return _taskcontext.Tasks.FirstOrDefault(t => t.Id == id && t.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter tarefa: " + ex.Message);
            }
        }


        public List<ToDo.API.Models.Task> GetTasks(string userId)
        {
            try
            {
                return _taskcontext.Tasks.Where(t => t.UserId == userId).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao obter tarefas: " + ex.Message);
            }
        }


        public void UpdateTask(ToDo.API.Models.Task task)
        {
            try
            {
                _taskcontext.Tasks.Update(task);
                _taskcontext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao atualizar tarefa: " + ex.Message);
            }
        }
    }

}
