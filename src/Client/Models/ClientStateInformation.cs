using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Simucraft.Client.Models
{
    public class ClientStateInformation
    {
        public ClientState Mode { get; set; }

        public GameCharacter PlacementGameCharacter { get; set; }

        public Guid? SelectedGameCharacterId { get; set; }

        public bool ShowContextMenu { get; set; }

        public IEnumerable<Coordinate> Coordinates { get; set; } = new List<Coordinate>();

        public AttackDimensions AttackDimensions { get; set; }

        public Coordinate Origin { get; set; }

        public IEnumerable<Coordinate> AttackCoordinates { get; set; } = new List<Coordinate>();

        public Guid? RulesetEntityId { get; set; }

        public RulesetEntityType RulesetEntityType { get; set; }

        public bool HideInvisibleGameCharacters { get; set; }

        public Guid UserId { get; set; }

        public bool IsOwner { get; set; }

        public void Clear()
        {
            this.PlacementGameCharacter = null;
            this.ShowContextMenu = false;
            this.AttackDimensions = null;
            this.Coordinates = new List<Coordinate>();
            this.AttackCoordinates = new List<Coordinate>();
            this.Mode = ClientState.None;
            this.Origin = null;
            this.RulesetEntityId = null;
        }

        public static ClientStateInformation Empty =>
            new ClientStateInformation
            {

            };
    }
}
