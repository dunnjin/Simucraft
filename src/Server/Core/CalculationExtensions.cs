using Simucraft.Server.Common;
using Simucraft.Server.Models;
using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Simucraft.Server.Core
{
    public static class CalculationExtensions
    {
        private const string EXPRESSION_REGEX = @"\b[a-zA-Z ]{2,}\b";
        private const string DICE_REGEX = @"(\d+)d(\d+)";

        public static EquationResult CalculateExpression(this GameCharacter gameCharacter, string expression) =>
            CalculateExpression(gameCharacter, expression, 0, 0);

        public static EquationResult CalculateExpression(this GameCharacter gameCharacter, string expression, int damage, int hitchance)
        {
            var random = new Random();

            if (string.IsNullOrEmpty(expression))
                return new EquationResult();

            var equation = expression;

            var fixedStats = new GameCharacterStat[]
            {
                new GameCharacterStat { Name = "hp", Value = gameCharacter.HealthPoints.ToString(), },
                new GameCharacterStat { Name = "health points", Value = gameCharacter.HealthPoints.ToString(), },
                new GameCharacterStat { Name = "healthpoints", Value = gameCharacter.HealthPoints.ToString(), },
                new GameCharacterStat { Name = "level", Value = gameCharacter.Level.ToString(), },
                new GameCharacterStat { Name = "movement", Value = gameCharacter.Movement.ToString(), },
                new GameCharacterStat { Name = "carryingcapacity", Value = ((int)Math.Floor(gameCharacter.CarryingCapacity)).ToString(), },
                new GameCharacterStat { Name = "carrying capacity", Value = ((int)Math.Floor(gameCharacter.CarryingCapacity)).ToString(), },
                new GameCharacterStat { Name = "hitchance", Value = hitchance.ToString(), },
                new GameCharacterStat { Name = "hit chance", Value = hitchance.ToString(), },
                new GameCharacterStat { Name = "damage", Value = damage.ToString(), },
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

            var equationExpression = equation;
            // Find dice rolls and inject random value.
            foreach (var match in equation.RegexMatches(DICE_REGEX))
            {
                var splitDice = match.Split('d');
                var diceCount = int.Parse(splitDice.First());
                var diceSides = int.Parse(splitDice.Last());
                var diceResult = Enumerable
                    .Range(1, diceCount)
                    .Select(c => random.Next(1, diceSides + 1))
                    .ToList();

                var regex = new Regex(match);
                equation = regex.Replace(equation, $"({diceResult.Sum()})", 1);
                equationExpression = regex.Replace(equationExpression, $"({match} [{string.Join(", ", diceResult)}])");
            }

            var result = (int)Math.Floor(Convert.ToDouble(new DataTable().Compute(equation, null)));

            return new EquationResult
            {
                Result = result,
                VerboseResult = equationExpression,
            };
        }

        public static int GetTurnOrder(this Ruleset ruleset, GameCharacter gameCharacter)
        {
            var turnOrderFormula = ruleset.TurnOrderExpression?.ToLower();
            if (string.IsNullOrEmpty(turnOrderFormula))
                return 0;

            return gameCharacter.CalculateExpression(turnOrderFormula).Result;
        }

        public static Guid? GetNextTurnId(this Game game)
        {
            var gameCharacters = game.GameCharacters
                .OrderByDescending(c => c.TurnOrder)
                .ThenBy(c => c.Id)
                .ToList();

            if (!gameCharacters.Any(c => c.IsVisible))
                return null;

            if (!game.CurrentTurnId.HasValue)
                return gameCharacters.First().Id;

            var currentIndex = gameCharacters.IndexOf(gameCharacters.Single(c => c.Id == game.CurrentTurnId.Value));

            return GetNextVisibleCharacter(currentIndex);

            Guid GetNextVisibleCharacter(int index)
            {
                var nextIndex = (index + 1).Wrap(0, gameCharacters.Count - 1);
                var nextCharacter = gameCharacters[nextIndex];

                if (!nextCharacter.IsVisible)
                    return GetNextVisibleCharacter(nextIndex);

                return nextCharacter.Id;
            }
        }

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
    }
}
