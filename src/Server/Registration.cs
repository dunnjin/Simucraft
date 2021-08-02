using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SendGrid;
using Simucraft.Server.Common;
using Simucraft.Server.DataAccess;
using Simucraft.Server.Hubs;
using Simucraft.Server.Middleware;
using Simucraft.Server.Services;
using Simucraft.Server.Strategies;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Simucraft.Server
{
    public static class Registration
    {
        public static IServiceCollection AddSimucraft(this IServiceCollection services)
        {
            var cosmosDBConnectionString = Environment.GetEnvironmentVariable(Constants.AZURE_DATABASE_CONNECTION_STRING);
            var split = cosmosDBConnectionString.Split(";");
            var endpoint = Regex.Match(split.First(s => s.Contains("AccountEndpoint")), @"(?<=AccountEndpoint=).*").Value;
            var key = Regex.Match(split.First(s => s.Contains("AccountKey")), @"(?<=AccountKey=).*").Value;

            services.AddDbContext<SimucraftContext>(c =>
                 c.UseCosmos(endpoint, key, "Simucraft"));

            services.AddTransient<IMapService, MapService>();
            services.AddTransient<IRulesetService, RulesetService>();
            services.AddTransient<ICharacterService, CharacterService>();
            services.AddTransient<IWeaponService, WeaponService>();
            services.AddTransient<IEquipmentService, EquipmentService>();
            services.AddTransient<ISpellService, SpellService>();
            services.AddTransient<ISkillService, SkillService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<IBlobStorage, AzureBlobStorage>();
            services.AddTransient<IRulesetBlobStorage, RulesetBlobStorage>();
            services.AddTransient<IGameHubService, GameHubService>();
            services.AddTransient<IGameStrategyService, GameStrategyService>();
            services.AddTransient<IGameStrategy, LoadMapStrategy>();
            services.AddTransient<IGameStateStrategyService, GameStateStrategyService>();
            services.AddTransient<IGameStateStrategy, AddCharacterStrategy>();
            services.AddTransient<IGameStateStrategy, RequestPlacementStrategy>();
            services.AddTransient<IGameStateStrategy, StartCombatStrategy>();
            services.AddTransient<IGameStateStrategy, EndCombatStrategy>();
            services.AddTransient<IGameStateStrategy, EndTurnStrategy>();
            services.AddTransient<IGameStateStrategy, CombatPlacementStrategy>();
            services.AddTransient<IGameStateStrategy, CombatAttackStrategy>();
            services.AddTransient<IGameStateStrategy, UpdateGameCharacterStrategy>();
            services.AddTransient<IGameStateStrategy, RollGameCharacterStrategy>();
            services.AddTransient<IGameStateStrategy, CombatRequestMoveStrategy>();
            services.AddTransient(f => new SendGridClient(Environment.GetEnvironmentVariable(Constants.SENDGRID_APIKEY)));
            services.AddTransient<IEmailService, SendGridEmailService>(f => new SendGridEmailService(f.GetService<SendGridClient>(), Environment.GetEnvironmentVariable(Constants.SIMUCRAFT_EMAIL)));
            services.AddScoped<ExceptionFilterMiddleware>();

            return services;
        }

        public static IEndpointRouteBuilder AddSimucraft(this IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapHub<GameHub>("/gameHub");

            return endpointRouteBuilder;
        }
    }
}
