
using MBKC.API.Middlewares;

using MBKC.Repository.Infrastructures;
using MBKC.Service.Errors;
using MBKC.Service.Services.Implementations;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using MBKC.Service.DTOs.Orders;
using MBKC.API.Validators.Orders;
using Hangfire;
using System.Security.Cryptography.Xml;
using Hangfire.Storage.SQLite;
using MBKC.API.Constants;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Filters;
using MBKC.API.Authorization;

namespace MBKC.API.Extentions
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

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IVerificationService, VerificationService>();
            services.AddScoped<IBankingAccountService, BankingAccountService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<ICashierService, CashierService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IKitchenCenterService, KitchenCenterService>();
            services.AddScoped<IKitchenCenterMoneyExchangeService, KitchenCenterMoneyExchangeService>();
            services.AddScoped<IPartnerProductService, PartnerProductService>();
            services.AddScoped<IMoneyExchangeService, MoneyExchangeService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IPartnerService, PartnerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IStorePartnerService, StorePartnerService>();
            services.AddScoped<IWalletService, WalletService>();
            services.AddScoped<IConfigurationService, ConfigurationService>();
            services.AddScoped<IShipperPaymentService, ShipperPaymentService>();
            services.AddScoped<IDashBoardService, DashBoardService>();
            services.AddScoped<IUserDevicceService, UserDeviceService>();

            services.AddHangfire(config => config
                                .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                                .UseSimpleAssemblyNameTypeSerializer()
                                .UseRecommendedSerializerSettings()
                                .UseSQLiteStorage(HangfireConstant.DatabaseName));
            services.AddHangfireServer();
            return services;
        }

        public static void AddJwtValidation(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWTAuth:Key"])),
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        // Call this to skip the default logic and avoid using the default response
                        context.HandleResponse();

                        // Write to the response in any way you wish
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsJsonAsync(new
                        {
                            Message = JsonConvert.DeserializeObject<List<ErrorDetail>>(ErrorUtil.GetErrorString("Unauthorized", "You are not allowed to access this API."))
                        });
                    }
                };
            });
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
                    Title = "MBKC Application API",
                    Description = "The MBKC Application API is built for the Order Management System for Multi-Brand Kitchen Center."
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            return services;
        }

        public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services)
        {
            services.AddTransient<ExceptionMiddleware>();
            return services;
        }

        public static WebApplication AddApplicationConfig(this WebApplication app)
        {
            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(CorsConstant.PolicyName);
            app.UseAuthentication();
            app.UseAuthorization();
            //Add middleware extentions
            app.ConfigureExceptionMiddleware();
            app.MapControllers();
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthenticationFilter() }
            });
            BackgroundJob.Enqueue<IConfigurationService>(cf => cf.StartAllBackgroundJob());
            return app;
        }
    }
}
