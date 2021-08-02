using Simucraft.Server.Models;
using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class RulesetTemplates
    {
        public IRulesetTemplate GetTemplate(string id)
        {
            switch(id)
            {
                case "42a782cf-bbcd-48ca-9207-084cb627a9b8" :
                    return new Dnd5thEditionTemplate();
                default:
                    return default;
            }
        }
    }

    public class Dnd5thEditionTemplate : IRulesetTemplate
    {
        public Ruleset Ruleset(Guid userId) =>
            new Ruleset
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Description = "Starter kit for the D&D 5th Edition's core rulebook.",
                MovementOffset = 3,
                Name = "D&D 5th edition",
                TurnOrderExpression = "1d20 + Dexterity Modifier",
            };

        public IEnumerable<Character> Characters(Guid userId, Guid rulesetId) => new List<Character>
        {
            new Character
            {
                Id = Guid.NewGuid(),
                CarryingCapacity = "10 * Strength",
                Category = "Templates",
                Description = "Copy this to quickly create D&D characters.",
                HealthPoints = "10",
                Height = 1,
                Width = 1,
                Level = 1,
                Movement = 6,
                Name = "Level 1 Character",
                RulesetId = rulesetId,
                UserId = userId,
                Stats = new List<CharacterStat>
                {
                    new CharacterStat { Name = "AC", Value = "10 + Dexterity Modifier" },

                    new CharacterStat { Name = "Strength", Value = "10" },
                    new CharacterStat { Name = "Strength Modifier", Value = "(Strength - 10) / 2" },
                    new CharacterStat { Name = "Dexterity", Value = "10" },
                    new CharacterStat { Name = "Dexterity Modifier", Value = "(Dexterity - 10) / 2" },
                    new CharacterStat { Name = "Constitution", Value = "10" },
                    new CharacterStat { Name = "Constitution Modifier", Value = "(Constitution - 10) / 2" },
                    new CharacterStat { Name = "Intelligence", Value = "10" },
                    new CharacterStat { Name = "Intelligence Modifier", Value = "(Intelligence - 10) / 2" },
                    new CharacterStat { Name = "Wisdom", Value = "10" },
                    new CharacterStat { Name = "Wisdom Modifier", Value = "(Wisdom - 10) / 2" },
                    new CharacterStat { Name = "Charisma", Value = "10" },
                    new CharacterStat { Name = "Charisma Modifier", Value = "(Charisma - 10) / 2" },

                    new CharacterStat { Name = "Simple Weapon Proficiency", Value = "Proficiency" },
                    new CharacterStat { Name = "Martial Weapon Proficiency", Value = "Proficiency" },
                    new CharacterStat { Name = "Proficiency", Value = "2 + (Level / 5)" },
                },
                Skills = new List<CharacterSkill>
                {
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Strength", Expression = "1d20 + Strength Modifier", Category = "Ability Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Dexterity", Expression = "1d20 + Dexterity Modifier", Category = "Ability Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Constitution", Expression = "1d20 + Constitution Modifier", Category = "Ability Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Intelligence", Expression = "1d20 + Intelligence Modifier", Category = "Ability Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Wisdom", Expression = "1d20 + Wisdom Modifier", Category = "Ability Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Charisma", Expression = "1d20 + Charisma Modifier", Category = "Ability Checks" },

                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Strength", Expression = "1d20 + Strength Modifier", Category = "Saving Throws" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Dexterity", Expression = "1d20 + Dexterity Modifier", Category = "Saving Throws" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Constitution", Expression = "1d20 + Constitution Modifier", Category = "Saving Throws" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Intelligence", Expression = "1d20 + Intelligence Modifier", Category = "Saving Throws" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Wisdom", Expression = "1d20 + Wisdom Modifier", Category = "Saving Throws" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Charisma", Expression = "1d20 + Charisma Modifier", Category = "Saving Throws" },

                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Acrobatics", Expression = "1d20 + Dexterity Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Animal Handling", Expression = "1d20 + Wisdom Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Arcana", Expression = "1d20 + Intelligence Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Athletics", Expression = "1d20 + Strength Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Deception", Expression = "1d20 + Charisma Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "History", Expression = "1d20 + Intelligence Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Insight", Expression = "1d20 + Wisdom Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Intimidation", Expression = "1d20 + Charisma Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Investigation", Expression = "1d20 + Intelligence Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Medicine", Expression = "1d20 + Wisdom Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Nature", Expression = "1d20 + Intelligence Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Perception", Expression = "1d20 + Wisdom Modifier" , Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Performance", Expression = "1d20 + Charisma Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Persuasion", Expression = "1d20 + Charisma Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Religion", Expression = "1d20 + Intelligence Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Sleight of Hand", Expression = "1d20 + Dexterity Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Stealth", Expression = "1d20 + Dexterity Modifier", Category = "Skill Checks" },
                    new CharacterSkill { Id = Guid.NewGuid(), Name = "Survival", Expression = "1d20 + Wisdom Modifier", Category = "Skill Checks" },
                },
            },
        };

        public IEnumerable<Weapon> Weapons(Guid userId, Guid rulesetId) => new List<Weapon>
        {
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Club",
                Description = "Light",
                Category = "Simple Melee Weapons",

                DamageTypes = "bludgeoning",
                Cost = "1 silver",
                Weight = 2,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d4 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d4",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Dagger",
                Description = "Finesse, light, thrown (range 20/60)",
                Category = "Simple Melee Weapons",

                DamageTypes = "piercing",
                Cost = "2 gold",
                Weight = 1,
                Range = 4,

                HitChanceSelf = "1d20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d4 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d4",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Greatclub",
                Description = "Two-handed",
                Category = "Simple Melee Weapons",

                DamageTypes = "bludgeoning",
                Cost = "2 silver",
                Weight = 10,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Handaxe",
                Description = "Light, thrown (range 20/60)",
                Category = "Simple Melee Weapons",

                DamageTypes = "slashing",
                Cost = "5 gold",
                Weight = 2,
                Range = 4,

                HitChanceSelf = "1d20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d6 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Javelin",
                Description = "Thrown (range 30/120)",
                Category = "Simple Melee Weapons",

                DamageTypes = "piercing",
                Cost = "5 silver",
                Weight = 2,
                Range = 6,

                HitChanceSelf = "1d20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d6 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Light Hammer",
                Description = "Light, thrown (range 20/60)",
                Category = "Simple Melee Weapons",

                DamageTypes = "bludgeoning",
                Cost = "2 gold",
                Weight = 2,
                Range = 4,

                HitChanceSelf = "1d20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d4 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Mace",
                Description = null,
                Category = "Simple Melee Weapons",

                DamageTypes = "bludgeoning",
                Cost = "5 gold",
                Weight = 4,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d6 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Quarterstaff (One-Handed)",
                Description = "Versatile (1d8)",
                Category = "Simple Melee Weapons",

                DamageTypes = "bludgeoning",
                Cost = "2 silver",
                Weight = 4,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d6 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Quarterstaff (Two-Handed)",
                Description = "Versatile (1d8)",
                Category = "Simple Melee Weapons",

                DamageTypes = "bludgeoning",
                Cost = "2 silver",
                Weight = 4,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Sickle",
                Description =  "Light",
                Category = "Simple Melee Weapons",

                DamageTypes = "slashing",
                Cost = "1 gold",
                Weight = 2,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d4 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d4",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Spear (One-Handed)",
                Description =  "Thrown (range 20/60), versatile (1d8)",
                Category = "Simple Melee Weapons",

                DamageTypes = "piercing",
                Cost = "1 gold",
                Weight = 3,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d6 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Spear (Two-Handed)",
                Description =  "Thrown (range 20/60), versatile (1d8)",
                Category = "Simple Melee Weapons",

                DamageTypes = "piercing",
                Cost = "1 gold",
                Weight = 3,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Light Crossbow",
                Description =  "Ammunition (range 80/320), loading, two-handed",
                Category = "Simple Ranged Weapons",

                DamageTypes = "piercing",
                Cost = "25 gold",
                Weight = 5,
                Range = 16,

                HitChanceSelf = "1d20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Dart",
                Description =  "Finesse, thrown (range 20/60)",
                Category = "Simple Ranged Weapons",

                DamageTypes = "piercing",
                Cost = "5 copper",
                Weight = 0.25m,
                Range = 4,

                HitChanceSelf = "1d20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d4 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d4",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Shortbow",
                Description =  "Ammunition (range 80/320), two-handed",
                Category = "Simple Ranged Weapons",

                DamageTypes = "piercing",
                Cost = "25 gold",
                Weight = 2,
                Range = 16,

                HitChanceSelf = "1d20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d6 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Sling",
                Description =  "Ammunition (range 30/120)",
                Category = "Simple Ranged Weapons",

                DamageTypes = "bludgeoning",
                Cost = "1 silver",
                Weight = 0,
                Range = 5,

                HitChanceSelf = "1d20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d4 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d4",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Battleaxe (One-Handed)",
                Description =  "Versatile (1d10)",
                Category = "Martial Melee Weapons",

                DamageTypes = "slashing",
                Cost = "10 gold",
                Weight = 4,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Battleaxe (Two-Handed)",
                Description =  "Versatile (1d10)",
                Category = "Martial Melee Weapons",

                DamageTypes = "slashing",
                Cost = "10 gold",
                Weight = 4,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d10 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d10",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Flail",
                Description = null,
                Category = "Martial Melee Weapons",

                DamageTypes = "bludgeoning",
                Cost = "20 gold",
                Weight = 6,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Glaive",
                Description = "Heavy, reach, two-handed",
                Category = "Martial Melee Weapons",

                DamageTypes = "slashing",
                Cost = "20 gold",
                Weight = 6,
                Range = 2,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d10 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d10",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Greataxe",
                Description = "Heavy, two-handed",
                Category = "Martial Melee Weapons",

                DamageTypes = "slashing",
                Cost = "30 gold",
                Weight = 7,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d12 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d12",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Greatsword",
                Description = "Heavy, two-handed",
                Category = "Martial Melee Weapons",

                DamageTypes = "slashing",
                Cost = "50 gold",
                Weight = 6,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "2d6 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "2d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Halberd",
                Description = "Heavy, reach, two-handed",
                Category = "Martial Melee Weapons",

                DamageTypes = "slashing",
                Cost = "20 gold",
                Weight = 6,
                Range = 2,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d10 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d10",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Lance",
                Description = "Reach, special",
                Category = "Martial Melee Weapons",

                DamageTypes = "slashing",
                Cost = "10 gold",
                Weight = 6,
                Range = 2,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d12 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d12",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Longsword (One-Handed)",
                Description = "Versatile (1d10)",
                Category = "Martial Melee Weapons",

                DamageTypes = "slashing",
                Cost = "15 gold",
                Weight = 3,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Longsword (Two-Handed)",
                Description = "Versatile (1d10)",
                Category = "Martial Melee Weapons",

                DamageTypes = "slashing",
                Cost = "15 gold",
                Weight = 3,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d10 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d10",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Maul",
                Description = "Heavy, two-handed",
                Category = "Martial Melee Weapons",

                DamageTypes = "bludgeoning",
                Cost = "50 gold",
                Weight = 10,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "2d6 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "2d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Morningstar",
                Description = null,
                Category = "Martial Melee Weapons",

                DamageTypes = "piercing",
                Cost = "15 gold",
                Weight = 10,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Pike",
                Description = "Heavy, reach, two-handed",
                Category = "Martial Melee Weapons",

                DamageTypes = "piercing",
                Cost = "5 gold",
                Weight = 18,
                Range = 2,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d10 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d10",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Rapier",
                Description = "Finesse",
                Category = "Martial Melee Weapons",

                DamageTypes = "piercing",
                Cost = "25 gold",
                Weight = 2,
                Range = 1,

                HitChanceSelf = "1d20 + Dexterity Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Scimitar",
                Description = "Finesse, light",
                Category = "Martial Melee Weapons",

                DamageTypes = "slashing",
                Cost = "25 gold",
                Weight = 2,
                Range = 1,

                HitChanceSelf = "1d20 + Dexterity Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d6 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Shortsword",
                Description = "Finesse, light",
                Category = "Martial Melee Weapons",

                DamageTypes = "piercing",
                Cost = "10 gold",
                Weight = 2,
                Range = 1,

                HitChanceSelf = "1d20 + Dexterity Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d6 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Trident (One-Handed)",
                Description = "Thrown (range 20/60), versatile (1d8)",
                Category = "Martial Melee Weapons",

                DamageTypes = "piercing",
                Cost = "5 gold",
                Weight = 4,
                Range = 4,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d6 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Trident (Two-Handed)",
                Description = "Thrown (range 20/60), versatile (1d8)",
                Category = "Martial Melee Weapons",

                DamageTypes = "piercing",
                Cost = "5 gold",
                Weight = 4,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "War Pick",
                Description = null,
                Category = "Martial Melee Weapons",

                DamageTypes = "piercing",
                Cost = "5 gold",
                Weight = 2,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Warhammer (One-Handed)",
                Description = "Versatile (1d10)",
                Category = "Martial Melee Weapons",

                DamageTypes = "bludgeoning",
                Cost = "15 gold",
                Weight = 2,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Warhammer (Two-Handed)",
                Description = "Versatile (1d10)",
                Category = "Martial Melee Weapons",

                DamageTypes = "bludgeoning",
                Cost = "15 gold",
                Weight = 2,
                Range = 1,

                HitChanceSelf = "1d20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Strength Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d10 + Strength Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d10",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Whip",
                Description = "Finesse, reach",
                Category = "Martial Melee Weapons",

                DamageTypes = "slashing",
                Cost = "2 gold",
                Weight = 2,
                Range = 2,

                HitChanceSelf = "1d20 + Dexterity Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1d4 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d4",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Blowgun",
                Description = "Ammunition (range 25/100), loading",
                Category = "Martial Ranged Weapons",

                DamageTypes = "piercing",
                Cost = "10 gold",
                Weight = 1,
                Range = 5,

                HitChanceSelf = "1d20 + Dexterity Modifier + Martial Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Martial Weapon Proficiency + Bonus To Attack",

                Damage = "1 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Hand Crossbow",
                Description =  "Ammunition (range 30/120), light, loading",
                Category = "Martial Ranged Weapons",

                DamageTypes = "piercing",
                Cost = "75 gold",
                Weight = 3,
                Range = 6,

                HitChanceSelf = "1d20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d6 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d6",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Heavy Crossbow",
                Description =  "Ammunition (range 100/400), heavy, loading, two-handed",
                Category = "Martial Ranged Weapons",

                DamageTypes = "piercing",
                Cost = "50 gold",
                Weight = 18,
                Range = 20,

                HitChanceSelf = "1d20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d10 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d10",
            },
            new Weapon
            {
                Id = Guid.NewGuid(),
                RulesetId = rulesetId,
                UserId = userId,

                Name = "Longbow",
                Description =  "Ammunition (range 150/600), heavy, two-handed",
                Category = "Martial Ranged Weapons",

                DamageTypes = "piercing",
                Cost = "50 gold",
                Weight = 2,
                Range = 30,

                HitChanceSelf = "1d20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",
                HitChanceOperator = ">",
                HitChanceTarget = "AC - 1",

                CriticalChanceSelf = "hitchance",
                CriticalChanceOperator = "=",
                CriticalChanceTarget = "20 + Dexterity Modifier + Simple Weapon Proficiency + Bonus To Attack",

                Damage = "1d8 + Dexterity Modifier + Bonus To Damage",
                DamageOperator = "+",
                CriticalDamage = "1d8",
            },
        };
    }
}
