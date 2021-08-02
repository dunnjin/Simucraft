using Simucraft.Client.Common;
using Simucraft.Client.Core;
using Simucraft.Client.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Simucraft.Client.Core
{
    public static class CalculationExtensions
    {
        public const string EXPRESSION_REGEX = @"\b[a-zA-Z ]{2,}\b";
        private const string DICE_REGEX = @"(\d+)d(\d+)";

        // TODO: Make the parameters better than this, maybe need a builder.
        public static IEnumerable<Coordinate> GetWeaponTargets(this GameInformation game, RulesetInformation ruleset, GameStateInformation gameState, GameCharacter gameCharacter, Weapon weapon)
        {
            var targets = game.GetCoordinates(gameState, gameCharacter.X, gameCharacter.Y, weapon.Range, ruleset.MovementOffset);
            return targets;
        }

        public static AttackDimensions GetAttackDimensions(this Weapon weapon) =>
            new AttackDimensions
            {
                Height = 1,
                Width = 1,
                Shape = SpellShape.Rectangle,
            };

        public static IEnumerable<Coordinate> GetCoordinates(this AttackDimensions attackDimensions, Coordinate origin)
        {
            if (attackDimensions.Shape == SpellShape.Rectangle)
            {
                yield return new Coordinate
                {
                    X = origin.X,
                    Y = origin.Y,
                };
            }
        }


        public static IEnumerable<Coordinate> GetMovementCoordinates(this RulesetInformation ruleset, GameInformation game, GameStateInformation gameState, GameCharacter gameCharacter)
        {
            if (gameCharacter == null)
                return Enumerable.Empty<Coordinate>();

            // Diagnol tiles must be checked second after non diagnol.
            var neighbourCoordinates = new Coordinate[]
            {
                new Coordinate { X = 0, Y = -1 },
                new Coordinate { X = 1, Y = 0 },
                new Coordinate { X = 0, Y = 1 },
                new Coordinate { X = -1, Y = 0 },

                new Coordinate { X = -1, Y = -1 },
                new Coordinate { X = -1, Y = 1 },
                new Coordinate { X = 1, Y = 1 },
                new Coordinate { X = 1, Y = -1 },
            };

            var visitedTiles = new HashSet<Coordinate>();

            var queue = new Queue<(int X, int Y, int Value)>();
            queue.Enqueue((gameCharacter.X, gameCharacter.Y, 0));

            while (queue.Any())
            {
                var tile = queue.Dequeue();

                //if (visitedTiles.Any(t => t.X == tile.X && t.Y == tile.Y))
                //    continue;

                //visitedTiles.Add(
                //    new Coordinate
                //    {
                //        X = tile.X,
                //        Y = tile.Y,
                //        Value = tile.Value,
                //    });

                neighbourCoordinates
                    .Select(c =>
                        new
                        {
                            X = c.X + tile.X,
                            Y = c.Y + tile.Y,
                            Value = tile.Value + (Math.Abs(c.X) == Math.Abs(c.Y) ? ruleset.MovementOffset : 2),
                        })
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
                    .ToList()
                    .ForEach(c =>
                    {
                        visitedTiles.Add(
                           new Coordinate
                           {
                               X = c.X,
                               Y = c.Y,
                               Value = c.Value,
                           });

                        queue.Enqueue((c.X, c.Y, c.Value));
                    });
            }

            return visitedTiles;
        }

        private static IEnumerable<Coordinate> GetCoordinates(this GameInformation game, GameStateInformation gameState, int x, int y, int distance, int movementOffset)
        {
            // Diagnol tiles must be checked second after non diagnol.
            var neighbourCoordinates = new Coordinate[]
            {
                new Coordinate { X = 0, Y = -1 },
                new Coordinate { X = 1, Y = 0 },
                new Coordinate { X = 0, Y = 1 },
                new Coordinate { X = -1, Y = 0 },

                new Coordinate { X = -1, Y = -1 },
                new Coordinate { X = -1, Y = 1 },
                new Coordinate { X = 1, Y = 1 },
                new Coordinate { X = 1, Y = -1 },
            };

            var visitedTiles = new HashSet<Coordinate>();

            var queue = new Queue<(int X, int Y, int Value)>();
            queue.Enqueue((x, y, 0));

            while (queue.Any())
            {
                var tile = queue.Dequeue();

                if (visitedTiles.Any(t => t.X == tile.X && t.Y == tile.Y))
                    continue;

                visitedTiles.Add(
                    new Coordinate
                    {
                        X = tile.X,
                        Y = tile.Y,
                        Value = tile.Value,
                    });

                neighbourCoordinates
                    .Select(c =>
                        new
                        {
                            X = c.X + tile.X,
                            Y = c.Y + tile.Y,
                            Value = tile.Value + (Math.Abs(c.X) == Math.Abs(c.Y) ? movementOffset : 2),
                        })
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
                            var currentNeighbors = queue.ToList();

                            // TODO: Need to fix. its cutting off edges.
                            //if (!currentNeighbors.Any(n => 
                            //    (c.X == n.X && (c.Y - 1) == n.Y) || 
                            //    ((c.X + 1) == n.X && c.Y == n.Y) || 
                            //    (c.X == n.X && (c.Y + 1) == n.Y) || 
                            //    ((c.X - 1) == n.X && c.Y == n.Y)))
                            //    return false;
                        }

                        if (c.Value > (2 * distance) + 1)
                            return false;

                        return true;
                    })
                    .ToList()
                    .ForEach(c => queue.Enqueue((c.X, c.Y, c.Value)));
            }

            return visitedTiles;
        }

        public static IEnumerable<string> GenerateKeywords(this Ruleset ruleset) =>
            (ruleset.TurnOrderExpression
               ?.RegexMatches(EXPRESSION_REGEX)
                .Select(s => s) ?? Enumerable.Empty<string>())
            .Concat(ruleset.Weapons.SelectMany(w => w.GenerateKeywords()))
            .Concat(ruleset.Equipment.SelectMany(e => e.GenerateKeywords()))
            .Concat(ruleset.Spells.SelectMany(s => s.GenerateKeywords()))
            .Concat(ruleset.Skills.SelectMany(s => s.GenerateKeywords()))
            .DistinctBy(s => s.ToLower())
            .OrderBy(s => s)
            .WithoutReserved();

        public static IEnumerable<string> GenerateKeywords(this Weapon weapon) => string
            .Join(",",
                new string[]
                {
                    weapon.HitChanceSelf,
                    weapon.HitChanceTarget,
                    weapon.CriticalChanceSelf,
                    weapon.CriticalChanceTarget,
                    weapon.Damage,
                    weapon.CriticalDamage,
                })
            .RegexMatches(EXPRESSION_REGEX)
            .Select(s => s);

        public static IEnumerable<string> GenerateKeywords(this Equipment equipment) =>
            string.Join(",",
                new string[]
                {
                    equipment.CriticalDamageExpression,
                    equipment.CriticalHitChanceSelfExpression,
                    equipment.CriticalHitChanceTargetExpression,
                    equipment.DamageExpression,
                    equipment.HitChanceSelfExpression,
                    equipment.HitChanceTargetExpression,
                }
                .Concat(equipment.PassiveExpressions.Select(e => $"{e.SelfExpression} {e.TargetExpression}")))
            .RegexMatches(EXPRESSION_REGEX)
            .Select(s => s);

        public static IEnumerable<string> GenerateKeywords(this Spell skill) => string
            .Join(",",
                new string[]
                {
                    skill.HitChanceSelf,
                    skill.HitChanceTarget,
                    skill.CriticalChanceSelf,
                    skill.CriticalChanceTarget,
                    skill.Damage,
                    skill.CriticalDamage,
                    skill.ResistChanceSelf,
                    skill.ResistChanceTarget,
                })
                //.Concat(skill.StatEffects.Select(e => $"{e.Stat} {e.Effect}")))
            .RegexMatches(EXPRESSION_REGEX)
            .Select(s => s);

        public static IEnumerable<string> GenerateKeywords(this Skill skill) => string
            .Join(",", skill.Expressions.Select(e => $"{e.SelfExpression} {e.TargetExpression}"))
            .RegexMatches(EXPRESSION_REGEX)
            .Select(s => s);

        public static IEnumerable<string> WithoutReserved(this IEnumerable<string> keywords) =>
            keywords.Where(s => !Constants.RESERVED_KEYWORDS.Contains(s));

        public static bool IsNumberExpression(this string expression)
        {
            try
            {
                if (string.IsNullOrEmpty(expression))
                    return true;

                expression
                    .RegexMatches(EXPRESSION_REGEX)
                    .ToList()
                    .ForEach(m => expression = expression.Replace(m, "0"));

                // Replace dice rolls.
                expression
                    .RegexMatches(DICE_REGEX)
                    .ToList()
                    .ForEach(m =>
                    {
                        var splitDice = m.Split('d');
                        var diceCount = int.Parse(splitDice.First());
                        var diceSides = int.Parse(splitDice.Last());
                        var diceResult = 0;

                        var regex = new Regex(m);
                        expression = regex.Replace(expression, $"{diceResult}", 1);
                    });

                Convert.ToDouble(new DataTable().Compute(expression, null));

                return true;
            }
            catch
            {
                return false;
            }
        }

        public static int CalculateExpression(this GameCharacter gameCharacter, string expression)
        {
            var random = new Random();

            if (string.IsNullOrEmpty(expression))
                return 0;

            var equation = expression;
            var equationExpression = expression;

            var fixedStats = new GameCharacterStat[]
            {
                new GameCharacterStat { Name = "hp", Value = gameCharacter.HealthPoints.ToString(), },
                new GameCharacterStat { Name = "health points", Value = gameCharacter.HealthPoints.ToString(), },
                new GameCharacterStat { Name = "healthpoints", Value = gameCharacter.HealthPoints.ToString(), },
                new GameCharacterStat { Name = "level", Value = gameCharacter.Level.ToString(), },
                new GameCharacterStat { Name = "movement", Value = gameCharacter.Movement.ToString(), },
                new GameCharacterStat { Name = "carryingcapacity", Value = ((int)Math.Floor(gameCharacter.CarryingCapacity)).ToString(), },
                new GameCharacterStat { Name = "carrying capacity", Value = ((int)Math.Floor(gameCharacter.CarryingCapacity)).ToString(), },
            };

            // Limit the number of replaced expressions to prevent infinite loops.
            foreach (var index in Enumerable.Range(1, 5))
            {
                var matches = equation.RegexMatches(EXPRESSION_REGEX);
                if (!matches.Any())
                    break;

                // Find the characters expressions and inject them into equation.
                foreach (var match in matches)
                {
                    var replacement = gameCharacter.Stats.FirstOrDefault(s => s.Name.ToLower() == match.ToLower())?.Value ??
                                      fixedStats.FirstOrDefault(s => s.Name.ToLower() == match.ToLower())?.Value ?? "0";
                    equation = equation.Replace(match, $"({replacement})");
                }
            }

            // Find dice rolls and inject random value.
            foreach (var match in equation.RegexMatches(DICE_REGEX))
            {
                var splitDice = match.Split('d');
                var diceCount = int.Parse(splitDice.First());
                var diceSides = int.Parse(splitDice.Last());
                var diceResult = Enumerable
                    .Range(1, diceCount)
                    .Select(c => random.Next(1, diceSides + 1));

                var regex = new Regex(match);
                equation = regex.Replace(equation, $"({diceResult.Sum()})", 1);
                equationExpression = regex.Replace(equationExpression, $"({match} [{string.Join(", ", diceResult)}])");
            }

            var result = (int)Math.Floor(Convert.ToDouble(new DataTable().Compute(equation, null)));
            return result;
        }

    }
}
