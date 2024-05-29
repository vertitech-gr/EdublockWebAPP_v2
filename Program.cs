using Edu_Block.DAL.EF;
using Edu_Block.DAL;
using MediatR;
using Edu_Block_dev.CQRS.Commands.UserCommand;
using Microsoft.AspNetCore.CookiePolicy;
using Edu_Block_dev.CQRS.Commands;
using DinkToPdf.Contracts;
using DinkToPdf;
using Microsoft.Extensions.FileProviders;
using Edu_Block_dev.Authorization;
using Edu_Block_dev.Helpers;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Edu_Block_dev.CQRS.Query.EduCertificate;
using Edu_Block_dev.CQRS.Command_Handler.EmployeeRequest;
using Edu_Block_dev.CQRS.Commands.EduUserRequest;
using Edu_Block_dev.CQRS.Query.EduUserRequest;
using Edu_Block_dev.CQRS.Query_Handler.EduUserRequest;
using Edu_Block_dev.CQRS.Commands.EduUniversity;
using Edu_Block_dev.CQRS.Query.EduUniversity;
using Edu_Block_dev.CQRS.Commands.EduUser;
using Edu_Block_dev.CQRS.Command_Handler.EduUser;
using Edu_Block_dev.CQRS.Query.EduUser;
using Edu_Block_dev.CQRS.Query_Handler.EduUser;
using Edu_Block_dev.CQRS.Commands.EduCertificate;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Edu_Block_dev.CQRS.Commands.EduRole;
using Edu_Block_dev.CQRS.Command_Handler.EduRole;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CookiePolicyOptions>(Options =>
{
    Options.MinimumSameSitePolicy = SameSiteMode.None;
    Options.HttpOnly = HttpOnlyPolicy.None;
    Options.Secure = CookieSecurePolicy.None;
});

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
var settingsFilename = string.IsNullOrEmpty(env) ? "appsettings.json" : $"appsettings.{env}.json";

var config = new ConfigurationBuilder()
     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
     .AddJsonFile(settingsFilename, optional: false, reloadOnChange: true)
     .Build();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<IJwtUtils, JwtUtils>();
builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));

//builder.Services.AddAuthentication(options => {
//     options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//     options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//}).AddJwtBearer(option =>
//    {
//        option.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true,
//            ValidateIssuerSigningKey = true,
//            ValidIssuer = builder.Configuration["Jwt:Issuer"],
//            ValidAudience = builder.Configuration["Jwt:Audience"],
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
//        };
//    })

EFBootstrap.Configure(builder.Services, config);
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateDockIssuerCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUniversityCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CertificateDIDCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UserProfileCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetUserProfileQuery).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetUniversitiesQuery).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(GetCertificatesQuery).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InsertCertificateCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(AddRoleCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ResendOtpCommand).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ResendOtpCommandHandler).Assembly));
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddSingleton<IConverter, SynchronizedConverter>();
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddSingleton<ITools, PdfTools>();
builder.Services.AddScoped<IRequestHandler<UserLoginCommand, ApiResponse<object>>, UserLoginCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GetUserByEmailQuery, User>, GetUserByEmailQueryHandler>();
builder.Services.AddScoped<IRequestHandler<CreateUserRequestCommand, ApiResponse<object>>, UserRequestCommandHandler>();
builder.Services.AddScoped<IRequestHandler<AddRoleCommand, ApiResponse<object>>, RoleCommandHandler>();
builder.Services.AddScoped<IRequestHandler<GetRequestsQuery, ApiResponse<object>>, GetRequestsQueryHandler>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Edu Block", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.ApiKey
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
              Scheme = "oauth2",
              Name = "Bearer",
              In = ParameterLocation.Header,

            },
            new List<string>()
          }
        });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = "924394256777-cpj67kbncag9ffif2ohoi46tostafb60.apps.googleusercontent.com";
    options.ClientSecret = "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";
});

var facebookAuthConfig = config.GetSection("FacebookAuth");
var facebookAppId = facebookAuthConfig["AppId"];
var facebookAppSecret = facebookAuthConfig["AppSecret"];

builder.Services.AddAuthentication()
    .AddFacebook(options =>
    {
        options.ClientId = facebookAppId;
        options.ClientSecret = facebookAppSecret;
    });

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
             policy
            .AllowAnyMethod()
            .AllowAnyHeader();
        });
});

//builder.Logging.AddDbLogger(options =>
//{
//    builder.Configuration.GetSection("Logging")
//    .GetSection("Database").GetSection("Options").Bind(options);
//});


//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie()
//.AddCookie()
//        .AddGoogle(options =>
//        {
//            options.ClientId = _configuration["GoogleAuthConfig:ClientId"];
//            options.ClientSecret = _configuration["GoogleAuthConfig:ClientSecret"];
//        });

var app = builder.Build();
app.UseCookiePolicy();
app.UseStaticFiles();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
            Path.Combine(app.Environment.WebRootPath, "Certificate")),
    RequestPath = "/Certificate"
});

//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(
//            Path.Combine("C:\\Users\\bladmin\\source\\repos\\EDUBLOCK\\edublock-v1-backend\\wwwroot", "Certificate")),
//    RequestPath = "/Certificate"
//});

app.UseCors();
app.UseRouting();

app.UseMiddleware<JwtMiddleware>();
//app.UseMiddleware<RequestLoggingMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();
