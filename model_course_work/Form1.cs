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
        Model Pr1 = new Model();

        public Form1()
        {
            InitializeComponent();
            // Добавление строки в таблицы операндов
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

            groupBox2.Enabled = false;
            groupBox4.Enabled = false;
            table_A.ClearSelection();
            table_B.ClearSelection();
            start.Enabled = false;

            Pr1 = new Model(A, B);
            Pr1.Run();
            
            is_start = true;
            table_A.Enabled = false;
            table_B.Enabled = false;
            checkBox_a0.Checked = true;
            // Если выбран автоматический режим
            if (Auto_rb.Checked)
            {
                while (!Pr1.Stop)
                {
                    // Если выбран режим микропрограммы
                    if (MP_rb.Checked)
                    {
                        Pr1.Microprogram();
                        DisplayStatesInGSA(Pr1.State);
                    } // режим ОА и УА
                    else
                        OAandYA();
                    // Вывод операндов на форму
                    Pr1.ViewOperands(ref table_AM, ref table_BM, ref table_C, ref table_CR);
                }
                C = Pr1.C;
                // Вывод результата - переменной С
                ViewC();

                is_start = false;
                start.Enabled = false;
                button_tact.Enabled = false;
                return;
            }
            // Активация кнопки такта
            button_tact.Enabled = true;
        }
        
        // Обработчик нажатия на кнопку Такт
        private void TactBtn_Click(object sender, EventArgs e) 
        {
            // Если выбран пошаговый режим
            if (Tact_rb.Checked)
            {
                if (is_start)
                {
                    if (!Pr1.Stop)
                    {
                        if (MP_rb.Checked)
                        {
                            Pr1.Microprogram();
                            DisplayStatesInGSA(Pr1.State);
                        }
                        else
                        {
                            OAandYA();
                        }
                        Pr1.ViewOperands(ref table_AM, ref table_BM,
                            ref table_C, ref table_CR);
                    }
                    else
                    {
                        C = Pr1.C;
                        ViewC();
                        is_start = false;
                        start.Enabled = false;
                        button_tact.Enabled = false;
                    }
                }
            }
        }

        // Отображение состояния на ГСА
        private void DisplayStatesInGSA(int state)
        {
            // Сброс
            ResetCheckboxesInGSA();
            switch (state)
            {
                case 0:
                    checkBox_a0.Checked = true; break;
                case 1:
                    gsaA1_cb.Checked = true; break;
                case 2:
                    gsaA2_cb.Checked = true; break;
                case 3:
                    gsaA3_cb.Checked = true; break;
                case 4:
                    gsaA4_cb.Checked = true; break;
                case 5:
                    gsaA5_cb.Checked = true; break;
                case 6:
                    gsaA6_cb.Checked = true; break;
                case 7:
                    gsaA7_cb.Checked = true; break;
                case 8:
                    gsaA8_cb.Checked = true; break;
                case 9:
                    gsaAk_cb.Checked = true; break;
            }
        }

        // Обработчик нажатия на кнопку Сброс
        private void ResetBtn_Click(object sender, EventArgs e) //сброс
        {
            A = B = C = 0;
            //B = 0;
            //C = 0;
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
            A0_cb.Checked = true;

            groupBox2.Enabled = true;
            groupBox4.Enabled = true;
            InitDataTables();
        }

        // Обработчик нажатия по ячейкам таблицы ввода числа А
        private void Data_a_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Индекс колонки, на которую нажал пользователь
            int column = e.ColumnIndex;
            table_A[column, 0].Value = (table_A[column, 0].Value.ToString() == "0") ? 1 : 0;

            A = GetUintValue(table_A);
            textBox_a.Text = GetDecimalValue(table_A);
        }

        // Обработчик нажатия по ячейкам таблицы ввода числа В
        private void Data_b_CellClick(object sender, DataGridViewCellEventArgs e)
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

        private void OAandYA()
        {
            ViewQ(Pr1.Dt);

            // Память логических условий (ПЛУ)
            Pr1.LogicCondMemory(Pr1.X);

            // Комбинационная схема Y (КСY)
            Pr1.KSY(Pr1.X);
            ViewY(Pr1.Y);

            // Операционный автомат (ОА)
            Pr1.OA(Pr1.Y);
            ViewX(Pr1.X);

            // Комбинация схема D (КСD)
            Pr1.KSD(Pr1.X);
            ViewDt(Pr1.Dt);

            StateChanged(Pr1.Dt);
        }

        // Вывод Y на форму
        private void ViewY(bool[] Y)
        {
            for (int i = 0; i < Y.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        Yk_cb.Checked = Y[i]; break;
                    case 1:
                        Y1_cb.Checked = Y[i]; break;
                    case 2:
                        Y2_cb.Checked = Y[i]; break;
                    case 3:
                        Y3_cb.Checked = Y[i]; break;
                    case 4:
                        Y4_cb.Checked = Y[i]; break;
                    case 5:
                        Y5_cb.Checked = Y[i]; break;
                    case 6:
                        Y6_cb.Checked = Y[i]; break;
                    case 7:
                        Y7_cb.Checked = Y[i]; break;
                    case 8:
                        Y8_cb.Checked = Y[i]; break;
                    case 9:
                        Y9_cb.Checked = Y[i]; break;
                    case 10:
                        Y10_cb.Checked = Y[i]; break;
                    case 11:
                        Y11_cb.Checked = Y[i]; break;
                    case 12:
                        Y12_cb.Checked = Y[i]; break;
                    case 13:
                        Y13_cb.Checked = Y[i]; break;
                    case 14:
                        Y14_cb.Checked = Y[i]; break;
                    case 15:
                        Y15_cb.Checked = Y[i]; break;
                    case 16:
                        Y16_cb.Checked = Y[i]; break;
                    case 17:
                        Y17_cb.Checked = Y[i]; break;
                }
            }
        }

        // Вывод С на форму
        private void ViewC()
        {
            if (Pr1.PP == 1)
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

        // Вывод X на форму
        private void ViewX(bool[] X)
        {
            for (int i = 0; i < X.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        X0_cb.Checked = X[i]; break;
                    case 1:
                        X1_cb.Checked = X[i]; break;
                    case 2:
                        X2_cb.Checked = Pr1.Get_X2(); break;
                    case 3:
                        X3_cb.Checked = Pr1.Get_X3(); break;
                    case 4:
                        X4_cb.Checked = X[i]; break;
                    case 5:
                        X5_cb.Checked = Pr1.Get_X5(); break;
                    case 6:
                        X6_cb.Checked = X[i]; break;
                }
            }
        }

        // Вывод Q на форму
        private void ViewQ(int Q)
        {
            string strQ = ConvertTo4bit(Q);
            Q0_cb.Checked = (strQ[3] != '0');
            Q1_cb.Checked = (strQ[2] != '0');
            Q2_cb.Checked = (strQ[1] != '0');
            Q3_cb.Checked = (strQ[0] != '0');
        }

        // Вывод Dt на форму
        private void ViewDt(int Dt)
        {
            string strDt = ConvertTo4bit(Dt);
            D0_cb.Checked = (strDt[3] != '0');
            D1_cb.Checked = (strDt[2] != '0');
            D2_cb.Checked = (strDt[1] != '0');
            D3_cb.Checked = (strDt[0] != '0');
        }

        // Отображение состояния ГСА на форме
        private void StateChanged(int state)
        {
            ResetCheckboxesInGSA();
            ResetCheckboxesForStatesInOAandYA();
            switch (state)
            {
                case 0:
                    gsaAk_cb.Checked = true;
                    A0_cb.Checked = true;
                    break;
                case 1:
                    gsaA1_cb.Checked = true;
                    A1_cb.Checked = true;
                    break;
                case 2:
                    gsaA2_cb.Checked = true;
                    A2_cb.Checked = true;
                    break;
                case 3:
                    gsaA3_cb.Checked = true;
                    A3_cb.Checked = true;
                    break;
                case 4:
                    gsaA4_cb.Checked = true;
                    A4_cb.Checked = true;
                    break;
                case 5:
                    gsaA5_cb.Checked = true;
                    A5_cb.Checked = true;
                    break;
                case 6:
                    gsaA6_cb.Checked = true;
                    A6_cb.Checked = true;
                    break;
                case 7:
                    gsaA7_cb.Checked = true;
                    A7_cb.Checked = true;
                    break;
                case 8:
                    gsaA8_cb.Checked = true;
                    A8_cb.Checked = true;
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
                    table_CR[i, 0].Value = 0;
            }
            table_C[16, 0].Value = 0;
        }

        // Преобразование числа в строку длиной 4 элемента 
        private string ConvertTo4bit(int n)
        {
            string str = Convert.ToString(n, 2);
            if (str.Length == 1) str = "000" + str;
            if (str.Length == 2) str = "00" + str;
            if (str.Length == 3) str = "0" + str;
            return str;
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
