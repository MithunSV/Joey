using System;

namespace TicTacToe.Model
{
    public enum Player
    {
        PlayerO = -1,
        None = 0,
        PlayerX = 1
    }

    public class StatusChangedEventArgs : EventArgs
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public Player NewValue { get; set; }

        public StatusChangedEventArgs(int row, int column, Player value)
        {
            Row = row;
            Column = column;
            NewValue = value;
        }
    }

    public class Board
    {
        private const int SIZE = 3;

        private Player _turn = Player.None;
        private Player _winner = Player.None;
        private Player[,] _status = new Player[SIZE, SIZE];

        public Player Turn
        {
            get
            {
                return _turn;
            }
            private set
            {
                if (_turn != value)
                {
                    _turn = value;
                    if (TurnChanged != null)
                    {
                        TurnChanged();
                    }
                }
            }
        }
        public Player Winner
        {
            get
            {
                return _winner;
            }
            private set
            {
                if (_winner != value)
                {
                    _winner = value;
                    if (WinnerChanged != null)
                    {
                        WinnerChanged();
                    }
                }
            }
        }
        public Player[,] Status
        {
            get
            {
                return (Player[,])_status.Clone();
            }
        }

        public event Action TurnChanged;
        public event Action WinnerChanged;
        public delegate void StatusChangedHandler(StatusChangedEventArgs eventArgs);
        public event StatusChangedHandler StatusChanged;

        public void Clear()
        {
            _status = new Player[SIZE, SIZE];
            if (StatusChanged != null)
            {
                for (int i = 0; i < SIZE; i++)
                {
                    for (int j = 0; j < SIZE; j++)
                    {
                        StatusChanged(new StatusChangedEventArgs(i, j, Player.None));
                    }
                }
            }
            Turn = Player.None;
            Winner = Player.None;
        }

        public void Start(Player firstChance)
        {
            Clear();
            Turn = firstChance;
        }

        public void Mark(int rowIndex, int columnIndex)
        {
            if (rowIndex < 0 || rowIndex >= SIZE || columnIndex < 0 || columnIndex >= SIZE || Turn == Player.None || _status[rowIndex, columnIndex] != Player.None)
            {
                return;
            }

            _status[rowIndex, columnIndex] = Turn;
            if (StatusChanged != null)
            {
                StatusChanged(new StatusChangedEventArgs(rowIndex, columnIndex, Turn));
            }

            Player winner = CheckWinner();
            if (winner != Player.None)
            {
                Turn = Player.None;
                Winner = winner;
            }
            else
            {
                Turn = Turn == Player.PlayerX ? Player.PlayerO : Player.PlayerX;
            }
        }

        private Player CheckWinner()
        {
            int cellTotal = 0;

            //Rows
            for (int i = 0; i < SIZE; i++)
            {
                cellTotal = 0;
                for (int j = 0; j < SIZE; j++)
                {
                    cellTotal += (int)_status[i, j];
                }
                if (Math.Abs(cellTotal) == SIZE)
                {
                    return cellTotal > 0 ? Player.PlayerX : Player.PlayerO;
                }
            }

            //Columns
            for (int i = 0; i < SIZE; i++)
            {
                cellTotal = 0;
                for (int j = 0; j < SIZE; j++)
                {
                    cellTotal += (int)_status[j, i];
                }
                if (Math.Abs(cellTotal) == SIZE)
                {
                    return cellTotal > 0 ? Player.PlayerX : Player.PlayerO;
                }
            }

            //principal diagonal
            cellTotal = 0;
            for (int i = 0; i < SIZE; i++)
            {
                cellTotal += (int)_status[i, i];
            }
            if (Math.Abs(cellTotal) == SIZE)
            {
                return cellTotal > 0 ? Player.PlayerX : Player.PlayerO;
            }

            //secondary diagonal
            cellTotal = 0;
            for (int i = 0, j = SIZE - 1; i < SIZE; i++, j--)
            {
                cellTotal += (int)_status[i, j];
            }
            if (Math.Abs(cellTotal) == SIZE)
            {
                return cellTotal > 0 ? Player.PlayerX : Player.PlayerO;
            }

            return Player.None;
        }
    }
}
