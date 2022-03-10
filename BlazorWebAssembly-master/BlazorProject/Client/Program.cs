using Blazored.Modal;
using Blazored.Toast;
using BlazorProject.Client;
using BlazorProject.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient<IEmployeeService, EmployeeService>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddHttpClient<IDepartmentService, DepartmentService>(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

//builder.Services.AddScoped<IIndexedDbFactory, IndexedDbFactory>();

// Configure HttpClient for use when talking to server backend
builder.Services.AddHttpClient("WebApplication1.ServerAPI",
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddMudServices();
builder.Services.AddScoped<IndexedEdbStore>();

builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredModal();

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

//builder.Services.AddScoped<EmployeeAdaptor>();
await builder.Build().RunAsync();
