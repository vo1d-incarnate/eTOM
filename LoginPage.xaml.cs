﻿using Npgsql;
using Syncfusion.Windows.Shared;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
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

        public static string HashPassword(string password)
        {
            byte[] salt;
            byte[] hash;

            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            // Комбинируем соль и хеш в одну строку
            var saltBase64 = Convert.ToBase64String(salt);
            var hashBase64 = Convert.ToBase64String(hash);
            //var combinedHash = string.Concat(saltBase64, hashBase64);

            var combinedBytes = new byte[salt.Length + hash.Length];
            Buffer.BlockCopy(salt, 0, combinedBytes, 0, salt.Length);
            Buffer.BlockCopy(hash, 0, combinedBytes, salt.Length, hash.Length);

            var combinedHash = Convert.ToBase64String(combinedBytes);
            Console.WriteLine(combinedHash.Length);
            Console.WriteLine(combinedHash);
            return combinedHash;
        }

        public static bool VerifyPassword(string password, string storedHash)
        {
            try
            {
                // Разделяем хранимый хеш на соль и хеш пароля
                var combinedBytes = Convert.FromBase64String(storedHash);
                var saltBytes = new byte[128];
                var storedHashBytes = new byte[combinedBytes.Length - 128];

                Buffer.BlockCopy(combinedBytes, 0, saltBytes, 0, 128);
                Buffer.BlockCopy(combinedBytes, 128, storedHashBytes, 0, storedHashBytes.Length);

                using (var hmac = new HMACSHA512(saltBytes))
                {
                    var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                    Console.WriteLine(Convert.ToBase64String(computedHash));
                    // Сравниваем вычисленный хеш с хранимым хешем
                    for (int i = 0; i < computedHash.Length; i++)
                    {
                        if (computedHash[i] != storedHashBytes[i])
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        private void logIn(object sender, RoutedEventArgs e)
        {
            if (login == null || string.IsNullOrWhiteSpace(login.Text)) { MessageBox.Show("Введите имя"); return; }
            else if (password == null || string.IsNullOrWhiteSpace(password.Text)) { MessageBox.Show("Введите фамилию"); return; }

            try
            {
                connect.Open();
                string sql = @"SELECT * FROM public." + '\u0022' + "User_login" + '\u0022' + " WHERE " + "login=" + '\u0027' + login.Text + '\u0027' + ";";
                NpgsqlCommand cmd = new NpgsqlCommand(sql, connect);
                cmd.ExecuteNonQuery();
                
                NpgsqlDataAdapter iAdapter = new NpgsqlDataAdapter(cmd);
                DataTable iDataSet = new DataTable();
                iAdapter.Fill(iDataSet);
                connect.Close();


                //if (true)
                if (VerifyPassword(password.Text, iDataSet.Rows[0][2].ToString()))
                {
                    MessageBox.Show("Успешно");
                    /*
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    Window.GetWindow(this).Close();*/
                    // qqqqqqqq

                    connect.Open();
                    string sql1 = @"SELECT * FROM public." + '\u0022' + "user_roles" + '\u0022' + " WHERE " + "user_id=" + iDataSet.Rows[0][0].ToString() + ";";
                    NpgsqlCommand cmd1 = new NpgsqlCommand(sql1, connect);
                    cmd1.ExecuteNonQuery();

                    NpgsqlDataAdapter iAdapter1 = new NpgsqlDataAdapter(cmd1);
                    DataTable iDataSet1 = new DataTable();
                    iAdapter1.Fill(iDataSet1);

                    string sql2 = @"SELECT * FROM public." + '\u0022' + "Roles" + '\u0022' + " WHERE " + "id=" + iDataSet1.Rows[0][2].ToString() + ";";
                    NpgsqlCommand cmd2 = new NpgsqlCommand(sql2, connect);
                    cmd2.ExecuteNonQuery();

                    NpgsqlDataAdapter iAdapter2 = new NpgsqlDataAdapter(cmd2);
                    DataTable iDataSet2 = new DataTable();
                    iAdapter2.Fill(iDataSet2);


                    connect.Close();


                    MainWindow mainWindow = new MainWindow(iDataSet2.Rows[0][1].ToString(), (int)iDataSet.Rows[0][0]);
                    mainWindow.Show();
                    Window.GetWindow(this).Close();



                } else
                {
                    MessageBox.Show("Неправильный логин или пароль");
                }

                //MessageBox.Show("");

            } catch (Exception ex)
            {
                connect .Close();
                MessageBox.Show("Error: " + ex.Message);
                Console.WriteLine(ex.Message);
            }

        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                logIn(sender, e);
            }
        }
    }
}
