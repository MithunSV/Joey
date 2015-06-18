using System;

namespace TicTacToe.AI
{
    internal enum Player
    {
        First = 1,
        None = 0,
        Second = -1
    }

    internal class Brain
    {
        private const int BOARD_SIZE = 3;

        private Neuron rootNode = null;
        private Neuron currentNode = null;

        internal Player[,] Status
        {
            get
            {
                return (Player[,])currentNode.Status.Clone();
            }
        }

        internal Player NextTurn
        {
            get
            {
                return currentNode.NextTurn;
            }
        }

        internal Player Winner
        {
            get
            {
                return currentNode.Winner;
            }
        }

        internal Brain()
        {
            rootNode = new Neuron(Player.First);
            currentNode = rootNode;
        }

        internal void ChangeStatus(int rowIndex, int columnIndex)
        {
            currentNode = currentNode.UpdatePosition(rowIndex, columnIndex);
        }

        internal void ResetStatus()
        {
            currentNode = rootNode;
        }

        internal void GetNextBestStep(out int rowIndex, out int columnIndex)
        {
            currentNode.GetFavourableStep(out rowIndex, out columnIndex);
        }
    }
}
