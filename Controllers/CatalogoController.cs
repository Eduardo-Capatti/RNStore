using Microsoft.AspNetCore.Mvc;
using RNStore.Repositories;
using RNStore.Models;

namespace RNStore.Controllers;

public class CatalogoController : Controller
{

    private ICatalogoRepository repository;

    public CatalogoController (ICatalogoRepository repository)
    {
        this.repository = repository;
    }

    public ActionResult Index()
    {
        var sliders = repository.ReadSlides();

        ViewBag.slides = sliders;
        
        return View(repository.Read());
    }


    public ActionResult Produto(int idProduto, int idCalcado)
    {
        return View(repository.Read(idProduto, idCalcado));
    }


    public ActionResult Buscar(string buscarProduto)
    {
        var sliders = repository.ReadSlides();

        ViewBag.slides = sliders;

        if(buscarProduto == null)
        { 
            return View("Index", repository.Read());
        } 

        var result = repository.Buscar(buscarProduto);

        if (!result.Any())
        {
            return View("Index", repository.Read());
        }
        
        return View("Index", result);
    }

    public ActionResult Carrinho()
    {
        return View();
    }
}
