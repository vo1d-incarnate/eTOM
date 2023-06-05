using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Npgsql;
using Syncfusion.Windows.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
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
using Path = System.IO.Path;

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
        private UserCredential credential;
        
        // Метод для создания текстового сообщения в формате RFC 2822
        private static string CreateMessage(string sender, string recipient, string subject, string body)
        {
            var messageBuilder = new StringBuilder();
            messageBuilder.AppendLine("MIME-Version: 1.0");
            messageBuilder.AppendLine($"To: {recipient}");
            messageBuilder.AppendLine($"From: {sender}");
        
            messageBuilder.AppendLine($"Subject: =?utf-8?B?{Base64UrlEncode(Encoding.UTF8.GetBytes(subject))}?=");
            
            
            
            //messageBuilder.AppendLine($"Subject: {subject}");
            messageBuilder.AppendLine("Content-Type: text/plain; charset=UTF-8");
            messageBuilder.AppendLine("Content-Transfer-Encoding: base64");
            messageBuilder.AppendLine("");
            var encodedBody = Convert.ToBase64String(Encoding.UTF8.GetBytes(body));

            // Добавляем закодированный текст сообщения в тело MIME-сообщения
            messageBuilder.AppendLine(encodedBody);

            // Кодируем текст сообщения в формат Base64
            //var encoded = Base64UrlEncode(Encoding.UTF8.GetBytes(messageBuilder.ToString()));
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(messageBuilder.ToString()));
            

            return encoded;
        }

        // Метод для кодирования байтового массива в формат Base64 URL-safe
        private static string Base64UrlEncode(byte[] input)
        {            
            Console.WriteLine(Convert.ToBase64String(input, 0, input.Length));
            /*return Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");*/
            return Convert.ToBase64String(input);
        }

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
            StatsByMonths_Upload();
            StatsTarifs_Upload();
            Provider_Upload();

            registration.Content = new RegistrationPage();

            if (rolesLocal != "ADMIN")
            {
                otchet.Visibility = Visibility.Collapsed;
                otchet1.Visibility = Visibility.Collapsed;
                reg.Visibility = Visibility.Collapsed;
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
                    frame.Margin = new Thickness(0, 0, 0, 0);
                    frame.Navigate(new zayavLifeline(j));
                    Zayavki.Items.Add(new TabItem
                    {
                        Header = new TextBlock { Text = "Заявка №" + j.ToString() },
                        Content = frame
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
                string sql = @"SELECT id, name_client, surname, fathername, contractnumb, address FROM public." + '\u0022' + "Clients" + '\u0022' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataSet iDataSet = new DataSet();
                iAdapter.Fill(iDataSet, "Clients");
                connecting.Close();
                
                clients.IsReadOnly = true;
                clients.DataContext = iDataSet;
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
            try
            {
                connecting.Open();
                string sql = @"SELECT model, status, client_id FROM public." + '\u0022' + "Equipment" + '\u0022' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataSet iDataSet = new DataSet();
                iAdapter.Fill(iDataSet, "Equipment");
                connecting.Close();

                DataTable ct = iDataSet.Tables[0];

                Excel.Application objExcel = new Excel.Application();
                Excel.Workbook workbook = objExcel.Workbooks.Add();
                Excel.Worksheet sheet = workbook.ActiveSheet;
                sheet.Cells[1, 1] = "Модель";
                sheet.Cells[1, 2] = "Статус";
                sheet.Cells[1, 3] = "id Клиента";
                

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

        private void findClient (object sender, RoutedEventArgs e)
        {
            try
            {
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
                connecting.Close();

                clients.IsReadOnly = true;
                clients.DataContext = iDataSet;
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

        class DataPoint
        {
            public int Forecast { get; set; }


            public DataPoint(int forecast)
            {
                Forecast = forecast;
            }
        }
        // Создаем список точек для графика
        string GetForecastString(List<DataPoint> dataPoints)
        {
            string dataString = "";
            foreach (var point in dataPoints)
            {
                dataString += point.Forecast + ",";
            }
            return dataString.TrimEnd(',');
        }

        private void StatsByMonths_Upload()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            try
            {
                DataTable iDataTable = Stats_Upload();

                DataTable iDataTable_out = new DataTable();

                iDataTable_out.Columns.Add("service_id", typeof(int));
                iDataTable_out.Columns.Add("date", typeof(DateTime));

                for (int i = 0; i < iDataTable.Rows.Count; i++)
                {
                    iDataTable_out.Rows.Add();
                    iDataTable_out.Rows[i][0] = iDataTable.Rows[i][0];
                    var dateTimefromDB = iDataTable.Rows[i][1];
                    //iDataTable_out.Rows[i][1] = DateTime.Parse(dateTimefromDB.ToString()).ToShortDateString();
                    iDataTable_out.Rows[i][1] = (DateTime)iDataTable.Rows[i][1];
                }
                Console.WriteLine("До" + iDataTable_out.Rows.Count);
                for (int i = iDataTable_out.Rows.Count - 1; i >= 0; i--)
                {
                    if (((DateTime)iDataTable_out.Rows[i][1]).Year != (DateTime.Now.Year))
                    {
                        iDataTable_out.Rows.Remove(iDataTable_out.Rows[i]);
                        iDataTable_out.AcceptChanges();
                    }
                }
                Console.WriteLine("После" + iDataTable_out.Rows.Count);
                int month4 = DateTime.Now.Month;
                int month3 = month4 - 1;
                int month2 = month3 - 1;
                int month1 = month2 - 1;
                int month0 = month1 - 1;

                string strMonth4;
                string strMonth3;
                string strMonth2;
                string strMonth1;
                string strMonth0;

                strMonth4 = monthToString(month4);
                strMonth3 = monthToString(month3);
                strMonth2 = monthToString(month2);
                strMonth1 = monthToString(month1);
                strMonth0 = monthToString(month0);

                int countM0 = 0;
                int countM1 = 0;
                int countM2 = 0;
                int countM3 = 0;
                int countM4 = 0;

                for (int i = 0; i < iDataTable_out.Rows.Count; i++)
                {
                    if (((DateTime)iDataTable_out.Rows[i][1]).Month == month0)
                    {
                        countM0++;
                    }
                }
                for (int i = 0; i < iDataTable_out.Rows.Count; i++)
                {
                    if (((DateTime)iDataTable_out.Rows[i][1]).Month == month1)
                    {
                        countM1++;
                    }
                }
                for (int i = 0; i < iDataTable_out.Rows.Count; i++)
                {
                    if (((DateTime)iDataTable_out.Rows[i][1]).Month == month2)
                    {
                        countM2++;
                    }
                }
                for (int i = 0; i < iDataTable_out.Rows.Count; i++)
                {
                    if (((DateTime)iDataTable_out.Rows[i][1]).Month == month3)
                    {
                        countM3++;
                    }
                }
                for (int i = 0; i < iDataTable_out.Rows.Count; i++)
                {
                    if (((DateTime)iDataTable_out.Rows[i][1]).Month == month4)
                    {
                        countM4++;
                    }
                }

                int[] dataValue = new int[5];
                dataValue[0] = countM0;
                dataValue[1] = countM1;
                dataValue[2] = countM2;
                dataValue[3] = countM3;
                dataValue[4] = countM4;

                int maxZayav = dataValue.Max();
                for (int i = 0; i < dataValue.Length; i++)
                {
                    dataPoints.Add(new DataPoint(dataValue[i]));
                }

                int yAxisStep = 1;
                if (maxZayav > 10)
                {
                    yAxisStep = 10;
                } else if (maxZayav > 100)
                {
                    yAxisStep = 100;
                }
                // Формируем URL для запроса к API
                string url = "https://chart.googleapis.com/chart" +
                    "?cht=lc" + // Тип графика - линейный
                    "&chs=340x190" + // Размер графика
                    "&chxt=x,y" + // Оси X и Y
                    "&chxr=0,0,4,1|1,0," + Math.Round(maxZayav * 1.2) + "," + yAxisStep + // Диапазоны значений осей
                    "&chds=0," + Math.Round(maxZayav*1.2) + // Минимальное и максимальное значение данных
                    "&chco=117B8E" + // Цвета линий
                    "&chxs=0,FFF9F3,12,0,lt|1,FFF9F3,12,0,lt" +
                    "&chd=t:" + GetForecastString(dataPoints) + // Данные графика
                    "&chxl=0:|" + strMonth0 + "|" + strMonth1 + "|" + strMonth2 + "|" + strMonth3 + "|" + strMonth4 +
                    "&chdl=Прирост клиентов" + // Легенда графика
                    "&chtt=Подключённые заявки за месяц" + // Заголовок графика
                    "&chts=FFF9F3" +
                    "&chdls=FFF9F3" + // Цвет текста легенды
                    "&chdlp=b" + // Выравнивание легенды 
                    "&chf=bg,s,2C4370" + // Фоновый цвет графика
                    "&chg=25," + (100/(Math.Round(maxZayav * 1.2)/yAxisStep)) + // Сетка
                    "&chc=FFF9F3"; // Цвет линий осей

                // Отправляем запрос к API и получаем ответ в формате изображения
                WebClient client = new WebClient();
                byte[] imageBytes = client.DownloadData(url);

                // Создаем BitmapImage из полученных байтов и устанавливаем его в качестве источника изображения для элемента Image
                BitmapImage chartBitmap = new BitmapImage();
                chartBitmap.BeginInit();
                chartBitmap.StreamSource = new System.IO.MemoryStream(imageBytes);
                chartBitmap.EndInit();
                chartMonths.Source = chartBitmap;
            }
            catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }
 
        private void StatsTarifs_Upload()
        {
            List<DataPoint> dataPoints = new List<DataPoint>();
            try
            {
                connecting.Open();
                string sql1 = @"
                   SELECT serv_name
                   FROM public.""Services""
                   ;";
                NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connecting);
                NpgsqlDataAdapter iAdapter1 = new NpgsqlDataAdapter(cmd1);
                DataTable iDataTable1 = new DataTable();
                iAdapter1.Fill(iDataTable1);
                connecting.Close();


                DataTable iDataTable = Stats_Upload();

                DataTable iDataTable_out = new DataTable();

                iDataTable_out.Columns.Add("service_id", typeof(int));
                iDataTable_out.Columns.Add("date", typeof(DateTime));

                for (int i = 0; i < iDataTable.Rows.Count; i++)
                {
                    iDataTable_out.Rows.Add();
                    iDataTable_out.Rows[i][0] = iDataTable.Rows[i][0];
                    var dateTimefromDB = iDataTable.Rows[i][1];
                    //iDataTable_out.Rows[i][1] = DateTime.Parse(dateTimefromDB.ToString()).ToShortDateString();
                    iDataTable_out.Rows[i][1] = (DateTime)iDataTable.Rows[i][1];
                }
                Console.WriteLine("До " + iDataTable_out.Rows.Count);
                for (int i = iDataTable_out.Rows.Count - 1; i >= 0; i--)
                {
                    if (((DateTime)iDataTable_out.Rows[i][1]).Year != (DateTime.Now.Year))
                    {
                        iDataTable_out.Rows.Remove(iDataTable_out.Rows[i]);
                        iDataTable_out.AcceptChanges();
                    }
                    else if (((DateTime)iDataTable_out.Rows[i][1]).Month != (DateTime.Now.Month))
                    {
                        iDataTable_out.Rows.Remove(iDataTable_out.Rows[i]);
                        iDataTable_out.AcceptChanges();
                    }
                }
                Console.WriteLine("После " + iDataTable_out.Rows.Count);

                int countTarif0 = 0;
                int countTarif1 = 0;
                int countTarif2 = 0;
                int countTarif3 = 0;

                for (int i = 0; i < iDataTable_out.Rows.Count; i++)
                {
                    if ((int)iDataTable_out.Rows[i][0] == 1)
                    {
                        countTarif0++;
                    }
                }
                for (int i = 0; i < iDataTable_out.Rows.Count; i++)
                {
                    if ((int)iDataTable_out.Rows[i][0] == 2)
                    {
                        countTarif1++;
                    }
                }
                for (int i = 0; i < iDataTable_out.Rows.Count; i++)
                {
                    if ((int)iDataTable_out.Rows[i][0] == 3)
                    {
                        countTarif2++;
                    }
                }
                for (int i = 0; i < iDataTable_out.Rows.Count; i++)
                {
                    if ((int)iDataTable_out.Rows[i][0] == 4)
                    {
                        countTarif3++;
                    }
                }

                int[] dataValue = new int[4];
                dataValue[0] = countTarif0;
                dataValue[1] = countTarif1;
                dataValue[2] = countTarif2;
                dataValue[3] = countTarif3;

                int maxZayav = dataValue.Max();
                for (int i = 0; i < dataValue.Length; i++)
                {
                    if (dataValue[i] != 0)
                    {
                        dataPoints.Add(new DataPoint(dataValue[i]));
                    }
                }

                int yAxisStep = 1;
                if (maxZayav > 10)
                {
                    yAxisStep = 10;
                }
                else if (maxZayav > 100)
                {
                    yAxisStep = 100;
                }
                // Формируем URL для запроса к API
                string url = "https://chart.googleapis.com/chart" +
                    "?cht=p" + // Тип графика - линейный
                    "&chs=340x190" + // Размер графика
                    "&chxt=x,y" + // Оси X и Y
                    "&chxr=0,0,4,1|1,0," + Math.Round(maxZayav * 1.2) + "," + yAxisStep + // Диапазоны значений осей
                    "&chds=0," + Math.Round(maxZayav * 1.2) + // Минимальное и максимальное значение данных
                    "&chco=117B8E" + // Цвета линий
                    "&chxs=0,FFF9F3,12,0,lt|1,FFF9F3,12,0,lt" +
                    "&chd=t:" + GetForecastString(dataPoints) + // Данные графика
                    "&chxl=0:|" + iDataTable1.Rows[0][0] + "|" + iDataTable1.Rows[1][0] + "|" + iDataTable1.Rows[2][0] + "|" + iDataTable1.Rows[3][0] +
                    "&chdl=" + countTarif0 + "|" + countTarif1 + "|" + countTarif2 + "|" + countTarif3 + // Легенда графика
                    "&chtt=Подключённые заявки за месяц" + // Заголовок графика
                    "&chts=FFF9F3" +
                    "&chdls=FFF9F3" + // Цвет текста легенды
                    "&chdlp=b" + // Выравнивание легенды 
                    "&chf=bg,s,2C4370" + // Фоновый цвет графика
                    "&chg=25," + (100 / (Math.Round(maxZayav * 1.2) / yAxisStep)) + // Сетка
                    "&chc=FFF9F3"; // Цвет линий осей

                // Отправляем запрос к API и получаем ответ в формате изображения
                WebClient client = new WebClient();
                byte[] imageBytes = client.DownloadData(url);

                // Создаем BitmapImage из полученных байтов и устанавливаем его в качестве источника изображения для элемента Image
                BitmapImage chartBitmap = new BitmapImage();
                chartBitmap.BeginInit();
                chartBitmap.StreamSource = new System.IO.MemoryStream(imageBytes);
                chartBitmap.EndInit();
                chartTarifs.Source = chartBitmap;

                // Заполняем поля на странице
                statsMore.Text = "С тарифом " + iDataTable1.Rows[0][0].ToString() + ": " + countTarif0;
                statsMore1.Text = "С тарифом " + iDataTable1.Rows[1][0].ToString() + ": " + countTarif1;
                statsMore2.Text = "С оборудованием: " + countTarif3;

                statsPlan.Text = "С тарифом " + iDataTable1.Rows[0][0].ToString() + ": " + (countTarif0+1);
                statsPlan1.Text = "С тарифом " + iDataTable1.Rows[1][0].ToString() + ": " + (countTarif1 + 1);
                statsPlan2.Text = "С оборудованием: " + (countTarif3 + 1);
            }
            catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private DataTable Stats_Upload()
        {
            try
            {
                connecting.Open();
                string sql = @"
                   SELECT service_id, created_at
                   FROM public.""Zayavki""
                   WHERE user_id = " + userIdLocal + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataTable iDataTable = new DataTable();
                iAdapter.Fill(iDataTable);
                connecting.Close();
                return iDataTable;
            } catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show(ex.Message);
                return null;
            }
        }

        string monthToString(int month)
        {
            string strMonth = string.Empty;
            switch (month)
            {
                case 1:
                    strMonth = "Январь";
                    break;
                case 2:
                    strMonth = "Февраль";
                    break;
                case 3:
                    strMonth = "Март";
                    break;
                case 4:
                    strMonth = "Апрель";
                    break;
                case 5:
                    strMonth = "Май";
                    break;
                case 6:
                    strMonth = "Июнь";
                    break;
                case 7:
                    strMonth = "Июль";
                    break;
                case 8:
                    strMonth = "Август";
                    break;
                case 9:
                    strMonth = "Сентябрь";
                    break;
                case 10:
                    strMonth = "Октябрь";
                    break;
                case 11:
                    strMonth = "Ноябрь";
                    break;
                case 12:
                    strMonth = "Декабрь";
                    break;
            }
            return strMonth;
        }



        private void Provider_Upload()
        {
            try
            {
                connecting.Open();
                string sql = @"SELECT * FROM public.""Equipment"";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connecting);
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataTable iDataTable = new DataTable();
                iAdapter.Fill(iDataTable);
                connecting.Close();

                providerEquip.Maximum = iDataTable.Rows.Count;
                int countAtClient = 0;

                for (int i = 0; i < iDataTable.Rows.Count; i++)
                {
                    if ((bool)iDataTable.Rows[i][2] == true)
                    {
                        countAtClient++;
                    }
                }
                providerEquip.Value = countAtClient;

                providerEquipTxt.Text = "У клиентов: " + countAtClient + " | На складе: " + (iDataTable.Rows.Count - countAtClient);


            }
            catch (Exception ex)
            {
                connecting.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private void CheckStatus(object sender, RoutedEventArgs e)
        {
            bool pingable = false;
            Ping pinger = null;
            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send("192.168.31.1");
                pingable = reply.Status == IPStatus.Success;
                if (pingable)
                {
                    providerStatusResult.Text = "Соединение установлено.\nУстройство работает стабильно.";
                } else
                {
                    providerStatusResult.Text = "Соединение не установлено.\nМогут быть проблемы с работой устройства.";
                }
            }
            catch (PingException ex)
            {
                MessageBox.Show(ex.Message);
                // Discard PingExceptions and return false;
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }
        }
        //
        private void SendRequest(object sender, RoutedEventArgs e)
        {
            using (var stream = new FileStream("../../resources/credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    new[] { GmailService.Scope.GmailCompose, GmailService.Scope.GmailSend, GmailService.Scope.GmailModify },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            // Создание сервиса Gmail
            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "eTOM.App"
            });
            
            /*
            // Получение списка сообщений
            var listRequest = service.Users.Messages.List("me");
            listRequest.MaxResults = 10;
            var response = listRequest.Execute();
            IList<Message> messages = response.Messages;
            if (messages != null && messages.Count > 0)
            {
                foreach (var message in messages)
                {
                    Console.WriteLine("Message Id: " + message.Id);
                }
            }
            else
            {
                Console.WriteLine("No messages found.");
            }*/

            var email = new Google.Apis.Gmail.v1.Data.Message();
            email.Raw = CreateMessage("iiiythuk.2003@gmail.com", "iiiythuk.2003@gmail.com", "Тема", "Текст");
            Console.WriteLine(email.Raw);
            try
            {
                // Отправка письма
                service.Users.Messages.Send(email, "me").Execute();
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Console.WriteLine(ex.Message);
            }
        }
    }
}