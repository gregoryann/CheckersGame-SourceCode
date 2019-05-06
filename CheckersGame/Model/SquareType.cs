namespace CheckersGame.Model
{
    /// <summary>
    /// Represents the state of a square in a game of checkers.
    /// </summary>
    public enum SquareType
    {
        RedKing = 'R',
        BlackKing = 'B',
        RedPiece = 'r',
        BlackPiece = 'b',
        Empty = '_',
        Illegal = '-'
    }
}