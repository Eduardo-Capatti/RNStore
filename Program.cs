using RNStore.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ICatalogoRepository>(_ => 
    new CatalogoDatabaseRepository(
        builder.Configuration.GetConnectionString("default")));;

builder.Services.AddSession();
builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();


app.UseSession();
app.MapControllerRoute("default","{controller=Catalogo}/{action=Index}/{id?}");


app.Run();
