using DirectoryChangesTracker.Configuration;
using DirectoryChangesTracker.Repositories;
using DirectoryChangesTracker.Services;
using DirectoryChangesTracker.Validators;
using Microsoft.Extensions.Options;

namespace DirectoryChangesTracker
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.
			builder.Services.AddControllersWithViews();

			// Add services via DI
			builder.Services.Configure<DirectoryScannerSettings>(builder.Configuration.GetSection("DirectoryScannerSettings"));
			builder.Services.AddScoped<IDirectoryValidator, DirectoryValidator>();
			builder.Services.AddScoped<IDirectoryScanner, DirectoryScanner>();
			builder.Services.AddScoped<IDirectorySnapshotComparer, DirectorySnapshotComparer>();
			builder.Services.AddScoped<IFileValidator, FileValidator>();
			builder.Services.AddScoped<IDirectoryScanRepository>(sp =>
			{
				DirectoryScannerSettings settings = sp.GetRequiredService<IOptions<DirectoryScannerSettings>>().Value;
				return new JsonDirectoryScanRepository(settings.GetResolvedOutputPath());
			});


			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (!app.Environment.IsDevelopment())
			{
				app.UseExceptionHandler("/Home/Error");
				// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
				app.UseHsts();
			}

			app.UseHttpsRedirection();
			app.UseStaticFiles();

			app.UseRouting();

			app.UseAuthorization();

			app.MapControllerRoute(
				name: "default",
				pattern: "{controller=Home}/{action=Index}/{id?}");

			app.Run();
		}
	}
}
