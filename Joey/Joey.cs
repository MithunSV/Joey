
using System;
using TicTacToe.Model;
namespace TicTacToe.AI
{
    public enum LearningIntensity
    {
        idle,
        observer,
        player,
        learner
    }

    public class Joey
    {
        private const int BOARD_SIZE = 3;

        private Brain _myBrain = null;
        private Board _gameBoard = null;
        private LearningIntensity _learningMode = LearningIntensity.idle;
        private Model.Player _myMark = Model.Player.None;

        public event Action<string> MessageRaised;
        
        private void _gameBoard_TurnChanged()
        {
            if (_learningMode == LearningIntensity.idle || _learningMode == LearningIntensity.observer)
            {
                return;
            }
            if (_gameBoard.Turn != Model.Player.None && ((_gameBoard.Turn == Model.Player.PlayerX && _myBrain.NextTurn == Player.First) ||
                                                         (_gameBoard.Turn == Model.Player.PlayerO && _myBrain.NextTurn == Player.Second)))
            {
                if (_learningMode == LearningIntensity.learner || _gameBoard.Turn == _myMark)
                {
                    int nextStep_rowIndex, nextStep_columnIndex;
                    _myBrain.GetNextBestStep(out nextStep_rowIndex, out nextStep_columnIndex);
                    if (nextStep_rowIndex > -1 && nextStep_columnIndex > -1)
                    {
                        System.Threading.Thread.Sleep(1000);
                        _gameBoard.Mark(nextStep_rowIndex, nextStep_columnIndex);
                    }
                }
            }
        }

        private void _gameBoard_StatusChanged(StatusChangedEventArgs eventArgs)
        {
            if (_learningMode == LearningIntensity.idle)
            {
                return;
            }
            if (eventArgs.NewValue != Model.Player.None)
            {
                if (eventArgs.Row < 0 || eventArgs.Row >= BOARD_SIZE || eventArgs.Column < 0 || eventArgs.Column >= BOARD_SIZE)
                {
                    RaiseMessage("What the hell! What kind of sick joke is this?");
                    return;
                }
                if ((eventArgs.NewValue == Model.Player.PlayerX && _myBrain.NextTurn != Player.First) ||
                    (eventArgs.NewValue == Model.Player.PlayerO && _myBrain.NextTurn != Player.Second))
                {
                    RaiseMessage("Hey that's not fair! That's out of turn.");
                    return;
                }
                if (_myBrain.Status[eventArgs.Row, eventArgs.Column] != Player.None)
                {
                    RaiseMessage("Hey that's not fair! Symbols can't be replaced.");
                    return;
                }

                _myBrain.ChangeStatus(eventArgs.Row, eventArgs.Column);

                if (_myBrain.Winner == Player.None && _myBrain.NextTurn == Player.None)
                {
                    RaiseMessage("Give me 500 bucks and we'll call it even.");
                    return;
                }
                if ((_myBrain.Winner == Player.First && _myMark == Model.Player.PlayerX) ||
                    (_myBrain.Winner == Player.Second && _myMark == Model.Player.PlayerO))
                {
                    RaiseMessage("Ha Ha....You lose SUCKER!!");
                    return;
                }
                if ((_myBrain.Winner == Player.First && _myMark == Model.Player.PlayerO) ||
                    (_myBrain.Winner == Player.Second && _myMark == Model.Player.PlayerX))
                {
                    RaiseMessage("Hey....How YOU Doin! I like your style.");
                    return;
                }
            }
        }

        private void RaiseMessage(string msg)
        {
            if (_learningMode == LearningIntensity.player || _learningMode == LearningIntensity.learner && MessageRaised != null)
            {
                MessageRaised(msg);
            }
        }

        public Joey(Board gameBoard)
        {
            _myBrain = new Brain();
            _gameBoard = gameBoard;
            _gameBoard.StatusChanged += _gameBoard_StatusChanged;
            _gameBoard.TurnChanged += _gameBoard_TurnChanged;
        }

        public void StartPlay(LearningIntensity learningMode, TicTacToe.Model.Player symbol)
        {
            _learningMode = learningMode;
            _myMark = symbol;
            _myBrain.ResetStatus();
        }
    }
}
