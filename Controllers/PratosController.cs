using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; // Se você precisar proteger as rotas

// Modelo para representar um Prato
public class PratoResponse
{
    public int IdPrato { get; set; }
    public string NomePrato { get; set; }
    public string Descricao { get; set; }
    public string Foto { get; set; }
    public string Local { get; set; }
    public string Cidade { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public int Comentarios { get; set; }
    public bool UsuarioCurtiu { get; set; } // Indica se o usuário logado curtiu
    public bool UsuarioDescurtiu { get; set; } // Indica se o usuário logado descurtiu
}

[ApiController]
[Route("api/pratos")]
public class PratosController : ControllerBase
{
    private readonly IPratoService _pratoService; // Sua interface de serviço para pratos
    // Supondo que você tenha acesso ao ID do usuário logado de alguma forma (autenticação)
    // Para este exemplo, vamos passar o ID do usuário como parâmetro nas requisições.

    public PratosController(IPratoService pratoService)
    {
        _pratoService = pratoService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PratoResponse>>> GetPratos([FromQuery] int? usuarioId)
    {
        if (!usuarioId.HasValue)
        {
            // Se não houver usuário logado, retorne os pratos sem informações de interação do usuário
            var pratos = await _pratoService.ListarPratos();
            return Ok(pratos.Select(p => new PratoResponse
            {
                IdPrato = p.IdPrato,
                NomePrato = p.NomePrato,
                Descricao = p.Descricao,
                Foto = p.Foto,
                Local = p.Local,
                Cidade = p.Cidade,
                Likes = p.Likes,
                Dislikes = p.Dislikes,
                Comentarios = p.Comentarios,
                UsuarioCurtiu = false,
                UsuarioDescurtiu = false
            }));
        }
        else
        {
            // Se houver um usuário logado, retorne os pratos com informações de interação
            var pratosComInteracao = await _pratoService.ListarPratosComInteracao(usuarioId.Value);
            return Ok(pratosComInteracao.Select(p => new PratoResponse
            {
                IdPrato = p.IdPrato,
                NomePrato = p.NomePrato,
                Descricao = p.Descricao,
                Foto = p.Foto,
                Local = p.Local,
                Cidade = p.Cidade,
                Likes = p.Likes,
                Dislikes = p.Dislikes,
                Comentarios = p.Comentarios,
                UsuarioCurtiu = p.UsuarioCurtiu,
                UsuarioDescurtiu = p.UsuarioDescurtiu
            }));
        }
    }

    [HttpPost("{id}/like")]
    public async Task<IActionResult> LikePrato(int id, [FromQuery] int usuarioId)
    {
        var resultado = await _pratoService.AdicionarLike(id, usuarioId);
        if (resultado)
        {
            return Ok(); // Ou retorne o novo número de likes
        }
        return BadRequest("Não foi possível dar like.");
    }

    [HttpDelete("{id}/like")]
    public async Task<IActionResult> UnlikePrato(int id, [FromQuery] int usuarioId)
    {
        var resultado = await _pratoService.RemoverLike(id, usuarioId);
        if (resultado)
        {
            return Ok(); // Ou retorne o novo número de likes
        }
        return BadRequest("Não foi possível remover o like.");
    }

    [HttpPost("{id}/dislike")]
    public async Task<IActionResult> DislikePrato(int id, [FromQuery] int usuarioId)
    {
        var resultado = await _pratoService.AdicionarDislike(id, usuarioId);
        if (resultado)
        {
            return Ok(); // Ou retorne o novo número de dislikes
        }
        return BadRequest("Não foi possível dar dislike.");
    }

    [HttpDelete("{id}/dislike")]
    public async Task<IActionResult> UndislikePrato(int id, [FromQuery] int usuarioId)
    {
        var resultado = await _pratoService.RemoverDislike(id, usuarioId);
        if (resultado)
        {
            return Ok(); // Ou retorne o novo número de dislikes
        }
        return BadRequest("Não foi possível remover o dislike.");
    }
}