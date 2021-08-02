using AutoMapper;
using Simucraft.Server.Core;
using Simucraft.Server.Models;
using System;

namespace Simucraft.Server.Mappings
{
    public class RulesetProfile : Profile
    {
        public RulesetProfile()
        {
            base.CreateMap<RulesetRequest, Ruleset>();
            base.CreateMap<Ruleset, RulesetResponse>();

            base.CreateMap<CharacterRequest, Character>();
            base.CreateMap<Character, CharacterResponse>();
            base.CreateMap<CharacterStatRequest, CharacterStat>();
            base.CreateMap<CharacterStat, CharacterStatResponse>();
            base.CreateMap<CharacterSkillRequest, CharacterSkill>();
            base.CreateMap<CharacterSkill, CharacterSkillResponse>();

            base.CreateMap<WeaponRequest, Weapon>();
            base.CreateMap<Weapon, WeaponResponse>();

            base.CreateMap<EquipmentRequest, Equipment>()
                .ForMember(d => d.EquipmentType, c => c.MapFrom(s => Enum.Parse<EquipmentType>(s.EquipmentType)));
            base.CreateMap<EquipmentExpressionRequest, EquipmentExpression>();
            base.CreateMap<Equipment, EquipmentResponse>()
                .ForMember(d => d.EquipmentType, c => c.MapFrom(s => s.EquipmentType.ToString()));
            base.CreateMap<EquipmentExpression, EquipmentExpressionResponse>();

            base.CreateMap<SpellRequest, Spell>();
            base.CreateMap<Spell, SpellResponse>();

            base.CreateMap<SkillRequest, Skill>()
                .ForMember(d => d.SkillType, c => c.MapFrom(s => Enum.Parse<SkillType>(s.SkillType)));
            base.CreateMap<SkillExpressionRequest, SkillExpression>();
            base.CreateMap<Skill, SkillResponse>()
                .ForMember(d => d.SkillType, c => c.MapFrom(s => s.SkillType.ToString()));
            base.CreateMap<SkillExpression, SkillExpressionResponse>();
        }
    }
}
