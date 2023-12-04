using QRCoder;
using VoteOnIt.BlazorFrontEnd.Components;
using VoteOnIt.BlazorFrontEnd.Services;
using VoteOnIt.ServiceDefaults; 

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IPollService, PollServiceApi>();
builder.Services.AddOutputCache();

builder.Services.AddHttpClient<PollServiceApi>(client => client.BaseAddress = new("http://pollApiService"));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Map("/qr", (HttpResponse response, string? link = null) =>
{
    // return a qr code that points to the provided link
    if (link == null)
    {
        return Results.BadRequest();
    }

    QRCodeGenerator gen = new QRCodeGenerator();
    QRCodeData data = gen.CreateQrCode(link, QRCodeGenerator.ECCLevel.L);
    byte[] bytes = new PngByteQRCode(data).GetGraphic(10);

    return Results.Bytes(bytes, "image/png");
});

app.MapDefaultEndpoints();

app.Run();
