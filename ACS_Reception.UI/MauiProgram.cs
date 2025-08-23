using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using MongoDB.Driver;
using ACS_Reception.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using ACS_Reception.Application;
using ACS_Reception.Persistence;
using ACS_Reception.Domain.Abstractions;
using Microcharts.Maui;

namespace ACS_Reception.UI;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
        string settingsStream = "ACS_Reception.UI.appsettings.json";

        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiCommunityToolkit()
			.UseMauiApp<App>()
            .UseMicrocharts()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream(settingsStream);
        builder.Configuration.AddJsonStream(stream!);

        var mongoDBSettings = builder.Configuration.GetSection("MongoDBSettings");
        var mongoClient = new MongoClient(mongoDBSettings.GetSection("AtlasURI").Value);
        
        var options = new DbContextOptionsBuilder<AppDbContext>().UseMongoDB(mongoClient, mongoDBSettings.GetSection("DatabaseName").Value!).Options;
#if DEBUG
        builder.Logging.AddDebug();
#endif

        string? libPath = builder.Configuration.GetSection("SerializerLib").Value;
        if (libPath != null && File.Exists(libPath))
        {
            var assembly = Assembly.LoadFrom(libPath);

            var types = assembly.GetTypes();
            var serializerType = types.Where(t => t.GetInterfaces().Contains(typeof(ISerializer))).FirstOrDefault();
            if (serializerType != null)
            {
                var serializer = Activator.CreateInstance(serializerType);
                builder.Services.AddSingleton<ISerializer>((serializer as ISerializer)!);
            }
        }

        builder.Services
            .AddApplication()
            .AddPersistence(options)
            .RegisterPages()
            .RegisterViewModels();

        return builder.Build();
	}
}
