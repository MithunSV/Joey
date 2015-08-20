using System;
using System.Collections.Generic;

namespace TicTacToe.AI
{
    internal class Neuron
    {
        private const int BOARD_SIZE = 3;

        private Neuron Parent = null;
        private Dictionary<int, Neuron> Children = new Dictionary<int,Neuron>();
        private int player1FavourableCount = 0;
        private int player2FavourableCount = 0;
        private double orderStateValue = 0;

        internal Player Winner { get; private set; }
        internal Player NextTurn { get; private set; }
        internal Player[,] Status { get; private set; }

        private void UpdateFavourableCount(Player winner)
        {
            if (winner == Player.First)
            {
                player1FavourableCount++;
            }
            else if (winner == Player.Second)
            {
                player2FavourableCount++;
            }
            UpdateOrderStateValue();
            if (Parent != null)
            {
                Parent.UpdateFavourableCount(winner);
            }
        }

        private void UpdateOrderStateValue()
        {
            if (player1FavourableCount == 0)
            {
                orderStateValue = -1;
                return;
            }
            if (player2FavourableCount == 0)
            {
                orderStateValue = 1;
                return;
            }

            double player1WinRatio = (double)player1FavourableCount / (player1FavourableCount + player2FavourableCount);
            double player2WinRatio = 1 - player1WinRatio;

            double disorder = -player1WinRatio * Math.Log(player1WinRatio, 2) - player2WinRatio * Math.Log(player2WinRatio);
            orderStateValue = (player1WinRatio < 0.5 ? -1 : 1) * (1 - disorder);
        }

        private Neuron AddNewChild(int rowIndex, int columnIndex)
        {
            Neuron NewNode = new Neuron();

            NewNode.NextTurn = this.NextTurn == Player.First ? Player.Second : Player.First;
            NewNode.Status = (Player[,])Status.Clone();
            NewNode.Status[rowIndex, columnIndex] = this.NextTurn;

            this.Children.Add(rowIndex * BOARD_SIZE + columnIndex, NewNode);
            NewNode.Parent = this;

            Player winner = NewNode.CheckWinner();
            if (winner == Player.None)
            {
                bool boardFull = true;
                for (int i = 0; i < BOARD_SIZE; i++)
                {
                    for (int j = 0; j < BOARD_SIZE; j++)
                    {
                        if (NewNode.Status[i, j] == Player.None)
                        {
                            boardFull = false;
                            break;
                        }
                    }
                    if (!boardFull)
                    {
                        break;
                    }
                }
                if (boardFull)
                {
                    NewNode.NextTurn = Player.None;
                }
            }
            if (winner != Player.None)
            {
                NewNode.NextTurn = Player.None;
                NewNode.Winner = winner;
                NewNode.UpdateFavourableCount(winner);
            }
            return NewNode;
        }

        private Player CheckWinner()
        {
            int cellTotal = 0;

            //Rows
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                cellTotal = 0;
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    cellTotal += (int)Status[i, j];
                }
                if (Math.Abs(cellTotal) == BOARD_SIZE)
                {
                    return cellTotal > 0 ? Player.First : Player.Second;
                }
            }

            //Columns
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                cellTotal = 0;
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    cellTotal += (int)Status[j, i];
                }
                if (Math.Abs(cellTotal) == BOARD_SIZE)
                {
                    return cellTotal > 0 ? Player.First : Player.Second;
                }
            }

            //principal diagonal
            cellTotal = 0;
            for (int i = 0; i < BOARD_SIZE; i++)
            {
                cellTotal += (int)Status[i, i];
            }
            if (Math.Abs(cellTotal) == BOARD_SIZE)
            {
                return cellTotal > 0 ? Player.First : Player.Second;
            }

            //secondary diagonal
            cellTotal = 0;
            for (int i = 0, j = BOARD_SIZE - 1; i < BOARD_SIZE; i++, j--)
            {
                cellTotal += (int)Status[i, j];
            }
            if (Math.Abs(cellTotal) == BOARD_SIZE)
            {
                return cellTotal > 0 ? Player.First : Player.Second;
            }

            return Player.None;
        }

        internal Neuron(Player nextTurn = Player.None)
        {
            Winner = Player.None;
            NextTurn = nextTurn;
            Status = new Player[BOARD_SIZE, BOARD_SIZE];
        }

        internal Neuron UpdatePosition(int rowIndex, int columnIndex)
        {
            int index = rowIndex * BOARD_SIZE + columnIndex;

            if (Children.ContainsKey(index))
            {
                return Children[index];
            }
            else
            {
                return AddNewChild(rowIndex, columnIndex);
            }
        }

        internal void GetFavourableStep(out int rowIndex, out int columnIndex)
        {
            int favourableCount = 0;
            int unFavourableCount = 0;
            int meanFavourableCount = 0;
            int maxFavourableCount = 0;
            int minUnFavourableCount = 0;
            int maxMeanFavourableCount = 0;
            double orderValue = 0;
            double favourableOrderValue = 0;
            bool chooseMostFavourable = (NextTurn == Player.First);
            chooseMostFavourable = true;

            rowIndex = -1;
            columnIndex = -1;

            for (int i = 0; i < BOARD_SIZE; i++)
            {
                for (int j = 0; j < BOARD_SIZE; j++)
                {
                    int index = i * BOARD_SIZE + j;
                    if (Status[i, j] == Player.None)
                    {
                        if (!Children.ContainsKey(index))
                        {
                            AddNewChild(i, j);
                        }
                        favourableCount = NextTurn == Player.First ? Children[index].player1FavourableCount : Children[index].player2FavourableCount;
                        unFavourableCount = NextTurn == Player.First ? Children[index].player2FavourableCount : Children[index].player1FavourableCount;
                        meanFavourableCount = favourableCount - unFavourableCount;
                        orderValue = Children[index].orderStateValue;
                        if (rowIndex == -1)
                        {
                            maxFavourableCount = favourableCount;
                            minUnFavourableCount = unFavourableCount;
                            maxMeanFavourableCount = favourableCount - unFavourableCount;
                            favourableOrderValue = orderValue;
                            rowIndex = i;
                            columnIndex = j;
                        }
                        if (Children[index].Winner == NextTurn)
                        {
                            rowIndex = i;
                            columnIndex = j;
                            return;
                        }
                        foreach (KeyValuePair<int, Neuron> lookAheadNode in Children[index].Children)
                        {
                            if (lookAheadNode.Value.Winner == Children[index].NextTurn)
                            {
                                rowIndex = lookAheadNode.Key / BOARD_SIZE;
                                columnIndex = lookAheadNode.Key % BOARD_SIZE;
                                return;
                            }
                        }
                        //if (meanFavourableCount >= maxMeanFavourableCount)
                        //{
                        //    if (meanFavourableCount > maxMeanFavourableCount)
                        //    {
                        //        maxFavourableCount = favourableCount;
                        //        minUnFavourableCount = unFavourableCount;
                        //        maxMeanFavourableCount = meanFavourableCount;
                        //        rowIndex = i;
                        //        columnIndex = j;
                        //    }
                        //    else if (chooseMostFavourable && favourableCount > maxFavourableCount)
                        //    {
                        //        maxFavourableCount = favourableCount;
                        //        rowIndex = i;
                        //        columnIndex = j;
                        //    }
                        //    else if (!chooseMostFavourable && unFavourableCount < minUnFavourableCount)
                        //    {
                        //        minUnFavourableCount = unFavourableCount;
                        //        rowIndex = i;
                        //        columnIndex = j;
                        //    }
                        //}
                        if (NextTurn == Player.First && orderValue > favourableOrderValue)
                        {
                            favourableOrderValue = orderValue;
                            rowIndex = i;
                            columnIndex = j;
                        }
                        else if (NextTurn == Player.Second && orderValue < favourableOrderValue)
                        {
                            favourableOrderValue = orderValue;
                            rowIndex = i;
                            columnIndex = j;
                        }
                    }
                }
            }
        }
    }
}
