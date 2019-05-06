using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace CheckersGame.Model
{
    public class CheckersModel
    {
        private const int rowsPerPlayer = 3;
        public SquareType[,] Board { get; private set; }
        public int BoardWidth { get; private set; }
        public int BoardHeight { get; private set; }

        public CheckersModel(int boardWidth = 8, int boardHeight = 8)
        {
            BoardWidth = boardWidth;
            BoardHeight = boardHeight;
            Board = new SquareType[BoardHeight, BoardWidth];

            for (int row = 0; row < BoardHeight; row++)
            {
                for (int col = 0; col < BoardWidth; col++)
                {
                    Board[row, col] = GetStartingSquareValue(row, col);
                }
            }
        }

        /// <summary>
        /// This private constructor is used by the Clone method so that the board isn't 
        /// populated twice - i.e. once with initial positions and once with the cloned board.
        /// with
        /// </summary>
        /// <param name="BoardToClone"></param>
        private CheckersModel(SquareType[,] BoardToClone)
        {
            BoardWidth = BoardToClone.GetLength(1);
            BoardHeight = BoardToClone.GetLength(0);
            Board = (SquareType[,])BoardToClone.Clone();
        }

        private SquareType GetStartingSquareValue(int row, int col)
        {
            //Shortcircuit the check for any squares that should never be reachable
            if (row % 2 == 1 && col % 2 == 0 ||
                row % 2 == 0 && col % 2 == 1)
                return SquareType.Illegal;

            if (row < rowsPerPlayer) return SquareType.RedPiece;

            if (BoardHeight - row <= rowsPerPlayer) return SquareType.BlackPiece;

            return SquareType.Empty;
        }

        /// <summary>
        /// A deep clone feature so that board states can be tracked and grown as needed by game history and AI
        /// </summary>
        /// <returns></returns>
        public CheckersModel Clone()
        {
            return new CheckersModel(Board);
        }
        
        /// <summary>
        /// A heuristic analysis of the current layout so it can be given a score.
        /// </summary>
        /// <returns></returns>
        public float BoardValue()
        {
            throw new NotImplementedException();
        }

        public List<Move> GetPossibleMoves(PlayerColour playersTurn)
        {
            var pieceMoveMap = new Dictionary<string, List<Move>>();
            var foundMandatoryMove = false;
            for (var row = 0; row < BoardHeight; row++)
            {
                for (var col = 0; col < BoardWidth; col++)
                {
                    var moves = GetMovesForPosition(playersTurn, row, col);

                    if (moves == null)
                        continue;
                    if (moves[0].IsMandatory)
                    {
                        if(!foundMandatoryMove)
                            pieceMoveMap = new Dictionary<string, List<Move>>();
                        foundMandatoryMove = true;
                        pieceMoveMap.Add(row + "-" + col, moves);
                        continue;
                    }   
                    if(!foundMandatoryMove)
                        pieceMoveMap.Add(row + "-" + col, moves);
                }
            }

            //Convert the nice dictionary of per-piece moves into a linear collection. 
            return pieceMoveMap.SelectMany(kvp => kvp.Value).ToList();
        }

        private List<Move> GetMovesForPosition(PlayerColour playersTurn, int row, int col)
        {
            //If it isn't your turn, move on to the next square
            if (!IsPlayersPiece(playersTurn, row, col)) return null;

            var directions = TryGetPossibleMoveDirections(row, col);
            if (directions == null) return null;

            var moves = new List<Move>();
            var foundMandatoryMove = false;
            foreach (var dir in directions)
            {
                var possibleMove = new Move(row, col, row + dir.Item1, col + dir.Item2);
                //Don't use out of bound moves, or moves that would stack a players pieces on top of each other
                if (possibleMove.IsTargetOutOfBounds(BoardHeight, BoardWidth) ||
                   IsPlayersPiece(playersTurn, possibleMove.ToRow, possibleMove.ToCol)) continue;

                //Check if a normal non-capturing move is allowed
                if (Board[possibleMove.ToRow, possibleMove.ToCol] == SquareType.Empty)
                {
                    if (!foundMandatoryMove)
                        moves.Add(possibleMove);
                    continue;
                }

                //once here you must be looking at an enemy, so handle it
                possibleMove = possibleMove.AdvanceInDirection(dir.Item1, dir.Item2);
                if (!possibleMove.IsTargetOutOfBounds(BoardHeight, BoardWidth) &&
                    Board[possibleMove.ToRow, possibleMove.ToCol] == SquareType.Empty)
                {
                    //reset the moves in case they contained non-optional moves
                    if (!foundMandatoryMove) moves = new List<Move>();
                    moves.Add(possibleMove);
                    foundMandatoryMove = true;
                }

                //if(Board[possibleMove.ToRow, possibleMove.ToCol] == SquareType.Empty) continue;
            }
            return moves.Count > 0 ? moves : null;
        }

        #region Group pieces by ally type - again to avoid recalculating and having large nested ifs
        private static readonly SquareType[] redAllies = { SquareType.RedKing, SquareType.RedPiece };
        private static readonly SquareType[] blackAllies = { SquareType.BlackKing, SquareType.BlackPiece };
        #endregion
        private bool IsPlayersPiece(PlayerColour playersTurn, int row, int col)
        {
            var allies = playersTurn == PlayerColour.RedPlayer ? redAllies : blackAllies;
            return allies.Contains(Board[row, col]);
        }

        #region hard-coded shared direction lists to prevent re-creating for every possible move and improve move calculation speed
        private static readonly List<Tuple<int,int>> dirKing = new List<Tuple<int,int>>() {new Tuple<int, int>(-1,-1), new Tuple<int, int>(1, 1), new Tuple<int, int>(1, -1) , new Tuple<int, int>(-1, 1)};
        private static readonly List<Tuple<int, int>> dirRed = new List<Tuple<int, int>>() { new Tuple<int, int>(1, 1), new Tuple<int, int>(1, -1) };
        private static readonly List<Tuple<int, int>> dirBlack = new List<Tuple<int, int>>() { new Tuple<int, int>(-1, -1), new Tuple<int, int>(-1, 1) };
        #endregion
        public List<Tuple<int, int>> TryGetPossibleMoveDirections(int row, int col)
        {
            var piece = Board[row, col];

            switch (piece)
            {
                case SquareType.RedPiece:
                    return dirRed;
                case SquareType.BlackPiece:
                    return dirBlack;
                case SquareType.BlackKing:
                case SquareType.RedKing:
                    return dirKing;
            }

            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int row = 0; row < BoardHeight; row++)
            {
                for (int col = 0; col < BoardWidth; col++)
                {
                    sb.Append((char)Board[row, col] + " ");
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }

        public bool TryMakeMove(PlayerColour player, Move move)
        {
            if (!GetPossibleMoves(player).Contains(move)) return false;

            makeMove(move);
            return true;
        }

        /// <summary>
        /// Implement a given move - includes simple 1 step moves
        /// captures, and king-mes.
        /// As this is a private helper, moves will not be checked 
        /// for legality.
        /// </summary>
        /// <param name="move"></param>
        private void makeMove(Move move)
        {
            var currPiece = Board[move.FromRow, move.FromCol];
            
            //Remove the piece where it is currently located
            Board[move.FromRow, move.FromCol] = SquareType.Empty;
            
            //Remove captured piece if there is one.
            if(move.IsMandatory)
                Board[(move.FromRow + move.ToRow)/2, (move.FromCol + move.ToCol) / 2] = SquareType.Empty;
            
            //King-me logic
            if(currPiece == SquareType.RedPiece && move.ToRow >= BoardHeight-1)
                currPiece = SquareType.RedKing;
            if (currPiece == SquareType.BlackPiece && move.ToRow <= 0)
                currPiece = SquareType.BlackKing;

            //place the (potentially upgraded) piece back on the board
            Board[move.ToRow, move.ToCol] = currPiece;
        }

        public int CountPiecesOfColour(PlayerColour toCount)
        {
            var total = 0;
            for (var row = 0; row < BoardHeight; row++)
            {
                for (var col = 0; col < BoardWidth; col++)
                {
                    if (IsPlayersPiece(toCount, row, col))
                        total++;
                }
            }
            return total;
        }
    }
}