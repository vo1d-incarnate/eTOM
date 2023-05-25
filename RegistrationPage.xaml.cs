﻿using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace eTOM
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        private string connectPostgre = String.Format("Server=Localhost;Port=5432;User Id=postgres;password=MmV8qd-+1!;Database=eTOM");
        private NpgsqlConnection connect;
        public RegistrationPage()
        {
            InitializeComponent();
            connect = new NpgsqlConnection(connectPostgre);
        }

        private void signup(object sender, RoutedEventArgs e)
        {
            try
            {
                connect.Open();
                string sql = @"INSERT INTO public." + '\u0022' + "User_login" + '\u0022' + " (login, password) VALUES (" + '\u0027' + login.Text + '\u0027' + ", " + '\u0027' + password.Text + '\u0027' + ");";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                cmd.ExecuteNonQuery();

                string sql1 = @"SELECT id FROM public." + '\u0022' + "User_login" + '\u0022' + " WHERE login=" + '\u0027' + login.Text + '\u0027' + ";";
                NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connect);
                cmd1.ExecuteNonQuery();
                NpgsqlDataAdapter iAdapter1 = new NpgsqlDataAdapter(cmd1);
                DataTable iDataTable1 = new DataTable();
                iAdapter1.Fill(iDataTable1);

                string sql2 = @"INSERT INTO public." + '\u0022' + "User_info" + '\u0022' + " (user_id, name, surname, fathername) VALUES (" + '\u0027' + iDataTable1.Rows[0][0].ToString() + '\u0027' + ", " + '\u0027' + name.Text + '\u0027' + ", " + '\u0027' + surname.Text + '\u0027' + ", " + '\u0027' + fathername.Text + '\u0027' + ");";
                NpgsqlCommand cmd2 = new NpgsqlCommand(sql2, connect);
                cmd2.ExecuteNonQuery();
                connect.Close();

            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}