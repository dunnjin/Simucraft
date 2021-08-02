using Simucraft.Server.Core;
using Simucraft.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Simucraft.Tests
{
    public class GameCharacterTests
    {
        private GameCharacter _target;

        public GameCharacterTests()
        {
            _target = new GameCharacter
            {
                Id = Guid.NewGuid(),
                MaxCarryingCapacity = "10 * Strength",
                Description = "Copy this to quickly create D&D characters.",
                MaxHealthPoints = "10",
                Height = 1,
                Width = 1,
                Level = 1,
                Movement = 6,
                Name = "Level 1 Character",
                Stats = new List<GameCharacterStat>
                {
                    new GameCharacterStat { Name = "AC", Value = "10 + DexterityModifier" },
                    new GameCharacterStat { Name = "Initiative", Value = "1d20 + DexterityModifier" },
                    new GameCharacterStat { Name = "HitDice", Value = "1d10" },

                    new GameCharacterStat { Name = "Strength", Value = "10" },
                    new GameCharacterStat { Name = "StrengthModifier", Value = "(Strength - 10) / 2" },
                    new GameCharacterStat { Name = "Dexterity", Value = "10" },
                    new GameCharacterStat { Name = "DexterityModifier", Value = "(Dexterity - 10) / 2" },
                    new GameCharacterStat { Name = "Constitution", Value = "10" },
                    new GameCharacterStat { Name = "ConstitutionModifier", Value = "(Constitution - 10) / 2" },
                    new GameCharacterStat { Name = "Intelligence", Value = "10" },
                    new GameCharacterStat { Name = "IntelligenceModifier", Value = "(Intelligence - 10) / 2" },
                    new GameCharacterStat { Name = "Wisdom", Value = "10" },
                    new GameCharacterStat { Name = "WisdomModifier", Value = "(Wisdom - 10) / 2" },
                    new GameCharacterStat { Name = "Charisma", Value = "10" },
                    new GameCharacterStat { Name = "CharismaModifier", Value = "(Charisma - 10) / 2" },

                    new GameCharacterStat { Name = "SimpleWeaponProficiency", Value = "Proficiency" },
                    new GameCharacterStat { Name = "MartialWeaponProficiency", Value = "Proficiency" },
                    new GameCharacterStat { Name = "Proficiency", Value = "(2 + (Level / 5)" },
                },
                Skills = new List<GameCharacterSkill>
                {
                    new GameCharacterSkill { Name = "Strength Saving Throw", Expression = "1d20 + StrengthModifier" },
                    new GameCharacterSkill { Name = "Dexterity Saving Throw", Expression = "1d20 + DexterityModifier" },
                    new GameCharacterSkill { Name = "Constitution Saving Throw", Expression = "1d20 + ConstitutionModifier" },
                    new GameCharacterSkill { Name = "Intelligence Saving Throw", Expression = "1d20 + IntelligenceModifier" },
                    new GameCharacterSkill { Name = "Wisdom Saving Throw", Expression = "1d20 + WisdomModifier" },
                    new GameCharacterSkill { Name = "Charisma Saving Throw", Expression = "1d20 + CharismaModifier" },

                    new GameCharacterSkill { Name = "Acrobatics", Expression = "1d20 + DexterityModifier" },
                    new GameCharacterSkill { Name = "Animal Handling", Expression = "1d20 + WisdomModifier" },
                    new GameCharacterSkill { Name = "Arcana", Expression = "1d20 + IntelligenceModifier" },
                    new GameCharacterSkill { Name = "Athletics", Expression = "1d20 + StrengthModifier" },
                    new GameCharacterSkill { Name = "Deception", Expression = "1d20 + CharismaModifier" },
                    new GameCharacterSkill { Name = "History", Expression = "1d20 + IntelligenceModifier" },
                    new GameCharacterSkill { Name = "Insight", Expression = "1d20 + WisdomModifier" },
                    new GameCharacterSkill { Name = "Intimidation", Expression = "1d20 + CharismaModifier" },
                    new GameCharacterSkill { Name = "Investigation", Expression = "1d20 + IntelligenceModifier" },
                    new GameCharacterSkill { Name = "Medicine", Expression = "1d20 + WisdomModifier" },
                    new GameCharacterSkill { Name = "Nature", Expression = "1d20 + IntelligenceModifier" },
                    new GameCharacterSkill { Name = "Perception", Expression = "1d20 + WisdomModifier" },
                    new GameCharacterSkill { Name = "Performance", Expression = "1d20 + CharismaModifier" },
                    new GameCharacterSkill { Name = "Persuasion", Expression = "1d20 + CharismaModifier" },
                    new GameCharacterSkill { Name = "Religion", Expression = "1d20 + IntelligenceModifier" },
                    new GameCharacterSkill { Name = "Sleight of Hand", Expression = "1d20 + DexterityModifier" },
                    new GameCharacterSkill { Name = "Stealth", Expression = "1d20 + DexterityModifier" },
                    new GameCharacterSkill { Name = "Survival", Expression = "1d20 + WisdomModifier" },
                },
            };
        }
    }
}
