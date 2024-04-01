using BlazorAuthentication.Components;
using BlazorAuthentication.Components.Account;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Composition;
using System.Security.Claims;
using XmlIdentity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PasswordHasherOptions>(options => options.IterationCount = 600_000);

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

builder.Services.AddTransient<IUserStore<ApplicationUser>, XmlUserStore>();
builder.Services.AddTransient<IRoleStore<IdentityRole>, XmlRoleStore>();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredLength = 16;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredUniqueChars = 5;
})
    .AddUserStore<XmlUserStore>()
    .AddRoles<IdentityRole>()
    .AddRoleStore<XmlRoleStore>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ForumWebHost");
/*
builder.Services.AddDataProtection()
    .UseCustomCryptographicAlgorithms(new CngCbcAuthenticatedEncryptorConfiguration
    {
        // Passed to BCryptOpenAlgorithmProvider
        EncryptionAlgorithm = "AES",
        EncryptionAlgorithmProvider = null,

        // Specified in bits
        EncryptionAlgorithmKeySize = 512,

        // Passed to BCryptOpenAlgorithmProvider
        HashAlgorithm = "SHA512",
        HashAlgorithmProvider = null
    })
    .SetApplicationName("ForumWebHost")
    .PersistKeysToFileSystem(new DirectoryInfo(path))
    .ProtectKeysWithDpapiNG();
*/
builder.Services.AddAuthorization(options =>
{
  
    options.AddPolicy("RequireAdminRole", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, new[] { UserRoleEnum.NetworkAdministrator.ToString(), UserRoleEnum.SiteAdministrator.ToString(), UserRoleEnum.Administrator.ToString() });
    });
});



// builder.Services.AddIdentity<ApplicationUser, IdentityRole>();


// builder.Services.AddIdentity<ApplicationUser, IdentityRole>();
/*


builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    //         .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddRoles<ApplicationUser>()
//    .AddRoleStore<IRoleStore<ApplicationUser>>()
    .AddDefaultTokenProviders();
*/

//builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SiteAdministrator", policy => policy.RequireRole("SiteAdministrator"));
    options.AddPolicy("Administrator", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("Maintainer", policy => policy.RequireRole("Maintainer"));
    options.AddPolicy("MonitorPlus", policy => policy.RequireRole("MonitorPlus"));
    options.AddPolicy("Monitor", policy => policy.RequireRole("Monitor"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
      app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    foreach (var role in Enum.GetNames(typeof(UserRoleEnum)))
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}



app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
