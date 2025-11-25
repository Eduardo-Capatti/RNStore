using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RNStore.Models;
using RNStore.Repositories;


namespace RNStore.Controllers;

public class SliderController: Controller
{
    private ISliderRepository repository;

    public SliderController (ISliderRepository repository)
    {
        this.repository = repository;
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

        return View(repository.Read());
    }
    
    [HttpGet]
    public ActionResult Create()
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Slider slider)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        int? idUsuario = HttpContext.Session.GetInt32("idUsuario");

        if(idUsuario == null)
        {
            return RedirectToAction("Login", "User");
        }
        else
        {
            slider.idFuncionario = idUsuario.Value;
        }
        

        bool TemCampoNulo(Slider slide)
        {
            if (slide == null) return true;

            return slide.GetType()
                    .GetProperties()
                    .Where(p => p.CanRead && p.Name != "idSlider" && p.Name != "img" && p.Name != "nomePessoa")
                    .Any(p => p.GetValue(slide) == null);
        }

        if (TemCampoNulo(slider))
        {
            ViewBag.Error = "Preencha todas as informações!";
            return View(slider); 
        }

        if (slider.downloadImg != null && slider.downloadImg.Length > 0)
        {
            var fileName = await UploadSlider(slider.downloadImg);
            slider.img = fileName;
        }

        repository.Create(slider);

        return RedirectToAction("Index");
    }

    [HttpGet]
    public ActionResult Update(int idSlider)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        var Slider = repository.Read(idSlider);
        return View(Slider);
    }

    [HttpPost]
    public ActionResult Update(Slider slider)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        bool TemCampoNulo(Slider slider)
        {
            if (slider == null) return true;

            return slider.GetType()
                .GetProperties()
                .Where(p => p.CanRead && p.Name == "status")
                .Any(p => p.GetValue(slider) == null);
        }

        if (TemCampoNulo(slider))
        {
            ViewBag.Error = "Preencha todas as informações!";
            var slider_escolhido = repository.Read(slider.idSlider);
            return View("Update", slider_escolhido);
        }

        repository.Update(slider);

        return RedirectToAction("Index");
    }


    public ActionResult Delete(int idSlider)
    {
        if (VerificaSession())
        {
            return RedirectToAction("Login", "User");
        }

        try
        {
            repository.Delete(idSlider);

            return RedirectToAction("Index");
        }
        catch (SqlException)
        {
            ViewBag.Error = "Não foi possível excluir o funcionário, contate o suporte para mais detalhes!";

            return View("Index", repository.Read());
        }

    }
    
    private async Task<string> UploadSlider(IFormFile file)
    {
        var fileName = Path.GetFileName(file.FileName);
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img/slider", fileName);

        using (var stream = new FileStream(path, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }

}
