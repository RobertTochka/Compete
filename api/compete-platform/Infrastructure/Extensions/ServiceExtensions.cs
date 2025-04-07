using AspNetCoreRateLimit;
using compete_platform.Infrastructure;
using compete_platform.Infrastructure.Interceptors;
using compete_platform.Infrastructure.Middlewares;
using compete_platform.Infrastructure.Services;
using compete_platform.Infrastructure.Services.FileStorage;
using compete_platform.Infrastructure.Services.HostServices;
using compete_platform.Infrastructure.Services.LobbyErrorHandler;
using compete_platform.Infrastructure.Services.LobbyService;
using compete_platform.Infrastructure.Services.PayEventer;
using compete_platform.Infrastructure.Services.PayEventsRepository;
using compete_platform.Infrastructure.Services.PaymentService;
using compete_platform.Infrastructure.Services.PayRepository;
using compete_poco.Hubs;
using compete_poco.Infrastructure.AutoMapperProfiles;
using compete_poco.Infrastructure.Data;
using compete_poco.Infrastructure.Middlewares;
using compete_poco.Infrastructure.Services;
using compete_poco.Infrastructure.Services.ChatService;
using compete_poco.Infrastructure.Services.LobbyService;
using compete_poco.Infrastructure.Services.TimeNotifiers;
using compete_poco.Infrastructure.Services.TokenProvider;
using compete_poco.Infrastructure.Services.UserRepository;
using compete_poco.Infrastructure.Services.UserService;
using Compete_POCO_Models.Infrastrcuture.Data;
using CompeteGameServerHandler.Infrastructure.Services;
using CompeteGameServerHandler.Infrastructure.Services.ServerRunner;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SteamWebAPI2.Utilities;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace compete_poco.Infrastructure.Extensions;

public static class ServiceExtensions
{
    private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
    {
        if (options.SameSite == SameSiteMode.None)
        {
                options.SameSite = SameSiteMode.Unspecified;
        }
    }
    public static IServiceCollection AddAuth(this IServiceCollection services, AppConfig config)
    {

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
            options.OnAppendCookie = cookieContext =>
                CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            options.OnDeleteCookie = cookieContext =>
                CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
        });
        services.AddAuthentication(options =>
        {
            options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
            .AddCookie("Cookies", opt =>
            {
                opt.ExpireTimeSpan = TimeSpan.FromDays(1);
                opt.LoginPath = "/api/auth/enter";
                opt.LogoutPath = "/api/auth/exit";
                opt.Events.OnValidatePrincipal = (_) =>
                {
                    var cooki = _.Request.Cookies[".AspNetCore.Cookies"];
                    return Task.CompletedTask;
                };
                opt.Events.OnRedirectToLogin = (_) =>
                {
                    _.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return Task.CompletedTask;
                };
            })
        .AddSteam((options) =>
        {
            options.ApplicationKey = config.SteamApiKey;
            options.SaveTokens = true;
            options.CallbackPath = "/api/auth/signin";
            options.Events.OnTicketReceived = context_ =>
            {
                var steamUserAsClaims = context_.Principal;
                var identityUser = context_.HttpContext.User;
                return Task.CompletedTask;
            };
            options.Events.OnAuthenticated = context_ =>
            {
                var steamUserAsClaims = context_.Identity;
                var nameIdentifier = steamUserAsClaims!.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
                var name = steamUserAsClaims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                context_.HttpContext.User.Claims.Append(new Claim(ClaimTypes.NameIdentifier, nameIdentifier!));
                context_.HttpContext.User.Claims.Append(new Claim(ClaimTypes.Name, name!));
                return Task.CompletedTask;
            };
        });
        services.AddSingleton<IAuthorizationHandler, AdminClaimHandler>();
        services.AddAuthorization(options =>
        {
            options.AddPolicy("RequireAdmin", policy =>
            {
                policy.Requirements.Add(new AdminClaimRequirement());
                policy.AddAuthenticationSchemes(CookieAuthenticationDefaults.AuthenticationScheme);
            });
        });
        return services;
    }
    public static IServiceCollection AddAppServices(
        this IServiceCollection services, 
        AppConfig config)
    {
        var webInterfaceFactory = new SteamWebInterfaceFactory(config.SteamApiKey);

        #region aspnet
        services.Configure<KestrelServerOptions>(options =>
        {
            options.AllowSynchronousIO = true;
        });
        services.AddMemoryCache();
        services.AddMvc(options =>
        {
            options.SuppressAsyncSuffixInActionNames = false;
        });
        services
            .AddHttpClient();
        services.AddSingleton<IUserIdProvider, UserIdProvider>();
        services.AddCors();
        services.AddSignalR()
           .AddHubOptions<EventHub>(opt => opt
               .AddFilter(new EventHubExceptionHandler()))
            .AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });
        services.AddControllers()
         .AddJsonOptions(opt =>
         {
             opt.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
         });
        #endregion
        #region app services
        services.AddAutoMapper(typeof(AutoProfile));
        services.AddSingleton<PublishValuableEventsWhenSaving>();
        services.AddStackExchangeRedisCache(opt => opt.Configuration = config.Redis);
        services.AddScoped<CConfigRepository, ConfigRepository>();
        services.AddScoped<IConfigService, ConfigService>();
        services.AddScoped<IFileStorage, FileStorage>();
        services.AddDbContext<ApplicationContext>((sp, opt) =>
        {
            var interceptor = sp.GetRequiredService<PublishValuableEventsWhenSaving>();
            opt.UseNpgsql(config.SqlKey, b => b.MigrationsAssembly("compete-platform"));
            opt.AddInterceptors(interceptor);
        }, optionsLifetime: ServiceLifetime.Singleton);
        services.AddDbContextFactory<ApplicationContext>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<CUserRepository, UserRepository>();
        services.AddScoped<CServerRepository, ServerRepository>();
        services.AddScoped<CPlatformEventRepository, PlatformEventRepository>();
        services.AddScoped<CReportRepository, ReportRepository>();
      

        services.Configure<IpRateLimitOptions>(options =>
        {
            options.EnableEndpointRateLimiting = true;
            options.StackBlockedRequests = false;
            options.RealIpHeader = "X-Forwarded-For";
            options.ClientIdHeader = "Client-Id";
            options.HttpStatusCode = 429;
            options.GeneralRules = new List<RateLimitRule>
            {
                new RateLimitRule
                {
                    Endpoint = "*",
                    Period = "1s",
                    Limit = 7
                }
            };
        });

        services.Configure<IpRateLimitPolicies>(options =>
        {
            options.IpRules = new List<IpRateLimitPolicy>();
        });


        services.AddInMemoryRateLimiting();
        services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        services.AddScoped<CPayRepository, PayRepository>();
        services.AddScoped<ILobbyService, LobbyService>();
        services.AddScoped<CLobbyRepository, LobbyRepository>();
        services.AddSingleton<ITokenProvider, JWTTokenProvider>();
        services.AddSingleton<VetoTimeNotifier, SignalRVetoTimeNotifier>();
        services.AddSingleton<MatchPrepareNotifier, SignalRMatchPrepareNotifier>();
        services.AddSingleton<IActionTimeScheduler, ActionTimeScheduler>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IServerRunner, ServerRunner>();
        services.AddScoped<IServerService, ServerService>();
        services.AddScoped<IStatsService, StatsService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddTransient<ControlSizeOfPage>();
        services.AddTransient<UserSteamAuth>();
        services.AddTransient<GameServerAuthMiddleware>();
        services.AddTransient<GlobalExceptionHandler>();
        services.AddAuth(config);
        #region hosted services
        services.AddHostedService<ProcessUserAwardsForMatches>();
        services.AddHostedService<HandleStaleLobbies>();
        services.AddHostedService<UserRatingUpdater>();
        services.AddHostedService<CheckServersHealthy>();
        #endregion
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<CPayEventsRepository, PayEventRepository>();
        services.AddScoped<IPayEventer, PayEventer>();
        services.AddScoped<ILobbyChangingNotifier, SignalRLobbyChangingNotifier>();
        services.AddScoped<ILobbyErrorHandler, LobbyErrorHandler>();
        return services.AddSingleton(webInterfaceFactory);
        #endregion

    }
}
