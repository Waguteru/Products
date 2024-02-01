using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;


namespace Products
{
    public partial class Form1 : Form
    {

        DataBase dataBase = new DataBase();

        int selectedRow;

        enum RowState
        {
            Existed,
            New,
            Modfied,
            ModfiedNew,
            Deleted
        }

        public Form1()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }


        private void CreateColumns()
        {
            dataGridView1.Columns.Add("id_products", "Id товара"); //0
            dataGridView1.Columns.Add("name_products", "название"); //1
            dataGridView1.Columns.Add("price_products", "цена"); //2
            dataGridView1.Columns.Add("type_products", "категория"); //3
            dataGridView1.Columns.Add("article", "артикул"); //4
        }

        private void ReadSingleRow(DataGridView dgw, IDataRecord record)
        {
            dgw.Rows.Add(record.GetInt32(0), record.GetString(1), record.GetInt64(2), record.GetString(3), record.GetInt64(4), RowState.ModfiedNew);
        }

        private void RefreshDataGrid(DataGridView dgw)
        {
            dgw.Rows.Clear();
            string queryString = $"select * from products_tbl";

            NpgsqlCommand command = new NpgsqlCommand(queryString, dataBase.GetConnecting());

            dataBase.OpenConnecting();

            NpgsqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                ReadSingleRow(dgw, reader);
            }
            reader.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CreateColumns();
            RefreshDataGrid(dataGridView1);
        }


        private void Search(DataGridView dgw)
        {
            dgw.Rows.Clear();

            string searchString = $"select * from products_tbl where concat (name_products, price_products) like '%" + textBox1.Text + "%'";

            NpgsqlCommand comm = new NpgsqlCommand(searchString, dataBase.GetConnecting());

            dataBase.OpenConnecting();

            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }

            read.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Search(dataGridView1);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == "название (по возрастанию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Ascending);
            }
            if (comboBox1.SelectedItem == "категория (по возрастанию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Ascending);
            }
            if (comboBox1.SelectedItem == "цена (по возрастанию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Ascending);
            }


            if (comboBox1.SelectedItem == "название (по убыванию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[1], ListSortDirection.Descending);
            }
            if (comboBox1.SelectedItem == "категория (по убыванию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[3], ListSortDirection.Descending);
            }
            if (comboBox1.SelectedItem == "цена (по убыванию)")
            {
                dataGridView1.Sort(dataGridView1.Columns[2], ListSortDirection.Descending);
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            selectedRow = e.RowIndex;

            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dataGridView1.Rows[selectedRow];

                textBox2.Text = row.Cells[1].Value.ToString();
                textBox3.Text = row.Cells[3].Value.ToString();
                textBox4.Text = row.Cells[2].Value.ToString();
                textBox5.Text = row.Cells[4].Value.ToString();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QRCoder.QRCodeGenerator QR = new QRCoder.QRCodeGenerator();
            var MyData = QR.CreateQrCode("название товара: " + textBox2.Text + "\n" + "категория товара: " + textBox3.Text + "\n" + "цена: " + textBox4.Text, QRCoder.QRCodeGenerator.ECCLevel.H);
            var code = new QRCoder.QRCode(MyData);
            pictureBox1.Image = code.GetGraphic(50);
        }

        private void DrawBarcode(string code, int resolution = 20) // resolution - пикселей на миллиметр
        {
            int numberCount = 15; // количество цифр
            float height = 25.93f * resolution; // высота штрих кода
            float lineHeight = 22.85f * resolution; // высота штриха
            float leftOffset = 3.63f * resolution; // свободная зона слева
            float rightOffset = 2.31f * resolution; // свободная зона справа
                                                    //штрихи, которые образуют правый и левый ограничивающие знаки,
                                                    //а также центральный ограничивающий знак должны быть удлинены вниз на 1,65мм
            float longLineHeight = lineHeight + 1.65f * resolution;
            float fontHeight = 2.75f * resolution; // высота цифр
            float lineToFontOffset = 0.165f * resolution; // минимальный размер от верхнего края цифр до нижнего края штрихов
            float lineWidthDelta = 0.15f * resolution; // ширина 0.15*{цифра}
            float lineWidthFull = 1.35f * resolution; // ширина белой полоски при 0 или 0.15*9
            float lineOffset = 0.2f * resolution; // между штрихами должно быть расстояние в 0.2мм

            float width = leftOffset + rightOffset + 6 * (lineWidthDelta + lineOffset) + numberCount * (lineWidthFull + lineOffset); // ширина штрих-кода

            Bitmap bitmap = new Bitmap((int)width, (int)height); // создание картинки нужных размеров
            Graphics g = Graphics.FromImage(bitmap); // создание графики

            Font font = new Font("Arial", fontHeight, FontStyle.Regular, GraphicsUnit.Pixel); // создание шрифта

            StringFormat fontFormat = new StringFormat(); // Центрирование текста
            fontFormat.Alignment = StringAlignment.Center;
            fontFormat.LineAlignment = StringAlignment.Center;

            float x = leftOffset; // позиция рисования по x
            for (int i = 0; i < numberCount; i++)
            {
                int number = Convert.ToInt32(code[i].ToString()); // число из кода
                if (number != 0)
                {
                    g.FillRectangle(Brushes.Black, x, 0, number * lineWidthDelta, lineHeight); // рисуем штрих
                }
                RectangleF fontRect = new RectangleF(x, lineHeight + lineToFontOffset, lineWidthFull, fontHeight); // рамки для буквы
                g.DrawString(code[i].ToString(), font, Brushes.Black, fontRect, fontFormat); // рисуем букву
                x += lineWidthFull + lineOffset; // смещаем позицию рисования по x
                if (i == 0 && i == numberCount / 2 && i == numberCount - 1) // если это начало, середина или конец кода рисуем разделители
                {
                    for (int j = 0; j < 2; j++) // рисуем 2 линии разделителя
                    {
                        g.FillRectangle(Brushes.Black, x, 0, lineWidthDelta, longLineHeight); // рисуем длинный штрих
                        x += lineWidthDelta + lineOffset; // смещаем позицию рисования по x
                    }
                }
            }
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom; // делаем чтобы картинка помещалась в pictureBox
            pictureBox2.Image = bitmap; // устанавливаем картинку
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DrawBarcode(textBox5.Text);
        }


        public void AllTypes(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnecting();

            string query = $"select * from products_tbl";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnecting());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnecting();
        }

        public void Home(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnecting();

            string query = $"select * from products_tbl where type_products LIKE 'для дома'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnecting());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnecting();
        }

        public void Pet(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnecting();

            string query = $"select * from products_tbl where type_products LIKE 'животные'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnecting());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnecting();
        }

        public void Electronics(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnecting();

            string query = $"select * from products_tbl where type_products LIKE 'электроника'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnecting());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnecting();
        }

        public void Clother(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnecting();

            string query = $"select * from products_tbl where type_products LIKE 'одежда'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnecting());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnecting();
        }

        public void Meal(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnecting();

            string query = $"select * from products_tbl where type_products LIKE 'еда'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnecting());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnecting();
        }

        public void Book(DataGridView dgw)
        {
            dgw.Rows.Clear();

            dataBase.OpenConnecting();

            string query = $"select * from products_tbl where type_products LIKE 'книги'";
            NpgsqlCommand comm = new NpgsqlCommand(query, dataBase.GetConnecting());
            NpgsqlDataReader read = comm.ExecuteReader();

            while (read.Read())
            {
                ReadSingleRow(dgw, read);
            }
            read.Close();
            dataBase.CloseConnecting();
        }



        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedItem == "Все типы")
            {
                AllTypes(dataGridView1);
            }
            if (comboBox2.SelectedItem == "для дома")
            {
                Home(dataGridView1);
            }
            if (comboBox2.SelectedItem == "животные")
            {
                Pet(dataGridView1);
            }
            if (comboBox2.SelectedItem == "электроника")
            {
                Electronics(dataGridView1);
            }
            if (comboBox2.SelectedItem == "одежда")
            {
                Clother(dataGridView1);
            }
            if (comboBox2.SelectedItem == "еда")
            {
                Meal(dataGridView1);
            }
            if (comboBox2.SelectedItem == "книги")
            {
                Book(dataGridView1);
            }
        }
    }
}

