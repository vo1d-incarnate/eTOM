﻿using Npgsql;
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
        private string connectPostgre = String.Format("Server=Localhost;Port=5432;User Id=postgres;password=MmV8qd-+1!;Database=eTOM");
        private NpgsqlConnection connect;
        public ZayavAdd()
        {
            connect = new NpgsqlConnection(connectPostgre);
            InitializeComponent();

            try
            {
                connect.Open();
                string sql = @"SELECT serv_name FROM public." + '\u0022' + "Services" + '\u0022' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                cmd.ExecuteNonQuery();
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataTable iDataTable = new DataTable();
                iAdapter.Fill(iDataTable);
                connect.Close();

                string[] comboboxItems = new string[iDataTable.Rows.Count];

                for (int i = 0; i < iDataTable.Rows.Count; i++)
                {
                    comboboxItems[i] = iDataTable.Rows[i][0].ToString();
                }
                
                service_choose.ItemsSource = comboboxItems;



            } catch (Exception ex)
            {
                connect.Close();
                MessageBox.Show(ex.Message);
            }

            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (comment.Text == null || string.IsNullOrWhiteSpace(comment.Text)) { MessageBox.Show("Введите комментарий"); return; }

            try
            {
                connect.Open();

                string sql = @"SELECT * FROM public." + '\u0022' + "Services" + '\u0022' + " WHERE " + "serv_name=" + '\u0027' + service_choose.Text +'\u0027' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                cmd.ExecuteNonQuery();
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataTable iDataTable = new DataTable();
                iAdapter.Fill(iDataTable);


                string sql1 = @"INSERT INTO public." + '\u0022' + "Zayavki" + '\u0022' + "(service_id, comment) VALUES (" + '\u0027' + iDataTable.Rows[0][0].ToString() + '\u0027' + ", " + '\u0027' + comment.Text + '\u0027' + ");";
                NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connect);
                cmd1.ExecuteNonQuery();
                
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

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}
