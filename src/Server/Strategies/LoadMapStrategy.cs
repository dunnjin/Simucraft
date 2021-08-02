using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using Simucraft.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Simucraft.Server.Strategies
{
    public class LoadMapStrategy : GameStrategy<RequestLoadMap>
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public LoadMapStrategy(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        // TODO: Add userId and validate
        public override async Task<GameInformationResponse> ExecuteAsync(Guid gameId, RequestLoadMap request)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            var ruleset = await _simucraftContext.Rulesets.SingleAsync(r => r.Id == game.RulesetId);
            var map = await _simucraftContext.Maps.SingleAsync(m => m.Id == request.MapId);

            var characters = await _simucraftContext.Characters
                .Where(c => c.RulesetId == game.RulesetId)
                .ToListAsync();
            var weapons = await _simucraftContext.Weapons
                .Where(w => w.RulesetId == game.RulesetId)
                .ToListAsync();


            // Clear out any game characters that were not favorited.
            game.GameCharacters = game.GameCharacters
                .Where(c => c.IsFavorite)
                .ToList();

            // Reset visibility of existing game characters.
            foreach (var gameCharacter in game.GameCharacters)
                gameCharacter.IsVisible = false;

            // Since these characters are coming from a map they need to start visible.
            foreach (var mapCharacter in map.MapCharacters)
            {
                var character = characters.First(c => c.Id == mapCharacter.CharacterId);
                var gameCharacter = _mapper.Map<GameCharacter>(character);
                gameCharacter.Id = Guid.NewGuid();
                gameCharacter.TurnOrder = ruleset.GetTurnOrder(gameCharacter);
                gameCharacter.X = mapCharacter.X;
                gameCharacter.Y = mapCharacter.Y;
                gameCharacter.IsVisible = true;
                gameCharacter.HealthPoints = gameCharacter.CalculateExpression(gameCharacter.MaxHealthPoints).Result;
                gameCharacter.Weapons = _mapper.Map<ICollection<GameCharacterWeapon>>(weapons.Where(w => character.WeaponIds.Contains(w.Id)));

                game.GameCharacters.Add(gameCharacter);
            }

            // Reset state mode.
            game.GameStateMode = GameStateMode.None;
            game.Width = map.Width;
            game.Height = map.Height;
            game.TileWidth = map.TileWidth;
            game.TileHeight = map.TileHeight;
            game.CollisionTiles = _mapper.Map<IList<GameCollisionTile>>(map.CollisionTiles);
            game.MapId = map.Id;
            game.ImageUrl = map.ImageUrl;

            await _simucraftContext.SaveChangesAsync();

            var response = _mapper.Map<GameInformationResponse>(game);
            return response;
        }
    }
}
