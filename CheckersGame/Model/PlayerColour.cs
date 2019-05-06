namespace CheckersGame.Model
{
    public enum PlayerColour
    {
        RedPlayer,
        BlackPlayer
    }

    public static class PlayerColourHelper
    {
        public static PlayerColour MyEnemy(this PlayerColour currentColour)
        {
            return currentColour == PlayerColour.BlackPlayer ? PlayerColour.RedPlayer : PlayerColour.BlackPlayer;
        }
    }
}