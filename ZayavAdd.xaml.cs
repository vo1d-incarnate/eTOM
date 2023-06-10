using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Shapes;

namespace eTOM
{
    /// <summary>
    /// Логика взаимодействия для ZayavAdd.xaml
    /// </summary>
    public partial class ZayavAdd : Window
    {
        private string connectPostgre = String.Format("Server=Localhost;Port=5432;User Id=postgres;password=MmV8qd-+1!;Database=eTOM"); // Подключение к бд
        private NpgsqlConnection connect;
        private int userIdLocal;
        public ZayavAdd(int userId)
        {
            userIdLocal = userId;
            connect = new NpgsqlConnection(connectPostgre);
            InitializeComponent();

            try
            {
                connect.Open();
                string sql = @"SELECT serv_name FROM public." + '\u0022' + "Services" + '\u0022' + ";"; // Строка SQL запроса
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                cmd.ExecuteNonQuery();
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataTable iDataTable = new DataTable();
                iAdapter.Fill(iDataTable);
                
                string[] comboboxItems = new string[iDataTable.Rows.Count];
                for (int i = 0; i < iDataTable.Rows.Count; i++)
                {
                    comboboxItems[i] = iDataTable.Rows[i][0].ToString();
                }
                
                service_choose.ItemsSource = comboboxItems;

                string sql1 = @"SELECT contractnumb FROM public." + '\u0022' + "Clients" + '\u0022' + ";"; // Строка SQL запроса
                NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connect);
                cmd1.ExecuteNonQuery();
                NpgsqlDataAdapter iAdapter1 = new NpgsqlDataAdapter(cmd1);
                DataTable iDataTable1 = new DataTable();
                iAdapter1.Fill(iDataTable1);

                string[] comboboxItems1 = new string[iDataTable1.Rows.Count];
                for (int i = 0; i < iDataTable1.Rows.Count; i++)
                {
                    comboboxItems1[i] = iDataTable1.Rows[i][0].ToString();
                }

                client_choose.ItemsSource = comboboxItems1;

                connect.Close();

            } catch (Exception ex)
            {
                connect.Close();
                MessageBox.Show(ex.Message);
            }

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (comment.Text == null || string.IsNullOrWhiteSpace(comment.Text)) { MessageBox.Show("Введите комментарий"); return; }
            else if (client_choose.Text == null || string.IsNullOrWhiteSpace(client_choose.Text)) { MessageBox.Show("Выберите клиента"); return; }

            try
            {
                connect.Open();

                string sql = @"SELECT id FROM public." + '\u0022' + "Services" + '\u0022' + " WHERE " + "serv_name=" + '\u0027' + service_choose.Text +'\u0027' + ";"; // Строка SQL запроса
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                cmd.ExecuteNonQuery();
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataTable iDataTable = new DataTable();
                iAdapter.Fill(iDataTable);

                string sql1 = @"SELECT id FROM public." + '\u0022' + "Clients" + '\u0022' + " WHERE " + "contractnumb=" + '\u0027' + client_choose.Text + '\u0027' + ";"; // Строка SQL запроса
                NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connect);
                cmd1.ExecuteNonQuery();
                NpgsqlDataAdapter iAdapter1 = new NpgsqlDataAdapter(cmd1);
                DataTable iDataTable1 = new DataTable();
                iAdapter1.Fill(iDataTable1);

                string sql2 = @"INSERT INTO public." + '\u0022' + "Zayavki" + '\u0022' + "(service_id, comment, user_id, client_id) VALUES (" + '\u0027' + iDataTable.Rows[0][0].ToString() + '\u0027' + ", " + '\u0027' + comment.Text + '\u0027' + ", " + '\u0027' + userIdLocal.ToString() + '\u0027' + ", " + '\u0027' + iDataTable1.Rows[0][0].ToString() + '\u0027' + ");"; // Строка SQL запроса
                NpgsqlCommand cmd2 = new NpgsqlCommand(sql2, connect);
                cmd2.ExecuteNonQuery();
                
                connect.Close();
                MessageBox.Show("Данные добавлены");
                this.Close();
            } catch (Exception ex)
            {
                connect.Close();
                MessageBox.Show(ex.Message);
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            service_choose.Text = null;
            client_choose.Text = null;
            comment.Text = string.Empty;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}