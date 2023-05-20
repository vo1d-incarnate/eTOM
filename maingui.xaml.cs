using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
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

        public maingui()
        {
            InitializeComponent();
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
                string sql = @"SELECT id, name_client, surname, fathername, docnumb, address
	FROM public." + '\u0022' + "Clients" + '\u0022' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataSet iDataSet = new DataSet();
                iAdapter.Fill(iDataSet, "Clients");

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

        private void Reload_page(object sender, RoutedEventArgs e)
        {
            Clients_table();

        }

        private void ClientAdd_click(object sender, RoutedEventArgs e)
        {
            ClientAdd clientAdd = new ClientAdd();
            clientAdd.Show();
        }


        private void Client_edit_click(object sender, RoutedEventArgs e)
        {


            DataRowView rowView = clients.SelectedValue as DataRowView;
            ClientEdit clientEdit = new ClientEdit();
            string idData = rowView[0].ToString();
            clientEdit.idData = idData;
            //   serv_edit.test.Text += rowView[0].ToString();
            clientEdit.Show();

        }


        private void Client_excel(object sender, RoutedEventArgs e)
        {
            try
            {
                connecting.Open();
                string sql = @"SELECT name_client, surname, fathername, docnumb, address FROM public." + '\u0022' + "Clients" + '\u0022' + ";";
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
                sheet.Cells[1, 4] = "Номер документов";
                sheet.Cells[1, 5] = "Адрес";

                Excel.Range range = sheet.Range[sheet.Cells[2, 1], sheet.Cells[ct.Rows.Count, ct.Columns.Count]];
                for (int i = 0; i < ct.Rows.Count; ++i)
                    for (int j = 0; j < ct.Columns.Count; ++j)
                    {
                        range.Cells[1 + i, 1 + j] = ct.Rows[i][j].ToString();
                        //  MessageBox.Show(ct.Rows[i][j].ToString());

                        if (j == 2)
                        {
                            range.Cells[1 + i, 1 + j] = double.Parse(ct.Rows[i][j].ToString());
                        }
                        else if (j == 9)
                        {
                            string dateExcel = ct.Rows[i][j].ToString().Remove(ct.Rows[i][j].ToString().LastIndexOf(@" ")); ;
                            range.Cells[1 + i, 1 + j] = dateExcel;
                        }
                        if (ct.Rows[i][j].ToString() == "True")
                        {
                            range.Cells[1 + i, 1 + j] = "Да";
                        }
                        else if (ct.Rows[i][j].ToString() == "False")
                        {
                            range.Cells[1 + i, 1 + j] = "Нет";
                        }

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





    }
}
