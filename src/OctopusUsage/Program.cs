using System.Reflection;
using OctopusUsage.Modules.EnergyUsage.Extensions;
using Simcube.AspNetCore.Modules.Microsoft.Extensions;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets(Assembly.GetExecutingAssembly());
}

builder.Host.UseSerilog((_, lc) => lc
    .WriteTo.Console());

builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddMemoryCache();

builder.RegisterModules();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();

app.MapModuleEndpoints();
app.ExecuteModulePreRunActions();

// Fire jobs on Service Startup.
await app.ExecuteRegisteredJobsAsync().ConfigureAwait(false);

app.Run();