using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDo.API.Models;
using ToDo.API.Repositories.Contracts;

namespace ToDo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TaskController(ITaskRepository taskRepository, UserManager<ApplicationUser> userManager)
        {
            _taskRepository = taskRepository;
            _userManager = userManager;
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddTask([FromBody] ToDo.API.Models.Task task)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized("Usuário não autenticado");
                }

                task.UserId = userId;
                task.DateCreation = DateTime.Now;
                _taskRepository.AddTask(task);

                return Ok("Tarefa adicionada com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao adicionar tarefa: {ex.Message}");
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteTask(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized("Usuário não autenticado");
                }

                var taskToDelete = _taskRepository.GetTask(id, userId);

                if (taskToDelete == null)
                {
                    return NotFound("Tarefa não encontrada");
                }

                _taskRepository.DeleteTask(taskToDelete);

                return Ok("Tarefa excluída com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao excluir tarefa: {ex.Message}");
            }
        }


        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetTask(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized("Usuário não autenticado");
                }

                var task = _taskRepository.GetTask(id, userId);

                if (task == null)
                {
                    return NotFound("Tarefa não encontrada ou não pertence ao usuário.");
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter tarefa: {ex.Message}");
            }
        }


        [Authorize]
        [HttpGet("")]
        public IActionResult GetTasks()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (userId == null)
                {
                    return Unauthorized("Usuário não autenticado");
                }

                var tasks = _taskRepository.GetTasks(userId);

                // Retornar as tarefas encontradas
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao obter tarefas: {ex.Message}");
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateTask(int id, [FromBody] ToDo.API.Models.Task task)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (userId == null)
                {
                    return Unauthorized("Usuário não autenticado");
                }

                var existingTask = _taskRepository.GetTask(id,userId);

                if (existingTask == null)
                {
                    return NotFound("Tarefa não encontrada");
                }

                var taskProperties = typeof(ToDo.API.Models.Task).GetProperties();
                bool anyFieldModified = false;

                foreach (var prop in taskProperties)
                {
                    if (prop.Name != "Id" && prop.Name != "DateUpdated")
                    {
                        var value = prop.GetValue(task);
                        if (value != null && !string.IsNullOrEmpty(value.ToString()))
                        {
                            prop.SetValue(existingTask, value);
                            anyFieldModified = true;
                        }
                    }
                }

                if (anyFieldModified)
                {
                    existingTask.DateUpdated = DateTime.Now;
                }

                _taskRepository.UpdateTask(existingTask);

                return Ok("Tarefa atualizada com sucesso");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao atualizar tarefa: {ex.Message}");
            }
        }

        [HttpGet("modelo")]
        public IActionResult Modelo()
        {
            return Ok(new ToDo.API.Models.Task());
        }
    }
}
