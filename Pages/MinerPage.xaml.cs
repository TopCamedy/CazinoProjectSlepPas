using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CazinoProjectSlepPas.Pages
{
    /// <summary>
    /// Логика взаимодействия для MinerPage.xaml
    /// </summary>
    public partial class MinerPage : Page
    {
        private double currentBet;
        private double currentMultiplier = 1.0;
        private double currentWin = 0;
        private bool[,] bombLocations = new bool[5, 5];
        private bool gameStarted = false;
        private Random random = new Random();
        private int bombsCount = 5;

        public MinerPage()
        {
            InitializeComponent();
        }

        private void BetButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(DepositTextBox.Text, out double bet) && bet > 0)
            {
                currentBet = bet;
                currentWin = 0;
                StatusText.Text = $"Поставлено: {currentBet}";
                PlayButton.IsEnabled = true;
                CashOutButton.IsEnabled = false;
                gameStarted = false;
                ResetGame();
            }
            else
            {
                MessageBox.Show("Введите корректную сумму для ставки!");
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentBet <= 0)
            {
                MessageBox.Show("Сначала сделайте ставку!");
                return;
            }
            if (MainNavBar.Account.NowBalance < Convert.ToInt64(currentBet))

            {
                MessageBox.Show("Сначала внесите депозит!");
                return;
            }

            InitializeGame();
            gameStarted = true;
            PlayButton.IsEnabled = false;
            CashOutButton.IsEnabled = true;
            StatusText.Text = $"Игра началась! Множитель: {currentMultiplier:F2}x";

            MainNavBar.Account.NowBalance -= Convert.ToInt64(currentBet);
        }

        private void CashOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (!gameStarted || currentWin == 0)
            {
                MessageBox.Show("Нечего забирать!");
                return;
            }

            MessageBox.Show($"Вы забрали {currentWin:F2}!");
            StatusText.Text = $"Вы забрали {currentWin:F2}";
            gameStarted = false;
            PlayButton.IsEnabled = true;
            CashOutButton.IsEnabled = false;
            DisableAllCells();

            MainNavBar.Account.NowBalance += Convert.ToInt64(currentWin);
        }

        private void InitializeGame()
        {
            currentMultiplier = 1.0;
            currentWin = 0;
            ResetGame();

            int bombsPlaced = 0;
            while (bombsPlaced < bombsCount)
            {
                int row = random.Next(0, 5);
                int col = random.Next(0, 5);

                if (!bombLocations[row, col])
                {
                    bombLocations[row, col] = true;
                    bombsPlaced++;
                }
            }
        }

        private void ResetGame()
        {
            var grid = (Panel)Cell00.Parent;
            var buttonBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF42469E"));

            foreach (Button btn in grid.Children)
            {
                btn.Content = "";
                btn.IsEnabled = true;
                btn.Background = buttonBrush;
                btn.Foreground = Brushes.Black; // Устанавливаем черный цвет по умолчанию
            }

            bombLocations = new bool[5, 5];
        }

        private void DisableAllCells()
        {
            var grid = (Panel)Cell00.Parent;
            foreach (Button btn in grid.Children)
            {
                btn.IsEnabled = false;
            }
        }

        private void Cell_Click(object sender, RoutedEventArgs e)
        {
            if (!gameStarted)
            {
                MessageBox.Show("Нажмите 'Играть' чтобы начать!");
                return;
            }

            Button clickedCell = (Button)sender;
            string[] position = clickedCell.Tag.ToString().Split(',');
            int row = int.Parse(position[0]);
            int col = int.Parse(position[1]);

            clickedCell.IsEnabled = false;

            if (bombLocations[row, col])
            {
                clickedCell.Content = "💣";
                clickedCell.Foreground = Brushes.Black; // Черный цвет для бомбы
                clickedCell.Background = Brushes.Red;
                GameOver(false);
            }
            else
            {
                currentWin = currentBet * currentMultiplier;
                clickedCell.Content = $"+{currentWin:F2}";
                clickedCell.Foreground = Brushes.Black; // Черный цвет для выигрыша
                clickedCell.Background = Brushes.LightGreen;

                currentMultiplier += 0.3;
                StatusText.Text = $"Текущий выигрыш: {currentWin:F2} | Множитель: {currentMultiplier:F2}x";

                var grid = (Panel)clickedCell.Parent;
                int activeCells = grid.Children.Cast<Button>().Count(b => b.IsEnabled);
                int remainingSafeCells = activeCells - (bombsCount - CountRevealedBombs());

                if (remainingSafeCells == 0)
                {
                    GameOver(true);
                }
            }
        }

        private int CountRevealedBombs()
        {
            int revealed = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (bombLocations[i, j] && !GetButtonByPosition(i, j).IsEnabled)
                    {
                        revealed++;
                    }
                }
            }
            return revealed;
        }

        private Button GetButtonByPosition(int row, int col)
        {
            string name = $"Cell{row}{col}";
            return (Button)FindName(name);
        }

        private void GameOver(bool isWin)
        {
            gameStarted = false;
            CashOutButton.IsEnabled = false;
            PlayButton.IsEnabled = true;

            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    if (bombLocations[i, j])
                    {
                        var btn = GetButtonByPosition(i, j);
                        btn.Content = "💣";
                        btn.Background = Brushes.Red;
                    }
                }
            }

            if (isWin)
            {
                StatusText.Text = $"Поздравляем! Вы выиграли {currentWin:F2}";
                MessageBox.Show($"Вы победили! Ваш выигрыш: {currentWin:F2}");
            }
            else
            {
                currentWin = 0;
                StatusText.Text = "Игра окончена. Вы проиграли ставку.";
                MessageBox.Show("К сожалению, вы наткнулись на бомбу и потеряли ставку.");
            }
        }
    }
}

