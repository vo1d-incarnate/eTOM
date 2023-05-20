using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Логика взаимодействия для ClientEdit.xaml
    /// </summary>
    public partial class ClientEdit : Window
    {
        private string connectPostgre = String.Format("Server=Localhost;Port=5432;User Id=postgres;password=MmV8qd-+1!;Database=eTOM");
        private NpgsqlConnection connect;
        public string idData { get; set; }
        public ClientEdit()
        {
            //  idData = (string)NavigationService.GetNavigationData(this);
            InitializeComponent();

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            connect = new NpgsqlConnection(connectPostgre);
            Data_Upload();

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить услугу?", "Услуга удалена", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    connect.Open();

                    string sql = @"DELETE FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE id = " + idData + ";";
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                    cmd.ExecuteNonQuery();
                    connect.Close();

                    this.Close();
                }

            }
            catch (Exception ex)
            {
                connect.Close();
                MessageBox.Show("Error: " + ex.Message);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            if (name == null || string.IsNullOrWhiteSpace(name.Text)) { MessageBox.Show("Введите имя"); return; }
            else if (surname == null || string.IsNullOrWhiteSpace(surname.Text)) { MessageBox.Show("Введите фамилию"); return; }
            else if (docNumb == null || string.IsNullOrWhiteSpace(docNumb.Text)) { MessageBox.Show("Введите номер документа"); return; }
            else if (address == null || string.IsNullOrWhiteSpace(address.Text)) { MessageBox.Show("Введите адрес"); return; }

            try
            {
                if (MessageBox.Show("Вы уверены, что хотите внести изменения?", "Изменения внесены", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //  string channelsVar = "true";
                    // string priceBack = price.Text.Remove(price.Text.LastIndexOf(@","));
                    // string speedBack = speed.Text.Remove(speed.Text.LastIndexOf(@" "));
                    connect.Open();


                    string sql = @"UPDATE public." + '\u0022' + "Clients" + '\u0022' + "SET name_client=" + '\u0027' + name.Text + '\u0027' + ", surname=" + '\u0027' +surname.Text + '\u0027' + ", fathername=" + '\u0027' + fatherName.Text + '\u0027' + ", docnumb=" + '\u0027' + docNumb.Text + '\u0027' + ", address=" + '\u0027' + address.Text + '\u0027' + " WHERE id = " + idData + ";";
                    sql = sql.Replace("Нет", "false");
                    sql = sql.Replace("Да", "true");
                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                    cmd.ExecuteNonQuery();
                    connect.Close();
                    Data_Upload();
                    MessageBox.Show("Изменения сохранены");
                }

            }
            catch (Exception ex)
            {
                connect.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Data_Upload()
        {
            // MessageBox.Show(idData);
            try
            {

                connect.Open();
                string sql = @"SELECT * FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE id = " + idData + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataTable iDataSet = new DataTable();
                iAdapter.Fill(iDataSet);
                DataRow[] data_row = iDataSet.Select();
                name.Text = data_row[0]["name_client"].ToString();
                surname.Text = data_row[0]["surname"].ToString();
                fatherName.Text = data_row[0]["fathername"].ToString();
                docNumb.Text = data_row[0]["docnumb"].ToString();
                address.Text = data_row[0]["address"].ToString();


                connect.Close();

            }
            catch (Exception ex)
            {
                connect.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }

    }
}
