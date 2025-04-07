using AspNetCoreRateLimit;
using compete_platform.Infrastructure;
using compete_platform.Infrastructure.Extensions;
using compete_platform.Infrastructure.Middlewares;
using compete_poco.Hubs;
using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Extensions;
using compete_poco.Infrastructure.Middlewares;
using Compete_POCO_Models.Infrastrcuture.Data;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using Serilog;
var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();
var config = builder.Configuration.Get<AppConfig>(opt => opt.BindNonPublicProperties = true);
builder.Services.AddSingleton(config);
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddTransient<ShowHeaders>();
builder.Host.UseSerilog(Log.Logger);
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});


// Добавляем Swagger в контейнер сервисов
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
});


builder.Services.AddAppServices(config);
builder.Logging.AddSerilog(Log.Logger);
var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    //await app.TestReportCreation();
    //await app.TestMultipleReportCreation();
    //await app.TestCompletedLobbyCancellation();
    //await app.TestLastPayEvents();
    //await app.TestLobbyCancel();
    //await app.CreateFakeDataForDatabase(true);
    //await app.TestUniqueConstraintOnPayment();
    //await app.CreateFakePayEvents();
    //await app.TestServiceForAwardProcessing();
    //app.TestConcurrencyUserBalanceUpdate();
    //await app.TestConcurencyPayment();
    //await app.TestTrnsactionApperarance();
    //await app.TestConcurencyWihdrawal();
}

app.UseSwagger();
app.UseSwaggerUI();

await app.CheckMigrationsAsync();
app.UseIpRateLimiting();
await app.CheckCommonChatExisting();
app.CheckContentFolderForExisting();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, AppDictionary.UploadedFiles)),
    RequestPath = AppConfig.RelativeFilePath,
});
await app.CheckDefaultConfigsExisting();
app.UseSession();
app.UseCors(opt => opt
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:5027")
    .AllowCredentials());
app.UseMiddleware<ShowHeaders>();
app.UseMiddleware<GlobalExceptionHandler>();
app.UseMiddleware<GameServerAuthMiddleware>();
app.UseMiddleware<ControlSizeOfPage>();
app.UseCookiePolicy();
app.UseAuthentication();
app.MapHub<EventHub>("/api/eventHub");
app.UseMiddleware<UserSteamAuth>();
app.UseAuthorization();

app.MapControllers();

app.Run();
