using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using cli_life;

namespace Test
{
    [TestClass]
    public class CellTests
    {
        [TestMethod]
        public void DetermineNextLiveState_CellWithTwoLiveNeighbors_StaysAlive()
        {
            // Arrange
            var cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });

            // Act
            cell.DetermineNextLiveState();

            // Assert
            Assert.IsTrue(cell.IsAliveNext);
        }

        [TestMethod]
        public void DetermineNextLiveState_CellWithThreeLiveNeighbors_StaysAlive()
        {
            // Arrange
            var cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });

            // Act
            cell.DetermineNextLiveState();

            // Assert
            Assert.IsTrue(cell.IsAliveNext);
        }

        [TestMethod]
        public void DetermineNextLiveState_CellWithOneLiveNeighbor_Dies()
        {
            // Arrange
            var cell = new Cell { IsAlive = true };
            cell.neighbors.Add(new Cell { IsAlive = false });

            // Act
            cell.DetermineNextLiveState();

            // Assert
            Assert.IsFalse(cell.IsAliveNext);
        }

        [TestMethod]
        public void DetermineNextLiveState_DeadCellWithThreeLiveNeighbors_BecomesAlive()
        {
            // Arrange
            var cell = new Cell { IsAlive = false };
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });
            cell.neighbors.Add(new Cell { IsAlive = true });

            // Act
            cell.DetermineNextLiveState();

            // Assert
            Assert.IsTrue(cell.IsAliveNext);
        }

        [TestMethod]
        public void Advance_CellWithIsAliveNext_True_SetsIsAliveToTrue()
        {
            // Arrange
            var cell = new Cell { IsAliveNext = true };

            // Act
            cell.Advance();

            // Assert
            Assert.IsTrue(cell.IsAlive);
        }
    }

    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        public void GetTotalElementCount_BoardWithOneCell_ReturnsOne()
        {
            // Arrange
            var board = new Board(10, 10, 1);
            board.Cells[0, 0].IsAlive = true;

            // Act
            var count = board.GetTotalElementCount();

            // Assert
            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void GetTotalElementCount_EmptyBoard_ReturnsZero()
        {
            // Arrange
            var board = new Board(10, 10, 1);

            // Act
            var count = board.GetTotalElementCount();

            // Assert
            Assert.AreEqual(0, count);
        }   
    }
}    
