using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CheckersGame.Controller;
using CheckersGame.Model;
using CheckersGame.View;

namespace CheckersGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer dispatcherTimer;
        

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            gameController.TryAIMove();
        }

        private GameController gameController;
        private WpfCanvasRenderer gameRenderer;

        private Tuple<int,int> previousClick;
        private Tuple<int, int> currentClick;

        private int nextMoveTimer = 0;

        public MainWindow()
        {
            InitializeComponent();
            initialiseGameData();
        }

        private void initialiseGameData()
        {
            gameRenderer = new WpfCanvasRenderer(400, 400, imgBoardImage);
            gameController = new GameController(updateView, displayWinningMessage);
            updateView();

            //if a timer already exists, stop it and create a new one with the new context

            dispatcherTimer?.Stop();
            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 2);
            dispatcherTimer.Start();
        }

        private void borderImgContainer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var newClick = new Tuple<int, int>(
                    (int)e.GetPosition(borderImgContainer).Y / 50,
                    (int)e.GetPosition(borderImgContainer).X / 50);

            previousClick = currentClick;
            currentClick = newClick;

            gameController.UpdateHumanPlayer(previousClick, currentClick);
            gameController.PromptPlayerForMove();
        }
        
        private void updateView()
        {
            tb_MoveHistory.Text = string.Join("\n", gameController.MoveHistory);
            gameRenderer.Render(gameController.CurrentBoard);
            lbl_PlayerToMove.Content = $"{gameController.CurrentMoveColour} To Move";
            nextMoveTimer = 0;
        }

        private void displayWinningMessage()
        {
            lbl_PlayerToMove.Content = $"{gameController.CurrentMoveColour} Won!";
            dispatcherTimer.Stop();
        }

        private void btn_undo_Click(object sender, RoutedEventArgs e)
        {
            gameController.Undo();
        }

        private void btn_redo_Click(object sender, RoutedEventArgs e)
        {
            gameController.Redo();
        }

        private void btn_reset_Click(object sender, RoutedEventArgs e)
        {
            initialiseGameData();
        }

        private void cb_RedPlayerType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            gameController?.setPlayerTypes(cb_RedPlayerType.SelectedItem.ToString(), cb_BlackPlayerType.SelectedItem.ToString());
        }
    }
}
