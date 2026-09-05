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
		mauiAppBuilder.Services.AddTransient<VoucherDetailPage>();
		mauiAppBuilder.Services.AddTransient<ReturnMaterialPage>();
		mauiAppBuilder.Services.AddSingleton<VoucherListPage>();
		mauiAppBuilder.Services.AddSingleton<ReturnedVoucherListPage>();
		return mauiAppBuilder;
	}

	public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<MainViewModel>();
		mauiAppBuilder.Services.AddSingleton<ImportArticlesViewModel>();
		mauiAppBuilder.Services.AddTransient<VoucherDetailViewModel>();
		mauiAppBuilder.Services.AddTransient<ReturnMaterialViewModel>();
		mauiAppBuilder.Services.AddSingleton<VoucherListViewModel>();
		mauiAppBuilder.Services.AddSingleton<ReturnedVoucherListViewModel>();
		return mauiAppBuilder;
	}

	public static MauiAppBuilder RegisterRepositories(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<IMaterialInOutDatabase, MaterialInOutDatabase>();
		mauiAppBuilder.Services.AddSingleton<IArticleRepository, ArticleRepository>();
		return mauiAppBuilder;
	}

	public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
	{
		mauiAppBuilder.Services.AddSingleton<IArticlesListReader, ArticlesListReader>();
		mauiAppBuilder.Services.AddSingleton<IReportService, ReportService>();
		return mauiAppBuilder;
	}
}
