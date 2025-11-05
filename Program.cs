using RNStore.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ICatalogoRepository>(_ =>
    new CatalogoDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));

builder.Services.AddTransient<ITamanhoRepository>(_ => 
    new TamanhoDatabaseRepository(
        builder.Configuration.GetConnectionString("Default")));

builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();


app.UseSession();

//rotas tamanho
app.MapControllerRoute(
    name: "tamanho_index",
    pattern: "Tamanho/Index",
    defaults: new { controller = "Tamanho", action = "Index" }
);

app.MapControllerRoute(
    name: "tamanho_create",
    pattern: "Tamanho/Create",
    defaults: new { controller = "Tamanho", action = "Create" }
);

app.MapControllerRoute(
    name: "tamanho_delete",
    pattern: "Tamanho/Delete/{idTamanho?}",
    defaults: new { controller = "Tamanho", action = "Delete" }
);

app.MapControllerRoute(
    name: "tamanho_update",
    pattern: "Tamanho/Update/{idTamanho?}",
    defaults: new { controller = "Tamanho", action = "Update" }
);


//rota default
app.MapControllerRoute("default","{controller=Catalogo}/{action=Index}/{idProduto?}/{idCalcado?}");


app.Run();
