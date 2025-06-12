using Microsoft.AspNetCore.Mvc;
using System.Linq;
using SeuProjeto.Data;
using SeuProjeto.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SeuProjeto.Controllers
{
    [Route("[controller]")]
    public class PratoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PratoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Exibir todos os pratos (padrão + usuários)
        public IActionResult Index()
        {
            var pratos = _context.Pratos.ToList();
            return View(pratos);
        }

        // Formulário para novo prato
        public IActionResult Create()
        {
            return View();
        }

        // Salvar novo prato
        [HttpPost]
        public IActionResult Create(Prato model, IFormFile imagem)
        {
            if (ModelState.IsValid)
            {
                if (imagem != null && imagem.Length > 0)
                {
                    var fileName = Path.GetFileName(imagem.FileName);
                    var dirPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img");
                    if (!Directory.Exists(dirPath))
                        Directory.CreateDirectory(dirPath);

                    var filePath = Path.Combine(dirPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        imagem.CopyTo(stream);
                    }

                    model.Foto = "/img/" + fileName;
                }

                _context.Pratos.Add(model);
                _context.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet("ListaJson")]
        public IActionResult ListaJson()
        {
            var pratos = _context.Pratos.ToList();
            return Json(pratos);
        }
    }
}