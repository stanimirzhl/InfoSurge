using InfoSurge.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddCoreServices();
builder.Services.AddDbServices(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.AddAccountOptions();

var app = builder.Build();

app.ApplyDatabaseMigrations();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
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
