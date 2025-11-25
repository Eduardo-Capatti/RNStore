using Microsoft.AspNetCore.Mvc;
using RNStore.Repositories;
using Microsoft.Data.SqlClient;
using RNStore.Models;

namespace RNStore.Controllers;

public class EstoqueController: Controller
{
    private IEstoqueRepository repository;

    public EstoqueController(IEstoqueRepository repository)
    {
        this.repository = repository;
    }

    public ActionResult Filtrar(List<int> coresSelecionadas, List<int> tamanhosSelecionados, List<int> marcasSelecionadas, string nomeCalcado, int idProduto, decimal valorMinimo, decimal valorMaximo)
    {
        var index = repository.ReadFiltro(coresSelecionadas, tamanhosSelecionados, marcasSelecionadas, nomeCalcado, idProduto, valorMinimo, valorMaximo);
        ViewBag.Filtros = repository.Filtros();
        return View("Index", index);
    }

    public bool VerificaSession()
    {
        int? idUsuario = HttpContext.Session.GetInt32("idUsuario");

        if (idUsuario == null)
        {
            return true;
        }

        return false;
    }
    
    public ActionResult Index()
    {

        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        ViewBag.Filtros = repository.Filtros();
        return View(repository.Read());
    }


    [HttpGet]
    public ActionResult Create()
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        return View(repository.Create());
    }

    [HttpPost]
    public ActionResult Create(Estoque estoque)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        bool TemCampoNulo(Estoque estoque)
        {
            if (estoque == null) return true;

            return estoque.GetType()
                .GetProperties()
                .Where(p => p.CanRead && (p.Name == "calcadoId" || p.Name == "corId" || p.Name == "qtd" || p.Name == "valorIE" || p.Name == "tamanhoId" || p.Name == "idFornecedor" || p.Name == "preco"))
                .Any(p =>
                    p.GetValue(estoque) == null ||
                    (p.GetValue(estoque) is string s && string.IsNullOrWhiteSpace(s)) ||
                    (p.GetValue(estoque) is int i && i == 0) ||
                    (p.GetValue(estoque) is decimal d && d == 0) ||
                    (p.GetValue(estoque) is double db && db == 0)
                );

        }

        if (TemCampoNulo(estoque))
        {
            ViewBag.Error = "Preencha todas as informações!";
            ViewBag.Produto = estoque;
            return View("Create", repository.Create());
        }

        
        if (repository.Verificar(estoque) == 1) 
        {
            return RedirectToAction("Index");
        }
       
        repository.Create(estoque);
        return RedirectToAction("Index");
       
    }

    [HttpGet]
    public ActionResult Update(int idEstoque)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        var estoque = repository.Read(idEstoque);
        estoque.imagens ??= new List<Imagem>();
        return View(estoque);
    }

    [HttpPost]
    public ActionResult Update(Estoque estoque)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        bool TemCampoNulo(Estoque estoque)
        {
            if (estoque == null) return true;

            return estoque.GetType()
                .GetProperties()
                .Where(p => p.CanRead && (p.Name == "preco" || p.Name == "promocao" || p.Name == "valorIE"))
                .Any(p => p.GetValue(estoque) == null);
        }

        if (TemCampoNulo(estoque))
        {
            ViewBag.Error = "Preencha todas as informações!";
            var estoque_escolhido = repository.Read(estoque.idProduto);
            return View("Update", estoque_escolhido);
        }    

        repository.Update(estoque);

        return RedirectToAction("Index");
    }


    public ActionResult Delete(int idEstoque)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        try
        {
            repository.Delete(idEstoque);

            return RedirectToAction("Index");
        }
        catch (SqlException)
        {
            TempData["Error"] = "Não foi possível excluir o produto, pois ele está sendo usado!";

            return RedirectToAction("Index");
        }

    }

    public ActionResult DeleteImg(int idImagem, int idEstoque)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        try
        {
            repository.DeleteImg(idImagem);

            return View("Update", repository.Read(idEstoque));
        }
        catch (SqlException)
        {
            ViewBag.Error = "Não foi possível excluir a imagem!";

            return View("Update", repository.Read(idEstoque));
        }

    }


    [HttpPost]
    public async Task<IActionResult> CreateImg(Estoque estoque)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        bool TemCampoNulo(Estoque estoque)
        {
            if (estoque == null) return true;

            return estoque.GetType()
                    .GetProperties()
                    .Where(p => p.CanRead && (p.Name == "downloadImg" || p.Name == "idProduto"))
                    .Any(p => p.GetValue(estoque) == null);
        }

        if (TemCampoNulo(estoque))
        {
            ViewBag.Error = "Preencha todas as informações!";
            return RedirectToAction("Index");
        }

        if (estoque.downloadImg != null && estoque.downloadImg.Length > 0)
        {
            var fileName = await Upload(estoque.downloadImg);
            estoque.img = fileName;
        }

        repository.CreateImg(estoque);
        var estoque_escolhido = repository.Read(estoque.idProduto);
        return RedirectToAction("Update", new { idEstoque = estoque.idProduto });
    }
    
    [HttpPost]
    public ActionResult UpdateImg(Estoque estoque)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        bool TemCampoNulo(Estoque estoque)
        {
            if (estoque == null) return true;

            return estoque.GetType()
                .GetProperties()
                .Where(p => p.CanRead && (p.Name == "calcadoId" || p.Name == "corId" || p.Name == "idImagem"))
                .Any(p => p.GetValue(estoque) == null);
        }

        if (TemCampoNulo(estoque))
        {
            ViewBag.Error = "Preencha todas as informações!";
            var estoque_escolhido = repository.Read(estoque.idProduto);
            return View("Update", estoque_escolhido);
        }

        repository.UpdateImg(estoque);
        var estoque_ = repository.Read(estoque.idProduto);
        return View("Update", estoque_);

    }

    private async Task<string> Upload(IFormFile file)
    {
        var fileName = Path.GetFileName(file.FileName);
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/produtos", fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }
}
