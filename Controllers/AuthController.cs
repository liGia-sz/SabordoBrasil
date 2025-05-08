using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.Password;
using System.Threading.Tasks;

// Modelo para a requisição de login
public class LoginRequest
{
    public string Email { get; set; }
    public string Senha { get; set; }
}

// Modelo para a resposta de erro de login
public class LoginErrorResponse
{
    public string Message { get; set; }
    public string Field { get; set; } // Opcional: para indicar qual campo tem o erro
}

// Modelo para a resposta de sucesso do login
public class LoginSuccessResponse
{
    public int UserId { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string ImagemUrl { get; set; } // Adicione a propriedade para a URL da imagem do usuário
    public int TotalLikes { get; set; }
    public int TotalDislikes { get; set; }
}

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IPasswordHasher<Usuario> _passwordHasher; // Use seu modelo de Usuário
    private readonly IUsuarioService _usuarioService; // Sua interface de serviço para usuários

    public AuthController(IPasswordHasher<Usuario> passwordHasher, IUsuarioService usuarioService)
    {
        _passwordHasher = passwordHasher;
        _usuarioService = usuarioService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        if (string.IsNullOrEmpty(model.Email))
        {
            return BadRequest(new LoginErrorResponse { Message = "O email é obrigatório.", Field = "email" });
        }
        if (string.IsNullOrEmpty(model.Senha))
        {
            return BadRequest(new LoginErrorResponse { Message = "A senha é obrigatória.", Field = "senha" });
        }

        var usuario = await _usuarioService.BuscarPorEmail(model.Email);

        if (usuario == null)
        {
            return Unauthorized(new LoginErrorResponse { Message = "Usuário ou senha incorreto." });
        }

        var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, model.Senha);

        if (result == PasswordVerificationResult.Success)
        {
            // Lógica para buscar o total de likes e dislikes do usuário
            var totais = await _usuarioService.ObterTotalLikesDislikes(usuario.IdUsuarios);

            return Ok(new LoginSuccessResponse
            {
                UserId = usuario.IdUsuarios,
                Nome = usuario.Nome,
                Email = usuario.Email,
                ImagemUrl = usuario.ImagemUrl, // Supondo que seu modelo de Usuário tenha essa propriedade
                TotalLikes = totais.TotalLikes,
                TotalDislikes = totais.TotalDislikes
            });
        }
        else
        {
            return Unauthorized(new LoginErrorResponse { Message = "Usuário ou senha incorreto." });
        }
    }
}