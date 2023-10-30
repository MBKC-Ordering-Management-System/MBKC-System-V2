using FluentValidation;
using MBKC.PrivateAPI.Middlewares;
using MBKC.PrivateAPI.Validators.Orders;
using MBKC.Repository.Infrastructures;
using MBKC.Service.DTOs.Orders;
using MBKC.Service.Services.Implementations;
using MBKC.Service.Services.Interfaces;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace MBKC.PrivateAPI.Extentions
{
    public static class DependencyExtention
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }

        public static IServiceCollection AddDbFactory(this IServiceCollection services)
        {
            services.AddScoped<IDbFactory, DbFactory>();
            return services;
        }

        public static IServiceCollection AddConfigSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "MBKC Application Private API",
                    Description = "The MBKC Application Private API is built for the Order Management System for Multi-Brand Kitchen Center and is used by MBKC Bot worker service."
                });
            });
            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IOrderService, OrderService>();
            return services;
        }

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<GetOrderRequest>, GetOrderValidator>();
            services.AddScoped<IValidator<PostOrderRequest>, PostOrderValidator>();
            services.AddScoped<IValidator<PutOrderIdRequest>, PutOrderIdValidator>();
            services.AddScoped<IValidator<PutOrderRequest>, PutOrderValidator>();
            return services;
        }

        public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services)
        {
            services.AddTransient<ExceptionMiddleware>();
            return services;
        }
    }
}
