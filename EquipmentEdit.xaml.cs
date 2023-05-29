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
    /// Логика взаимодействия для EquipmentEdit.xaml
    /// </summary>
    public partial class EquipmentEdit : Window
    {
        private string connectPostgre = String.Format("Server=Localhost;Port=5432;User Id=postgres;password=MmV8qd-+1!;Database=eTOM");
        private NpgsqlConnection connect;
        public string idData { get; set; }
        private string rolesLocal;
        public EquipmentEdit(string roles)
        {
            InitializeComponent();
            
            rolesLocal = roles;

            if (roles != "ADMIN")
            {
                delete.Visibility = Visibility.Collapsed;
            }
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
                if (MessageBox.Show("Вы уверены, что хотите удалить клиента?", "Клиент удален", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    connect.Open();

                    string sql = @"DELETE FROM public." + '\u0022' + "Equipments" + '\u0022' + "WHERE id = " + idData + ";";
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





        private void Data_Upload()
        {
            try
            {
                connect.Open();
                string sql = @"SELECT * FROM public." + '\u0022' + "Equipment" + '\u0022' + "WHERE id = " + idData + ";";
                
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataSet iDataSet = new DataSet();
                DataTable iDataTable = new DataTable();
                DataTable iDataTable_out = new DataTable();

                iAdapter.Fill(iDataTable);

                iDataTable_out.Columns.Add("id", typeof(int));
                iDataTable_out.Columns.Add("model", typeof(string));
                iDataTable_out.Columns.Add("status", typeof(string));
                iDataTable_out.Columns.Add("client_id", typeof(string));

                for (int i = 0; i < iDataTable.Rows.Count; i++)
                {
                    iDataTable_out.Rows.Add();
                    iDataTable_out.Rows[i][0] = iDataTable.Rows[i][0];
                    iDataTable_out.Rows[i][1] = iDataTable.Rows[i][1];
                    iDataTable_out.Rows[i][2] = iDataTable.Rows[i][2];
                   
                    if (iDataTable.Rows[i][3] != null && !string.IsNullOrWhiteSpace(iDataTable.Rows[i][3].ToString()) && iDataTable.Rows[i][3].ToString() != "")
                    {
                        string sql1 = @"SELECT id, contractnumb FROM public." + '\u0022' + "Clients" + '\u0022' + " WHERE id=" + '\u0027' + iDataTable.Rows[i][3] + '\u0027' + ";";
                        NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connect);
                        NpgsqlDataAdapter iAdapter1 = new NpgsqlDataAdapter(cmd1);
                        DataTable iDataTable1 = new DataTable();
                        iAdapter1.Fill(iDataTable1);
                        iDataTable_out.Rows[i][3] = iDataTable1.Rows[0][1];
                    }
                }

                DataRow[] data_row = iDataTable_out.Select();
                model.Text = data_row[0]["model"].ToString();

                if (data_row[0]["status"].ToString() == "False")
                {
                    status.IsChecked = false;
                }
                else
                {
                    status.IsChecked = true;
                }
                contractnumber.Text = data_row[0]["client_id"].ToString();

                connect.Close();

            }
            catch (Exception ex)
            {
                connect.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            if (model == null || string.IsNullOrWhiteSpace(model.Text)) { MessageBox.Show("Введите модель"); return; }
            
            
            try
            {
                if (MessageBox.Show("Вы уверены, что хотите внести изменения?", "Изменения внесены", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    //  string channelsVar = "true";
                    // string priceBack = price.Text.Remove(price.Text.LastIndexOf(@","));
                    // string speedBack = speed.Text.Remove(speed.Text.LastIndexOf(@" "));
                    connect.Open();

                    string sql;

                    if (contractnumber != null && !string.IsNullOrWhiteSpace(contractnumber.Text))
                    {
                        string sql1 = @"SELECT id FROM public." + '\u0022' + "Clients" + '\u0022' + " WHERE contractnumb=" + '\u0027' + contractnumber.Text + '\u0027' + ";";
                        NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connect);
                        NpgsqlDataAdapter iAdapter1 = new NpgsqlDataAdapter(cmd1);
                        DataTable iDataTable1 = new DataTable();
                        iAdapter1.Fill(iDataTable1);

                        sql = @"UPDATE public." + '\u0022' + "Equipment" + '\u0022' + "SET model=" + '\u0027' + model.Text + '\u0027' + ", status=" + '\u0027' + true + '\u0027' + ", client_id=" + '\u0027' + iDataTable1.Rows[0][0].ToString() + '\u0027' + " WHERE id = " + idData + ";";
                    }
                    else
                    {
                        sql = @"UPDATE public." + '\u0022' + "Equipment" + '\u0022' + "SET model=" + '\u0027' + model.Text + '\u0027' + ", status=" + '\u0027' + false + '\u0027' + ", client_id=null" + " WHERE id = " + idData + ";";
                    }
                    

                    NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                    cmd.ExecuteNonQuery();
                    connect.Close();
                    Data_Upload();
                    MessageBox.Show("Изменения сохранены");
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
