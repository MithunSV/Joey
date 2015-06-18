using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TicTacToe.AI;
using TicTacToe.Model;

namespace TicTacToe.View
{
    public partial class frmTicTacToe : Form
    {
        private Board _gameBoard;
        private Joey _joey;

        public frmTicTacToe()
        {
            InitializeComponent();
            _gameBoard = new Board();
            _gameBoard.TurnChanged += _gameBoard_TurnChanged;
            _gameBoard.WinnerChanged += _gameBoard_WinnerChanged;
            _gameBoard.StatusChanged += _gameBoard_StatusChanged;
            _joey = new Joey(_gameBoard);
            _joey.MessageRaised += _joey_MessageRaised;
        }

        void _joey_MessageRaised(string message)
        {
            StatusMessage.Text = message;
        }

        void _gameBoard_StatusChanged(StatusChangedEventArgs eventArgs)
        {
            if (eventArgs.Row == 0 && eventArgs.Column == 0)
            {
                MarkCell(btnA1, eventArgs.NewValue);
            }
            else if (eventArgs.Row == 0 && eventArgs.Column == 1)
            {
                MarkCell(btnA2, eventArgs.NewValue);
            }
            else if (eventArgs.Row == 0 && eventArgs.Column == 2)
            {
                MarkCell(btnA3, eventArgs.NewValue);
            }
            else if (eventArgs.Row == 1 && eventArgs.Column == 0)
            {
                MarkCell(btnB1, eventArgs.NewValue);
            }
            else if (eventArgs.Row == 1 && eventArgs.Column == 1)
            {
                MarkCell(btnB2, eventArgs.NewValue);
            }
            else if (eventArgs.Row == 1 && eventArgs.Column == 2)
            {
                MarkCell(btnB3, eventArgs.NewValue);
            }
            else if (eventArgs.Row == 2 && eventArgs.Column == 0)
            {
                MarkCell(btnC1, eventArgs.NewValue);
            }
            else if (eventArgs.Row == 2 && eventArgs.Column == 1)
            {
                MarkCell(btnC2, eventArgs.NewValue);
            }
            else if (eventArgs.Row == 2 && eventArgs.Column == 2)
            {
                MarkCell(btnC3, eventArgs.NewValue);
            }
            this.Refresh();
        }

        void _gameBoard_WinnerChanged()
        {
            switch (_gameBoard.Winner)
            {
                case Player.None:
                    lblWinner1.Visible = false;
                    lblWinner2.Visible = false;
                    break;
                case Player.PlayerO:
                    lblWinner1.Visible = false;
                    lblWinner2.Visible = true;
                    break;
                case Player.PlayerX:
                    lblWinner1.Visible = true;
                    lblWinner2.Visible = false;
                    break;
            }
        }

        void _gameBoard_TurnChanged()
        {
            switch (_gameBoard.Turn)
            {
                case Player.None:
                    cmbPlayer1.BackColor = Color.White;
                    cmbPlayer2.BackColor = Color.White;
                    break;
                case Player.PlayerO:
                    cmbPlayer1.BackColor = Color.White;
                    cmbPlayer2.BackColor = Color.LightGreen;
                    break;
                case Player.PlayerX:
                    cmbPlayer1.BackColor = Color.LightGreen;
                    cmbPlayer2.BackColor = Color.White;
                    break;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (cmbPlayer1.SelectedIndex == -1 || cmbPlayer2.SelectedIndex == -1)
            {
                MessageBox.Show("Please select both players.", "Incomplete Data", MessageBoxButtons.OK);
                return;
            }

            if (cmbPlayer1.SelectedIndex == 1 && cmbPlayer2.SelectedIndex == 1)
            {
                _joey.StartPlay(LearningIntensity.learner, Player.PlayerX);
            }
            else if (cmbPlayer1.SelectedIndex == 1 && cmbPlayer2.SelectedIndex == 0)
            {
                _joey.StartPlay(LearningIntensity.player, Player.PlayerX);
            }
            else if (cmbPlayer1.SelectedIndex == 0 && cmbPlayer2.SelectedIndex == 1)
            {
                _joey.StartPlay(LearningIntensity.player, Player.PlayerO);
            }
            else if (cmbPlayer1.SelectedIndex == 0 && cmbPlayer2.SelectedIndex == 0 && chkJoeyAsObserver.Checked)
            {
                _joey.StartPlay(LearningIntensity.observer, Player.PlayerX);
            }
            else
            {
                _joey.StartPlay(LearningIntensity.idle, Player.PlayerX);
            }
            _gameBoard.Start(Player.PlayerX);
        }

        private void btnCell_Click(object sender, EventArgs e)
        {
            switch (((Button)sender).Name)
            {
                case "btnA1":
                    _gameBoard.Mark(0, 0);
                    break;
                case "btnA2":
                    _gameBoard.Mark(0, 1);
                    break;
                case "btnA3":
                    _gameBoard.Mark(0, 2);
                    break;
                case "btnB1":
                    _gameBoard.Mark(1, 0);
                    break;
                case "btnB2":
                    _gameBoard.Mark(1, 1);
                    break;
                case "btnB3":
                    _gameBoard.Mark(1, 2);
                    break;
                case "btnC1":
                    _gameBoard.Mark(2, 0);
                    break;
                case "btnC2":
                    _gameBoard.Mark(2, 1);
                    break;
                case "btnC3":
                    _gameBoard.Mark(2, 2);
                    break;
            }
        }

        private void cmbPlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPlayer1.SelectedIndex == 0 && cmbPlayer2.SelectedIndex == 0)
            {
                chkJoeyAsObserver.Checked = true;
                chkJoeyAsObserver.Visible = true;
            }
            else
            {
                chkJoeyAsObserver.Visible = false;
            }
        }

        private void MarkCell(Button cell, Player mark)
        {
            switch (mark)
            {
                case Player.None:
                    cell.Text = String.Empty;
                    break;
                case Player.PlayerO:
                    cell.ForeColor = Color.Green;
                    cell.Text = "O";
                    break;
                case Player.PlayerX:
                    cell.ForeColor = Color.Blue;
                    cell.Text = "X";
                    break;
            }
        }
    }
}
