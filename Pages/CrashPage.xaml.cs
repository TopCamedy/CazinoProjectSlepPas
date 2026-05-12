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
using System.Windows.Threading;

namespace CazinoProjectSlepPas.Pages
{
    /// <summary>
    /// Логика взаимодействия для CrashPage.xaml
    /// </summary>
    public partial class CrashPage : Page
    {
        private double currentMultiplier = -1.0;
        private double betAmount = 0;
        private bool isGameRunning = false;
        private DispatcherTimer gameTimer;
        private Random random = new Random();
        private List<double> multiplierHistory = new List<double>();
        private DateTime gameStartTime;

        private double coefOnHover = 0.1;

        private double coefOnTime = 0.3;

        public CrashPage()
        {
            InitializeComponent();
            DataContext = MainNavBar.Account;
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);
            gameTimer.Tick += GameTimer_Tick;
            HistoryListView.ItemsSource = new List<string>();
        }

        private void BetButton_Click(object sender, RoutedEventArgs e)
        {
            if (double.TryParse(BetAmountTextBox.Text, out double amount) && amount > 0 && amount < double.Parse(MainNavBar.Account.NowBalance.ToString()))
            {
                betAmount = amount;
                StatusText.Text = $"Ставка: {betAmount}";
                StatusText.Foreground = Brushes.White;
            }
            else
            {
                StatusText.Text = "Неверная сумма ставки";
                StatusText.Foreground = Brushes.OrangeRed;
            }
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (betAmount <= 0)
            {
                StatusText.Text = "Сначала сделайте ставку";
                StatusText.Foreground = Brushes.OrangeRed;
                return;
            }

            isGameRunning = true;
            currentMultiplier = 1.0;
            multiplierHistory.Clear();
            gameStartTime = DateTime.Now;

            PlayButton.IsEnabled = false;
            BetButton.IsEnabled = false;
            CashOutButton.IsEnabled = true;

            MainNavBar.Account.NowBalance -= Int128.Parse(betAmount.ToString());
            StatusText.Foreground = Brushes.WhiteSmoke;
            gameTimer.Start();
        }

        private void GameTimer_Tick(object sender, EventArgs e)
        {
            // Увеличиваем множитель
            double increment = 0.001 + (random.NextDouble() * 0.005) * (DateTime.Now - gameStartTime).TotalMilliseconds / 500;
            currentMultiplier += increment;
            multiplierHistory.Add(currentMultiplier);

            // Обновляем интерфейс
            MultiplierText.Text = $"Множитель: {currentMultiplier:F2}x";
            StatusText.Text = $"Выйгрышшшшб: {betAmount * currentMultiplier:F2}x";
            UpdateMultiplierColor();
            DrawGraph();

            // Проверка на краш (10% шанс каждые 0.5 секунд)
            if ((DateTime.Now - gameStartTime).TotalMilliseconds % 500 < 20 &&
                random.NextDouble() < coefOnTime)
            {
                EndGame(false);
            }
        }

        private void CashOutButton_Click(object sender, RoutedEventArgs e)
        {
            if (isGameRunning)
            {
                EndGame(true);
            }
        }

        private void EndGame(bool cashOut)
        {
            gameTimer.Stop();
            isGameRunning = false;

            double winAmount = cashOut ? betAmount * currentMultiplier : 0;
            string result;
            if (cashOut)
            {
                result = $"Выигрыш: {winAmount:F2}";
                MainNavBar.Account.NowBalance += Convert.ToInt64(winAmount);
            }
            else
            {
                result = "Краш!";
                
            }



            // Добавляем в историю
            var history = HistoryListView.ItemsSource as List<string> ?? new List<string>();
            history.Insert(0, $"{DateTime.Now:T} - {result} (x{currentMultiplier:F2})");
            HistoryListView.ItemsSource = history;
            HistoryListView.Items.Refresh();

            // Обновляем статус
            StatusText.Text = result;
            StatusText.Foreground = cashOut ? Brushes.LightGreen : Brushes.OrangeRed;

            // Сбрасываем кнопки
            PlayButton.IsEnabled = true;
            BetButton.IsEnabled = true;
            CashOutButton.IsEnabled = false;
        }

        private void UpdateMultiplierColor()
        {
            if (currentMultiplier > 5.0)
                MultiplierText.Foreground = Brushes.LightGreen;
            else if (currentMultiplier > 3.0)
                MultiplierText.Foreground = Brushes.Yellow;
            else
                MultiplierText.Foreground = Brushes.White;
        }

        private void DrawGraph()
        {
            GraphCanvas.Children.Clear();

            if (multiplierHistory.Count < 2) return;

            double maxMultiplier = Math.Max(10, currentMultiplier * 1.2);
            double xStep = GraphCanvas.ActualWidth / (multiplierHistory.Count - 1);
            double yScale = GraphCanvas.ActualHeight / maxMultiplier;

            for (int i = 1; i < multiplierHistory.Count; i++)
            {
                var line = new Line()
                {
                    X1 = (i - 1) * xStep,
                    Y1 = GraphCanvas.ActualHeight - (multiplierHistory[i - 1] * yScale),
                    X2 = i * xStep,
                    Y2 = GraphCanvas.ActualHeight - (multiplierHistory[i] * yScale),
                    Stroke = Brushes.White,
                    StrokeThickness = 2
                };
                GraphCanvas.Children.Add(line);
            }
        }

        private void CasinoAlwaysWin(object sender, MouseEventArgs e)
        {
            // При наведении 
            if (random.NextDouble() < coefOnHover)
                EndGame(false);
        }
    }
}
