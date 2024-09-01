using System;
using API.Data;
using API.Helpers;
using API.Interfaces;
using API.Services;
using API.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
	public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
	{
		services.AddSingleton<PresenceTracker>()
			.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"))
			.AddScoped<ITokenService, TokenService>()
			.AddScoped<IPhotoService, PhotoService>()
			.AddScoped<IUnitOfWork, UnitOfWork>()
			.AddScoped<LogUserActivity>()
			.AddAutoMapper(typeof(AutoMapperProfiles).Assembly)
			.AddDbContext<DataContext>(options =>
			{
				var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
				string connStr;

				if (env is "Development")
					connStr = config.GetConnectionString("DefaultConnection");
				else
				{
					var connUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
					connUrl = connUrl.Replace("postgres://", string.Empty);
					var pgUserPass = connUrl.Split("@")[0];
					var pgHostPortDb = connUrl.Split("@")[1];
					var pgHostPort = pgHostPortDb.Split("/")[0];
					var pgDb = pgHostPortDb.Split("/")[1];
					var pgUser = pgUserPass.Split(":")[0];
					var pgPass = pgUserPass.Split(":")[1];
					var pgHost = pgHostPort.Split(":")[0];
					var pgPort = pgHostPort.Split(":")[1];

					connStr = $"Server={pgHost};Port={pgPort};User Id={pgUser};Password={pgPass};Database={pgDb}";
				}

				options.UseNpgsql(connStr);
			});

		return services;
	}
}