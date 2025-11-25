using Microsoft.AspNetCore.Mvc;
using RNStore.Repositories;
using RNStore.Models;
using QRCoder;
using static QRCoder.PayloadGenerator;

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
        var infos = new Catalogo
        {
            catalogo = repository.Read(),
            slides = repository.ReadSlides()

        };
        
        return View(infos);
    }


    public ActionResult Produto(int idProduto, int idCalcado)
    {    
        var produto = repository.Read(idProduto, idCalcado);
        var idCliente = HttpContext.Session.GetInt32("idCliente");

        if(idCliente != null)
        {
            produto.isOnCart = repository.IsOnCart(idProduto, idCliente);
        }
       
   
        return View(produto);
    }


    public ActionResult Buscar(string buscarProduto)
    {
        var infosErro = new Catalogo
        {
            catalogo = repository.Read(),
            slides = repository.ReadSlides()
        };

        if(buscarProduto == null)
        { 
            return View("Index", infosErro);
        } 

        var result = repository.Buscar(buscarProduto);

        if (!result.Any())
        {

            return View("Index", infosErro);
        }

        var infos = new Catalogo
        {
            catalogo = result,
            slides = repository.ReadSlides()
        };
        
        return View("Index", infos);
    }

    [HttpGet]
    public ActionResult Carrinho()
    {
        if(HttpContext.Session.GetInt32("idCliente") == null)
        {
            return RedirectToAction("Login", "Cliente");
        }
        var idCliente = HttpContext.Session.GetInt32("idCliente");

        return View(repository.Read(idCliente));
    }

    [HttpPost]
    public ActionResult Carrinho(Catalogo catalogo)
    {

        if(HttpContext.Session.GetInt32("idCliente") == null)
        {
            if(catalogo.idProduto != 0)
            {
                HttpContext.Session.SetInt32("idProduto", catalogo.idProduto);
                return RedirectToAction("Login", "Cliente");
            }
            
            return RedirectToAction("Login", "Cliente");
        }
        
        var idCliente = HttpContext.Session.GetInt32("idCliente");

        repository.Carrinho(catalogo, idCliente);

        return View(repository.Read(idCliente));
    }

    [HttpPost]
    public ActionResult AlterarQtdCarrinho(Compra compra)
    {
        if(compra.operacao == "remover")
        {
            if(compra.qtdIC == 1)
            {
                TempData["Error"] = "Não é possível diminuir ainda mais a quantidade do produto! Exclua-o do seu carrinho!";
                return RedirectToAction("Carrinho");
            }

            repository.RemoverQtdCarrinho(compra.idCompra, compra.idProduto, compra.valorIC);

            return RedirectToAction("Carrinho");

        }else if(compra.operacao == "adicionar")
        {
            if(compra.qtdDisponivel == compra.qtdIC)
            {
                TempData["Error"] = "Não é possível adicionar mais quantidades desse produto ao carrinho!";
                return RedirectToAction("Carrinho");
            }

            repository.AdicionarQtdCarrinho(compra.idCompra, compra.idProduto, compra.valorIC);

            return RedirectToAction("Carrinho");
        }

        return RedirectToAction("Carrinho");


    }

    public ActionResult Delete(Compra compra)
    {
        try
        {
            repository.Delete(compra);

            return RedirectToAction("Carrinho");
        }
        catch
        {
            TempData["Error"] = "Não foi possível tirar o produto do carrinho... Tente novamente mais tarde!";

            return RedirectToAction("Carrinho");
        }
        
    }

    public ActionResult ConfirmarCompra(Compra compra)
    {
        repository.ConfirmarCompra(compra.idCompra);

        return RedirectToAction("Index", "Catalogo");
    }
}
