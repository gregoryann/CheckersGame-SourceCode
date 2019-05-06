using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersGame.Model;

namespace CheckersGame.Controller
{
    public class HumanPlayer : IPlayer
    {
        private int fromRowClicked;
        private int fromColClicked;
        private int toRowClicked;
        private int toColClicked;

        public Move GetNextMove(CheckersModel gameModel)
        {
            return new Move(fromRowClicked, fromColClicked, toRowClicked, toColClicked);
        }

        public HumanPlayer(PlayerColour colour)
        {
            this.Colour = colour;
        }

        public PlayerColour Colour { get; private set; }

        public void setPieceTargets(Tuple<int, int> previousClick, Tuple<int, int> currentClick)
        {
            if (previousClick != null)
            {
                fromRowClicked = previousClick.Item1;
                fromColClicked = previousClick.Item2;
            }

            if (currentClick != null)
            {
                toRowClicked = currentClick.Item1;
                toColClicked = currentClick.Item2;
            }
        }
    }
}
