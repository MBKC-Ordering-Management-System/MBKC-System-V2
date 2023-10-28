using MBKC.PrivateAPI.Extentions;
using MBKC.PrivateAPI.Middlewares;

namespace MBKC.PrivateAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers().ConfigureApiBehaviorOptions(opts
                    => opts.SuppressModelStateInvalidFilter = true);
            builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddConfigSwagger();
            builder.Services.AddDbFactory();
            builder.Services.AddUnitOfWork();
            builder.Services.AddServices();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddValidators();
            builder.Services.AddExceptionMiddleware();
            //Middlewares
            builder.Services.AddTransient<ExceptionMiddleware>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();

            //Add middleware extentions
            app.ConfigureExceptionMiddleware();

            app.MapControllers();

            app.Run();
        }
    }
}