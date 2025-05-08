using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SabordoBrasil.Controllers
{
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
            try
            {
                var pratos = await _pratoService.ListarPratos();
                var pratosResponse = pratos.Select(p => MapPratoParaResponse(p, usuarioId)).ToList();
                return Ok(pratosResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao listar os pratos: {ex.Message}");
            }
        }

        private PratoResponse MapPratoParaResponse(Prato prato, int? usuarioId)
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
                NumeroComentarios = prato.Comentarios.Count,
                UsuarioCurtiu = usuarioId.HasValue && prato.UsuarioCurtiu,
                UsuarioDescurtiu = usuarioId.HasValue && prato.UsuarioDescurtiu
            };
        }

        [HttpPost("{id}/like")]
        [Authorize]
        public async Task<IActionResult> LikePrato(int id)
        {
            try
            {
                if (!await PratoExiste(id))
                {
                    return NotFound("Prato não encontrado.");
                }

                var usuarioId = ObterUsuarioIdLogado();
                var resultado = await _pratoService.AdicionarLike(id, usuarioId);
                if (resultado)
                {
                    return NoContent();
                }
                return BadRequest("Não foi possível dar like no prato.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao dar like no prato: {ex.Message}");
            }
        }

        [HttpDelete("{id}/like")]
        [Authorize]
        public async Task<IActionResult> UnlikePrato(int id)
        {
            try
            {
                if (!await PratoExiste(id))
                {
                    return NotFound("Prato não encontrado.");
                }

                var usuarioId = ObterUsuarioIdLogado();
                var resultado = await _pratoService.RemoverLike(id, usuarioId);
                if (resultado)
                {
                    return NoContent();
                }
                return BadRequest("Não foi possível remover o like do prato.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro ao remover o like do prato: {ex.Message}");
            }
        }

        private async Task<bool> PratoExiste(int id)
        {
            return await _pratoService.PratoExiste(id);
        }

        private int ObterUsuarioIdLogado()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(claim))
            {
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            return int.Parse(claim);
        }
    }
}