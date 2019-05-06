using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CheckersGame.Model;

namespace CheckersGame.Controller
{
    public class AIPlayer : IPlayer
    {
        public PlayerColour Colour { get; private set; }

        public AIPlayer(PlayerColour playerColour)
        {
            Colour = playerColour;
        }

        public Move GetNextMove(CheckersModel gameModel)
        {
            return miniMax(gameModel);
        }

        private Move miniMax(CheckersModel gameModel, int depth = 5)
        {
            var bestScore = double.MinValue;
            var bestMove = Move.Empty;
            foreach (var move in shuffle(gameModel.GetPossibleMoves(Colour)))
            {
                CheckersModel clone = gameModel.Clone();
                clone.TryMakeMove(Colour, move);
                    
                var score = minValue(clone, depth-1);
                if (score >= bestScore)
                {
                    bestMove = move;
                    bestScore = score;
                }
            }
            return bestMove;
        }

        private double minValue(CheckersModel gameClone, int depth)
        {
            var moves = shuffle(gameClone.GetPossibleMoves(Colour.MyEnemy()));

            if (moves.Count == 0)
                return double.MinValue;
            if (depth == 0)
                return gameClone.CountPiecesOfColour(Colour) - gameClone.CountPiecesOfColour(Colour.MyEnemy());
            
            var minFound = double.MaxValue;
            foreach (var move in moves)
            {
                CheckersModel clone = gameClone.Clone();
                if(!clone.TryMakeMove(Colour.MyEnemy(), move))
                    Console.WriteLine("YOU FUCKED UP");
                minFound = Math.Min(minFound, maxValue(clone, depth - 1));
            }
            return minFound;
        }

        private double maxValue(CheckersModel gameClone, int depth)
        {
            var moves = shuffle(gameClone.GetPossibleMoves(Colour));

            if (moves.Count == 0)
                return double.MinValue;
            if (depth == 0)
                return gameClone.CountPiecesOfColour(Colour) - gameClone.CountPiecesOfColour(Colour.MyEnemy());

            var maxFound = double.MinValue;
            foreach (var move in moves)
            {
                CheckersModel clone = gameClone.Clone();
                if(!clone.TryMakeMove(Colour, move))
                    Console.WriteLine("YOU ALSO FUCKED UP");
                maxFound = Math.Max(maxFound, minValue(clone, depth - 1));
            }
            return maxFound;
        }

        private static Random rng = new Random();
        /// <summary>
        /// Shuffles using a ficher yates shuffle. Used to ensure that given 2 moves of equal value
        /// The chosen one will be not always be the first in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toShuffle"></param>
        /// <returns></returns>
        private static List<T> shuffle<T>(List<T> toShuffle)
        {
            for (int i = 0; i < toShuffle.Count; i++)
            {
                var newIndex = rng.Next(i, toShuffle.Count);
                var temp = toShuffle[i];
                toShuffle[i] = toShuffle[newIndex];
                toShuffle[newIndex] = temp;
            }
            return toShuffle;
        }
    }
    
}
