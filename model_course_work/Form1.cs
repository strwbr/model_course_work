using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;
using CheckBox = System.Windows.Forms.CheckBox;

namespace model_course_work
{
    public partial class Form1 : Form
    {
        uint A = 0;
        uint B = 0;
        bool is_start = false;
        uint C = 0;
        int D = 0;
        Model Pr1 = new Model();

        public Form1()
        {
            InitializeComponent();

            table_C.Rows.Add();
            table_A.Rows.Add();
            table_B.Rows.Add();
            table_AM.Rows.Add();
            table_BM.Rows.Add();

            textBox_a.Text = "0";
            textBox_b.Text = "0";
            // Инициализация таблиц переменных
            InitDataTables();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            table_AM.ClearSelection();
            table_BM.ClearSelection();
            table_A.ClearSelection();
            table_B.ClearSelection();
            table_C.ClearSelection();
        }

        private void start_Click(object sender, EventArgs e) // пуск
        {
            //Model Program = new Model(A, B, C, D);
            Pr1 = new Model(A, B);

            groupBox2.Enabled = false;
            groupBox4.Enabled = false;
            table_A.ClearSelection();
            table_B.ClearSelection();
            start.Enabled = false;
            is_start = true;

            Pr1.Run();

            table_A.Enabled = false;
            table_B.Enabled = false;
            checkBox_a0.Checked = true;
            if (radioButton_auto.Checked)
            {
                while (!Pr1.Stop)
                {
                    if (radioButton_MP.Checked)
                    {
                        Pr1.Microprogram();
                        DisplayStatesInGSA(Pr1.State);
                    }
                    else
                    {
                        OAandYA();
                    }
                    Pr1.ShowValues(ref table_AM, ref table_BM, ref table_C, ref data_CR);
                }
                C = Pr1.C;
                ShowResult();

                is_start = false;
                start.Enabled = false;
                button_tact.Enabled = false;
                return;
            }
            button_tact.Enabled = true;
        }

        private void button_tact_Click(object sender, EventArgs e) //такт
        {
            if (radioButton_tact.Checked)
            {
                if (is_start)
                {
                    if (!Pr1.Stop)
                    {
                        if (radioButton_MP.Checked)
                        {
                            Pr1.Microprogram();
                            DisplayStatesInGSA(Pr1.State);
                        }
                        else
                        {
                            OAandYA();
                        }
                        Pr1.ShowValues(ref table_AM, ref table_BM,
                            ref table_C, ref data_CR);
                    }
                    else
                    {
                        C = Pr1.C;
                        ShowResult();
                        is_start = false;
                        start.Enabled = false;
                        button_tact.Enabled = false;
                    }
                }
            }
        }

        //заполнение чекбоксов на гса
        private void DisplayStatesInGSA(int state /*Model Program*/)
        {
            ResetCheckboxesInGSA();
            switch (state)
            {
                case 0:
                    checkBox_a0.Checked = true; break;
                case 1:
                    checkBox_a1.Checked = true; break;
                case 2:
                    checkBox_a2.Checked = true; break;
                case 3:
                    checkBox_a3.Checked = true; break;
                case 4:
                    checkBox_a4.Checked = true; break;
                case 5:
                    checkBox_a5.Checked = true; break;
                case 6:
                    checkBox_a6.Checked = true; break;
                case 7:
                    checkBox_a7.Checked = true; break;
                case 8:
                    checkBox_a8.Checked = true; break;
                case 9:
                    checkBox_a.Checked = true; break;
            }
        }

        private void reset_Click(object sender, EventArgs e) //сброс
        {
            A = B = C = 0;
            //B = 0;
            //C = 0;
            D = 0;
            is_start = false;
            start.Enabled = true;
            Pr1 = new Model();
            button_tact.Enabled = false;
            textBox_a.Text = "0";
            textBox_b.Text = "0";
            textBox_c.Text = "";
            table_A.Enabled = true;
            table_B.Enabled = true;

            ResetCheckboxesInGSA();
            checkBox_a0.Checked = true;

            ResetCheckboxesInOAandYA();
            checkBoxa0.Checked = true;

            groupBox2.Enabled = true;
            groupBox4.Enabled = true;
            InitDataTables();
        }

        // Обработчик нажатия по ячейкам таблицы ввода числа А
        private void data_a_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Индекс колонки, на которую нажал пользователь
            int column = e.ColumnIndex;
            table_A[column, 0].Value = (table_A[column, 0].Value.ToString() == "0") ? 1 : 0;

            A = GetUintValue(table_A);
            textBox_a.Text = GetDecimalValue(table_A);
        }

        // Обработчик нажатия по ячейкам таблицы ввода числа В
        private void data_b_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Индекс колонки, на которую нажал пользователь
            int column = e.ColumnIndex;
            table_B[column, 0].Value = (table_B[column, 0].Value.ToString() == "0") ? 1 : 0;

            B = GetUintValue(table_B);
            textBox_b.Text = GetDecimalValue(table_B);
        }

        // Преобразование 2-ичной СЧ в 10-ичную
        private uint GetUintValue(DataGridView dataGridView)
        {
            // Получение строкового представления двоичного числа
            string temp = "";
            for (int i = 0; i < dataGridView.ColumnCount; i++)
            {
                temp += dataGridView[i, 0].Value.ToString();
            }
            return Convert.ToUInt16(temp, 2);
        }

        // Получение числа из DataGridView в 10-ичной СЧ
        private string GetDecimalValue(DataGridView dataGridView)
        {
            double result = 0;
            // Преобразование 2-ичной СЧ в 10-ичную
            // Знаковый бит игнорируется
            for (int i = 1; i < dataGridView.ColumnCount; i++)
            {
                result += Double.Parse(dataGridView[i, 0].Value.ToString()) * Math.Pow(2, -i);
            }
            result = Math.Round(result, 5);

            string resultStr = result.ToString();
            // Получение знака числа
            if (dataGridView[0, 0].Value.ToString() == "1")
                resultStr = resultStr.Insert(0, "-");

            return resultStr;
        }

        private void ShowResult() //вывод результата в текстбокс на последнем такте
        {
            if (Pr1.GetPP() == 1)
            {
                textBox_c.Text = "Переполнение";
                return;
            }
            //заполняем массив 0 и 1 в цикле по всему регистру С
            for (int i = 16; i >= 0; i--)
            {
                table_C[i, 0].Value = C % 2;
                C = (UInt32)(C / 2);
            }

            textBox_c.Text = GetDecimalValue(table_C);
        }

        private void OAandYA()
        {
            ShowQ(Pr1.Get_Dt());

            // Память логических условий (ПЛУ)
            Pr1.Logic_Memory_Cond(Pr1.Get_X());

            // Комбинационная схема Y (КСY)
            Pr1.KSY(Pr1.Get_X());
            ShowY(Pr1.Get_Y());

            // Операционный автомат (ОА)
            Pr1.OA(Pr1.Get_Y());
            ShowX(Pr1.Get_X());

            // Комбинация схема D (КСD)
            Pr1.KSD(Pr1.Get_X());
            ShowD(Pr1.Get_Dt());

            StateChanged(Pr1.Get_Dt());
        }

        //Вывод Y на форму
        private void ShowY(byte[] Y)
        {
            for (int i = 0; i < Y.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        checkBox_yk.Checked = (Y[i] != 0); break;
                    case 1:
                        checkBox_y1.Checked = (Y[i] != 0); break;
                    case 2:
                        checkBox_y2.Checked = (Y[i] != 0); break;
                    case 3:
                        checkBox_y3.Checked = (Y[i] != 0); break;
                    case 4:
                        checkBox_y4.Checked = (Y[i] != 0); break;
                    case 5:
                        checkBox_y5.Checked = (Y[i] != 0); break;
                    case 6:
                        checkBox_y6.Checked = (Y[i] != 0); break;
                    case 7:
                        checkBox_y7.Checked = (Y[i] != 0); break;
                    case 8:
                        checkBox_y8.Checked = (Y[i] != 0); break;
                    case 9:
                        checkBox_y9.Checked = (Y[i] != 0); break;
                    case 10:
                        checkBox_y10.Checked = (Y[i] != 0); break;
                    case 11:
                        checkBox_y11.Checked = (Y[i] != 0); break;
                    case 12:
                        checkBox_y12.Checked = (Y[i] != 0); break;
                    case 13:
                        checkBox_y13.Checked = (Y[i] != 0); break;
                    case 14:
                        checkBox_y14.Checked = (Y[i] != 0); break;
                    case 15:
                        checkBox_y15.Checked = (Y[i] != 0); break;
                    case 16:
                        checkBox_y16.Checked = (Y[i] != 0); break;
                    case 17:
                        checkBox_y17.Checked = (Y[i] != 0); break;
                }
            }
        }

        // Вывод X на форму
        private void ShowX(bool[] X)
        {
            for (int i = 0; i < X.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        checkBox_x0.Checked = X[i];
                        break;
                    case 1:
                        checkBox_x1.Checked = X[i];
                        break;
                    case 2:
                        checkBox_x2.Checked = Pr1.Get_X2();
                        break;
                    case 3:
                        checkBox_x3.Checked = Pr1.Get_X3();
                        break;
                    case 4:
                        checkBox_x4.Checked = X[i];
                        break;
                    case 5:
                        checkBox_x5.Checked = Pr1.Get_X5();
                        break;
                    case 6:
                        checkBox_x6.Checked = X[i];
                        break;
                }
            }
        }

        //Вывод Q на форму
        private void ShowQ(int Dtr)
        {
            string binar = Convert.ToString(Dtr, 2);
            if (binar.Length == 1) binar = "000" + binar;
            if (binar.Length == 2) binar = "00" + binar;
            if (binar.Length == 3) binar = "0" + binar;
            checkBox_Q0.Checked = (binar[3] != '0');
            checkBox_Q1.Checked = (binar[2] != '0');
            checkBox_Q2.Checked = (binar[1] != '0');
            checkBox_Q3.Checked = (binar[0] != '0');
        }

        //Вывод D на форму
        private void ShowD(int Dtr)
        {
            string binar = Convert.ToString(Dtr, 2);
            if (binar.Length == 1) binar = "000" + binar;
            if (binar.Length == 2) binar = "00" + binar;
            if (binar.Length == 3) binar = "0" + binar;
            checkBox_D0.Checked = (binar[3] != '0');
            checkBox_D1.Checked = (binar[2] != '0');
            checkBox_D2.Checked = (binar[1] != '0');
            checkBox_D3.Checked = (binar[0] != '0');
        }

        //Вывод A на форму
        private void StateChanged(int state)
        {
            ResetCheckboxesInGSA();
            ResetCheckboxesForStatesInOAandYA();
            switch (state)
            {
                case 0:
                    checkBox_a.Checked = true;
                    checkBoxa0.Checked = true;
                    break;
                case 1:
                    checkBox_a1.Checked = true;
                    checkBoxa1.Checked = true;
                    break;
                case 2:
                    checkBox_a2.Checked = true;
                    checkBoxa2.Checked = true;
                    break;
                case 3:
                    checkBox_a3.Checked = true;
                    checkBoxa3.Checked = true;
                    break;
                case 4:
                    checkBox_a4.Checked = true;
                    checkBoxa4.Checked = true;
                    break;
                case 5:
                    checkBox_a5.Checked = true;
                    checkBoxa5.Checked = true;
                    break;
                case 6:
                    checkBox_a6.Checked = true;
                    checkBoxa6.Checked = true;
                    break;
                case 7:
                    checkBox_a7.Checked = true;
                    checkBoxa7.Checked = true;
                    break;
                case 8:
                    checkBox_a8.Checked = true;
                    checkBoxa8.Checked = true;
                    break;
            }
        }

        // Заполнение строк таблиц с битами чисел нулями
        public void InitDataTables()
        {
            for (int i = 0; i < 16; i++)
            {
                table_A[i, 0].Value = 0;
                table_B[i, 0].Value = 0;
                table_AM[i, 0].Value = 0;
                table_BM[i, 0].Value = 0;
                table_AM[i + 16, 0].Value = 0;
                table_BM[i + 16, 0].Value = 0;
                table_C[i, 0].Value = 0;
                table_C[i, 0].Value = 0;
                if (i < 4)
                    data_CR[i, 0].Value = 0;
            }
            table_C[16, 0].Value = 0;
        }

        // Сброс флажков состояний на ГСА
        private void ResetCheckboxesInGSA()
        {
            foreach (CheckBox cb in panel1.Controls.OfType<CheckBox>())
            {
                cb.Checked = false;
            }
        }

        // Сброс всех флажков для ОА и УА
        private void ResetCheckboxesInOAandYA()
        {
            foreach (CheckBox cb in panel2.Controls.OfType<CheckBox>())
            {
                cb.Checked = false;
            }
        }

        // Сброс флажков, отображающий состояния для ОА и УА
        private void ResetCheckboxesForStatesInOAandYA()
        {
            foreach (CheckBox cb in panel2.Controls.OfType<CheckBox>())
            {
                string nameCb = cb.Text;
                if (nameCb[0] == 'a')
                {
                    cb.Checked = false;
                }
            }
        }
    }
}
