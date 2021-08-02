using System;
using System.Collections.Generic;

namespace Simucraft.Server.Core
{
    public class RulesetInformationResponse
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public bool IsOwner { get; set; }

        public int MovementOffset { get; set; }

        public string TurnOrderExpression { get; set; }

        public ICollection<WeaponResponse> Weapons { get; set; } = new List<WeaponResponse>();

        public ICollection<EquipmentResponse> Equipment { get; set; } = new List<EquipmentResponse>();

        public ICollection<SpellResponse> Spells { get; set; } = new List<SpellResponse>();

        public ICollection<SkillResponse> Skills { get; set; } = new List<SkillResponse>();

        public ICollection<CharacterResponse> Characters { get; set; } = new List<CharacterResponse>();

        public ICollection<MapResponse> Maps { get; set; } = new List<MapResponse>();
    }
}
