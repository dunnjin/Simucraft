using AutoMapper;
using Microsoft.Azure.Cosmos.Linq;
using Simucraft.Server.Core;
using Simucraft.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Simucraft.Server.Mappings
{
    public class GameProfile : Profile
    {
        public GameProfile()
        {
            base.CreateMap<GameRequest, Game>();
            base.CreateMap<Game, GameResponse>();
            base.CreateMap<ChatMessageRequest, ChatMessage>();
            base.CreateMap<ChatMessage, ChatMessageResponse>();
            base.CreateMap<AuthorizedUserRequest, AuthorizedUser>();
            base.CreateMap<AuthorizedUser, AuthorizedUserResponse>();
            base.CreateMap<InvitedUser, InvitedUserResponse>();

            base.CreateMap<GameCharacter, GameCharacterResponse>();
            base.CreateMap<GameCharacterStat, GameCharacterStatResponse>();
            base.CreateMap<GameCharacterStatRequest, GameCharacterStat>();

            base.CreateMap<CharacterSkill, GameCharacterSkill>()
                .ForMember(d => d.SkillId, c => c.MapFrom(s => s.Id));
            base.CreateMap<GameCharacterSkillRequest, GameCharacterSkill>();
            base.CreateMap<GameCharacterSkill, GameCharacterSkillResponse>();

            base.CreateMap<Character, GameCharacter>()
                .ForMember(s => s.CharacterId, c => c.MapFrom(d => d.Id))
                .ForMember(d => d.HealthPoints, c => c.Ignore())
                .ForMember(s => s.MaxHealthPoints, c => c.MapFrom(d => d.HealthPoints))
                .ForMember(d => d.CarryingCapacity, c => c.Ignore())
                .ForMember(s => s.MaxCarryingCapacity, c => c.MapFrom(d => d.CarryingCapacity))
                ;
            base.CreateMap<GameCharacterRequest, GameCharacter>();

            base.CreateMap<GameNoteRequest, GameNote>();
            base.CreateMap<GameNote, GameNoteResponse>();

            base.CreateMap<Game, GameInformationResponse>();
            base.CreateMap<GameInformationRequest, Game>()
                .ForMember(d => d.Id, c => c.Ignore())
                .ForMember(d => d.RulesetId, c => c.Ignore())
                .ForMember(d => d.UserId, c => c.Ignore());

            base.CreateMap<GameStateInformationRequest, Game>()
                .ForMember(d => d.Id, c => c.Ignore())
                .ForMember(d => d.RulesetId, c => c.Ignore())
                .ForMember(d => d.UserId, c => c.Ignore());
            base.CreateMap<Game, GameStateInformationResponse>();

            base.CreateMap<CharacterStat, GameCharacterStat>();
            base.CreateMap<CollisionTile, GameCollisionTile>();
            base.CreateMap<GameCollisionTile, ColliisionTileResponse>();

            base.CreateMap<Weapon, GameCharacterWeapon>()
                .ForMember(d => d.WeaponId, c => c.MapFrom(s => s.Id));
            base.CreateMap<GameCharacterWeaponRequest, GameCharacterWeapon>();
            base.CreateMap<GameCharacterWeapon, GameCharacterWeaponResponse>();
        }
    }
}
