using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SabordoBrasil.Models;

namespace SabordoBrasil.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IPasswordHasher<UsuarioDTO> _passwordHasher;
        private readonly SeuDbContext _context;

        public AuthController(SeuDbContext context, IPasswordHasher<UsuarioDTO> passwordHasher)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UsuarioDTO usuarioDTO)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(usuarioDTO.Nome) || string.IsNullOrWhiteSpace(usuarioDTO.Email) || string.IsNullOrWhiteSpace(usuarioDTO.Senha))
            {
                return BadRequest("Todos os campos são obrigatórios.");
            }

            if (await _context.Usuarios.AnyAsync(u => u.Email == usuarioDTO.Email))
            {
                return Conflict("Email já está em uso.");
            }

            var novoUsuario = new Usuario
            {
                Nome = usuarioDTO.Nome,
                Email = usuarioDTO.Email,
                ImagemUrl = usuarioDTO.ImagemUrl
            };

            novoUsuario.Senha = _passwordHasher.HashPassword(novoUsuario, usuarioDTO.Senha);

            _context.Usuarios.Add(novoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuário registrado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(loginDTO.Email) || string.IsNullOrWhiteSpace(loginDTO.Senha))
            {
                return BadRequest("Email e senha são obrigatórios.");
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
            if (usuario == null)
            {
                return Unauthorized("Email ou senha inválidos.");
            }

            var resultadoVerificacao = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, loginDTO.Senha);

            if (resultadoVerificacao == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Email ou senha inválidos.");
            }

            return Ok(new { Message = "Login realizado com sucesso!" });
        }
    }

    public class UsuarioDTO
    {
        public string? Nome { get; set; }
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public string? ImagemUrl { get; set; }
    }

    public class LoginDTO
    {
        public string? Email { get; set; }
        public string? Senha { get; set; }
    }
}