using System;
using System.Windows;
using System.Windows.Controls;

namespace TicTacToewithAI
{
    public partial class MainWindow : Window
    {
        private Button[,] buttons;
        private string[,] board;
        private string currentPlayer = "X";
        private string aiPlayer = "O";
        private string humanPlayer = "X";
        private string difficulty = "Hard";

        public MainWindow()
        {
            InitializeComponent();
            buttons = new Button[3, 3]
            {
                { Button00, Button01, Button02 },
                { Button10, Button11, Button12 },
                { Button20, Button21, Button22 }
            };
            ResetBoard();
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            ResetBoard();
        }

        private void ResetBoard()
        {
            board = new string[3, 3];
            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                {
                    buttons[r, c].Content = "";
                    buttons[r, c].IsEnabled = true;
                }

            currentPlayer = humanPlayer;
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            Button clicked = sender as Button;
            int row = Grid.GetRow(clicked);
            int col = Grid.GetColumn(clicked);

            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (buttons[r, c] == clicked)
                    {
                        MakeMove(r, c, humanPlayer);
                        if (!IsGameOver())
                        {
                            AIMove();
                        }
                        return;
                    }
        }

        private void MakeMove(int row, int col, string player)
        {
            if (board[row, col] == null)
            {
                board[row, col] = player;
                buttons[row, col].Content = player;
                buttons[row, col].IsEnabled = false;
            }
        }

        private void AIMove()
        {
            (int row, int col) move;
            if (difficulty == "Easy")
                move = GetRandomMove();
            else if (difficulty == "Medium" && new Random().NextDouble() < 0.5)
                move = GetRandomMove();
            else
                move = GetBestMove();

            MakeMove(move.row, move.col, aiPlayer);
            IsGameOver();
        }

        private (int, int) GetRandomMove()
        {
            Random rand = new Random();
            while (true)
            {
                int r = rand.Next(3);
                int c = rand.Next(3);
                if (board[r, c] == null)
                    return (r, c);
            }
        }

        private (int, int) GetBestMove()
        {
            int bestScore = int.MinValue;
            (int row, int col) bestMove = (-1, -1);

            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    if (board[r, c] == null)
                    {
                        board[r, c] = aiPlayer;
                        int score = Minimax(board, 0, false);
                        board[r, c] = null;
                        if (score > bestScore)
                        {
                            bestScore = score;
                            bestMove = (r, c);
                        }
                    }
            return bestMove;
        }

        private int Minimax(string[,] board, int depth, bool isMaximizing)
        {
            string result = CheckWinner();
            if (result == aiPlayer) return 10 - depth;
            if (result == humanPlayer) return depth - 10;
            if (IsFull()) return 0;

            if (isMaximizing)
            {
                int maxEval = int.MinValue;
                for (int r = 0; r < 3; r++)
                    for (int c = 0; c < 3; c++)
                        if (board[r, c] == null)
                        {
                            board[r, c] = aiPlayer;
                            int eval = Minimax(board, depth + 1, false);
                            board[r, c] = null;
                            maxEval = Math.Max(maxEval, eval);
                        }
                return maxEval;
            }
            else
            {
                int minEval = int.MaxValue;
                for (int r = 0; r < 3; r++)
                    for (int c = 0; c < 3; c++)
                        if (board[r, c] == null)
                        {
                            board[r, c] = humanPlayer;
                            int eval = Minimax(board, depth + 1, true);
                            board[r, c] = null;
                            minEval = Math.Min(minEval, eval);
                        }
                return minEval;
            }
        }

        private bool IsGameOver()
        {
            string winner = CheckWinner();
            if (winner != null)
            {
                MessageBox.Show($"{winner} wins!");
                DisableAllButtons();
                return true;
            }
            if (IsFull())
            {
                MessageBox.Show("It's a draw!");
                return true;
            }
            return false;
        }

        private string CheckWinner()
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] != null && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                    return board[i, 0];
                if (board[0, i] != null && board[0, i] == board[1, i] && board[1, i] == board[2, i])
                    return board[0, i];
            }
            if (board[0, 0] != null && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                return board[0, 0];
            if (board[0, 2] != null && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                return board[0, 2];
            return null;
        }

        private bool IsFull()
        {
            foreach (var cell in board)
                if (cell == null) return false;
            return true;
        }

        private void DisableAllButtons()
        {
            foreach (var btn in buttons)
                btn.IsEnabled = false;
        }

        private void DifficultyBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var item = DifficultyBox.SelectedItem as ComboBoxItem;
            difficulty = item?.Content.ToString() ?? "Hard";
            ResetBoard();
        }
    }
}
