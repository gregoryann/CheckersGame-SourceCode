using System;
using System.Collections.Generic;
using System.Resources;
using System.Windows.Documents;
using CheckersGame.Model;
using CheckersGame.View;

namespace CheckersGame.Controller
{
    public class GameController
    {
        public List<Move> MoveHistory;
        private CheckersModel model;
        public SquareType[,] CurrentBoard => model.Board;
        private Action UIUpdateActionOnLegalMove;
        private Action UIUpdateActionOnWinningMove;

        private IPlayer player1;
        private IPlayer player2;
        private IPlayer currentPlayer;

        public string CurrentMoveColour => currentPlayer.Colour == PlayerColour.RedPlayer ? "Red" : "Black";
        
        public GameController(Action uiUpdateActionOnLegalMove, Action uiUpdateActionOnWinningMove)
        {
            player1 = new HumanPlayer(PlayerColour.RedPlayer);
            //player1 = new AIPlayer(PlayerColour.RedPlayer);
            player2 = new HumanPlayer(PlayerColour.BlackPlayer);
            //player2 = new AIPlayer(PlayerColour.BlackPlayer);
            Reset();
            this.UIUpdateActionOnLegalMove = uiUpdateActionOnLegalMove;
            this.UIUpdateActionOnWinningMove = uiUpdateActionOnWinningMove;
        }

        public void Reset(List<Move> playUpTo = null)
        {
            model = new CheckersModel();
            currentPlayer = player1;
            MoveHistory = new List<Move>();

            //No moves to play? Then skip out early
            if (playUpTo == null) return;

            foreach (var move in playUpTo)
            {
                makeMove(move);
            }
        }

        public void PromptPlayerForMove()
        {
            var move = currentPlayer.GetNextMove(model);
            makeMove(move);
        }

        private void makeMove(Move move)
        {
            if (model.TryMakeMove(currentPlayer.Colour, move))
            {
                currentPlayer = player1 == currentPlayer ? player2 : player1;
                MoveHistory.Add(move);
                UIUpdateActionOnLegalMove();

                CheckForGameCompletion();
            }
        }

        private void CheckForGameCompletion()
        {
            if (model.GetPossibleMoves(currentPlayer.Colour).Count == 0)
            {
                //Toggle the player so that it is the winners turn.
                currentPlayer = player1 == currentPlayer ? player2 : player1;
                UIUpdateActionOnWinningMove();
            }
        }

        public void UpdateHumanPlayer(Tuple<int,int> previousClick, Tuple<int,int> currentClick)
        {
            if (previousClick == null || currentClick == null)
                //TODO - consider adding a parameter that will draw something on the GUI if the initial click is on a piece that player owns... 
                return;

            //is the current player human?
            var asHuman = currentPlayer as HumanPlayer;
            if (asHuman != null)
            {
                 asHuman.setPieceTargets(previousClick, currentClick);
            }
        }

        #region Undo/Redo logic
        private readonly Stack<Move> undoneMoves = new Stack<Move>();
        /// <summary>
        /// Remove the last move from move history, and replay the 
        /// game from scratch to get the last known good states.
        /// This is much more efficient than try to reverse a single move -
        /// consider undoing a capture where you don't know if a 
        /// plain piece or king was taken.
        /// </summary>
        public void Undo()
        {
            if (MoveHistory.Count == 0) return;

            undoneMoves.Push(MoveHistory[MoveHistory.Count-1]);
            MoveHistory.RemoveAt(MoveHistory.Count-1);
            Reset(MoveHistory);
            UIUpdateActionOnLegalMove();
        }

        /// <summary>
        /// This takes moves off the undoneMoves stack and tries to 
        /// make them again. If a redo move isn't legal it will still
        /// be removed from the list, but it won't get available later.
        /// </summary>
        public void Redo()
        {
            if (undoneMoves.Count == 0) return;

            var moveToMake = undoneMoves.Pop();
            makeMove(moveToMake);
        }
        #endregion

        /// <summary>
        /// If the current player is an AI, try to get it to make a move.
        /// </summary>
        public void TryAIMove()
        {
            var asAI = currentPlayer;
            if (asAI != null)
                PromptPlayerForMove();
        }

        public void setPlayerTypes(string redPlayer, string blackPlayer)
        {
            player1 = redPlayer == "Human"
                      ? new HumanPlayer(PlayerColour.RedPlayer) as IPlayer
                      : new AIPlayer(PlayerColour.RedPlayer) as IPlayer;
            player2 = blackPlayer == "Human"
                      ? new HumanPlayer(PlayerColour.BlackPlayer) as IPlayer
                      : new AIPlayer(PlayerColour.BlackPlayer) as IPlayer;
            currentPlayer = currentPlayer.Colour == PlayerColour.RedPlayer ? player1 : player2;
        }
    }
}