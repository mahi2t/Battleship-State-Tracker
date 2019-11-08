namespace Battleship.Tracker.Features.StateTracker
{
    public class BattleBoard
    {
        public BoardCell[,] BoardCells { get; set; }
        public int BoardSize { get; set; }

        public int Length { get; set; }
    }
}
