using Core.Application.Handlers;
using Core.Application.Interfaces;
using Core.ApplicationLayer.Services;
using Infrastructure.InfrastructureLayer.Configuration;
using Infrastructure.InfrastructureLayer.Data;
using Infrastructure.InfrastructureLayer.Repositories;
using Infrastructure.InfrastructureLayer.Services;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Entity Framework
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add configuration
builder.Services.Configure<WalletSettings>(builder.Configuration.GetSection(WalletSettings.SectionName));

// Add MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(CreatePaymentHandler).Assembly));

// Add repositories
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add services
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IBlockchainService, BlockchainService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();

// Add background service for payment monitoring
builder.Services.AddHostedService<PaymentMonitoringService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
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
    pattern: "{controller=Payment}/{action=Index}/{id?}");

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.Run();