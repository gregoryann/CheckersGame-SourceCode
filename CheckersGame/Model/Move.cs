using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersGame.Model
{
    /// <summary>
    /// A single move for any 2d board game.
    /// </summary>
    public struct Move
    {
        public int FromRow, FromCol, ToRow, ToCol;
        public Move(int fromRow, int fromCol, int toRow, int toCol)
        {
            FromRow = fromRow;
            FromCol = fromCol;
            ToRow = toRow;
            ToCol = toCol;
        }

        /// <summary>
        /// Keeps the current move's From details, but adds an extra direction step to the existing To details
        /// </summary>
        /// <param name="rowDirection"></param>
        /// <param name="colDirection"></param>
        /// <returns></returns>
        public Move AdvanceInDirection(int rowDirection, int colDirection)
        {
            return new Move(FromRow, FromCol, ToRow + rowDirection, ToCol + colDirection);
        }
        
        public bool IsMandatory => Math.Abs(FromCol - ToCol) > 1;

        public static readonly Move Empty = new Move(-1, -1, -1, -1);

        private const string rowLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public override string ToString()
        {
            return $"({rowLetters[FromRow]},{FromCol+1}) -> ({rowLetters[ToRow]},{ToCol+1})";
        }

        public bool IsTargetOutOfBounds(int totalRows, int totalCols)
        {
            return ToRow < 0 || ToCol < 0 || ToRow >= totalRows || ToCol >= totalCols;
        }
    }
}