using Microsoft.AspNetCore.Mvc;
using SeuProjeto.Data;
using SeuProjeto.Models;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

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

            // Criptografar a senha antes de salvar
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(usuario.Senha));
                usuario.Senha = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return Ok(new { mensagem = "Cadastro realizado com sucesso!" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            // Criptografar a senha recebida para comparar
            string senhaCriptografada;
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(model.Senha));
                senhaCriptografada = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }

            var usuario = _context.Usuarios
                .FirstOrDefault(u => u.Email == model.Email && u.Senha == senhaCriptografada);

            if (usuario != null)
                return Ok(new { sucesso = true, nome = usuario.Nome });

            return Unauthorized(new { sucesso = false, mensagem = "E-mail ou senha incorretos." });
        }
    }
}