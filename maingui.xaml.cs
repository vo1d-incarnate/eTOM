using Npgsql;
using Syncfusion.Windows.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


using Excel = Microsoft.Office.Interop.Excel;

namespace eTOM
{
    /// <summary>
    /// Логика взаимодействия для maingui.xaml
    /// </summary>
    public partial class maingui : Page
    {

        private string connectPostgre = String.Format("Server=Localhost;Port=5432;User Id=postgres;password=MmV8qd-+1!;Database=eTOM");
        private NpgsqlConnection connecting;
        private string rolesLocal;
        private int userIdLocal;
        

        public maingui(string roles, int userId)
        {
            rolesLocal = roles;
            userIdLocal = userId;
            MessageBox.Show("Вы вошли с правами: " + roles);
            Console.WriteLine(userId.ToString());

            InitializeComponent();
            connecting = new NpgsqlConnection(connectPostgre);
            LoadZayav();
            Equipment_table();

            registration.Content = new RegistrationPage();

            if (rolesLocal != "ADMIN")
            {
                otchet.Visibility = Visibility.Collapsed;
                otchet1.Visibility = Visibility.Collapsed;
            }
            


        }


        private void LoadZayav()
        {
            try
            {
                Zayavki.Items.Clear();
                connecting.Open();
                string sql = @"SELECT * FROM public." + '\u0022' + "Zayavki" + '\u0022' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                cmd.ExecuteNonQuery();

                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataTable iDataSet = new DataTable();
                iAdapter.Fill(iDataSet);
                connecting.Close();

                for (int i = 0; i < iDataSet.Rows.Count; i++)
                {
                    int j = (int)iDataSet.Rows[i][0];
                    Frame frame = new Frame();
                    frame.Navigate(new zayavLifeline(j));
                    Zayavki.Items.Add(new TabItem
                    {
                        Header = new TextBlock { Text = "Заявка №" + j.ToString() }, // установка заголовка вкладки

                        Content = frame // установка содержимого вкладки
                    });
                }
            }
            catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show("Error" + ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            connecting = new NpgsqlConnection(connectPostgre);
            Clients_table();
        }

        private void Clients_table()
        {
            try
            {
                connecting.Open();
                string sql = @"SELECT id, name_client, surname, fathername, contractnumb, address
	FROM public." + '\u0022' + "Clients" + '\u0022' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataSet iDataSet = new DataSet();
                iAdapter.Fill(iDataSet, "Clients");
                connecting.Close();
                //     (services.Columns[4] as DataGridTextColumn).Binding.StringFormat = "dd.MM.yyyy";
                clients.IsReadOnly = true;
                clients.DataContext = iDataSet;

                
                connecting.Close();

            }
            catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show("Error" + ex.Message);
            }
        }

        private void Equipment_table()
        {
            try
            {
                connecting.Open();
                string sql = @"SELECT id, model, status, client_id FROM public." + '\u0022' + "Equipment" + '\u0022' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
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

                    if ((bool)iDataTable.Rows[i][2] == false)
                    {
                        iDataTable_out.Rows[i][2] = "На складе";
                    }
                    else
                    {
                        iDataTable_out.Rows[i][2] = "У клиента";
                    }
                    if (iDataTable.Rows[i][3] != null && !string.IsNullOrWhiteSpace(iDataTable.Rows[i][3].ToString()) && iDataTable.Rows[i][3].ToString() != "")
                    {
                        string sql1 = @"SELECT id, contractnumb FROM public." + '\u0022' + "Clients" + '\u0022' + " WHERE id=" + '\u0027' + iDataTable.Rows[i][3] + '\u0027' + ";";
                        NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connecting);
                        NpgsqlDataAdapter iAdapter1 = new NpgsqlDataAdapter(cmd1);
                        DataTable iDataTable1 = new DataTable();
                        iAdapter1.Fill(iDataTable1);
                        iDataTable_out.Rows[i][3] = iDataTable1.Rows[0][1];
                    }
                }

                iDataTable_out.TableName = "Equipment"; 
                iDataSet.Tables.Add(iDataTable_out);


                equipment.ItemsSource = iDataSet.Tables[0].DefaultView;
                equipment.IsReadOnly = true;
                equipment.DataContext = iDataSet;

                connecting.Close();

            }
            catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show("Error" + ex.Message);
            }
        }

        private void Reload_page(object sender, RoutedEventArgs e)
        {
            Clients_table();
        }

        private void Reload_zayav(object sender, RoutedEventArgs e)
        {
            LoadZayav();
        }
        private void Reload_equipment(object sender, RoutedEventArgs e)
        {
            Equipment_table();
        }

        private void ClientAdd_click(object sender, RoutedEventArgs e)
        {
            ClientAdd clientAdd = new ClientAdd();
            clientAdd.Show();
        }

        private void EquipmentAdd_click(object sender, RoutedEventArgs e)
        {
            EquipmentAdd equipmentAdd = new EquipmentAdd();
            equipmentAdd.Show();
        }

        private void Client_edit_click(object sender, RoutedEventArgs e)
        {

            DataRowView rowView = clients.SelectedValue as DataRowView;
            ClientEdit clientEdit = new ClientEdit(rolesLocal);
            string idData = rowView[0].ToString();
            clientEdit.idData = idData;
            //   serv_edit.test.Text += rowView[0].ToString();
            clientEdit.Show();

        }

        private void Equipment_edit_click(object sender, RoutedEventArgs e)
        {
            DataRowView rowView = equipment.SelectedValue as DataRowView;
            EquipmentEdit equipmentEdit = new EquipmentEdit(rolesLocal);
            string idData = rowView[0].ToString();
            equipmentEdit.idData = idData;
            equipmentEdit.Show();
        }


        

        private void Client_excel(object sender, RoutedEventArgs e)
        {
            try
            {
                connecting.Open();
                string sql = @"SELECT name_client, surname, fathername, docnumb, address, telnumb, contractnumb, balance FROM public." + '\u0022' + "Clients" + '\u0022' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataSet iDataSet = new DataSet();
                iAdapter.Fill(iDataSet, "Clients");
                //       services.IsReadOnly = true;
                //     services.DataContext = iDataSet;

                connecting.Close();


                DataTable ct = iDataSet.Tables[0];

                Excel.Application objExcel = new Excel.Application();
                Excel.Workbook workbook = objExcel.Workbooks.Add();
                Excel.Worksheet sheet = workbook.ActiveSheet;
                sheet.Cells[1, 1] = "Имя";
                sheet.Cells[1, 2] = "Фамилия";
                sheet.Cells[1, 3] = "Отчество";
                sheet.Cells[1, 4] = "Номер документа";
                sheet.Cells[1, 5] = "Адрес";
                sheet.Cells[1, 6] = "Номер телефона";
                sheet.Cells[1, 7] = "Номер договора";
                sheet.Cells[1, 8] = "Баланс";


                Excel.Range range = sheet.Range[sheet.Cells[2, 1], sheet.Cells[ct.Rows.Count, ct.Columns.Count]];

                for (int i = 0; i < ct.Rows.Count; ++i)
                    for (int j = 0; j < ct.Columns.Count; ++j)
                    {
                        range.Cells[1 + i, 1 + j] = ct.Rows[i][j].ToString();
                    }

                sheet.Cells.EntireColumn.AutoFit();
                sheet.Cells.EntireRow.AutoFit();
                sheet.PageSetup.Orientation = Excel.XlPageOrientation.xlLandscape;
                sheet.PageSetup.Zoom = false;
                sheet.PageSetup.FitToPagesWide = 1;
                sheet.PageSetup.FitToPagesTall = false;
                sheet.PageSetup.ScaleWithDocHeaderFooter = true;
                sheet.PageSetup.AlignMarginsHeaderFooter = true;
                range = sheet.Range["A1", "X1"];
                range.Font.Bold = true;
                sheet.Range["A1", "X1"].HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                objExcel.Visible = true;
            }

            catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show("Error" + ex.Message);
            }

        }

        private void Equipment_excel(object sender, RoutedEventArgs e)
        {

        }


        private void findClient (object sender, RoutedEventArgs e)
        {
            try
            {
                //string searchParamBack = searchParam.Text.Remove(searchParam.Text.LastIndexOf(@" "));
                if (searchParam.Text == null || string.IsNullOrWhiteSpace(searchParam.Text)) { MessageBox.Show("Выберите поле для поиска"); return; }
                else  if (searchText.Text == null || string.IsNullOrWhiteSpace(searchText.Text)) { MessageBox.Show("Введите данные для поиска"); return; }


                connecting.Open();

                string sql = null;
                switch (searchParam.Text)
                {
                    case "Имя":
                        sql = @"SELECT * FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE name_client = " + '\u0027' + searchText.Text + '\u0027' + ";";
                        break;
                    case "Фамилия":
                        sql = @"SELECT * FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE surname = " + '\u0027' + searchText.Text + '\u0027' + ";";
                        break;
                    case "Отчество":
                        sql = @"SELECT * FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE fathername = " + '\u0027' + searchText.Text + '\u0027' + ";";
                        break;
                    case "Номер договора":
                        sql = @"SELECT * FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE contractnumb = " + '\u0027' + searchText.Text + '\u0027' + ";";
                        break;
                    case "Адрес":
                        sql = @"SELECT * FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE address = " + '\u0027' + searchText.Text + '\u0027' + ";";
                        break;
                }


                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataSet iDataSet = new DataSet();
                iAdapter.Fill(iDataSet, "Clients");

                clients.IsReadOnly = true;
                clients.DataContext = iDataSet;

                connecting.Close();
            } catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show("Error" + ex.Message);
            }
        }


        private void findEquipment(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (searchParam_equipment.Text == null || string.IsNullOrWhiteSpace(searchParam_equipment.Text)) { MessageBox.Show("Выберите поле для поиска"); return; }
                else if (searchText_equipment.Text == null || string.IsNullOrWhiteSpace(searchText_equipment.Text)) { MessageBox.Show("Введите данные для поиска"); return; }

                
                

                DataView dv = new DataView(((DataView)equipment.ItemsSource).ToTable());
                


                //connecting.Open();

                //string sql = null;
                switch (searchParam_equipment.Text)
                {
                    case "Номер":
                        dv.RowFilter = "id=" + searchText_equipment.Text;
                        break;
                    case "Модель":
                        dv.RowFilter = "model=" + searchText_equipment.Text;
                        break;
                    case "Статус":
                        dv.RowFilter = "status=" + searchText_equipment.Text;
                        break;
                    case "Номер договора":
                        dv.RowFilter = "client_id=" + searchText_equipment.Text;
                        break;
                }
                equipment.ItemsSource = dv;
                /*
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataSet iDataSet = new DataSet();
                iAdapter.Fill(iDataSet, "Clients");

                clients.IsReadOnly = true;
                clients.DataContext = iDataSet;

                connecting.Close();*/
            }
            catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show("Error" + ex.Message);
            }
        }


        private void logOut(object sender, RoutedEventArgs e)
        {
            UserLoginWindow userLoginWindow = new UserLoginWindow();
            userLoginWindow.Show();
            Window.GetWindow(this).Close();
        }

        private void ZayavAdd_click(object sender, RoutedEventArgs e)
        {
            ZayavAdd zayavAdd = new ZayavAdd(userIdLocal);
            zayavAdd.Show();
        }






    }
}
/*


                string searchParamBack = searchParam.Text.Remove(searchParam.Text.LastIndexOf(@" "));

                connecting.Open();

                string sql = null;
                switch (searchParam.Text.Remove(searchParam.Text.LastIndexOf(@" ")))
                {
                    case "Имя":
                        sql = @"SELECT * FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE name_client = "+ '\u0027' + searchText.Text + '\u0027' + ";";
                        break;
                    case "Фамилия":
                        sql = @"SELECT * FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE surname = "+ '\u0027' + searchText.Text + '\u0027' + ";";
                        break;
                    case "Отчество":
                        sql = @"SELECT * FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE fathername = "+ '\u0027' + searchText.Text + '\u0027' + ";";
                        break;
                    case "Номер документа":
                        sql = @"SELECT * FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE docnumb = "+ '\u0027' + searchText.Text + '\u0027' + ";";
                        break;
                    case "Адрес":
                        sql = @"SELECT * FROM public." + '\u0022' + "Clients" + '\u0022' + "WHERE address = "+ '\u0027' + searchText.Text + '\u0027' + ";";
                        break;
                }


*/