using ACS_Reception.Application;
using ACS_Reception.UI.Pages;
using ACS_Reception.UI.ViewModels;

namespace ACS_Reception.UI
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterPages(this IServiceCollection services)
        {
            services.AddTransient<DoctorsPage>()
                .AddTransient<CardsPage>()
                .AddTransient<CardDetailsPage>()
                .AddTransient<AddOrEditDoctorPage>()
                .AddTransient<AddOrEditCardPage>()
                .AddTransient<DoctorDetailsPage>()
                .AddTransient<RecordDetailsPage>()
                .AddTransient<AttendancePage>()
                .AddTransient<ChartsPage>();

            return services;
        }

        public static IServiceCollection RegisterViewModels(this IServiceCollection services)
        {
            services.AddTransient<DoctorsPageViewModel>()
                .AddTransient<CardsPageViewModel>()
                .AddTransient<CardDetailsPageViewModel>()
                .AddTransient<AddOrEditDoctorPageViewModel>()
                .AddTransient<AddOrEditCardPageViewModel>()
                .AddTransient<DoctorDetailsPageViewModel>()
                .AddTransient<RecordDetailsPageViewModel>()
                .AddTransient<AttendancePageViewModel>();

            return services;
        }

        public static IServiceCollection RegisterMisc(this IServiceCollection services)
        {
            services.AddSingleton<AttendanceDistributor>();

            return services;
        }
    }
}
