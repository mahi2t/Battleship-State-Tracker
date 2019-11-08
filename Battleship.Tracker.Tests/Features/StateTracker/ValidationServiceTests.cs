using Battleship.Tracker.Features.StateTracker;
using Moq;
using NUnit.Framework;
using System;

namespace Battleship.Tracker.Tests.Features.StateTracker
{
    [TestFixture]
    public class ValidationServiceTests
    {
        private Mock<ICacheService> cacheService;
        private IValidationService validationService;
        private BattleshipRequest request = new BattleshipRequest()
        {
            Length = 3,
            Orientation = "vertical",
            Position = new Coordinates { X = 2, Y = 2 }
        };

        [SetUp]
        public void Setup()
        {
            cacheService = new Mock<ICacheService>();
            validationService = new ValidationService(cacheService.Object);
        }

        [TestCase(true, 5, 6)]
        [TestCase(true, 3, 4)]
        [TestCase(false, 5, 0)]
        [TestCase(true, 0, 0)]
        public void IsBattleBoardCreated_Should_verify_if_board_is_created_or_not(bool isCreated, int boardSize, int expected)
        {
            // Arrange
            var response = Tuple.Create<bool, BattleBoard>(isCreated, new BattleBoard { BoardSize = boardSize, Length = 2 });
            cacheService.Setup(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>())).Returns(response);
            // Act
            var actual = validationService.IsBattleBoardCreated();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual.Item2);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsShipPlacedOnBoard_Shoult_return_true_or_false_based_on_value_exists_in_cache_or_not(bool expected)
        {
            // Arrange
            var result = Tuple.Create<bool, bool>(expected, false);
            cacheService.Setup(x => x.GetDataFromCache<bool>(It.IsAny<string>())).Returns(result);

            // Act
            var actual = validationService.IsShipPlacedOnBoard();
            // Assert
            cacheService.Verify(x => x.GetDataFromCache<bool>(It.IsAny<string>()), Times.Once);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(Orientation.Horizontal, 3, 2, 4, true, true)]
        [TestCase(Orientation.Horizontal, 3, 3, 4, false, false)]
        [TestCase(Orientation.Vertical, 3, 2, 4, true, true)]
        public void IsBoardCellEmpty_Should_return_true_or_false_based_on_cell_state(Orientation orientation,
            int xPosition, int yPostiion, int endPosition, bool isEmpty, bool expected)
        {
            // Arrange
            var battleBoard = new BattleBoard() { BoardCells = new BoardCell[6, 6], Length = 2, BoardSize = 5 };
            battleBoard.BoardCells[xPosition, yPostiion] = new BoardCell { IsEmpty = isEmpty, IsHit = false };

            var response = Tuple.Create<bool, BattleBoard>(true, battleBoard);
            cacheService.Setup(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>())).Returns(response);

            // Act
            var actual = validationService.IsBoardCellEmpty(xPosition, yPostiion, endPosition, orientation);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(true, true)]
        [TestCase(false, false)]
        public void IsShipDrowned_Should_return_true_or_false_based_on_ship_status(bool isHit, bool expected)
        {
            // Arrange
            var battleBoard = new BattleBoard() { BoardCells = new BoardCell[6, 6], Length = 3, BoardSize = 5 };
            battleBoard.BoardCells[1, 2] = new BoardCell { IsEmpty = false, IsHit = isHit };
            battleBoard.BoardCells[1, 3] = new BoardCell { IsEmpty = false, IsHit = isHit };
            battleBoard.BoardCells[1, 4] = new BoardCell { IsEmpty = false, IsHit = isHit };

            var response = Tuple.Create<bool, BattleBoard>(true, battleBoard);
            cacheService.Setup(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>())).Returns(response);

            // Act
            var actual = validationService.IsShipDrowned();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(2, 20, false)]
        [TestCase(0, 5, false)]
        [TestCase(5, 0, false)]
        [TestCase(6, 2, false)]
        [TestCase(0, 0, false)]
        [TestCase(1, 1, true)]
        [TestCase(2, 2, true)]
        [TestCase(3, 3, true)]
        [TestCase(4, 4, true)]
        [TestCase(5, 5, true)]
        [TestCase(5, 6, false)]
        [TestCase(6, 5, false)]
        public void IsInRangeToAttack_Should_return_true_or_false_based_on_input(int xPosition, int yPosition, bool expected)
        {
            // Arrange
            var battleBoard = new BattleBoard() { BoardCells = new BoardCell[6, 6], Length = 2, BoardSize = 5 };
            battleBoard.BoardCells[1, 2] = new BoardCell { IsEmpty = false, IsHit = false };
            battleBoard.BoardCells[1, 3] = new BoardCell { IsEmpty = false, IsHit = false };
            battleBoard.BoardCells[1, 4] = new BoardCell { IsEmpty = false, IsHit = false };

            var response = Tuple.Create<bool, BattleBoard>(true, battleBoard);
            cacheService.Setup(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>())).Returns(response);

            // Act
            var actual = validationService.IsInRangeToAttack(xPosition, yPosition);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        [TestCase(2,"horizontal",4,4,true,5,Orientation.Horizontal)]
        [TestCase(3, "horizontal", 1, 2, true, 3, Orientation.Horizontal)]
        [TestCase(3, "horizontal", 8, 2, false, 10, Orientation.Horizontal)]
        [TestCase(4, "Vertical", 8, 2, false, 5, Orientation.Vertical)]
        [TestCase(4, "Vertical", 0, 0, false, 3, Orientation.Vertical)]
        [TestCase(4, "Vertical", 10, 10, false, 13, Orientation.Vertical)]
        [TestCase(4, "Vertical", 3,1, true, 4, Orientation.Vertical)]
        public void IsValidPosition_Should_return_true_or_false_based_on_input_position(int length, string orientation, int xPostion, int yPosition,
            bool isExpected, int expectedEndPosition, Orientation expecetedEnum)
        {
            // Arrange
            request = new BattleshipRequest()
            {
                Length = length,
                Orientation = orientation,
                Position = new Coordinates { X = xPostion, Y = yPosition }
            };

            // Act
            var actual = validationService.IsValidPosition(request, 5);

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(isExpected, actual.Item1);
            Assert.AreEqual(expectedEndPosition, actual.Item2);
            Assert.AreEqual(expecetedEnum, actual.Item3);
        }
    }


}
