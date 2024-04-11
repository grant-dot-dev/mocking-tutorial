using Service;

namespace WebApi
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment)
        {
            // You can access the hosting environment here if needed
            var isDevelopment = environment.IsDevelopment();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add services to the container.
            services.AddSingleton<ITodoService, TodoService>();

            // Register controllers
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
