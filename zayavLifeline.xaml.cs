using Npgsql;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Логика взаимодействия для Page1.xaml
    /// </summary>
    public partial class zayavLifeline : Page
    {
        private string connectPostgre = String.Format("Server=Localhost;Port=5432;User Id=postgres;password=MmV8qd-+1!;Database=eTOM");
        private NpgsqlConnection connecting;
        private int zayavIdLocal;
        public zayavLifeline(int zayavId)
        {
            zayavIdLocal = zayavId;
            InitializeComponent();
        }
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            connecting = new NpgsqlConnection(connectPostgre);
            ZayavLifeline_table();
            ZayavInfo();
        }

        private void ZayavInfo()
        {
            try
            {
                connecting.Open();
                string sql = @"SELECT id, service_id, comment, created_at, user_id, client_id FROM public." + '\u0022' + "Zayavki" + '\u0022' + " WHERE id=" + '\u0027' + zayavIdLocal + '\u0027' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataTable iDataSet = new DataTable();
                iAdapter.Fill(iDataSet);

                if (iDataSet.Rows[0][1] == null)
                {
                    tarif.Text = "Тариф не выбран";
                }
                else {
                    string sql1 = @"SELECT id, serv_name FROM public." + '\u0022' + "Services" + '\u0022' + " WHERE id=" + '\u0027' + iDataSet.Rows[0][1].ToString() + '\u0027' + ";";
                    NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connecting);
                    NpgsqlDataAdapter iAdapter1 = new NpgsqlDataAdapter(cmd1);
                    DataTable iDataSet1 = new DataTable();
                    iAdapter1.Fill(iDataSet1);
                    

                    tarif.Text = "Тариф " + iDataSet1.Rows[0][1].ToString();
                }
                string sql2 = @"SELECT login FROM public." + '\u0022' + "User_login" + '\u0022' + " WHERE id=" + '\u0027' + iDataSet.Rows[0][4].ToString() + '\u0027' + ";";
                NpgsqlCommand cmd2 = new NpgsqlCommand(sql2, connecting);
                NpgsqlDataAdapter iAdapter2 = new NpgsqlDataAdapter(cmd2);
                DataTable iDataSet2 = new DataTable();
                iAdapter2.Fill(iDataSet2);

                string sql3 = @"SELECT contractnumb FROM public." + '\u0022' + "Clients" + '\u0022' + " WHERE id=" + '\u0027' + iDataSet.Rows[0][5].ToString() + '\u0027' + ";";
                NpgsqlCommand cmd3 = new NpgsqlCommand(sql3, connecting);
                NpgsqlDataAdapter iAdapter3 = new NpgsqlDataAdapter(cmd3);
                DataTable iDataSet3 = new DataTable();
                iAdapter3.Fill(iDataSet3);



                contractnumb.Text = "Договор №" + iDataSet3.Rows[0][0].ToString();
                user.Text = iDataSet2.Rows[0][0].ToString();
                zayavNumb.Text = "Заявка №" + iDataSet.Rows[0][0].ToString();
                comment.Text = iDataSet.Rows[0][2].ToString();
                createdAt.Text = iDataSet.Rows[0][3].ToString();


                connecting.Close();


            } catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show("Error" + ex.Message);
            }
        }

        private void ZayavLifeline_table()
        {
            try
            {
                connecting.Open();
                string sql = @"SELECT id, comment, created_at FROM public." + '\u0022' + "Zayav_lifeline" + '\u0022' + " WHERE zayav_id=" + '\u0027' + zayavIdLocal + '\u0027' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataSet iDataSet = new DataSet();
                iAdapter.Fill(iDataSet, "Zayav_lifeline");

                connecting.Close();

                Zayav_lifeline.IsReadOnly = true;
                Zayav_lifeline.DataContext = iDataSet;

            }
            catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show("Error" + ex.Message);
            }
        }

        private void Reload_page(object sender, RoutedEventArgs e)
        {
            ZayavLifeline_table();
        }

        private void ZayavLifelineAdd_click(object sender, RoutedEventArgs e)
        {
            ZayavLifelineAdd zayavLifelineAdd = new ZayavLifelineAdd(zayavIdLocal);
            zayavLifelineAdd.Show();
        }
    }
}
