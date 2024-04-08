using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Core.Filters;
using System.Text;
using Core.GeneralHelpers;
using Business.HostedServices;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;

namespace Business.ServiceRegistrations
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration conf)
        {
            services.AddControllers(x =>
            {
                x.Filters.Add<ExceptionFilter>();
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddHttpContextAccessor();

            // helpers
            AddHelpers(services);

            // hosted services
            HostedServices(services);

            // cors
            AddCorsPolicy(services, conf);

            // jwt
            AddJwtPolicy(services, conf);

            // data protection
            DataProtectionPolicy(services);

            // static folders
            CreateStaticFolders(conf);

            return services;
        }

        private static void DataProtectionPolicy(IServiceCollection services)
        {
            var directory = Directory.GetCurrentDirectory().Split("\\");
            services.AddDataProtection().UseCryptographicAlgorithms(new AuthenticatedEncryptorConfiguration()
            {
                EncryptionAlgorithm = EncryptionAlgorithm.AES_256_GCM,
                ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
            }).PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(directory.Take(directory.Length - 2)
              .Aggregate("", Path.Combine), "keys"))).SetApplicationName("app_name").SetDefaultKeyLifetime(TimeSpan.FromDays(14));
        }
        private static void CreateStaticFolders(IConfiguration conf)
        {
            conf.GetSection("StaticFolderPaths").GetChildren().Select(x => Path.Combine(Directory.GetCurrentDirectory(), x.Value!))
                .Where(x => !Directory.Exists(x)).ToList().ForEach((path) => { Directory.CreateDirectory(path); });
        }
        private static void HostedServices(IServiceCollection services)
        {
            services.AddHostedService<RabbitMqHostedService>();
        }
        private static void AddHelpers(IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqHelper, RabbitMqHelper>();
            services.AddSingleton<IRestSharpHelper, RestSharpHelper>();
        }
        private static void AddCorsPolicy(IServiceCollection services, IConfiguration conf)
        {
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins(conf.GetSection("Origins").GetChildren().Select(x => x.Value).ToArray()!);
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                    builder.Build();
                });
            });
        }
        private static void AddJwtPolicy(IServiceCollection services, IConfiguration conf)
        {
            services.AddAuthentication().AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.Events = new JwtBearerEvents
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                        {
                            Success = false,
                            Message = "Invalid access token."
                        }));
                    }
                };
                x.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(conf["Audience:Secret"]!)),
                    ValidIssuer = conf["Audience:Iss"],
                    ValidAudience = conf["Audience:Aud"],
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true
                };
            });
        }
    }
}