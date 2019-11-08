namespace Battleship.Tracker.Features.StateTracker
{
    public class Constants
    {
        public static class Boundaries
        {
            public const int X_LOWER_LIMIT = 0;
            public const int Y_LOWER_LIMIT = 0;
        }

        public static class Orientation
        {
            public const string HORIZONTAL = "HORIZONTAL";
            public const string VERTICAL = "VERTICAL";            
        }

        public static class CacheKeys {
            public const string BATTLE_BOARD = "BattleBoard";
            public const string IS_SHIP_PLACED_ON_BOARD = "IsShipPlacedOnBoard";
        }

        public static class Attack {
            public const string MISS = "Miss";
            public const string HIT = "Hit";
            public const string DROWNED = "Hit, Ship drowned";
        }

        public static class Messages
        {
            public const string BOARD_NOT_CREATED_YET = "Board is not created yet, please create board.";
            public const string SHIP_IS_ALREADY_PLACED = "Ship is already placed on board.";
            public const string CANNOT_PLACE_OUTSIDE_BOARD = "Cannot place the ship outside the board.";
            public const string CELL_IS_OCCUPIED = "Cell is already occupied by ship.";
        }
    }
}
