using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

// Modelo para representar um Prato (deve corresponder à sua tabela no banco)
public class PratoDto
{
    public int IdPrato { get; set; }
    public string NomePrato { get; set; }
    public string Descricao { get; set; }
    public string Foto { get; set; }
    public string Local { get; set; } // Supondo que você tenha essa informação
    public string Cidade { get; set; } // Supondo que você tenha essa informação
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public int Comentarios { get; set; }
}

[ApiController]
[Route("api/pratos")]
public class PratosController : ControllerBase
{
    // Aqui você injetaria seu serviço ou repositório para acessar os dados do banco
    private static readonly List<PratoDto> _pratosSimulados = new List<PratoDto>()
    {
        new PratoDto { IdPrato = 1, NomePrato = "Feijão Tropeiro", Descricao = "Delicioso...", Foto = "/images/feijaotropeiro.jpg", Local = "BH", Cidade = "MG", Likes = 10, Dislikes = 2, Comentarios = 5 },
        new PratoDto { IdPrato = 2, NomePrato = "Lasanha", Descricao = "Clássica...", Foto = "/images/lasanha.jpg", Local = "SP", Cidade = "SP", Likes = 15, Dislikes = 1, Comentarios = 8 },
        // ... adicione mais pratos simulados
    };

    [HttpGet]
    public ActionResult<IEnumerable<PratoDto>> GetPratos()
    {
        // Em um cenário real, você buscaria os dados do banco aqui
        return Ok(_pratosSimulados);
    }
}