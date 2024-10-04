using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace УП_Зайцева
{
    public partial class Authorization : Form
    {
        private readonly db DataBase;
        public Authorization()
        {
            InitializeComponent();
            DataBase = new db();
        }

        public void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
            textBox2.Text = string.Empty;

        }
        public string GetUserRole(string username, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Должность FROM Сотрудники WHERE ФИО = @ФИО";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ФИО", username);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }
            return "Unknown";
        }

        public void button2_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Введите имя пользователя и пароль.");
                return;
            }

            string connectionString = DataBase.GetConnectionString();
            int userid = GetUserID(username, connectionString); // Получаем userid из базы данных

            if (AuthenticateUser(username, password, connectionString))
            {
                string userRole = GetUserRole(username, connectionString); // Получаем роль пользователя

                MessageBox.Show("Авторизация успешна!");


                this.Hide();
                Form1 maimMuni = new Form1(userid); // Передаем userid в конструктор формы MaimMuni
                maimMuni.ShowDialog();
            }
            else
            {
                MessageBox.Show("Неверное имя пользователя или пароль.");
            }

        }
        public bool AuthenticateUser(string username, string password, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT COUNT(*) FROM Сотрудники WHERE ФИО = @ФИО AND Телефон = @Пароль";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ФИО", username);
                    command.Parameters.AddWithValue("@Пароль", password);
                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public int GetUserID(string username, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Сотрудник_ID FROM Сотрудники WHERE ФИО = @ФИО";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ФИО", username);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return (int)result;
                    }
                }
            }
            return -1;
        }
    }
   
}
