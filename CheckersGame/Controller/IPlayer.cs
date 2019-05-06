using System.Security.Cryptography.X509Certificates;
using CheckersGame.Model;

namespace CheckersGame.Controller
{
    public interface IPlayer
    {
        /// <summary>
        /// If the IPlayer needs information, or the ability to look
        /// forward, it needs the game model.
        /// If the player doesn't use that info, you can pass in null
        /// </summary>
        /// <param name="gameModel"></param>
        /// <returns></returns>
        Move GetNextMove(CheckersModel gameModel);
        PlayerColour Colour { get; }
    }
}