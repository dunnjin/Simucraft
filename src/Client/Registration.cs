using Microsoft.Extensions.DependencyInjection;
using Simucraft.Client.Services;

namespace Simucraft.Client
{
    public static class RegistrationExtensions
    {
        public static IServiceCollection AddSimucraft(this IServiceCollection services)
        {
            services.AddTransient<IMapService, MapService>();
            services.AddTransient<IRulesetService, RulesetService>();
            services.AddTransient<ICharacterService, CharacterService>();
            services.AddTransient<IWeaponService, WeaponService>();
            services.AddTransient<IEquipmentService, EquipmentService>();
            services.AddTransient<ISpellService, SpellService>();
            services.AddTransient<ISkillService, SkillService>();
            services.AddTransient<IGameService, GameService>();
            services.AddTransient<GameHubService>();

            return services;
        }
    }
}
