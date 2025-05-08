using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace SabordoBrasil.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IPasswordHasher<UsuarioDTO> _passwordHasher;

        public SeuDbContext Context { get; }

        public AuthController(SeuDbContext context, IPasswordHasher<UsuarioDTO> passwordHasher)
        {
            Context = context;
            _passwordHasher = passwordHasher;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UsuarioDTO usuarioDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Verifique se o email já existe
            if (await Context.Usuarios.AnyAsync(u => u.Email == usuarioDTO.Email))
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

            Context.Usuarios.Add(novoUsuario);
            await Context.SaveChangesAsync();

            return Ok(new { Message = "Usuário registrado com sucesso!" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuario = await Context.Usuarios.FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
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