using FluentValidation;
using MBKC.API.Extentions;
using MBKC.API.Middlewares;
using MBKC.BAL.DTOs.Accounts;
using MBKC.BAL.DTOs.AccountTokens;
using MBKC.BAL.DTOs.Brands;
using MBKC.BAL.DTOs.FireBase;
using MBKC.BAL.DTOs.JWTs;
using MBKC.BAL.DTOs.KitchenCenters;
using MBKC.BAL.DTOs.Verifications;
using MBKC.BAL.Errors;
using MBKC.BAL.Services.Implementations;
using MBKC.BAL.Services.Interfaces;
using MBKC.BAL.Utils;
using MBKC.BAL.Validators.Accounts;
using MBKC.BAL.Validators.Authentications;
using MBKC.BAL.Validators.KitchenCenters;
using MBKC.BAL.Validators.Verifications;
using MBKC.BAL.Validators;
using MBKC.DAL.Infrastructures;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;
using MBKC.BAL.Validators.Categories;
using MBKC.BAL.DTOs.Categories;
using MBKC.BAL.Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(opts
                    => opts.SuppressModelStateInvalidFilter = true);
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
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

//JWT
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

//DI
builder.Services.Configure<JWTAuth>(builder.Configuration.GetSection("JWTAuth"));
builder.Services.Configure<Email>(builder.Configuration.GetSection("Verification:Email"));
builder.Services.AddScoped<IDbFactory, DbFactory>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IBankingAccountService, BankingAccountService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IBrandAccountService, BrandAccountService>();
builder.Services.AddScoped<ICashierService, CashierService>();
builder.Services.AddScoped<ICashierMoneyExchangeService, CashierMoneyExchangeService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IExtraCategoryService, ExtraCategoryService>();
builder.Services.AddScoped<IKitchenCenterService, KitchenCenterService>();
builder.Services.AddScoped<IKitchenCenterMoneyExchangeService, KitchenCenterMoneyExchangeService>();
builder.Services.AddScoped<IMappingProductService, MappingProductService>();
builder.Services.AddScoped<IMoneyExchangeService, MoneyExchangeService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderDetailService, OrderDetailService>();
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IShipperPaymentService, ShipperPaymentService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IStoreAccountService, StoreAccountService>();
builder.Services.AddScoped<IStoreMoneyExchangeService, StoreMoneyExchangeService>();
builder.Services.AddScoped<IStorePartnerService, StorePartnerService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IWalletService, WalletService>();
//Firebase Image
builder.Services.Configure<FireBaseImage>(builder.Configuration.GetSection("FireBaseImage"));

//AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//Validation


//Middlewares
builder.Services.AddTransient<ExceptionMiddleware>();

//add CORS
builder.Services.AddCors(cors => cors.AddPolicy(
                            name: "WebPolicy",
                            policy =>
                            {
                                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                            }
                        ));

//Validation
builder.Services.AddScoped<IValidator<AccountRequest>, AccountRequestValidator>();
builder.Services.AddScoped<IValidator<AccountTokenRequest>, AccountTokenRequestValidator>();
builder.Services.AddScoped<IValidator<EmailVerificationRequest>, EmailVerificationRequestValidator>();
builder.Services.AddScoped<IValidator<OTPCodeVerificationRequest>, OTPCodeVerifycationRequestValidator>();
builder.Services.AddScoped<IValidator<ResetPasswordRequest>, ResetPasswordRequestValidator>();
builder.Services.AddScoped<IValidator<CreateKitchenCenterRequest>, CreateKitchenCenterValidator>();
builder.Services.AddScoped<IValidator<UpdateKitchenCenterRequest>, UpdateKitchenCenterValidator>();
builder.Services.AddScoped<IValidator<PostBrandRequest>, PostBrandValidation>();
builder.Services.AddScoped<IValidator<UpdateBrandRequest>, UpdateBrandValidation>();
builder.Services.AddScoped<IValidator<PostCategoryRequest>, PostCategoryValidator>();
builder.Services.AddScoped<IValidator<UpdateCategoryRequest>, UpdateCategoryValidator>();

//Middlewares
builder.Services.AddTransient<ExceptionMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("WebPolicy");

app.UseAuthentication();

app.UseAuthorization();

//Add middleware extentions
app.ConfigureExceptionMiddleware();

app.MapControllers();

app.Run();
