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
    // Класс визуального слоя
    public partial class MainForm : Form
    {
        uint A = 0;
        uint B = 0;
        bool is_start = false;
        uint C = 0;
        Model model = new Model();

        public MainForm()
        {
            InitializeComponent();
            // Добавление строки в таблицы операндов
            Table_C.Rows.Add();
            Table_A.Rows.Add();
            Table_B.Rows.Add();
            Table_AM.Rows.Add();
            Table_BM.Rows.Add();

            InputA_tb.Text = "0";
            InputB_tb.Text = "0";
            // Инициализация таблиц переменных
            InitDataTables();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Table_AM.ClearSelection();
            Table_BM.ClearSelection();
            Table_A.ClearSelection();
            Table_B.ClearSelection();
            Table_C.ClearSelection();
        }

        private void StartBtn_Click(object sender, EventArgs e) // пуск
        {
            //Model Program = new Model(A, B, C, D);
            // Отключение элементов для выбора режимов
            ExeMode_gb.Enabled = false;
            ModelMode_gb.Enabled = false;
            // Очистка таблиц ввода А и В
            Table_A.ClearSelection();
            Table_B.ClearSelection();
            // Отключение кнопки Пуск
            StartBtn.Enabled = false;
            // Запуск моделирования
            model = new Model(A, B);
            model.Run();

            is_start = true;
            // Отключение возможности ввода новых значений А и В
            Table_A.Enabled = false;
            Table_B.Enabled = false;
            gsaA0_cb.Checked = true;
            // Если выбран автоматический режим
            if (Auto_rb.Checked)
            {
                while (!model.Stop)
                {
                    // Если выбран режим микропрограммы
                    if (MP_rb.Checked)
                    {
                        model.Microprogram();
                        DisplayStatesInGSA(model.State);
                    } // режим ОА и УА
                    else
                        TactOAandYA();
                    // Вывод операндов на форму
                    model.ViewOperands(ref Table_AM, ref Table_BM, ref Table_C, ref table_CR);
                }
                C = model.C;
                // Вывод результата - переменной С
                ViewC();

                is_start = false;
                StartBtn.Enabled = false;
                TactBtn.Enabled = false;
                return;
            }
            // Активация кнопки такта
            TactBtn.Enabled = true;
        }

        // Обработчик нажатия на кнопку Такт
        private void TactBtn_Click(object sender, EventArgs e)
        {
            // Если выбран пошаговый режим
            if (Tact_rb.Checked)
            {
                if (is_start)
                {
                    if (!model.Stop)
                    {
                        if (MP_rb.Checked)
                        {
                            model.Microprogram();
                            DisplayStatesInGSA(model.State);
                        }
                        else
                        {
                            TactOAandYA();
                        }
                        model.ViewOperands(ref Table_AM, ref Table_BM,
                            ref Table_C, ref table_CR);
                    }
                    else
                    {
                        C = model.C;
                        ViewC();
                        is_start = false;
                        StartBtn.Enabled = false;
                        TactBtn.Enabled = false;
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
                    gsaA0_cb.Checked = true; break;
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
            // Сброс всех параметров на начальные
            A = B = C = 0;
            is_start = false;
            StartBtn.Enabled = true;
            model = new Model();
            TactBtn.Enabled = false;
            InputA_tb.Text = "0";
            InputB_tb.Text = "0";
            ResC_tb.Text = "";
            Table_A.Enabled = true;
            Table_B.Enabled = true;
            // Сброс флажков, отображающих состояния на ГСА
            ResetCheckboxesInGSA();
            gsaA0_cb.Checked = true;
            // Сброс флажков на ОА и УА
            ResetCheckboxesInOAandYA();
            A0_cb.Checked = true;
            ExeMode_gb.Enabled = true;
            ModelMode_gb.Enabled = true;
            // Заполнение таблиц для операндов 0
            InitDataTables();
        }

        // Обработчик нажатия по ячейкам таблицы ввода числа А
        private void Data_a_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Индекс колонки, на которую нажал пользователь
            int column = e.ColumnIndex;
            // Изменение значения бита
            Table_A[column, 0].Value = (Table_A[column, 0].Value.ToString() == "0") ? 1 : 0;
            // Получение введенного пользователем значения А
            A = GetInputValue(Table_A);
            // Преобразование введенного значения А из 2-ичной в 10-ичную СЧ и вывод на форму
            InputA_tb.Text = GetDecimalValue(Table_A);
        }

        // Обработчик нажатия по ячейкам таблицы ввода числа В
        private void Data_b_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Индекс колонки, на которую нажал пользователь
            int column = e.ColumnIndex;
            // Изменение значения бита
            Table_B[column, 0].Value = (Table_B[column, 0].Value.ToString() == "0") ? 1 : 0;
            // Получение введенного пользователем значения В
            B = GetInputValue(Table_B);
            // Преобразование введенного значения А из 2-ичной в 10-ичную СЧ и вывод на форму
            InputB_tb.Text = GetDecimalValue(Table_B);
        }

        // Получение введенного числа
        private uint GetInputValue(DataGridView dataGridView)
        {
            // Получение строкового представления двоичного числа
            string temp = "";
            for (int i = 0; i < dataGridView.ColumnCount; i++)
            {
                temp += dataGridView[i, 0].Value.ToString();
            }
            return Convert.ToUInt16(temp, 2);
        }

        // Перевод числа из DataGridView (в 2-ичной СЧ) в 10-ичную СЧ
        private string GetDecimalValue(DataGridView dataGridView)
        {
            // Преобразование 2-ичной СЧ в 10-ичную
            // Знаковый бит игнорируется
            double result = 0;
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

        // Один такт ОА и УА
        private void TactOAandYA()
        {
            // Вывод Dt на форму
            ViewDt(model.Dt);
            // Память логических условий (ПЛУ)
            model.LogicCondMemory(model.X);
            // Память состояний (ПС)
            model.StateMemory();
            // Дешифратор
            model.Decoder();
            // Комбинационная схема Y (КСY)
            model.KSY(/*model.X*/);
            // Вывод вектора У на форму
            ViewY(model.Y);
            // Операционный автомат (ОА)
            model.OA(/*model.Y*/);
            // Вывод вектора Х на форму
            ViewX(model.X);
            // Комбинация схема D (КСD)
            model.KSD(/*model.X*/);
            // Вывод триггеров D на форму
            ViewD(model.Dt);
            // Вывод текущего состояния на форму
            ViewState(model.Dt);
        }

        // Вывод вектора Y на форму
        private void ViewY(bool[] Y)
        {
            for (int i = 0; i < Y.Length; i++)
            {
                // Для каждого Y соответствующий ему флажок переводится в его текущее состояние
                switch (i)
                {
                    case 0: Yk_cb.Checked = Y[i]; break;
                    case 1: Y1_cb.Checked = Y[i]; break;
                    case 2: Y2_cb.Checked = Y[i]; break;
                    case 3: Y3_cb.Checked = Y[i]; break;
                    case 4: Y4_cb.Checked = Y[i]; break;
                    case 5: Y5_cb.Checked = Y[i]; break;
                    case 6: Y6_cb.Checked = Y[i]; break;
                    case 7: Y7_cb.Checked = Y[i]; break;
                    case 8: Y8_cb.Checked = Y[i]; break;
                    case 9: Y9_cb.Checked = Y[i]; break;
                    case 10: Y10_cb.Checked = Y[i]; break;
                    case 11: Y11_cb.Checked = Y[i]; break;
                    case 12: Y12_cb.Checked = Y[i]; break;
                    case 13: Y13_cb.Checked = Y[i]; break;
                    case 14: Y14_cb.Checked = Y[i]; break;
                    case 15: Y15_cb.Checked = Y[i]; break;
                    case 16: Y16_cb.Checked = Y[i]; break;
                    case 17: Y17_cb.Checked = Y[i]; break;
                }
            }
        }

        // Вывод частного С на форму
        private void ViewC()
        {
            // Если произошло переполнение
            if (model.PP == 1)
            {
                ResC_tb.Text = "Переполнение";
                return;
            }
            // Заполняем массив 0 и 1 в цикле по всему регистру С
            //for (int i = 16; i >= 0; i--)
            //{
            //    Table_C[i, 0].Value = C % 2;
            //    C = (UInt32)(C / 2);
            //}

            // Вывод результата в 10-ичной СЧ
            ResC_tb.Text = GetDecimalValue(Table_C);
        }

        // Вывод вектора X на форму
        private void ViewX(bool[] X)
        {
            for (int i = 0; i < X.Length; i++)
            {
                // Для каждого Х соответствующий ему флажок переводится в его значение
                switch (i)
                {
                    case 0: X0_cb.Checked = X[i]; break;
                    case 1: X1_cb.Checked = X[i]; break;
                    case 2: X2_cb.Checked = model.Get_X2(); break;
                    case 3: X3_cb.Checked = model.Get_X3(); break;
                    case 4: X4_cb.Checked = X[i]; break;
                    case 5: X5_cb.Checked = model.Get_X5(); break;
                    case 6: X6_cb.Checked = X[i]; break;
                }
            }
        }

        // Вывод Dt на форму
        private void ViewDt(int Q)
        {
            string strQ = ConvertTo4bit(Q);
            Dt0_cb.Checked = (strQ[3] != '0');
            Dt1_cb.Checked = (strQ[2] != '0');
            Dt2_cb.Checked = (strQ[1] != '0');
            Dt3_cb.Checked = (strQ[0] != '0');
        }

        // Вывод Dt на форму
        private void ViewD(int Dt)
        {
            string strDt = ConvertTo4bit(Dt);
            D0_cb.Checked = (strDt[3] != '0');
            D1_cb.Checked = (strDt[2] != '0');
            D2_cb.Checked = (strDt[1] != '0');
            D3_cb.Checked = (strDt[0] != '0');
        }

        // Отображение состояния ГСА на форме
        private void ViewState(int state)
        {
            // Сброс флажков, отображающих текущее состояние А автомата
            ResetCheckboxesInGSA();
            ResetCheckboxesForStatesInOAandYA();
            // Отображение текущего А
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

        // Заполнение таблиц с битами операндов нулями
        public void InitDataTables()
        {
            for (int i = 0; i < 16; i++)
            {
                Table_A[i, 0].Value = 0; // Делимое
                Table_B[i, 0].Value = 0; // Делитель
                // Доп. регистр делимого 
                Table_AM[i, 0].Value = 0; // заполнение нулями 1-й половины таблицы (с 31 по 16 биты)
                Table_AM[i + 16, 0].Value = 0; // заполнение 2-й половины таблицы (с 15 по 0 биты)
                // Доп. регистр делителя
                Table_BM[i, 0].Value = 0;
                Table_BM[i + 16, 0].Value = 0;
                Table_C[i, 0].Value = 0; // Частное
                //Table_C[i, 0].Value = 0;
                // Счетчик (в таблице - 4 колонки)
                if (i < 4)
                    table_CR[i, 0].Value = 0;
            }
            Table_C[16, 0].Value = 0; // заполнение нулевого бита С (в таблице 17 колонок)
        }

        // Преобразование числа в строку длиной 4 элемента 
        private string ConvertTo4bit(int n)
        {
            string str = Convert.ToString(n, 2);
            // Добавление нулей слева в зависимости от длины полученной строки
            if (str.Length == 1) str = "000" + str;
            if (str.Length == 2) str = "00" + str;
            if (str.Length == 3) str = "0" + str;
            return str;
        }

        // Сброс флажков состояний на ГСА
        private void ResetCheckboxesInGSA()
        {
            // Проход по всем флажкам на ГСА и их сброс
            foreach (CheckBox cb in GSA_panel.Controls.OfType<CheckBox>())
            {
                cb.Checked = false;
            }
        }

        // Сброс всех флажков для ОА и УА
        private void ResetCheckboxesInOAandYA()
        {
            // Проход по всем флажкам на ОА и УА и их сброс
            foreach (CheckBox cb in OAandYA_panel.Controls.OfType<CheckBox>())
            {
                cb.Checked = false;
            }
        }

        // Сброс флажков, отображающий состояния для ОА и УА
        private void ResetCheckboxesForStatesInOAandYA()
        {
            // Проход по всем флажкам на ОА и УА
            foreach (CheckBox cb in OAandYA_panel.Controls.OfType<CheckBox>())
            {
                string nameCb = cb.Text;
                // Сброс тех флажков, в тексте которых есть символ "а" - состояние
                if (nameCb[0] == 'a')
                {
                    cb.Checked = false;
                }
            }
        }
    }
}
