using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace eTOM
{
    /// <summary>
    /// Логика взаимодействия для ClientAdd.xaml
    /// </summary>
    public partial class ClientAdd : Window
    {
        private string connectPostgre = String.Format("Server=Localhost;Port=5432;User Id=postgres;password=MmV8qd-+1!;Database=eTOM");
        private NpgsqlConnection connect;
        public ClientAdd()
        {
            InitializeComponent();
            connect = new NpgsqlConnection(connectPostgre);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены, что хотите добавить услугу?", "Услуга добавлена", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //  string channelsVar = "true";
                    // string priceBack = price.Text.Remove(price.Text.LastIndexOf(@","));
                    // string speedBack = speed.Text.Remove(speed.Text.LastIndexOf(@" "));
                    connect.Open();

                    string sql = @"INSERT INTO public." + '\u0022' + "Clients" + '\u0022' + "(name_client, surname, fathername, docnumb, address) VALUES (" + '\u0027' + name.Text + '\u0027' + ", " + '\u0027' + surname.Text + '\u0027' + ", " + '\u0027' + fatherName.Text + '\u0027' + ", " + '\u0027' + docNumb.Text + '\u0027' + ", " + '\u0027' + address.Text + '\u0027' + ");";
                    sql = sql.Replace("Нет", "false");
                    sql = sql.Replace("Да", "true");
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                    cmd.ExecuteNonQuery();
                    connect.Close();
                    MessageBox.Show("Данные добавлены");
                    this.Close();
                }

            }
            catch (Exception ex)
            {
                connect.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            name.Text = string.Empty;
            surname.Text = string.Empty;
            fatherName.Text = string.Empty;
            docNumb.Text = string.Empty;
            address.Text = string.Empty;
        }


        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
