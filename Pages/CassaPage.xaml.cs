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
    /// Логика взаимодействия для CassaPage.xaml
    /// </summary>
    public partial class CassaPage : Page
    {
        //public AccountClass account = new("dsb", "wregjh", new DateTime(1000, 10, 10), 0, 0);



        public CassaPage()
        {
            InitializeComponent();
            DataContext = MainNavBar.Account;
        }

        private void Deposit_Click(object sender, RoutedEventArgs e)
        {
            Int128 plusDep = 0;
            
            if(Int128.TryParse(DepositAmount.Text, out plusDep))
                if(plusDep > 0)

            MainNavBar.Account.NowBalance += plusDep;
        }

        private void Withdraw_Click(object sender, RoutedEventArgs e)
        {
            Int128 minusDep = 0;

            if (Int128.TryParse(WithdrawAmount.Text, out minusDep))
                if (minusDep > 0)

            if (minusDep > MainNavBar.Account.NowBalance)
                MainNavBar.Account.NowBalance = 0;
            else
                MainNavBar.Account.NowBalance -= minusDep;


        }

    }
}
