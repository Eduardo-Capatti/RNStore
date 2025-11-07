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
    
    public ActionResult Index()
    {
        int? idUsuario = HttpContext.Session.GetInt32("idUsuario");

        if (idUsuario == null)
        {
            return RedirectToAction("Login", "User");
        }

        ViewBag.idUsuario = idUsuario;
        return View(repository.Read());
    }


    [HttpGet]
    public ActionResult Create()
    {
        return View(repository.Create());
    }

    [HttpPost]
    public ActionResult Create(Estoque estoque)
    {
        bool TemCampoNulo(Estoque estoque)
        {
            if (estoque == null) return true;

            return estoque.GetType()
                .GetProperties()
                .Where(p => p.CanRead && (p.Name == "calcadoId" || p.Name == "corId" || p.Name == "calcadoId" || p.Name == "qtd" || p.Name == "valorIE" || p.Name == "calcadoId" || p.Name == "tamanhoId" || p.Name == "fornecedorId"))
                .Any(p => p.GetValue(estoque) == null);
        }

        if (TemCampoNulo(estoque))
        {
            ViewBag.Error = "Preencha todas as informações!";
            return View("Create", repository.Create());
        }

        repository.Create(estoque);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Update(int idEstoque)
    {
        var estoque = repository.Read(idEstoque);
        estoque.imagens ??= new List<Imagem>();
        return View(estoque);
    }

    [HttpPost]
    public ActionResult Update(Estoque estoque)
    {
        bool TemCampoNulo(Estoque estoque)
        {
            if (estoque == null) return true;

            return estoque.GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.Name == "preco" && p.Name == "promocao" && p.Name == "valorIE" && p.Name == "qtd")
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
        try
        {
            repository.Delete(idEstoque);

            return RedirectToAction("Index");
        }
        catch (SqlException)
        {
            ViewBag.Error = "Não foi possível excluir o produto, pois ele está sendo usado!";

            return View("Index", repository.Read());
        }

    }

    public ActionResult DeleteImg(int idImagem)
    {
        try
        {
            repository.DeleteImg(idImagem);

            return RedirectToAction("Index");
        }
        catch (SqlException)
        {
            ViewBag.Error = "Não foi possível excluir a imagem!";

            return View("Index", repository.Read());
        }

    }


    [HttpPost]
    public async Task<IActionResult> CreateImg(Estoque estoque)
    {
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

        return RedirectToAction("Index");
    }
    
    [HttpPost]
    public ActionResult UpdateImg(Estoque estoque)
    {
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

        return RedirectToAction("Index");
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
