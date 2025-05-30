using AuthAPI.DataBase;
using AuthAPI.Helper;
using AuthAPI.Helper.Builders;
using AuthAPI.Models;
using AuthAPI.Repositories;
using AuthAPI.Service;
using dotenv.net;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Config
{
    public static class AppConfig
    {
        public static void StartDependencies(WebApplicationBuilder builder)
        {
            var envVars = DotEnv.Read();
            string connectionString = envVars["DATABASECONNECTION"];
            string SecretKey = envVars["SECRETKEY"];

            try
            {
                ConfigureDataBase(builder.Services, connectionString);
                ConfigureRepositories(builder.Services);
                CORSConfig.ConfigureCORS(builder.Services);
                JWTConfig.ConfigureJWT(builder.Services, SecretKey);

                builder.Services.AddControllers();

                // Add services to the container.
                // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
                builder.Services.AddOpenApi();
            }
            catch (Exception ex)
            {
                throw new Exception($"Erro ao iniciar as dependencias: {ex.Message}", ex);
            }
        }

        public static void ConfigureDataBase(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
        }

        public static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IResponseBuilder<User>, AuthResponseBuilder>();
            services.AddScoped<IResponseBuilder<UserLoginDTO>, LoginResponseBuilder>();
            services.AddScoped<UserProfileService>();
            services.AddScoped<UserSingInService>();
            services.AddScoped<UserSingUpService>();
        }
    }
}