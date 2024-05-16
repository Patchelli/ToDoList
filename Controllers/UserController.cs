using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using ToDo.API.Models;
using ToDo.API.Repositories.Contracts;

namespace ToDo.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(IUserRepository userRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _userManager = userManager;
        }   

        [HttpPost("register")]
        public IActionResult Register([FromBody] User user)
        {
            try
            {
                if (!ModelState.IsValid)
                    return UnprocessableEntity(ModelState);

                var newUser = new ApplicationUser { FullName = user.Name, Email = user.Email, UserName = user.Email };
                var result = _userManager.CreateAsync(newUser, user.Password).Result;

                if (!result.Succeeded)
                {
                    var errors = new List<string>();
                    foreach (var error in result.Errors)
                    {
                        errors.Add(error.Description);
                    }
                    return BadRequest($"Usuário não cadastrado: {string.Join(", ", errors)}");
                }

                return Ok(newUser);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao cadastrar usuário: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] User user)
        {
            try
            {
                ModelState.Remove("Name");
                ModelState.Remove("ConfirmPassword");

                if (!ModelState.IsValid)
                    return UnprocessableEntity(ModelState);

                var userLogin = _userRepository.GetUser(user.Email, user.Password);

                if (userLogin == null)
                    return NotFound("Usuário não encontrado");

                _signInManager.SignInAsync(userLogin, true);

                // retornar token JWT
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao autenticar usuário: {ex.Message}");
            }
        }
    }
}
