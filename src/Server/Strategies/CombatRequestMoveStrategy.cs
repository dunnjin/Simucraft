using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Simucraft.Server.Core;
using Simucraft.Server.DataAccess;
using Simucraft.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Simucraft.Server.Common;

namespace Simucraft.Server.Strategies
{
    public class CombatRequestMoveStrategy : GameStateStrategy<RequestCombatMove>
    {
        private readonly SimucraftContext _simucraftContext;
        private readonly IMapper _mapper;

        public CombatRequestMoveStrategy(
            SimucraftContext simucraftContext,
            IMapper mapper)
        {
            _simucraftContext = simucraftContext;
            _mapper = mapper;
        }

        public override async Task<GameStateInformationResponse> RequestAsync(Guid gameId, Guid userId, RequestCombatMove request)
        {
            var game = await _simucraftContext.Games.SingleAsync(g => g.Id == gameId);
            var ruleset = await _simucraftContext.Rulesets.SingleAsync(r => r.Id == game.RulesetId);
            var isOwner = game.UserId == userId;

            var gameCharacter = game.GameCharacters.Single(c => c.Id == request.GameCharacterId);
            if (gameCharacter.UserId != userId && !isOwner)
                throw new InvalidOperationException("Unauthorized.");

            var neighbourCoordinates = new Coordinate[]
            {
                new Coordinate(0, -1),
                new Coordinate(1, 0),
                new Coordinate(0, 1),
                new Coordinate(-1, 0),

                new Coordinate(-1, -1),
                new Coordinate(-1, 1),
                new Coordinate(1, 1),
                new Coordinate(1, -1),
           };

            var visitedTiles = new List<ValueCoordinate>();
            var queue = new Queue<ValueCoordinate>();
            queue.Enqueue(new ValueCoordinate(gameCharacter.X, gameCharacter.Y, 0));

            while(queue.Any())
            {
                var tile = queue.Dequeue();
                var tiles = neighbourCoordinates
                    .Select(c => new ValueCoordinate(c.X + tile.X, c.Y + tile.Y, tile.Value + (Math.Abs(c.X) == Math.Abs(c.Y) ? ruleset.MovementOffset : 2)))
                    .Where(c =>
                    {
                        // Already visited check.
                        if (visitedTiles.Any(t => t.X == c.X && t.Y == c.Y))
                            return false;

                        // Bounds check.
                        if (c.X < 0 || c.X >= game.Width ||
                            c.Y < 0 || c.Y >= game.Height)
                            return false;

                        //// Existing character check.
                        //if (gameState.GameCharacters.Any(gc => gc.X == c.X && gc.Y == c.Y && gc.IsVisible))
                        //    return false;

                        // Collision check.
                        var gameTile = game.CollisionTiles.FirstOrDefault(t => t.X == c.X && t.Y == c.Y);

                        if (gameTile != null)
                        {
                            var isColliding = gameTile.CollisionType != CollisionType.None &&
                               (gameTile.CollisionType.HasFlag(CollisionType.Top | CollisionType.Right | CollisionType.Left | CollisionType.Bottom) ||
                               (Math.Sign(c.X) < 0 && gameTile.CollisionType.HasFlag(CollisionType.Right)) ||
                               (Math.Sign(c.X) > 0 && gameTile.CollisionType.HasFlag(CollisionType.Left)) ||
                               (Math.Sign(c.Y) < 0 && gameTile.CollisionType.HasFlag(CollisionType.Bottom)) ||
                               (Math.Sign(c.Y) > 0 && gameTile.CollisionType.HasFlag(CollisionType.Top)));

                            if (isColliding)
                                return false;
                        }

                        // Prevent diagonals from movement between collision tiles.
                        var isDiagonal = Math.Abs(c.X - tile.X) == Math.Abs(c.Y - tile.Y);
                        if (isDiagonal)
                        {
                            //var currentNeighbors = queue.ToList();

                            // TODO: Need to fix. its cutting off edges.
                            //if (!currentNeighbors.Any(n => 
                            //    (c.X == n.X && (c.Y - 1) == n.Y) || 
                            //    ((c.X + 1) == n.X && c.Y == n.Y) || 
                            //    (c.X == n.X && (c.Y + 1) == n.Y) || 
                            //    ((c.X - 1) == n.X && c.Y == n.Y)))
                            //    return false;
                        }

                        if (c.Value > (2 * gameCharacter.Movement) + 1)
                            return false;

                        return true;
                    })
                    .ToList();

                visitedTiles.AddRange(tiles);
                queue.EnqueueRange(tiles);
            }

            var response = _mapper.Map<GameStateInformationResponse>(game);
            response.Coordinates = visitedTiles
                .Select(c => new Coordinate(c.X, c.Y))
                .ToList();

            return response;
        }
    }
}
