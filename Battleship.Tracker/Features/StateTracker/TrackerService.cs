using System;

namespace Battleship.Tracker.Features.StateTracker
{
    /// <summary>
    /// ITrackerService
    /// </summary>
    public interface ITrackerService
    {
        string Attack(int xPosition, int yPosition);
        string CreateBoard(int size);
        string PlaceShipOnBoard(BattleshipRequest ship);
    }

    /// <summary>
    /// TrackerService
    /// </summary>
    public class TrackerService : ITrackerService
    {
        private readonly ICacheService cacheService;
        private readonly IValidationService validationService;

        public TrackerService(ICacheService cacheService,
            IValidationService validationService)
        {
            this.cacheService = cacheService;
            this.validationService = validationService;
        }

        /// <summary>
        /// Attack the ship with coordinates
        /// </summary>
        /// <param name="xPosition">xPosition</param>
        /// <param name="yPosition">yPosition</param>
        /// <returns>string</returns>
        public string Attack(int xPosition, int yPosition)
        {
            if (!validationService.IsInRangeToAttack(xPosition, yPosition))
            {
                return Constants.Attack.MISS;
            }

            var (_, battleBoard) = cacheService.GetDataFromCache<BattleBoard>(Constants.CacheKeys.BATTLE_BOARD);
            var attackedCell = battleBoard?.BoardCells[xPosition, yPosition];

            if (attackedCell != null && !attackedCell.IsEmpty && !attackedCell.IsHit)
            {
                attackedCell.IsHit = true;
                var isShipDrowned = validationService.IsShipDrowned();
                return isShipDrowned ? Constants.Attack.DROWNED : Constants.Attack.HIT;
            }

            return Constants.Attack.MISS;
        }

        /// <summary>
        /// Creates the board based on given size.
        /// </summary>
        /// <param name="size">size</param>
        /// <returns>string</returns>
        public string CreateBoard(int size)
        {
            var (isExists, battleBoard) = cacheService.GetDataFromCache<BattleBoard>(Constants.CacheKeys.BATTLE_BOARD);
            if (isExists && battleBoard?.BoardSize > 0)
            {
                return $"{battleBoard.BoardSize}X{battleBoard.BoardSize} board already created";
            }

            battleBoard = new BattleBoard()
            {
                // +1 is to ignore the zero based index row and column.
                BoardCells = new BoardCell[size + 1, size + 1],
                BoardSize = size
            };

            cacheService.CreateOrUpdateCache(Constants.CacheKeys.BATTLE_BOARD, battleBoard);
            return string.Empty;
        }

        /// <summary>
        /// Add ship to the board.
        /// </summary>
        /// <param name="battleship">battleship</param>
        /// <returns>string</returns>
        public string PlaceShipOnBoard(BattleshipRequest battleship)
        {
            var (isCreated, size, boardCells) = validationService.IsBattleBoardCreated();
            if (!isCreated)
            {
                return Constants.Messages.BOARD_NOT_CREATED_YET;
            }

            if (validationService.IsShipPlacedOnBoard())
            {
                return Constants.Messages.SHIP_IS_ALREADY_PLACED;
            }

            var (isValid, endCoordinate, orientation) = validationService.IsValidPosition(battleship, size - 1);
            if (!isValid)
            {
                return Constants.Messages.CANNOT_PLACE_OUTSIDE_BOARD;
            }

            if (!validationService.IsBoardCellEmpty(battleship.Position.X,
                battleship.Position.Y,
                endCoordinate, orientation))
            {
                return Constants.Messages.CELL_IS_OCCUPIED;
            }

            var battleBoard = new BattleBoard
            {
                BoardCells = boardCells,
                BoardSize = size - 1,
                Length = battleship.Length
            };

            if (orientation.Equals(Orientation.Horizontal))
            {
                for (var i = battleship.Position.X; i <= endCoordinate; i++)
                {
                    battleBoard.BoardCells[i, battleship.Position.Y] = new BoardCell { IsEmpty = false, IsHit = false };
                }
            }

            if (orientation.Equals(Orientation.Vertical))
            {
                for (var i = battleship.Position.Y; i <= endCoordinate; i++)
                {
                    battleBoard.BoardCells[battleship.Position.X, i] = new BoardCell { IsEmpty = false, IsHit = false };
                }
            }

            cacheService.CreateOrUpdateCache(Constants.CacheKeys.IS_SHIP_PLACED_ON_BOARD, true);
            cacheService.CreateOrUpdateCache(Constants.CacheKeys.BATTLE_BOARD, battleBoard);
            return string.Empty;
        }
    }
}
