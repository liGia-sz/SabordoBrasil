public class Comentario
{
    public int IdComentario { get; set; }
    public int PratoId { get; set; }
    public int UsuarioId { get; set; }
    public string NomeUsuario { get; set; }
    public string Texto { get; set; }
    public DateTime DataCriacao { get; set; }
    // Outras propriedades, se necessário
}
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/comentarios")]
public class ComentariosController : ControllerBase
{
    private readonly IComentarioService _comentarioService;
    private readonly IUsuarioService _usuarioService; // Para obter o nome do usuário

    public ComentariosController(IComentarioService comentarioService, IUsuarioService usuarioService)
    {
        _comentarioService = comentarioService;
        _usuarioService = usuarioService;
    }

    [HttpGet("{pratoId}")]
    public async Task<ActionResult<IEnumerable<Comentario>>> GetComentarios(int pratoId)
    {
        var comentarios = await _comentarioService.ListarComentariosPorPrato(pratoId);
        return Ok(comentarios);
    }

    [HttpPost]
    public async Task<IActionResult> PostComentario([FromBody] NovoComentarioRequest request)
    {
        var usuario = await _usuarioService.BuscarPorId(request.UsuarioId);
        if (usuario == null)
        {
            return BadRequest("Usuário não encontrado.");
        }

        var comentario = new Comentario
        {
            PratoId = request.PratoId,
            UsuarioId = request.UsuarioId,
            NomeUsuario = usuario.Nome,
            Texto = request.Texto,
            DataCriacao = DateTime.UtcNow
        };

        var sucesso = await _comentarioService.AdicionarComentario(comentario);
        if (sucesso)
        {
            return Ok();
        }
        return BadRequest("Erro ao adicionar comentário.");
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutComentario(int id, [FromBody] AtualizarComentarioRequest request)
    {
        var sucesso = await _comentarioService}