using PhotoCat.Application.Interfaces;
using PhotoCat.Application.Photos;
using PhotoCat.FileSystemScanner;
using PhotoCat.FileSystemScanner.Services;
using PhotoCat.Infrastructure;
using PhotoCat.Infrastructure.Photos;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IPhotoRepository, PhotoRepository>();
builder.Services.AddSingleton<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddSingleton<PhotoCat.FileSystemScanner.Services.PhotoSyncWorker>();
builder.Services.AddSingleton<FileSystemMonitor>();

builder.Services.AddHostedService<PhotoCat.FileSystemScanner.PhotoWorker>();

var host = builder.Build();
await host.RunAsync();
