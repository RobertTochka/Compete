using compete_platform.Dto;
using compete_platform.Infrastructure.Services;
using compete_platform.Infrastructure.Services.LobbyService;
using compete_platform.Infrastructure.Services.PaymentService;
using compete_poco.Infrastructure.Services;
using compete_poco.Infrastructure.Services.ChatService;
using compete_poco.Infrastructure.Services.UserRepository;
using compete_poco.Models;
using Compete_POCO_Models;
using Compete_POCO_Models.Infrastrcuture.Data;
using Compete_POCO_Models.Models;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Yandex.Checkout.V3;

namespace compete_platform.Infrastructure.Extensions;

public static class AppExtensions
{
    public static void CheckContentFolderForExisting(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var contentFolder = Path.Combine(env.ContentRootPath, AppDictionary.UploadedFiles);
        if (!Directory.Exists(contentFolder))
            Directory.CreateDirectory(contentFolder);

    }
    public static async Task<WebApplication> CheckMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateAsyncScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        if (ctx.Database.GetPendingMigrations().Any())
            await ctx.Database.MigrateAsync();
        return app;
    }
    public static async Task<WebApplication> CreateFakeDataForDatabase(this WebApplication app,
        bool lobbyPagingTest = false,
        bool userStatsTest = false)
    {
        using var scope = app.Services.CreateAsyncScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var user = await ctx.Users.FirstAsync(u => u.Id == 1);
        var secondaryUser = await ctx.Users.FirstAsync(u => u.Id == 3);
        if (lobbyPagingTest)
        {
            #region создание фейковых лобби для теста отображения доступных лобби для входа, то есть они непригодны для тестинга конфигурации

            var mockupLobbies = Enumerable.Range(1, 1).Select(t => new Lobby()
            {
                CreatorId = 1, // Hardcode already exsited user
                Public = true,
                ServerId = 5,// Hardcode already exsited server
                Status = LobbyStatus.Canceled,
                Bids = new List<UserBid>() { new() { Bid = 0, UserId = 1 } },
                Chat = new Chat(),
                MapActions = Enumerable.Range(1, 4)
                .Select(c => new MapActionInfo()
                {
                    ActionTime = DateTime.UtcNow,
                    IsPicked = true,
                    Map = (Map)c,
                    TeamId = 3
                }).ToList(),
                Teams = new List<Team>() { new()
            {
                CreatorId = 1,
                Users = new List<User>() { user } ,
                Chat = new Chat(),
                Name = $"{t}"
            },
                new()
            {
                CreatorId = 3,
                Users = new List<User>(){ secondaryUser} ,
                Chat = new Chat(),
                Name = $"{t+1}"
            }
                },
                PlayersAmount = (compete_poco.Models.Type)Random.Shared.Next((int)compete_poco.Models.Type.v1, (int)compete_poco.Models.Type.v5),
                CreateTime = DateTime.UtcNow
            });
            await ctx.Lobbies.AddRangeAsync(mockupLobbies);
            await ctx.SaveChangesAsync();
            #endregion
        }
        if (userStatsTest)
        {
            var awards = Enumerable
                .Range(1, 2000)
                .Select(t => Random.Shared.Next(-1, 2) * 500)
                .Select(s =>
                {
                    var award = AwardType.Lose;
                    if (s >= 0)
                    {
                        if (s == 0)
                            award = AwardType.MatchCanceled;
                        else
                            award = AwardType.Win;
                    }
                    return new UserAward()
                    {
                        Award = s,
                        AwardType = award,
                        UserId = 1,
                        PayTime = DateTime.UtcNow
                    };
                });
            var profit = awards.Sum(a => a.Award);
            var income = awards.Where(a => a.AwardType == AwardType.Win).Sum(a => a.Award);
            var awardLobby = new Lobby()
            {
                CreatorId = 1, // Hardcode already exsited user
                Public = true,
                ServerId = 5,// Hardcode already exsited server
                Status = LobbyStatus.Over,
                Bids = new List<UserBid>() { new() { Bid = 0, UserId = 1 } },
                Chat = new Chat(),
                Awards = awards.ToList(),
                Teams = new List<Team>() { new()
            {
                CreatorId = 1,
                Users = new List<User>() { user } ,
                Chat = new Chat(),
                Name = $"testc balance"
            } },
                PlayersAmount = (compete_poco.Models.Type)Random.Shared.Next((int)compete_poco.Models.Type.v1, (int)compete_poco.Models.Type.v5),
                CreateTime = DateTime.UtcNow
            };

            await ctx.Lobbies.AddAsync(awardLobby);
            await ctx.SaveChangesAsync();
            Console.WriteLine($"PROFIT - [{profit}\n INCOME - [${income}");
        }
        return app;
    }
    public static async Task<WebApplication> TestServiceForAwardProcessing(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var user = await ctx.Users.FirstAsync();
        var lobby = await ctx.Lobbies.FirstAsync();
        await ctx.Awards.AddRangeAsync(Enumerable.Range(0, 20).Select(t => new UserAward()
        {
            AwardType = AwardType.Win,
            LobbyId = lobby.Id,
            UserId = user.Id,
            PayTime = DateTime.MinValue,
            Award = Random.Shared.Next(2, 50)
        }));
        await ctx.SaveChangesAsync();
        return app;
    }
    public static WebApplication TestLobbyStateChanging(this WebApplication application)
    {
        Task.Run(async () =>
        {
            using var scope = application.Services.CreateScope();
            var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
            var user = await ctx.Users.Where(s => s.Id == 1).FirstAsync();
            var lobbySrv = scope.ServiceProvider.GetRequiredService<ILobbyService>();
            var lobby = await lobbySrv.CreateLobbyAsync(user.Id);
            var lobbyToChange = await ctx.Lobbies.Where(l => l.Id == lobby.Id).FirstAsync();
            await Task.Delay(10000);
            lobbyToChange.Status = LobbyStatus.Playing;
            await ctx.SaveChangesAsync();
        });
        return application;
    }
    public static async Task<WebApplication> TestConcurrencyUserBalanceUpdate(this WebApplication application)
    {
        var counter = 0;
        object mutex = new object();
        var tasks = Enumerable.Range(1, 25).Select(t => Task.Run(async () =>
        {
            using var scope = application.Services.CreateScope();
            var userSrv = scope.ServiceProvider.GetRequiredService<IUserService>();
            await userSrv.TopUpUserBalance(50, 2);
            lock (mutex)
            {
                counter++;
            }
        })).ToArray();
        await Task.WhenAll(tasks);
        return application;
    }
    public static async Task<WebApplication> TestConcurencyPayment(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var paySrv = scope.ServiceProvider.GetRequiredService<IPaymentService>();
        var payment = new Payment()
        {
            Amount = new() { Value = 50, Currency = "RUB" },
            Metadata = new Dictionary<string, string>() { { "UserId", "2" } },
            Id = "555"
        };
        await paySrv.HandleSuccessPayment(payment);
        return app;
    }
    public static async Task<WebApplication> TestTrnsactionApperarance(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var _userSrc = scope.ServiceProvider.GetRequiredService<CUserRepository>();
        Console.WriteLine();
        await _userSrc.SaveChangesAsync();
        return app;
    }
    public static async Task<WebApplication> TestConcurencyWihdrawal(this WebApplication app)
    {
        var counter = 0;
        object mutex = new object();
        var tasks = Enumerable.Range(1, 25).Select(t => Task.Run(async () =>
        {
            using var scope = app.Services.CreateScope();
            var userSrv = scope.ServiceProvider.GetRequiredService<IUserService>();
            var randomUser = 2;
            await userSrv.WithdrawalFunds(randomUser, 50);
            lock (mutex)
            {
                counter++;
            }
        })).ToArray();
        try
        {
            await Task.WhenAll(tasks);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        return app;
    }
    public static async Task CreateFakePayEvents(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var results = new List<PayEvent>();
        var user = await ctx.Users.Select(s => s.Id).FirstAsync();
        var payEvents = Enumerable.Range(1, 20).Select(s =>
        {
            var payout = Random.Shared.Next(0, 2) == 1;
            var sholudCreateResult = Random.Shared.Next(0, 2) == 1;
            var processing = new PayEvent()
            {
                Amount = 50,
                CreatedUtc = DateTime.UtcNow,
                UserId = user,
                PayState = payout ? PayState.RequestPayout : PayState.RequestTopUp,
            };
            if (sholudCreateResult)
            {
                var shouldBeFailed = Random.Shared.Next(0, 2) == 1;
                var minutesOfHandling = Random.Shared.Next(0, 5);
                PayState state;
                if (shouldBeFailed)
                {
                    if (payout)
                        state = PayState.RequestPayoutFailed;
                    else
                        state = PayState.TopUpFailed;
                }
                else
                {
                    if (payout)
                        state = PayState.RequestPayoutSuccess;
                    else
                        state = PayState.TopUpSuccess;
                }
                var resulting = new PayEvent()
                {
                    Amount = 50,
                    CreatedUtc = processing.CreatedUtc + TimeSpan.FromMinutes(minutesOfHandling),
                    UserId = user,
                    PayState = state
                };
                results.Add(resulting);
            }
            return processing;
        });
        results.AddRange(payEvents);
        ctx.PayEvents.AddRange(results);
        await ctx.SaveChangesAsync();
    }
    public static async Task CheckDefaultConfigsExisting(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var configProvider = scope.ServiceProvider.GetRequiredService<IConfigService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();
        try
        {
            await configProvider.CreateConfigsByDefault();
            logger.LogInformation("Конфигурацию по умолчанию создана");
        }
        catch (DbUpdateException ex)
        {
            if (ex.InnerException is PostgresException && ((PostgresException)ex.InnerException)
                .SqlState == PostgresErrorCodes.UniqueViolation)
                logger.LogInformation("Конфигурация по умолчанию уже создана");
        }
    }
    public static async Task CheckCommonChatExisting(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var chatServ = scope.ServiceProvider.GetRequiredService<IChatService>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<WebApplication>>();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        try
        {
            await chatServ.GetCommonChatId();
        }
        catch (InvalidOperationException)
        {
            logger.LogInformation("Creating common chat");
            await ctx.Chats.AddAsync(new());
            await ctx.SaveChangesAsync();
        }
    }
    public static async Task TestUniqueConstraintOnPayment(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var payemntId = Guid.NewGuid().ToString();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        await ctx.PayEvents.AddAsync(new()
        {
            Amount = 50,
            CreatedUtc = DateTime.UtcNow,
            UserId = 1,
            PaymentId = payemntId,
            PayState = PayState.TopUpSuccess,
        });
        await ctx.SaveChangesAsync();
        await ctx.PayEvents.AddAsync(new()
        {
            Amount = 50,
            CreatedUtc = DateTime.UtcNow,
            UserId = 1,
            PaymentId = payemntId,
            PayState = PayState.TopUpSuccess,
        });
        await ctx.SaveChangesAsync();
    }
    public static async Task TestReportCreation(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var reportProvider = scope.ServiceProvider.GetRequiredService<IReportService>();
        var rightAnswer = new CreateReportDto() { Content = "fd", Subject = "f", LobbyId = 42170, UserId = 1 };
        await reportProvider.OpenReport(rightAnswer);
        var wrongAnswer = new CreateReportDto() { Content = "fd", Subject = "fd", LobbyId = 42170, UserId = 3 };
        try
        {
            await reportProvider.OpenReport(wrongAnswer);
        }
        catch (ApplicationException)
        {
            Console.WriteLine("Not participan of lobby cannot create report to this lobby");
        }

    }
    public static async Task TestMultipleReportCreation(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var rightAnswers = Enumerable
            .Range(0, 1000)
            .Select(t => new Report() { Content = "fd", Subject = "f", LobbyId = 42170, UserId = 1 });
        await ctx.Reports.AddRangeAsync(rightAnswers);
        await ctx.SaveChangesAsync();

    }
    public static async Task TestLobbyCancel(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var user = await ctx.Users.FirstAsync(u => u.Id == 1);
        var lobbyServ = scope.ServiceProvider.GetRequiredService<ILobbyService>();
        var secondaryUser = await ctx.Users.FirstAsync(u => u.Id == 3);
        var mockupLobby = new Lobby()
        {
            CreatorId = 1, // Hardcode already exsited user
            Public = true,
            ServerId = 5,// Hardcode already exsited server
            Status = LobbyStatus.Playing,
            LastServerUpdate = DateTime.UtcNow,
            Bids = new List<UserBid>() { new() { Bid = 0, UserId = 1 } },
            Chat = new Chat(),
            MapActions = Enumerable.Range(1, 4)
            .Select(c => new MapActionInfo()
            {
                ActionTime = DateTime.UtcNow,
                IsPicked = true,
                Map = (Map)c,
                TeamId = 3
            }).ToList(),
            Teams = new List<Team>() { new()
        {
            CreatorId = 1,
            Users = new List<User>() { user } ,
            Chat = new Chat(),
            Name = $"{2}"
        },
            new()
        {
            CreatorId = 3,
            Users = new List<User>(){ secondaryUser} ,
            Chat = new Chat(),
            Name = $"{+1}"
        }
            },
            PlayersAmount = (compete_poco.Models.Type)Random.Shared.Next((int)compete_poco.Models.Type.v1, (int)compete_poco.Models.Type.v5),
            CreateTime = DateTime.UtcNow
        };
        await ctx.Lobbies.AddAsync(mockupLobby);
        await ctx.SaveChangesAsync();
        await lobbyServ.Cancel_Lobby(mockupLobby.Id);
    }
    public static async Task TestUserOperations(this WebApplication app)
    {
        var tasks = Enumerable.Range(1, 100).Select(t =>
        {
            var variant = Random.Shared.Next(0, 2) == 1;

            if (variant)
            {
                return Task.Run(async () =>
                {
                    try
                    {
                        using var scope = app.Services.CreateScope();
                        var userSrv = scope.ServiceProvider.GetRequiredService<IUserService>();
                        await userSrv.TopUpUserBalance(
                        50, 1, Guid.NewGuid().ToString(), null);
                        Console.WriteLine("Пополнение баланса прошло успешно");
                    }
                    catch (PostgresException ex)
                    {
                        Console.WriteLine(ex.MessageText);
                    }
                }
                );
            }
            return Task.Run(async () =>
            {
                try
                {
                    using var scope = app.Services.CreateScope();
                    var userSrv = scope.ServiceProvider.GetRequiredService<IUserService>();
                    await userSrv.SetUserAvailability(1, true);
                    Console.WriteLine("Статус доступности установился");
                }
                catch (PostgresException ex)
                {
                    Console.WriteLine(ex.MessageText);
                }
            });
        });
        await Task.WhenAll(tasks);
    }
    private static Lobby GetCompletedLobby()
    {
        var result = new Lobby()
        {
            Awards = new[]
            {
                new UserAward() {AwardType = AwardType.Win, Award = 50, UserId = 1 },
                new UserAward(){AwardType = AwardType.Lose, Award = -50, UserId = 2 },
            },
            Status = LobbyStatus.Over,
            CreatorId = 1,
            Chat = new(),
            Server = new() { Location = "f", Path = "f" }
        };
        return result;
    }
    public static async Task TestCompletedLobbyCancellation(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
        var lobbySrv = scope.ServiceProvider.GetRequiredService<ILobbyService>();
        var lobby = GetCompletedLobby();
        await ctx.Lobbies.AddAsync(lobby);
        await ctx.SaveChangesAsync();
        await lobbySrv.Cancel_Lobby(lobby.Id);
    }
}
