using AutoMapper;
using Simucraft.Server.Common;
using Simucraft.Server.Core;
using Simucraft.Server.Models;
using System.Collections.Generic;
using MapRequest = Simucraft.Server.Core.MapRequest;

namespace Simucraft.Server.Mappings
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            base.CreateMap<MapRequest, Map>()
                //.ForMember(d => d.CollisionLayerJson, c => c.MapFrom(s => s.CollisionLayer == null ? null : s.CollisionLayer.ToJson()))
                //.ForMember(d => d.MapCharactersJson, c => c.MapFrom(s => s.MapCharacters.ToJson()));
                ;
            base.CreateMap<Map, MapResponse>()
                //.ForMember(d => d.CollisionLayer, c => c.MapFrom(s => string.IsNullOrEmpty(s.CollisionLayerJson) ? null : s.CollisionLayerJson.FromJson<CollisionType[]>()))
                //.ForMember(d => d.MapCharacters, c => c.MapFrom(s => s.MapCharactersJson.FromJson<List<MapCharacter>>()))
                ;

            base.CreateMap<CollisionTileRequest, CollisionTile>();
            base.CreateMap<CollisionTile, ColliisionTileResponse>();
            base.CreateMap<MapCharacterRequest, MapCharacter>();
            base.CreateMap<MapCharacter, MapCharacterResponse>();
        }
    }
}
