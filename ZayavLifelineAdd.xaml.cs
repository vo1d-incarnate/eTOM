using Npgsql;
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
using System.Windows.Shapes;

namespace eTOM
{
    /// <summary>
    /// Логика взаимодействия для ZayavLifelineAdd.xaml
    /// </summary>
    public partial class ZayavLifelineAdd : Window
    {
        private string connectPostgre = String.Format("Server=Localhost;Port=5432;User Id=postgres;password=MmV8qd-+1!;Database=eTOM");
        private NpgsqlConnection connect;
        private int zayavIdLocal;
        public ZayavLifelineAdd(int zayavId)
        {
            zayavIdLocal = zayavId;
            InitializeComponent();
            connect = new NpgsqlConnection(connectPostgre);
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (comment == null || string.IsNullOrWhiteSpace(comment.Text)) { MessageBox.Show("Введите комментарий"); return; }
            
            try
            {
                connect.Open();
                string sql = @"INSERT INTO public." + '\u0022' + "Zayav_lifeline" + '\u0022' + "(comment, zayav_id) VALUES (" + '\u0027' + comment.Text + '\u0027' + ", " + '\u0027' + zayavIdLocal.ToString() + '\u0027' + ");";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                cmd.ExecuteNonQuery();
                connect.Close();

                MessageBox.Show("Данные добавлены");
                this.Close();
            }
            catch (Exception ex)
            {
                connect.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Clear(object sender, RoutedEventArgs e)
        {
            comment.Text = string.Empty;
        }

        private void Quit(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}