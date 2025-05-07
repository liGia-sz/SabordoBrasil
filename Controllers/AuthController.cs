using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.Password;

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

// Modelo para a resposta de sucesso do login (você pode incluir informações do usuário)
public class LoginSuccessResponse
{
    public int UserId { get; set; }
    public string Nome { get; set; }
    // Outras informações do usuário que você queira retornar
}

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IPasswordHasher<UsuarioDto> _passwordHasher;
    // Supondo que você tenha um serviço ou repositório para acessar os usuários no banco
    private readonly IUsuarioService _usuarioService; // Crie esta interface e implementação

    public AuthController(IPasswordHasher<UsuarioDto> passwordHasher, IUsuarioService usuarioService)
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

        // Verificar a senha usando o PasswordHasher
        var result = _passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, model.Senha);

        if (result == PasswordVerificationResult.Success)
        {
            // Login bem-sucedido
            return Ok(new LoginSuccessResponse { UserId = usuario.idUsuarios, Nome = usuario.Nome });
        }
        else
        {
            // Senha incorreta
            return Unauthorized(new LoginErrorResponse { Message = "Usuário ou senha incorreto." });
        }
    }
}