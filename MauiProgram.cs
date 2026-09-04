using Microsoft.Extensions.Logging;
using material_inout_desktop_v2.ViewModels;
using material_inout_desktop_v2.Repositories;
using material_inout_desktop_v2.Pages;
using material_inout_desktop_v2.Services;

namespace material_inout_desktop_v2;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			})
			.RegisterViews()
			.RegisterViewModels()
			.RegisterRepositories()
			.RegisterServices();

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

	public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<MainPage>();
		mauiAppBuilder.Services.AddSingleton<ImportArticlesPage>();
		return mauiAppBuilder;
	}

	public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<MainViewModel>();
		mauiAppBuilder.Services.AddSingleton<ImportArticlesViewModel>();
		return mauiAppBuilder;
	}

	public static MauiAppBuilder RegisterRepositories(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<IMaterialInOutDatabase, MaterialInOutDatabase>();
		return mauiAppBuilder;
	}

	public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<IArticlesListReader, ArticlesListReader>();
		return mauiAppBuilder;
	}
}
