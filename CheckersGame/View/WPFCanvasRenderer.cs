using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CheckersGame.Model;

namespace CheckersGame.View
{
    public class WpfCanvasRenderer
    {
        public int Width { get; private set; }
        public int Height { get; private set; }

        private System.Windows.Controls.Image _toDrawTo;

        public WpfCanvasRenderer(int width, int height, System.Windows.Controls.Image saveAddress)
        {
            _toDrawTo = saveAddress;
            Width = width;
            Height = height;
        }
        
        public Graphics Render(SquareType[,] board)
        {
            using (var bitmap = new Bitmap(Width, Height))
            {
                using (var canvas = Graphics.FromImage(bitmap))
                {
                    canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    board.Render(canvas).Save();

                    //https://stackoverflow.com/questions/10077498/show-drawing-image-in-wpf
                    var ms = new MemoryStream();
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    ms.Position = 0;
                    var bi = new BitmapImage();
                    bi.BeginInit();
                    bi.StreamSource = ms;
                    bi.EndInit();
                    _toDrawTo.Source = bi;

                    return canvas;
                }
            }
        }
    }

    public static class BoardRenderer
    {
        public static Graphics Render(this SquareType[,] board, Graphics renderTo)
        {
            Dictionary<SquareType, Image> typeToImageMap = null;
            try
            {
                //Either use the Images folder shared by the .exe, or use the VS resource relative to the bin folder
                var directoryChangePrefix = File.Exists("Images\\RedKing.png") ? "" : "..\\..\\";

                typeToImageMap = new Dictionary<SquareType, Image>()
                {
                    {SquareType.RedKing, Image.FromFile(directoryChangePrefix+"Images\\RedKing.png")},
                    {SquareType.RedPiece, Image.FromFile(directoryChangePrefix+"Images\\RedPiece.png")},
                    {SquareType.BlackKing, Image.FromFile(directoryChangePrefix+"Images\\BlackKing.png")},
                    {SquareType.BlackPiece, Image.FromFile(directoryChangePrefix+"Images\\BlackPiece.png")},
                };
                
                for (var row = 0; row < board.GetLength(0); row++)
                {
                    for (var col = 0; col < board.GetLength(1); col++)
                    {
                        if(typeToImageMap.ContainsKey(board[row, col]))
                            renderTo.DrawImage(typeToImageMap[board[row,col]], col * 50 - 2, row *50-2, 46, 46);
                    }
                }

                return renderTo;
            }
            finally
            {
                //Ensure that all the image files are closed so access isn't locked
                if (typeToImageMap != null)
                {
                    foreach (var image in typeToImageMap.Values)
                    {
                        image.Dispose();
                    }
                }
            }
        }
    }
}
