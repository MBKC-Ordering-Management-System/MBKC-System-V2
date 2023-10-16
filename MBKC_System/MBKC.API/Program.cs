using MBKC.API.Constants;
using MBKC.API.Extentions;
using MBKC.API.Middlewares;
using MBKC.Service.DTOs.JWTs;

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
builder.Services.AddValidators();
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

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(CorsConstant.PolicyName);

app.UseAuthentication();

app.UseAuthorization();

//Add middleware extentions
app.ConfigureExceptionMiddleware();

app.MapControllers();

app.Run();
