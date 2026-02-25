namespace StoringPassword.Services
{
    public static class ServiceProviderExtensions
    {
        public static void AddDbService(this IServiceCollection services)
        {
            services.AddTransient<GuestBookService>();
        }
    }
}
