using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Pharmacy_3.Data;
using Pharmacy_3.Interfaces;
using Pharmacy_3.Models;
using Pharmacy_3.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddDbContext<PharmacyContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddRoles<IdentityRole>()
	.AddEntityFrameworkStores<PharmacyContext>();


builder.Services.AddAuthorization(options =>
{
	options.AddPolicy("AdminOnly", policy =>
		policy.RequireRole("Admin"));
	options.AddPolicy("UserOnly", policy =>
		policy.RequireRole("User"));
});


builder.Services.AddRazorPages();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();


var scope = app.Services.CreateScope();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

// Create roles if they don't exist yet
if (!await roleManager.RoleExistsAsync("Admin"))
{
	await roleManager.CreateAsync(new IdentityRole("Admin"));
}

if (!await roleManager.RoleExistsAsync("User"))
{
	await roleManager.CreateAsync(new IdentityRole("User"));
}

// Create admin
var adminEmail = "admin@example.com";
var adminUser = await userManager.FindByEmailAsync(adminEmail);

if (adminUser == null)
{
	var newAdmin = new User("Admin", "Admin")
	{
		UserName = adminEmail,
		Email = adminEmail
	};

	var createAdminResult = await userManager.CreateAsync(newAdmin, "Ooq43$63");

	if (createAdminResult.Succeeded)
	{
		// Підтвердження електронної пошти
		var emailConfirmationToken = await userManager.GenerateEmailConfirmationTokenAsync(newAdmin);
		await userManager.ConfirmEmailAsync(newAdmin, emailConfirmationToken);

		// Додавання ролі адміністратора
		await userManager.AddToRoleAsync(newAdmin, "Admin");
		Console.WriteLine("Administrator created successfully!");
	}
	else
	{
		Console.WriteLine("Could not create an administrator:");
		foreach (var error in createAdminResult.Errors)
		{
			Console.WriteLine($"- {error.Description}");
		}
	}
}

app.UseStatusCodePagesWithRedirects("/Account/Login?returnUrl={0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

var supportedCultures = new[] { new CultureInfo("uk-UA") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
	DefaultRequestCulture = new RequestCulture("uk-UA"),
	SupportedCultures = supportedCultures,
	SupportedUICultures = supportedCultures
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
