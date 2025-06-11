using Microsoft.AspNetCore.Mvc;
using System.Linq;
using SeuProjeto.Data;
using SeuProjeto.Models;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace SeuProjeto.Controllers
{
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
        [ValidateAntiForgeryToken]
        public IActionResult Create(Prato prato, IFormFile fotoUpload)
        {
            if (ModelState.IsValid)
            {
                if (fotoUpload != null && fotoUpload.Length > 0)
                {
                    var fileName = Path.GetFileName(fotoUpload.FileName);
                    var filePath = Path.Combine("wwwroot/img", fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        fotoUpload.CopyTo(stream);
                    }

                    prato.Foto = "/img/" + fileName;
                }

                _context.Pratos.Add(prato);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(prato);
        }

        [HttpGet]
        public IActionResult ListaJson()
        {
            var pratos = _context.Pratos.ToList();
            return Json(pratos);
        }
    }
}