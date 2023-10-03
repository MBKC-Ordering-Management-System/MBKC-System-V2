using FluentValidation;
using MBKC.Service.DTOs.Accounts;
using MBKC.Service.DTOs.AccountTokens;
using MBKC.Service.DTOs.Brands;
using MBKC.Service.DTOs.Categories;
using MBKC.Service.DTOs.KitchenCenters;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.DTOs.Verifications;
using MBKC.API.validators.Verifications;
using MBKC.Repository.Infrastructures;
using MBKC.Service.Errors;
using MBKC.Service.Services.Implementations;
using MBKC.Service.Services.Interfaces;
using MBKC.Service.Utils;
using MBKC.API.Validators.Accounts;
using MBKC.API.Validators.Authentications;
using MBKC.API.Validators.Brands;
using MBKC.API.Validators.Categories;
using MBKC.API.Validators.KitchenCenters;
using MBKC.API.Validators.Stores;
using MBKC.API.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using MBKC.API.Middlewares;
using MBKC.Service.DTOs.BankingAccounts;
using MBKC.API.Validators.BankingAccounts;
using MBKC.Service.DTOs.Partners;
using MBKC.API.Validators.Partners;

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
            services.AddScoped<IBrandAccountService, BrandAccountService>();
            services.AddScoped<ICashierService, CashierService>();
            services.AddScoped<ICashierMoneyExchangeService, CashierMoneyExchangeService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IExtraCategoryService, ExtraCategoryService>();
            services.AddScoped<IKitchenCenterService, KitchenCenterService>();
            services.AddScoped<IKitchenCenterMoneyExchangeService, KitchenCenterMoneyExchangeService>();
            services.AddScoped<IMappingProductService, MappingProductService>();
            services.AddScoped<IMoneyExchangeService, MoneyExchangeService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<IPartnerService, PartnerService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IShipperPaymentService, ShipperPaymentService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IStoreAccountService, StoreAccountService>();
            services.AddScoped<IStoreMoneyExchangeService, StoreMoneyExchangeService>();
            services.AddScoped<IStorePartnerService, StorePartnerService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<IWalletService, WalletService>();
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

        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<AccountRequest>, AccountValidator>();
            services.AddScoped<IValidator<AccountTokenRequest>, AccountTokenValidator>();
            services.AddScoped<IValidator<EmailVerificationRequest>, EmailVerificationValidator>();
            services.AddScoped<IValidator<OTPCodeVerificationRequest>, OTPCodeVerifycationValidator>();
            services.AddScoped<IValidator<ResetPasswordRequest>, ResetPasswordValidator>();
            services.AddScoped<IValidator<CreateKitchenCenterRequest>, CreateKitchenCenterValidator>();
            services.AddScoped<IValidator<UpdateKitchenCenterRequest>, UpdateKitchenCenterValidator>();
            services.AddScoped<IValidator<RegisterStoreRequest>, RegisterStoreValidator>();
            services.AddScoped<IValidator<UpdateStoreRequest>, UpdateStoreValidator>();
            services.AddScoped<IValidator<PostBrandRequest>, PostBrandValidator>();
            services.AddScoped<IValidator<UpdateBrandRequest>, UpdateBrandValidator>();
            services.AddScoped<IValidator<PostCategoryRequest>, PostCategoryValidator>();
            services.AddScoped<IValidator<UpdateCategoryRequest>, UpdateCategoryValidator>();
            services.AddScoped<IValidator<UpdateBrandStatusRequest>, UpdateBrandStatusValidator>();
            services.AddScoped<IValidator<UpdateBrandProfileRequest>, UpdateBrandProfileValidator>();
            services.AddScoped<IValidator<UpdateKitchenCenterStatusRequest>, UpdateKitchenCenterStatusValidator>();
            services.AddScoped<IValidator<UpdateStoreStatusRequest>, UpdateStoreStatusValidator>();
            services.AddScoped<IValidator<ConfirmStoreRegistrationRequest>, ConfirmStoreRegistrationValidator>();
            services.AddScoped<IValidator<CreateBankingAccountRequest>, CreateBankingAccountValidator>();
            services.AddScoped<IValidator<UpdateBankingAccountStatusRequest>, UpdateBankingAccountStatusValidator>();
            services.AddScoped<IValidator<UpdateBankingAccountRequest>, UpdateBankingAccountValidator>();
            services.AddScoped<IValidator<PostPartnerRequest>, CreatePartnerValidator>();
            services.AddScoped<IValidator<UpdatePartnerRequest>, UpdatePartnerValidator>();
            return services;
        }

        public static IServiceCollection AddExceptionMiddleware(this IServiceCollection services)
        {
            services.AddTransient<ExceptionMiddleware>();
            return services;
        }
    }
}
