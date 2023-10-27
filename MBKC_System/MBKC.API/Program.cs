using FluentValidation;
using MBKC.API.Extentions;
using MBKC.API.Middlewares;
using MBKC.Service.DTOs.Accounts;
using MBKC.Service.DTOs.AccountTokens;
using MBKC.Service.DTOs.Brands;
using MBKC.Service.DTOs.KitchenCenters;
using MBKC.Service.DTOs.Stores;
using MBKC.Service.Errors;
using MBKC.Service.Services.Implementations;
using MBKC.Service.Services.Interfaces;

using MBKC.API.Validators.Accounts;
using MBKC.API.Validators.Authentications;
using MBKC.API.Validators.KitchenCenters;
using MBKC.API.Validators.Stores;
using MBKC.API.validators.Verifications;
using MBKC.API.Validators;
using MBKC.API.Constants;
using MBKC.Service.DTOs.Verifications;
using MBKC.Service.DTOs.JWTs;
using MBKC.Repository.FirebaseStorages.Models;
using MBKC.Service.Utils;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().ConfigureApiBehaviorOptions(opts
                    => opts.SuppressModelStateInvalidFilter = true);
builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddConfigSwagger();
//JWT
builder.AddJwtValidation();
//DI
builder.Services.Configure<JWTAuth>(builder.Configuration.GetSection("JWTAuth"));
builder.Services.AddDbFactory();
builder.Services.AddUnitOfWork();
builder.Services.AddServices();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddExceptionMiddleware();
//Middlewares
builder.Services.AddTransient<ExceptionMiddleware>();

//add CORS
builder.Services.AddCors(cors => cors.AddPolicy(
                            name: CorsConstant.PolicyName,
                            policy =>
                            {
                                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                            }
                        ));
//Middlewares
var app = builder.Build();
app.AddApplicationConfig();
app.Run();
