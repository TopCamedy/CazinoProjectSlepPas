using CazinoProjectSlepPas.Pages;
using System.Windows;
using System.Windows.Controls;

namespace CazinoProjectSlepPas
{
    public partial class MainWindow : Window
    {
        public static Frame Frame;

        public MainWindow()
        {
            InitializeComponent();
            Frame = MainFrame;
            MainFrame.Navigate(new LoginPage());
        }
    }
}