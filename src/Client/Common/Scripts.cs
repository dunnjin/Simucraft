using Microsoft.JSInterop;

namespace Simucraft.Client.Common
{
    internal static class Scripts
    {
        public class Map
        {
            public const string INITIALIZE = "map.initialize";
            public const string ADD_IMAGE = "map.addImage";
            public const string TOGGLE_GRID = "map.toggleGrid";
            public const string SET_ZOOM = "map.setZoom";
            public const string SET_COLLISION_TYPE = "map.setCollisionType";
            public const string GET_MAP = "map.getMap";
            public const string ADD_CHARACTER = "map.addCharacter";
            public const string SET_MAP_MENU_OPTION = "map.setMapMenuOption";
            public const string SELECT_CHARACTER = "map.setSelectedCharacter";
        }

        public class Game
        {
            public const string INITIALIZE = "game.initialize";
            public const string SET_ZOOM = "game.setZoom";
            public const string LOAD_GAME = "game.loadGame";
            public const string TOGGLE_GRID = "game.toggleGrid";
            public const string ADD_CHARACTER = "game.addCharacter";
            public const string GET_COORDINATES = "game.getCoordinates";
            public const string RESIZE_CANVAS = "game.resizeCanvas";
            public const string RENDER_GAME = "game.renderGame";
            public const string RENDER_GAMESTATE = "game.renderGameState";
            public const string RENDER_CLIENTSTATE = "game.renderClientState";
            public const string GET_DELETED_CHARACTER_ID = "game.getDeletedCharacterId";
            public const string GET_DRAGGED_CHARACTER = "game.getDraggingCharacter";
            public const string RESIZE_UI = "game.resizeUI";
        }

        public class Semantic
        {
            public const string SHOW_DELETE_CONFIRMATION = "semanticHelpers.showDeleteConfirmation";
            public const string DROPDOWN = "semanticHelpers.dropdown";
            public const string SIDEBAR = "semanticHelpers.sidebar";
        }

        public class Common
        {
            public const string SCROLL_TO_BOTTOM = "common.scrollToBottom";
            public const string CREATE_IMAGE = "common.createImageAsync";
        }
    }
}
