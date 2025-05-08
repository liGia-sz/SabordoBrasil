using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SabordoBrasil.Controllers
{
    [ApiController]
    [Route("api/comentarios")]
    public class CommentsController : ControllerBase
    {
        private readonly IComentarioService _comentarioService;
        private readonly SeuDbContext _context;

        public CommentsController(IComentarioService comentarioService, SeuDbContext context)
        {
            _comentarioService = comentarioService;
            _context = context;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AdicionarComentario([FromBody] ComentarioDTO comentarioDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var usuarioId = ObterUsuarioIdLogado();
            var pratoExiste = await _context.Pratos.AnyAsync(p => p.IdPrato == comentarioDTO.PratoId);

            if (!pratoExiste)
            {
                return NotFound("Prato não encontrado.");
            }

            var novoComentario = new Comentario
            {
                PratoId = comentarioDTO.PratoId,
                UsuarioId = usuarioId,
                Texto = comentarioDTO.Texto,
                DataCriacao = DateTime.UtcNow,
                NomeUsuario = User.FindFirstValue(ClaimTypes.Name)
            };

            await _comentarioService.AdicionarComentario(novoComentario);
            return CreatedAtAction("GetComentario", new { id = novoComentario.IdComentario }, novoComentario);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Comentario>> GetComentario(int id)
        {
            var comentario = await _comentarioService.ObterComentarioPorId(id);
            if (comentario == null)
            {
                return NotFound();
            }
            return Ok(comentario);
        }

        private int ObterUsuarioIdLogado()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }
    }

    public class ComentarioDTO
    {
        public int PratoId { get; set; }
        public string ?Texto { get; set; }
    }
}