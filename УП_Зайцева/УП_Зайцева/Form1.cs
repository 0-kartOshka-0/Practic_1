using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolBar;

namespace УП_Зайцева
{
    public partial class Form1 : Form
    {
        private int userid;
        private readonly db DataBase;
        private readonly string connectionString;
        public Form1(int userid)
        {
            InitializeComponent();
            this.userid = userid;

            DataBase = new db();
            connectionString = DataBase.GetConnectionString();
            string userRole = GetUserRole(userid, DataBase.GetConnectionString());
            dataGridView1.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        }
        private string GetUserRole(int Сотрудник_ID, string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT Должность FROM Сотрудники WHERE Сотрудник_ID = @Сотрудник_ID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Сотрудник_ID", Сотрудник_ID);
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        return result.ToString();
                    }
                }
            }
            return "Unknown";
        }

        private void настройкиToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void сменитьПользователяToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }

        private void сотрудникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem a in действияСТаблицейToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            foreach (ToolStripMenuItem a in таблицыToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            Отобразить_Сотрудников();
        }
        private void Отобразить_Сотрудников()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    panel_дтп.Visible = false;
                    panel_МЕРОПРИЯТИЯ.Visible = false;
                    panel_Сотрудники.Visible = false;
                    label1.Text = "Таблица Сотрудники";
                    connection.Open();
                    string query = @"SELECT s.Сотрудник_ID, s.ФИО, s.Должность, s.Телефон, s.Email
                                     FROM Сотрудники s ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable Сотрудники = new DataTable();
                        adapter.Fill(Сотрудники);
                        dataGridView1.DataSource = Сотрудники;
                        dataGridView1.Columns["Сотрудник_ID"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных о сотрудники: " + ex.Message);
            }
        }
        private void Найти_Сотрудника(string searchValue)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT s.Сотрудник_ID, s.ФИО, s.Должность, s.Телефон, s.Email
                                     FROM Сотрудники s 
                            WHERE s.Сотрудник_ID LIKE @searchValue OR s.ФИО LIKE @searchValue OR s.Должность LIKE @searchValue  
                            OR s.Телефон LIKE @searchValue OR s.Email LIKE @searchValue";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@searchValue", "%" + searchValue + "%");
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable Сотрудники = new DataTable();
                        adapter.Fill(Сотрудники);
                        dataGridView1.DataSource = Сотрудники;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при выполнении поиска: " + ex.Message);
            }
        }
        private void Удалить_Сотрудника(int Сотрудник_ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Сотрудники WHERE Сотрудник_ID = @Сотрудник_ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Сотрудник_ID", Сотрудник_ID);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Сотрудник успешно удален.");
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка при удалении сотрудника.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при удалении сотрудника: " + ex.Message);
            }
        }
        private void Добавить_Сотрудника()
        {
            Выполнить_Запрос_Сотрудники("Insert into Сотрудники(ФИО, Должность, Телефон, Email) " +
                                        "VALUES(@ФИО, @Должность, @Телефон, @Email);");
        }

        private void Изменить_Сотрудника()
        {
            Выполнить_Запрос_Сотрудники("Update Сотрудники set ФИО=@ФИО, Должность=@Должность, Телефон=@Телефон, Email=@Email Where Сотрудник_ID=@Сотрудник_ID;");
        }

        private void Выполнить_Запрос_Сотрудники(string query)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        string Сотрудник_ФИО = textBox2.Text;
                        string Сотрудник_Должность = textBox3.Text;
                        string Сотрудник_Телефон = maskedTextBox1.Text;
                        string Сотрудник_Почта = textBox4.Text;
                        string ID_Сотр = textBox6.Text;

                        cmd.Parameters.AddWithValue("@ФИО", Сотрудник_ФИО);
                        cmd.Parameters.AddWithValue("@Должность", Сотрудник_Должность);
                        cmd.Parameters.AddWithValue("@Телефон", Сотрудник_Телефон);
                        cmd.Parameters.AddWithValue("@Email", Сотрудник_Почта);
                        cmd.Parameters.AddWithValue("@Сотрудник_ID", ID_Сотр);
                        cmd.ExecuteNonQuery();
                    }
                    textBox2.Clear();
                    textBox3.Clear();
                    maskedTextBox1.Clear();
                    textBox4.Clear();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }
        private void дТПToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem a in действияСТаблицейToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            foreach (ToolStripMenuItem a in таблицыToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            Отобразить_ДТП();
        }
        private void Отобразить_ДТП()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    panel_дтп.Visible = false;
                    panel_МЕРОПРИЯТИЯ.Visible = false;
                    panel_Сотрудники.Visible = false;
                    label1.Text = "Таблица ДТП";
                    connection.Open();
                    string query = @"SELECT d.ДТП_ID, d.Дата, d.Место, d.Описание, d.Степень_серьезности, d.Создано, d.Обновлено
                                     FROM ДТП d ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable ДТП = new DataTable();
                        adapter.Fill(ДТП);
                        dataGridView1.DataSource = ДТП;
                        dataGridView1.Columns["ДТП_ID"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных о ДТП: " + ex.Message);
            }
        }
        private void Найти_ДТП(string searchValue)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT d.ДТП_ID, d.Дата, d.Место, d.Описание, d.Степень_серьезности, d.Создано, d.Обновлено
                                     FROM ДТП d 
                            WHERE d.ДТП_ID LIKE @searchValue OR d.Дата LIKE @searchValue OR d.Место LIKE @searchValue  OR d.Описание LIKE @searchValue 
                                 OR d.Степень_серьезности LIKE @searchValue OR d.Создано LIKE @searchValue OR d.Обновлено LIKE @searchValue";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@searchValue", "%" + searchValue + "%");
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable ДТП = new DataTable();
                        adapter.Fill(ДТП);

                        dataGridView1.DataSource = ДТП;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при выполнении поиска: " + ex.Message);
            }
        }
        private void Удалить_ДТП(int ДТП_ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM ДТП WHERE ДТП_ID = @ДТП_ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@ДТП_ID", ДТП_ID);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("ДТП успешно удалено.");
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка при удалении ДТП.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при удалении ДТП: " + ex.Message);
            }
        }
        private void Добавить_ДТП()
        {
            Выполнить_Запрос_ДТП("Insert into ДТП(ДТП.Дата, Место, Описание, Степень_серьезности, Создано) " +
                                        "VALUES(@Дата, @Место, @Описание, @Степень_серьезности, @Создано);");
        }
        private void Изменить_ДТП()
        {
            Выполнить_Запрос_ДТП("Update ДТП set Дата=@Дата, Место=@Место, Описание=@Описание, Степень_серьезности=@Степень_серьезности, Создано=@Создано Where ДТП_ID=@ДТП_ID;");
        }

        private void Выполнить_Запрос_ДТП(string query)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        DateTime ДТП_Дата;
                        if (!DateTime.TryParse(maskedTextBox2.Text, out ДТП_Дата))
                        {
                            MessageBox.Show("Некорректный формат даты.");
                            return;
                        }

                        string ДТП_Место = textBox5.Text;
                        string ДТП_Описание = richTextBox1.Text;
                        string ДТП_Степень_серьезности = comboBox1.Text;
                        string ДТП_Создано = textBox6.Text;
                        string ID_ДТП = textBox7.Text; 

                        cmd.Parameters.AddWithValue("@Дата", ДТП_Дата);
                        cmd.Parameters.AddWithValue("@Место", ДТП_Место);
                        cmd.Parameters.AddWithValue("@Описание", ДТП_Описание);
                        cmd.Parameters.AddWithValue("@Степень_серьезности", ДТП_Степень_серьезности);
                        cmd.Parameters.AddWithValue("@Создано", ДТП_Создано);
                        cmd.Parameters.AddWithValue("@ДТП_ID", ID_ДТП);

                        cmd.ExecuteNonQuery();
                    }
                    maskedTextBox2.Clear();
                    textBox5.Clear();
                    richTextBox1.Text = "";
                    comboBox1.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }
        private void участникиToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach(ToolStripMenuItem a in действияСТаблицейToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            foreach (ToolStripMenuItem a in таблицыToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            Отобразить_Участников();
        }
        private void Отобразить_Участников()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    panel_дтп.Visible = false;
                    panel_МЕРОПРИЯТИЯ.Visible = false;
                    panel_Сотрудники.Visible = false;
                    label1.Text = "Таблица Участники";
                    connection.Open();
                    string query = @"SELECT u.Участник_ID, u.ДТП_ID, u.ФИО, u.Адрес, u.Телефон, u.ТС, u.Документ
                                     FROM Участники u ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable Участники = new DataTable();
                        adapter.Fill(Участники);
                        dataGridView1.DataSource = Участники;
                        dataGridView1.Columns["Участник_ID"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных о участниках: " + ex.Message);
            }
        }
        private void тСToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem a in действияСТаблицейToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            foreach (ToolStripMenuItem a in таблицыToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            Отобразить_ТС();
        }
        private void Отобразить_ТС()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    panel_дтп.Visible = false;
                    panel_МЕРОПРИЯТИЯ.Visible = false;
                    panel_Сотрудники.Visible = false;
                    label1.Text = "Таблица ТС";
                    connection.Open();
                    string query = @"SELECT a.Авто_ID, a.Госномер, a.Марка, a.Модель, a.Год, a.Цвет
                                     FROM Автомобили a ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable ТС = new DataTable();
                        adapter.Fill(ТС);
                        dataGridView1.DataSource = ТС;
                        dataGridView1.Columns["Авто_ID"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных о транспортных средствах: " + ex.Message);
            }
        }
        private void документыToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem a in действияСТаблицейToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            foreach (ToolStripMenuItem a in таблицыToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            Отобразить_Документы();
        }
        private void Отобразить_Документы()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    panel_дтп.Visible = false;
                    panel_МЕРОПРИЯТИЯ.Visible = false;
                    panel_Сотрудники.Visible = false;
                    label1.Text = "Таблица Документы";
                    connection.Open();
                    string query = @"SELECT doc.Документ_ID, doc.ДТП_ID, doc.Тип_документа, doc.Номер_документа, doc.Дата_выдачи, doc.Дата_истечения
                                     FROM Документы doc ";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable Документы = new DataTable();
                        adapter.Fill(Документы);
                        dataGridView1.DataSource = Документы;
                        dataGridView1.Columns["Документ_ID"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных о документах: " + ex.Message);
            }
        }
        private void мероприятияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem a in действияСТаблицейToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            foreach (ToolStripMenuItem a in таблицыToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            Отобразить_Мероприятия();
        }
        private void Отобразить_Мероприятия()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    panel_дтп.Visible = false;
                    panel_МЕРОПРИЯТИЯ.Visible = false;
                    panel_Сотрудники.Visible = false;
                    label1.Text = "Таблица Мероприятия";
                    connection.Open();
                    string query = @"SELECT m.Мероприятие_ID, m.ДТП_ID, m.Дата_мероприятия, m.Описание_мероприятия, m.Сотрудник_ID
                                     FROM Мероприятия m";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable Мероприятия = new DataTable();
                        adapter.Fill(Мероприятия);
                        dataGridView1.DataSource = Мероприятия;
                        dataGridView1.Columns["Мероприятие_ID"].Visible = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при загрузке данных о мероприятиях: " + ex.Message);
            }
        }
        private void Найти_Мероприятие(string searchValue)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = @"SELECT m.Мероприятие_ID, m.ДТП_ID, m.Дата_мероприятия, m.Описание_мероприятия, m.Сотрудник_ID
                                     FROM Мероприятия m 
                            WHERE m.Мероприятие_ID LIKE @searchValue OR m.ДТП_ID LIKE @searchValue OR m.Дата_мероприятия LIKE @searchValue  
                            OR m.Описание_мероприятия LIKE @searchValue OR m.Сотрудник_ID LIKE @searchValue";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@searchValue", "%" + searchValue + "%");
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        DataTable Мероприятия = new DataTable();
                        adapter.Fill(Мероприятия);
                        dataGridView1.DataSource = Мероприятия;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при выполнении поиска: " + ex.Message);
            }
        }
        private void Удалить_Мероприятие(int Мероприятие_ID)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Мероприятия WHERE Мероприятие_ID = @Мероприятие_ID";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Мероприятие_ID", Мероприятие_ID);
                        int rowsAffected = command.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Мероприятие успешно удалено.");
                        }
                        else
                        {
                            MessageBox.Show("Произошла ошибка при удалении мероприятия.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при удалении мероприятия: " + ex.Message);
            }
        }
        private void Добавить_Меро()
        {
            Выполнить_Запрос_Меро("Insert into Мероприятия(ДТП_ID, Дата_мероприятия, Описание_мероприятия, Сотрудник_ID) " +
                                        "VALUES(@ДТП_ID, @Дата_мероприятия, @Описание_мероприятия, @Сотрудник_ID);");
        }
        private void Изменить_Меро()
        {
            Выполнить_Запрос_Меро("Update Мероприятия set ДТП_ID=@ДТП_ID, Дата_мероприятия=@Дата_мероприятия, Описание_мероприятия=@Описание_мероприятия, Сотрудник_ID=@Сотрудник_ID Where Мероприятие_ID=@Мероприятие_ID;");
        }
        private void Выполнить_Запрос_Меро(string query)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        string Меро_ID_ДТП = domainUpDown1.Text;
                        string Меро_Дата = maskedTextBox4.Text;
                        string Меро_Описание = richTextBox2.Text;
                        string Меро_ID_Сотрудника = domainUpDown2.Text;
                        string ID_Меро = textBox8.Text; 

                        cmd.Parameters.AddWithValue("@ДТП_ID", Меро_ID_ДТП);
                        cmd.Parameters.AddWithValue("@Дата_мероприятия", Меро_Дата);
                        cmd.Parameters.AddWithValue("@Описание_мероприятия", Меро_Описание);
                        cmd.Parameters.AddWithValue("@Сотрудник_ID", Меро_ID_Сотрудника);
                        cmd.Parameters.AddWithValue("@Мероприятие_ID", ID_Меро);

                        cmd.ExecuteNonQuery();
                    }
                    domainUpDown1.Text = "";
                    maskedTextBox4.Clear();
                    richTextBox2.Text = "";
                    domainUpDown2.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка: " + ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (дТПToolStripMenuItem.Checked)
            {
                string searchValue = textBox1.Text;
                Найти_ДТП(searchValue);
            }
            else if (мероприятияToolStripMenuItem.Checked)
            {
                string searchValue = textBox1.Text;
                Найти_Мероприятие(searchValue);
            }
            else if (сотрудникиToolStripMenuItem.Checked)
            {
                string searchValue = textBox1.Text;
                Найти_Сотрудника(searchValue);
            }
        }

        private void удалитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem a in действияСТаблицейToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            if (сотрудникиToolStripMenuItem.Checked)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int Сотрудник_ID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Сотрудник_ID"].Value);
                    DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить этого сотрудника?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        Удалить_Сотрудника(Сотрудник_ID);
                        Отобразить_Сотрудников();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите сотрудника для удаления.");
                }
            }
            else if (дТПToolStripMenuItem.Checked)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int ДТП_ID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["ДТП_ID"].Value);
                    DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить это ДТП?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        Удалить_ДТП(ДТП_ID);
                        Отобразить_ДТП();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите ДТП для удаления.");
                }
            }
            else if (мероприятияToolStripMenuItem.Checked)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {
                    int Мероприятие_ID = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Мероприятие_ID"].Value);
                    DialogResult result = MessageBox.Show("Вы уверены, что хотите удалить это мероприятие?", "Подтверждение удаления", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        Удалить_Мероприятие(Мероприятие_ID);
                        Отобразить_Мероприятия();
                    }
                }
                else
                {
                    MessageBox.Show("Выберите мероприятие для удаления.");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (добавитьToolStripMenuItem.Checked)
            {
                Добавить_Сотрудника();
                Отобразить_Сотрудников();
            }
            else if (изменитьToolStripMenuItem.Checked)
            {
                Изменить_Сотрудника();
                Отобразить_Сотрудников();
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            if (добавитьToolStripMenuItem.Checked)
            {
                Добавить_ДТП();
                Отобразить_ДТП();
            }
            else if (изменитьToolStripMenuItem.Checked)
            {
                Изменить_ДТП();
                Отобразить_ДТП();
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            if (добавитьToolStripMenuItem.Checked)
            {
                Добавить_Меро();
                Отобразить_Мероприятия();
            }
            else if (изменитьToolStripMenuItem.Checked)
            {
                Изменить_Меро();
                Отобразить_Мероприятия();
            }
        }
        private void Изменить()
        {
            try
            {
                if (сотрудникиToolStripMenuItem.Checked)
                {
                    UpdatePanelForTableИзменить("Сотрудники");
                }
                else if(дТПToolStripMenuItem.Checked)
                {
                    UpdatePanelForTableИзменить("ДТП");
                }
                else if (мероприятияToolStripMenuItem.Checked)
                {
                    UpdatePanelForTableИзменить("Мероприятия");
                }
                else
                {
                    // Если ни один пункт не выбран, скрываем все панели
                    UpdatePanelForTableИзменить("");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при реализации: " + ex.Message);
            }
        }
        private void Добавить()
        {
            try
            {
                // Определяем, какую таблицу выбрал пользователь и вызываем метод обновления панели
                if (сотрудникиToolStripMenuItem.Checked)
                {
                    UpdatePanelForTableДобавить("Сотрудники");
                }
                else if (дТПToolStripMenuItem.Checked)
                {
                    UpdatePanelForTableДобавить("ДТП");
                }
                else if (мероприятияToolStripMenuItem.Checked)
                {
                    UpdatePanelForTableДобавить("Мероприятия");
                }
                else
                {
                    // Если ни один пункт не выбран, скрываем все панели
                    UpdatePanelForTableДобавить("");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Произошла ошибка при реализации: " + ex.Message);
            }
        }

        private void добавитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem a in действияСТаблицейToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            Добавить();
            if (сотрудникиToolStripMenuItem.Checked)
            {
                textBox2.Clear();
                textBox3.Clear();
                maskedTextBox1.Clear();
                textBox4.Clear();
            }
            else if (дТПToolStripMenuItem.Checked)
            {
                maskedTextBox2.Clear();
                textBox5.Clear();
                richTextBox1.Text = "";
                comboBox1.Text = "";
                maskedTextBox3.Clear();
            }
            else if (мероприятияToolStripMenuItem.Checked)
            {
                domainUpDown1.Text = "";
                maskedTextBox4.Clear();
                richTextBox2.Text = "";
                domainUpDown2.Text = "";
            }
        }
        private void UpdatePanelForTableИзменить(string tableName)
        {

            switch (tableName)
            {
                case "Сотрудники":
                    panel_Сотрудники.Visible = true;
                    label1.Text = "Сотрудник Изменить";
                    button2.Text = "Изменить";
                    break;
                case "ДТП":
                    panel_дтп.Visible = true;
                    label1.Text = "ДТП Изменить";
                    button3.Text = "Изменить";
                    break;
                case "Мероприятия":
                    panel_МЕРОПРИЯТИЯ.Visible = true;
                    label1.Text = "Мероприятия Изменить";
                    button4.Text = "Изменить";
                    break;
                default:
                    break;
            }
        }
        private void UpdatePanelForTableДобавить(string tableName)
        {
            switch (tableName)
            {
                case "Сотрудники":
                    panel_Сотрудники.Visible = true;
                    label1.Text = "Сотрудники Добавить";
                    button2.Text = "Добавить";
                    break;
                case "ДТП":
                    panel_дтп.Visible = true;
                    label1.Text = "ДТП Добавить";
                    button3.Text = "Добавить";
                    break;
                case "Мероприятия":
                    panel_МЕРОПРИЯТИЯ.Visible = true;
                    label1.Text = "Мероприятия Добавить";
                    button4.Text = "Добавить";
                    break;
                default:
                    break;
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!добавитьToolStripMenuItem.Checked)
            {
                if (сотрудникиToolStripMenuItem.Checked)
                {
                    try
                    {
                        if (e.RowIndex >= 0 && изменитьToolStripMenuItem.Checked == true) // проверяем, что выбрана строка, а не заголовок или пустое пространство
                        {
                            try
                            {
                                DataGridViewRow row = dataGridView1.Rows[e.RowIndex];
                            textBox2.Text = row.Cells["ФИО"].Value.ToString();
                            textBox3.Text = row.Cells["Должность"].Value.ToString();
                            maskedTextBox1.Text = row.Cells["Телефон"].Value.ToString();
                            textBox4.Text = row.Cells["Email"].Value.ToString();
                            textBox6.Text = row.Cells["Сотрудник_ID"].Value.ToString();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Произошла ошибка при реализации: " + ex.Message);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Вы находитесь вне границ таблицы: " + ex.Message);
                    }
                }
                else if (дТПToolStripMenuItem.Checked)
                {
                    try
                    {
                        if (e.RowIndex >= 0 && изменитьToolStripMenuItem.Checked == true) // проверяем, что выбрана строка, а не заголовок или пустое пространство
                        {
                            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                            // Заполняем поля текущими значениями выбранной строки
                            DateTime дата;
                            if (DateTime.TryParse(row.Cells["Дата"].Value.ToString(), out дата))
                            {
                                maskedTextBox2.Text = дата.ToString("d"); // Формат даты по желанию; "d" - короткий формат
                            }
                            else
                            {
                                MessageBox.Show("Некорректный формат даты.");
                                return;
                            }
                            textBox5.Text = row.Cells["Место"].Value.ToString();
                            richTextBox1.Text = row.Cells["Описание"].Value.ToString();
                            comboBox1.Text = row.Cells["Степень_серьезности"].Value.ToString();
                            maskedTextBox3.Text = row.Cells["Создано"].Value.ToString();
                            textBox7.Text = row.Cells["ДТП_ID"].Value.ToString();

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Вы находитесь вне границ таблицы: " + ex.Message);
                    }
                }
                else if (мероприятияToolStripMenuItem.Checked)
                {
                    try
                    {
                        if (e.RowIndex >= 0 && изменитьToolStripMenuItem.Checked == true) // проверяем, что выбрана строка, а не заголовок или пустое пространство
                        {
                            DataGridViewRow row = dataGridView1.Rows[e.RowIndex];

                            domainUpDown1.Text = row.Cells["ДТП_ID"].Value.ToString();
                            maskedTextBox4.Text = row.Cells["Дата_мероприятия"].Value.ToString();
                            richTextBox2.Text = row.Cells["Описание_мероприятия"].Value.ToString();
                            domainUpDown2.Text = row.Cells["Сотрудник_ID"].Value.ToString();
                            textBox8.Text = row.Cells["Мероприятие_ID"].Value.ToString();

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Вы находитесь вне границ таблицы: " + ex.Message);
                    }
                }
            }
        }
        private void изменитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem a in действияСТаблицейToolStripMenuItem.DropDownItems)
            {
                a.Checked = false;
            }
            ((ToolStripMenuItem)sender).Checked = true;
            Изменить();
            try
            {
                dataGridView1_CellClick(dataGridView1, new DataGridViewCellEventArgs(0, dataGridView1.SelectedRows[0].Index));
            }
            catch
            {

            }
        }

        
    }
}
