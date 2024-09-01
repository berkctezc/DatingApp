using API.Extensions;
using API.Middleware;
using API.SignalR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API;

public class Startup(IConfiguration config)
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddApplicationServices(config)
			.AddControllers();
		services.AddCors()
			.AddIdentityServices(config)
			.AddSignalR();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		app.UseMiddleware<ExceptionMiddleware>()
			.UseHttpsRedirection()
			.UseRouting()
			.UseAuthorization()
			.UseCors(
				x => x.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials()
					.WithOrigins("https://localhost:4200"))
			.UseAuthentication()
			.UseDefaultFiles()
			.UseStaticFiles()
			.UseEndpoints(
				endpoints =>
				{
					endpoints.MapControllers();
					endpoints.MapHub<PresenceHub>("hubs/presence");
					endpoints.MapHub<MessageHub>("hubs/message");
					endpoints.MapFallbackToController("Index", "Fallback");
				});
	}
}