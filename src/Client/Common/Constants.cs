using System.Collections.Generic;

namespace Simucraft.Client.Common
{
    internal class Constants
    {
        // TODO: Replace with localization.
        public const string SAVING_ERROR = "An error occured while saving, please try again later.";
        public const string INITIALIZE_ERROR = "An error occured, please try again later.";
        public const string IMAGE_ERROR = "An error occured, make sure it's a valid image.";
        public const string DELETE_ERROR = "Unable to delete at this time, please try again later.";
        public const string COPY_ERROR = "Unable to copy at this time, please try again later.";

        public static readonly IEnumerable<string> DISPLAY_RESERVED_KEYWORDS = new List<string>
        {
            "hp",
            "level",
            "movement",
            "carrying capacity",
            "damage",
            "hit chance",
        };
        public static readonly IEnumerable<string> RESERVED_KEYWORDS = new List<string>
        {
            "hp",
            "healthpoints",
            "health points",
            "level",
            "movement",
            "carrying capacity",
            "carryingcapacity",
            "damage",
            "hitchance",
            "hit chance",
        };
    }
}
