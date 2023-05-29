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
    /// Логика взаимодействия для EquipmentAdd.xaml
    /// </summary>
    public partial class EquipmentAdd : Window
    {
        private string connectPostgre = String.Format("Server=Localhost;Port=5432;User Id=postgres;password=MmV8qd-+1!;Database=eTOM");
        private NpgsqlConnection connect;
        public EquipmentAdd()
        {
            InitializeComponent();
            connect = new NpgsqlConnection(connectPostgre);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            model.Text = string.Empty; 
            contractnumber.Text = string.Empty;
            status.IsChecked = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Text)) { MessageBox.Show("Введите модель"); return; }

            
            try
            {
                if (MessageBox.Show("Вы уверены, что хотите добавить клиента?", "Клиент добавлен", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    connect.Open();

                    string sql;
                    if (contractnumber != null && !string.IsNullOrWhiteSpace(contractnumber.Text))
                    {

                        string sql1 = @"SELECT id FROM public." + '\u0022' + "Clients" + '\u0022' + " WHERE contractnumb=" + '\u0027' + contractnumber.Text + '\u0027' + ";";
                        NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connect);
                        NpgsqlDataAdapter iAdapter1 = new NpgsqlDataAdapter(cmd1);
                        DataTable iDataTable1 = new DataTable();
                        iAdapter1.Fill(iDataTable1);

                        sql = @"INSERT INTO public." + '\u0022' + "Equipment" + '\u0022' + "(model, status, client_id) VALUES (" + '\u0027' + model.Text + '\u0027' + ", true, " + '\u0027' + iDataTable1.Rows[0][0].ToString() + '\u0027' + ");";
                    }
                    else
                    {
                        sql = @"INSERT INTO public." + '\u0022' + "Equipment" + '\u0022' + "(model, status) VALUES (" + '\u0027' + model.Text + '\u0027' + ", true);";
                    }
                    
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
    }
}
