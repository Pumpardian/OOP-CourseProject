using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using MongoDB.Driver;
using ACS_Reception.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using ACS_Reception.Application;
using ACS_Reception.Persistence;

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
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream(settingsStream);
        builder.Configuration.AddJsonStream(stream);


        var connStr = builder.Configuration
             .GetConnectionString("MongoDBConnection");
        var mongoClient = new MongoClient(connStr);

		var options = new DbContextOptionsBuilder<AppDbContext>().UseMongoDB(mongoClient, "ACS_Reception").Options;

#if DEBUG
        builder.Logging.AddDebug();
#endif

		builder.Services
			.AddApplication()
			.AddPersistence(options)
			.RegisterPages()
			.RegisterViewModels();

		return builder.Build();
	}
}
