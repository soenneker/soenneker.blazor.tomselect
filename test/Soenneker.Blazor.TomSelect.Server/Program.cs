using Soenneker.Blazor.TomSelect.Server.Components;
using Soenneker.Blazor.TomSelect.Registrars;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add TomSelect services
builder.Services.AddTomSelectInteropAsScoped();
builder.Services.AddHttpClient("Default", client =>
{
    client.BaseAddress = new Uri("http://localhost:5212");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
