using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
            if (name == null || string.IsNullOrWhiteSpace(name.Text)) { MessageBox.Show("Введите имя"); return; } 
            else if (surname == null || string.IsNullOrWhiteSpace(surname.Text)) { MessageBox.Show("Введите фамилию"); return; }
            else if (address == null || string.IsNullOrWhiteSpace(address.Text)) { MessageBox.Show("Введите адрес"); return; }
            else if (telNumb == null || string.IsNullOrWhiteSpace(telNumb.Text)) { MessageBox.Show("Введите номер телефона"); return; }
            else if (telNumb.Text.Length != Regex.Replace(telNumb.Text, @"[^0-9]", "").Length || telNumb.Text.Length != 10) { MessageBox.Show("Введите корректный номер телефона"); return; }
            else if (docNumb == null || string.IsNullOrWhiteSpace(docNumb.Text)) { MessageBox.Show("Введите номер документа"); return; }
            else if (docNumb.Text.Length != Regex.Replace(docNumb.Text, @"[^0-9]", "").Length || docNumb.Text.Length != 10) { MessageBox.Show("Введите корректный номер документа"); return; }

            try
            {
                if (MessageBox.Show("Вы уверены, что хотите добавить клиента?", "Клиент добавлен", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    connect.Open();

                    string sql = @"INSERT INTO public." + '\u0022' + "Clients" + '\u0022' + "(name_client, surname, fathername, address, telnumb, docnumb) VALUES (" + '\u0027' + name.Text + '\u0027' + ", " + '\u0027' + surname.Text + '\u0027' + ", " + '\u0027' + fatherName.Text + '\u0027' + ", " + '\u0027' + address.Text + '\u0027' + ", " + '\u0027' + telNumb.Text + '\u0027' + ", " + '\u0027' + docNumb.Text + '\u0027' + ");";
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