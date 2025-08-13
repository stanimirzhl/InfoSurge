using InfoSurge.Extensions;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddCoreServices();
builder.Services.AddEmailService(builder.Configuration);
builder.Services.AddDbServices(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.AddAccountOptions();


builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.AddControllersWithViews()
	.AddViewLocalization()
	.AddDataAnnotationsLocalization();

var app = builder.Build();

CultureInfo[] supportedCultures = new[]
{
	new CultureInfo("bg-BG")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
	DefaultRequestCulture = new RequestCulture("bg-BG"),
	SupportedCultures = supportedCultures,
	SupportedUICultures = supportedCultures
});

await app.ApplyDatabaseMigrations();

if (!app.Environment.IsDevelopment())
{
	app.UseDeveloperExceptionPage();
}
else
{
	app.UseExceptionHandler("/Home/Error/500");
	app.UseStatusCodePagesWithReExecute("/Home/Error/{0}");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "areas",
	pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
