using System;
using System.Linq;

namespace Battleship.Tracker.Features.StateTracker
{
    /// <summary>
    /// IValidationService
    /// </summary>
    public interface IValidationService
    {
        Tuple<bool, int, BoardCell[,]> IsBattleBoardCreated();
        bool IsBoardCellEmpty(int xPosition, int yPosition, int endPosition, Orientation orientation);
        bool IsInRangeToAttack(int xPosition, int yPosition);
        bool IsShipDrowned();
        bool IsShipPlacedOnBoard();
        Tuple<bool, int, Orientation> IsValidPosition(BattleshipRequest battleship, int boardSize);
    }

    /// <summary>
    /// ValidationService
    /// </summary>
    public class ValidationService : IValidationService
    {
        private readonly ICacheService cacheService;

        public ValidationService(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        /// <summary>
        /// Verifies if the board is created already.
        /// </summary>
        /// <returns>Tuple<bool, int, BoardCell[,]></returns>
        public Tuple<bool, int, BoardCell[,]> IsBattleBoardCreated()
        {
            var (isCreated, battleBoard) = cacheService.GetDataFromCache<BattleBoard>(Constants.CacheKeys.BATTLE_BOARD);
            isCreated = isCreated && battleBoard?.BoardSize > 0;
            return Tuple.Create(isCreated, isCreated  && battleBoard != null ? battleBoard.BoardSize + 1 : 0, battleBoard?.BoardCells);
        }

        /// <summary>
        /// Checks if the battle ship is placed or not?
        /// </summary>
        /// <returns>bool</returns>
        public bool IsShipPlacedOnBoard()
        {
            var (isPlaced, _) = cacheService.GetDataFromCache<bool>(Constants.CacheKeys.IS_SHIP_PLACED_ON_BOARD);
            return isPlaced;
        }

        /// <summary>
        /// Verifies if the cell is empty.
        /// </summary>
        /// <param name="xPosition">xPosition</param>
        /// <param name="yPosition">yPosition</param>
        /// <param name="endPosition">endPosition</param>
        /// <param name="orientation"></param>
        /// <returns>bool</returns>
        public bool IsBoardCellEmpty(int xPosition, int yPosition, int endPosition, Orientation orientation)
        {
            var (_, battleBoard) = cacheService.GetDataFromCache<BattleBoard>(Constants.CacheKeys.BATTLE_BOARD);
            switch (orientation)
            {
                case Orientation.Horizontal:
                    for (var i = xPosition; i <= endPosition; i++)
                    {
                        if (battleBoard.BoardCells[i, yPosition] != null &&
                            !battleBoard.BoardCells[i, yPosition].IsEmpty)
                        {
                            return false;
                        }
                    }
                    break;
                case Orientation.Vertical:
                    for (int i = yPosition; i <= endPosition; i++)
                    {
                        if (battleBoard.BoardCells[xPosition, i] != null &&
                            !battleBoard.BoardCells[xPosition, i].IsEmpty)
                        {
                            return false;
                        }
                    }
                    break;
            }
            return true;
        }

        /// <summary>
        /// Checks if the ship is alive or broken.
        /// </summary>
        /// <returns>bool</returns>
        public bool IsShipDrowned()
        {
            var (_, battleBoard) = cacheService.GetDataFromCache<BattleBoard>(Constants.CacheKeys.BATTLE_BOARD);
            var count = 0;
            foreach (var cell in battleBoard?.BoardCells)
            {
                if (cell != null && cell.IsHit && !cell.IsEmpty)
                {
                    count = count + 1;
                }
            }
            return count == battleBoard.Length;
        }

        /// <summary>
        /// Verifies if the provided input position is on the board range.
        /// </summary>
        /// <param name="xPosition">xPosition</param>
        /// <param name="yPosition">yPosition</param>
        /// <returns>bool</returns>
        public bool IsInRangeToAttack(int xPosition, int yPosition)
        {
            var (_, battleBoard) = cacheService.GetDataFromCache<BattleBoard>(Constants.CacheKeys.BATTLE_BOARD);
            if (battleBoard != null && battleBoard.BoardSize > 0)
            {
                var isInRange = Enumerable.Range(1, battleBoard.BoardSize);
                return isInRange.Contains(xPosition) && isInRange.Contains(yPosition);

            }
            return false;
        }

        /// <summary>
        /// Verifies if the passed in position is valid or not.
        /// </summary>
        /// <param name="battleship">battleship</param>
        /// <param name="boardSize">boardSize</param>
        /// <returns>Tuple<bool, int, Orientation></returns>
        public Tuple<bool, int, Orientation> IsValidPosition(BattleshipRequest battleship, int boardSize)
        {
            int shipEndCoordinate = -1;
            Orientation orientation;
            switch (battleship?.Orientation?.ToUpper())
            {
                case Constants.Orientation.VERTICAL:
                    shipEndCoordinate = battleship.Position.Y + battleship.Length - 1;
                    orientation = Orientation.Vertical;
                    break;
                case Constants.Orientation.HORIZONTAL:
                    shipEndCoordinate = battleship.Position.X + battleship.Length - 1;
                    orientation = Orientation.Horizontal;
                    break;
                default:
                    orientation = Orientation.Invalid;
                    break;

            }
            var validRange = Enumerable.Range(1, boardSize);

            var isValid = validRange.Contains(battleship.Position.X) &&
            validRange.Contains(battleship.Position.Y) &&
            validRange.Contains(shipEndCoordinate);
            return Tuple.Create(isValid, shipEndCoordinate, orientation);
        }
    }
}
