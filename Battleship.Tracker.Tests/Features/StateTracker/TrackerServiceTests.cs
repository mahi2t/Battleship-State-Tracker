using Battleship.Tracker.Features.StateTracker;
using Moq;
using NUnit.Framework;
using System;

namespace Battleship.Tracker.Tests.Features.StateTracker
{
    [TestFixture]
    public class TrackerServiceTests
    {
        private Mock<IValidationService> validationService;
        private Mock<ICacheService> cacheService;
        private ITrackerService trackerService;
        private BattleshipRequest request;

        [SetUp]
        public void Setup()
        {
            request = new BattleshipRequest()
            {
                Length = 2,
                Orientation = "vertical",
                Position = new Coordinates { X = 2, Y = 3 }
            };
            validationService = new Mock<IValidationService>();
            cacheService = new Mock<ICacheService>();
            trackerService = new TrackerService(cacheService.Object, validationService.Object);
        }

        [Test]
        public void CreateBoard_Should_create_board_upon_valid_request()
        {
            // Arrange
            var response = Tuple.Create<bool, BattleBoard>(false, null);
            cacheService.Setup(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>())).Returns(response);
            cacheService.Setup(x => x.CreateOrUpdateCache(It.IsAny<string>(), It.IsAny<object>()));

            // Act
            var result = trackerService.CreateBoard(10);

            // Assert
            Assert.AreEqual(result, string.Empty);
            cacheService.Verify(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>()), Times.Once);
            cacheService.Verify(x => x.CreateOrUpdateCache(It.IsAny<string>(), It.IsAny<object>()), Times.Once);
        }

        [Test]
        public void CreateBoard_Should_return_error_message_when_the_board_is_already_created()
        {
            // Arrange
            var response = Tuple.Create<bool, BattleBoard>(true, new BattleBoard { BoardSize = 10 });
            cacheService.Setup(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>())).Returns(response);
            cacheService.Setup(x => x.CreateOrUpdateCache(It.IsAny<string>(), It.IsAny<object>()));

            // Act
            var result = trackerService.CreateBoard(10);

            // Assert
            Assert.AreEqual(result, "10X10 board already created");
            cacheService.Verify(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>()), Times.Once);
            cacheService.Verify(x => x.CreateOrUpdateCache(It.IsAny<string>(), It.IsAny<object>()), Times.Never);
        }

        [TestCase(false, false, "Miss")]
        [TestCase(true, true, "Miss")]
        [TestCase(true, false, "Miss")]
        [TestCase(false, true, "Miss")]
        public void Attack_Should_return_miss_when_input_not_in_range_or_ship_not_placed(bool isInRange,
            bool isBoardCreated, string expected)
        {
            // Arrange
            var response = isBoardCreated ?
                Tuple.Create<bool, BattleBoard>(isBoardCreated,
                new BattleBoard() { BoardCells = new BoardCell[5, 5], Length = 2 })
                : Tuple.Create<bool, BattleBoard>(isBoardCreated, null);

            cacheService.Setup(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>())).Returns(response);
            validationService.Setup(x => x.IsInRangeToAttack(It.IsAny<int>(), It.IsAny<int>())).Returns(isInRange);

            // Act
            var actual = trackerService.Attack(2, 3);
            // Assert
            Assert.AreEqual(expected, actual);
            validationService.Verify(x => x.IsInRangeToAttack(It.IsAny<int>(), It.IsAny<int>()), Times.Once);

            if (isInRange)
            {
                cacheService.Verify(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>()), Times.Once);
            }
            else
            {
                cacheService.Verify(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>()), Times.Never);
            }
        }

        [TestCase(3, 3, "Miss")]
        [TestCase(0, 0, "Miss")]
        [TestCase(1, 2, "Miss")]
        [TestCase(3, 2, "Hit")]
        [TestCase(4, 2, "Hit")]
        [TestCase(5, 2, "Miss")]
        [TestCase(5, 5, "Miss")]
        [TestCase(2, 2, "Miss")]
        [TestCase(2, 1, "Miss")]
        [TestCase(1, 1, "Miss")]
        public void Attack_Should_return_hit_or_miss_based_on_input_matches_ship_coordinates_or_not(int xPosition,
         int yPosition, string expected)
        {
            // Arrange
            var battleBoard = new BattleBoard() { BoardCells = new BoardCell[6, 6], Length = 2 };
            battleBoard.BoardCells[3, 2] = new BoardCell { IsEmpty = false, IsHit = false };
            battleBoard.BoardCells[4, 2] = new BoardCell { IsEmpty = false, IsHit = false };
            var response = Tuple.Create<bool, BattleBoard>(true, battleBoard);

            cacheService.Setup(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>())).Returns(response);
            validationService.Setup(x => x.IsInRangeToAttack(It.IsAny<int>(), It.IsAny<int>())).Returns(true);

            // Act
            var actual = trackerService.Attack(xPosition, yPosition);
            // Assert
            Assert.AreEqual(expected, actual);
            validationService.Verify(x => x.IsInRangeToAttack(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
            cacheService.Verify(x => x.GetDataFromCache<BattleBoard>(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void PlaceShipOnBoard_should_return_error_message_if_board_not_created()
        {
            // Arrange
            var response = Tuple.Create(false, 5, new BoardCell[5, 5]);
            validationService.Setup(x => x.IsBattleBoardCreated()).Returns(response);

            // Act
            var actual = trackerService.PlaceShipOnBoard(request);

            // Assert
            validationService.Verify(x => x.IsBattleBoardCreated(), Times.Once);
            Assert.AreEqual("Board is not created yet, please create board.", actual);
        }

        [Test]
        public void PlaceShipOnBoard_should_return_error_message_if_trying_to_place_outside_board()
        {
            // Arrange
            var createBoardResponse = Tuple.Create(true, 5, new BoardCell[5, 5]);
            var response = Tuple.Create(false, 5, Orientation.Horizontal);
            validationService.Setup(x => x.IsBattleBoardCreated()).Returns(createBoardResponse);
            validationService.Setup(x => x.IsShipPlacedOnBoard()).Returns(false);
            validationService.Setup(x => x.IsValidPosition(It.IsAny<BattleshipRequest>(), It.IsAny<int>())).Returns(response);

            // Act
            var actual = trackerService.PlaceShipOnBoard(request);

            // Assert
            validationService.Verify(x => x.IsBattleBoardCreated(), Times.Once);
            validationService.Verify(x => x.IsShipPlacedOnBoard(), Times.Once);
            validationService.Verify(x => x.IsValidPosition(It.IsAny<BattleshipRequest>(), It.IsAny<int>()), Times.Once);
            Assert.AreEqual("Cannot place the ship outside the board.", actual);
        }

        [Test]
        public void PlaceShipOnBoard_should_return_error_message_if_ship_already_placed()
        {
            // Arrange
            var response = Tuple.Create(true, 5, new BoardCell[5, 5]);
            validationService.Setup(x => x.IsBattleBoardCreated()).Returns(response);
            validationService.Setup(x => x.IsShipPlacedOnBoard()).Returns(true);
            // Act
            var actual = trackerService.PlaceShipOnBoard(request);

            // Assert
            validationService.Verify(x => x.IsBattleBoardCreated(), Times.Once);
            validationService.Verify(x => x.IsShipPlacedOnBoard(), Times.Once);
            validationService.Verify(x => x.IsBoardCellEmpty(It.IsAny<int>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Orientation>()), Times.Never);
             
            Assert.AreEqual("Ship is already placed on board.", actual);
        }

        [Test]
        public void PlaceShipOnBoard_should_return_error_message_if_cell_is_not_empty()
        {
            // Arrange
            var createBoardResponse = Tuple.Create(true, 5, new BoardCell[5, 5]);
            var response = Tuple.Create(true, 5, Orientation.Horizontal);
            validationService.Setup(x => x.IsBattleBoardCreated()).Returns(createBoardResponse);
            validationService.Setup(x => x.IsShipPlacedOnBoard()).Returns(false);
            validationService.Setup(x => x.IsValidPosition(It.IsAny<BattleshipRequest>(), It.IsAny<int>())).Returns(response);
            validationService.Setup(x => x.IsBoardCellEmpty(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Orientation>()))
                .Returns(false);
            // Act
            var actual = trackerService.PlaceShipOnBoard(request);

            // Assert
            validationService.Verify(x => x.IsBattleBoardCreated(), Times.Once);
            validationService.Verify(x => x.IsShipPlacedOnBoard(), Times.Once);
            validationService.Verify(x => x.IsValidPosition(It.IsAny<BattleshipRequest>(), It.IsAny<int>()), Times.Once);
            validationService.Verify(x => x.IsBoardCellEmpty(It.IsAny<int>(),
               It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Orientation>()), Times.Once);
            Assert.AreEqual("Cell is already occupied by ship.", actual);
        }

        [TestCase(Orientation.Horizontal)]
        [TestCase(Orientation.Vertical)]
        public void PlaceShipOnBoard_should_place_ship_on_board(Orientation orientation)
        {
            // Arrange
            var createBoardResponse = Tuple.Create(true, 5, new BoardCell[6, 6]);
            var response = Tuple.Create(true, 5, orientation);
            validationService.Setup(x => x.IsBattleBoardCreated()).Returns(createBoardResponse);
            validationService.Setup(x => x.IsShipPlacedOnBoard()).Returns(false);
            validationService.Setup(x => x.IsValidPosition(It.IsAny<BattleshipRequest>(), It.IsAny<int>())).Returns(response);
            validationService.Setup(x => x.IsBoardCellEmpty(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Orientation>()))
                .Returns(true);
            cacheService.Setup(x => x.CreateOrUpdateCache(It.IsAny<string>(), It.IsAny<object>()));

            // Act
            var actual = trackerService.PlaceShipOnBoard(request);

            // Assert
            validationService.Verify(x => x.IsBattleBoardCreated(), Times.Once);
            validationService.Verify(x => x.IsShipPlacedOnBoard(), Times.Once);
            validationService.Verify(x => x.IsValidPosition(It.IsAny<BattleshipRequest>(), It.IsAny<int>()), Times.Once);
            validationService.Verify(x => x.IsBoardCellEmpty(It.IsAny<int>(),
               It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Orientation>()), Times.Once);
            cacheService.Verify(x => x.CreateOrUpdateCache(It.IsAny<string>(), It.IsAny<object>()), Times.Exactly(2));
        }
    }
}
