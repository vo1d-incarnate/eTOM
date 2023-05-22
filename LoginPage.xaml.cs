using Npgsql;
using Syncfusion.Windows.Shared;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eTOM
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        private string connectPostgre = String.Format("Server=Localhost;Port=5432;User Id=postgres;password=MmV8qd-+1!;Database=eTOM");
        private NpgsqlConnection connect;

        public LoginPage()
        {
            InitializeComponent();
            connect = new NpgsqlConnection(connectPostgre);
        }

        private void logIn(object sender, RoutedEventArgs e)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Text)) { MessageBox.Show("Введите имя"); return; }
            else if (password == null || string.IsNullOrWhiteSpace(password.Text)) { MessageBox.Show("Введите фамилию"); return; }

            try
            {
                connect.Open();
                string sql = @"SELECT * FROM public." + '\u0022' + "User_login" + '\u0022' + " WHERE " + "login=" + '\u0027' + login.Text + '\u0027' + " AND " + "password=" + '\u0027' + password.Text + '\u0027' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                cmd.ExecuteNonQuery();
                
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataTable iDataSet = new DataTable();
                iAdapter.Fill(iDataSet);
                connect.Close();


                if (iDataSet.Rows.Count != 0)
                {
                    MessageBox.Show("Успешно");

                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Window.GetWindow(this).Close();
                    

                    /*
                    connect.Open();
                    string sql1 = @"SELECT FROM public." + '\u0022' + "user_roles" + '\u0022' + "WHERE" + "user_id=" + iDataSet.Rows[0][0].ToString() + ");";
                    NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connect);
                    cmd.ExecuteNonQuery();

                    NpgsqlDataAdapter iAdapter1 = new NpgsqlDataAdapter(cmd1);
                    DataTable iDataSet1 = new DataTable();
                    iAdapter1.Fill(iDataSet1);

                    
                    string sql2 = @"SELECT FROM public." + '\u0022' + "Roles" + '\u0022' + "WHERE" + "id=" + iDataSet1.Rows[0][2].ToString() + ");";
                    NpgsqlCommand cmd2 = new NpgsqlCommand(sql2, connect);
                    cmd.ExecuteNonQuery();

                    NpgsqlDataAdapter iAdapter2 = new NpgsqlDataAdapter(cmd2);
                    DataTable iDataSet2 = new DataTable();
                    iAdapter1.Fill(iDataSet2);



                    connect.Close();


                    MainWindow mainWindow = new MainWindow((string)iDataSet2.Rows[0][1]);
                    mainWindow.Show();
                    this.Visibility = Visibility.Collapsed;*/
                } else
                {
                    MessageBox.Show("Неправильный логин или пароль");
                }

                //MessageBox.Show("");

            } catch (Exception ex)
            {
                connect.Close();
                MessageBox.Show("Error: " + ex.Message);
            }

        }


    }
}
