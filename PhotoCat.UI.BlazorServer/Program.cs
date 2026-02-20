using PhotoCat.Application;
using PhotoCat.Application.Interfaces;
using PhotoCat.Application.Photos;
using PhotoCat.Application.Photos.AddPhotoFile;
using PhotoCat.Infrastructure.Hashing;
using PhotoCat.Infrastructure.Metadata;
using PhotoCat.Infrastructure.Photos;
using PhotoCat.UI.BlazorServer.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(AddPhotoFileCommandHandler).Assembly);
});

builder.Services.AddScoped<IPhotoRepository, PhotoRepository>();
builder.Services.AddScoped<IChecksumService, Sha256ChecksumService>();
builder.Services.AddScoped<IExifExtractor, ExifExtractor>();
builder.Services.AddScoped<IFileTypeDetector, FileSignatureDetector>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
