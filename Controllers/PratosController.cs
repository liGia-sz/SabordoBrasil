using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SabordoBrasil.Services;
using SabordoBrasil.Models;
namespace SabordoBrasil.Controllers
{
    // Modelo para representar um Prato
    public class PratoResponse
    {
        public int IdPrato { get; set; }
        public string? NomePrato { get; set; }
        public string? Descricao { get; set; }
        public string? Foto { get; set; }
        public string? Local { get; set; }
        public string? Cidade { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public int NumeroComentarios { get; set; }
        public bool UsuarioCurtiu { get; set; }
        public bool UsuarioDescurtiu { get; set; }
    }
namespace SabordoBrasil.Models
{
    public class Prato
    {
        public int IdPrato { get; set; }
        public string? NomePrato { get; set; }
        public string? Descricao { get; set; }
        public string? Foto { get; set; }
        public string? Local { get; set; }
        public string? Cidade { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public List<Comentario> Comentarios { get; set; } = new();
        public bool UsuarioCurtiu { get; set; }
        public bool UsuarioDescurtiu { get; set; }
    }
}

namespace SabordoBrasil.Services
{
    public interface IPratoService
    {
        Task<IEnumerable<Prato>> ListarPratos();
        Task<IEnumerable<Prato>> ListarPratosComInteracao(int usuarioId);
        Task<bool> AdicionarLike(int idPrato, int usuarioId);
        Task<bool> RemoverLike(int idPrato, int usuarioId);
        Task<bool> AdicionarDislike(int idPrato, int usuarioId);
        Task<bool> RemoverDislike(int idPrato, int usuarioId);
    }

        public class Prato
        {
        }
    }

    [ApiController]
    [Route("api/pratos")]
    public class PratosController : ControllerBase
    {
        private readonly IPratoService _pratoService;

        public PratosController(IPratoService pratoService)
        {
            _pratoService = pratoService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PratoResponse>>> GetPratos([FromQuery] int? usuarioId)
        {
            var pratos = await _pratoService.ListarPratos();
            var pratosResponse = new List<PratoResponse>();

            if (usuarioId.HasValue)
            {
                var pratosComInteracao = await _pratoService.ListarPratosComInteracao(usuarioId.Value);
                pratosResponse = pratosComInteracao.Select(p => MapPratoParaResponseComInteracao(p)).ToList();
            }
            else
            {
                pratosResponse = pratos.Select(p => MapPratoParaResponseSemInteracao(p)).ToList();
            }

            return Ok(pratosResponse);
        }

        private PratoResponse MapPratoParaResponseSemInteracao(Prato prato) => new PratoResponse
        {
            IdPrato = prato.IdPrato,
            NomePrato = prato.NomePrato,
            Descricao = prato.Descricao,
            Foto = prato.Foto,
            Local = prato.Local,
            Cidade = prato.Cidade,
            Likes = prato.Likes,
            Dislikes = prato.Dislikes,
            NumeroComentarios = prato.Comentarios,
            UsuarioCurtiu = false,
            UsuarioDescurtiu = false
        };
        private PratoResponse MapPratoParaResponseComInteracao(Prato prato)
        {
            return new PratoResponse
            {
                IdPrato = prato.IdPrato,
                NomePrato = prato.NomePrato,
                Descricao = prato.Descricao,
                Foto = prato.Foto,
                Local = prato.Local,
                Cidade = prato.Cidade,
                Likes = prato.Likes,
                Dislikes = prato.Dislikes,
                NumeroComentarios = prato.Comentarios,
                UsuarioCurtiu = prato.UsuarioCurtiu,
                UsuarioDescurtiu = prato.UsuarioDescurtiu
            };
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikePrato(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();
            var resultado = await _pratoService.AdicionarLike(id, usuarioId);
            if (resultado)
            {
                return NoContent();
            }
            return BadRequest("Não foi possível dar like.");
        }

        [HttpDelete("{id}/like")]
        [Authorize]
        public async Task<IActionResult> UnlikePrato(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();
            var resultado = await _pratoService.RemoverLike(id, usuarioId);
            if (resultado)
            {
                return NoContent();
            }
            return BadRequest("Não foi possível remover o like.");
        }

        [HttpPost("{id}/dislike")]
        [Authorize]
        public async Task<IActionResult> DislikePrato(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();
            var resultado = await _pratoService.AdicionarDislike(id, usuarioId);
            if (resultado)
            {
                return NoContent();
            }
            return BadRequest("Não foi possível dar dislike.");
        }

        [HttpDelete("{id}/dislike")]
        [Authorize]
        public async Task<IActionResult> UndislikePrato(int id)
        {
            var usuarioId = ObterUsuarioIdLogado();
            var resultado = await _pratoService.RemoverDislike(id, usuarioId);
            if (resultado)
            {
                return NoContent();
            }
            return BadRequest("Não foi possível remover o dislike.");
        }

        private int ObterUsuarioIdLogado()
{
    var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
    return claim != null ? int.Parse(claim) : throw new UnauthorizedAccessException("Usuário não autenticado.");
}

    }
}