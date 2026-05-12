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

namespace CazinoProjectSlepPas.Pages
{
    /// <summary>
    /// Логика взаимодействия для MainNavBar.xaml
    /// </summary>
    public partial class MainNavBar : Page
    {
        public static User Account;

        public MainNavBar()
        {
            DataContext = Account;
            InitializeComponent();
            MainContentFrame.Navigate(new MainUsePage());

        }

        private void LogoNavBtn(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new MainUsePage());
        }

        private void CassaNavBtn(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new CassaPage());
        }

        private void ProfileNavBtn(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new ProfilePage());
        }

        private void Rullet1NavBtn(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new MinerPage());
        }

        private void Rullet2NavBtn(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new CrashPage());
        }

        private void Rullet3NavBtn(object sender, RoutedEventArgs e)
        {

        }

        private void HomeNavBtn(object sender, RoutedEventArgs e)
        {
            MainContentFrame.Navigate(new MainUsePage());
        }
    }

}
