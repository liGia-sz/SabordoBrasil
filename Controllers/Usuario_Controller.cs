using Microsoft.AspNetCore.Mvc;
using SeuProjeto.Data;
using SeuProjeto.Models;

namespace SeuProjeto.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("cadastrar")]
        public IActionResult Cadastrar([FromBody] Usuario usuario)
        {
            if (string.IsNullOrEmpty(usuario.Nome) || string.IsNullOrEmpty(usuario.Email) || string.IsNullOrEmpty(usuario.Senha))
            {
                return BadRequest(new { mensagem = "Por favor, preencha todos os campos." });
            }

            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                return BadRequest(new { mensagem = "E-mail já cadastrado." });
            }

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return Ok(new { mensagem = "Cadastro realizado com sucesso!" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] Usuario usuario)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.Email == usuario.Email && u.Senha == usuario.Senha);

            if (user == null)
            {
                return Unauthorized(new { mensagem = "E-mail ou senha inválidos." });
            }

            return Ok(new { mensagem = "Login realizado com sucesso!", usuario = user });
        }
    }
}