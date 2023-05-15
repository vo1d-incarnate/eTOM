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

namespace eTOM
{
    /// <summary>
    /// Логика взаимодействия для services.xaml
    /// </summary>
    public partial class services : Page
    {
        public services()
        {
            InitializeComponent();
        }

        private void Marketing_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Marketing());
        }
    }
}
