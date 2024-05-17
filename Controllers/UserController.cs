using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ToDo.API.DTO;
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
        private readonly IConfiguration _configuration;
        private readonly ITokenRepository _tokenRepository;

        public UserController(IUserRepository userRepository, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IConfiguration configuration, ITokenRepository tokenRepository)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _tokenRepository = tokenRepository;
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

                //_signInManager.SignInAsync(userLogin, true);

                return BuildShapeToken(userLogin);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao autenticar usuário: {ex.Message}");
            }
        }

        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] TokenDTO tokenDTO)
        {
            try
            {
                var refreshTokenDB = _tokenRepository.GetToken(tokenDTO.RefreshToken);

                if (refreshTokenDB == null)
                    return NotFound();

                refreshTokenDB.DateUpdate = DateTime.UtcNow;
                refreshTokenDB.IsExpired = true;

                _tokenRepository.UpdateToken(refreshTokenDB);

                var user = _userRepository.GetUserById(refreshTokenDB.UserId);
                
                return BuildShapeToken(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Erro ao atualizar token: {ex.Message}");
            }
        }   

        private TokenDTO BuildToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName, user.Email),
                new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub,user.Id)
            };

            var jwtKey = _configuration["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey??""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(1);

            JwtSecurityToken token = new JwtSecurityToken(
                                issuer: null,
                                audience: null,
                                claims: claims,
                                expires: expiration,
                                signingCredentials: creds
                            );


            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            var expirationRefreshToken = DateTime.UtcNow.AddHours(2);
            var refreshToken = Guid.NewGuid().ToString();

            var tokenDTO = new TokenDTO { Token = tokenString , ExpirationToken = expiration , RefreshToken = refreshToken, ExpirationRefreshToken = expirationRefreshToken };

            return tokenDTO;

        }
        private IActionResult BuildShapeToken(ApplicationUser userLogin)
        {
            // retornar token JWT
            var token = BuildToken(userLogin);

            var tokenModel = new Token
            {
                RefreshToken = token.RefreshToken,
                ExpirationRefreshToken = token.ExpirationRefreshToken,
                ExpirationToken = token.ExpirationToken,
                User = userLogin,
                DateCreation = DateTime.UtcNow,
                IsExpired = false
            };

            _tokenRepository.AddToken(tokenModel);

            return Ok(token);
        }
    }
}
