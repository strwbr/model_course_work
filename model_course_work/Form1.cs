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

namespace model_course_work
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            data.Rows.Add();
            data_a.Rows.Add();
            data_b.Rows.Add();
            data_ra.Rows.Add();
            data_rb.Rows.Add();
            FullNull();
        }
        uint A = 0;
        uint B = 0;
        uint AM = 0;
        uint BM = 0;
        bool is_start = false;
        uint C = 0;
        int D = 0;
        uint Ddd = 0;
        string a = "";
        string b = "";
        Model Pr1 = new Model();
        int[] mass = new int[32];

        private void CheckClick(object sender, EventArgs e)
        {
            //if (radioButton_auto.Checked)
            //{
            //    if (radioButton_auto.Checked == true)
            //    {
            //        radioButton_tact.Checked = false;
            //    }
            //}
            //if (radioButton_tact.Checked)
            //{
            //    if (radioButton_tact.Checked == true)
            //    {
            //        radioButton_auto.Checked = false;
            //    }
            //}
        }

        private void CheckClick2(object sender, EventArgs e)
        {
            //if (radioButton_MP.Checked)
            //{
            //    if (radioButton_MP.Checked == true)
            //    {
            //        radioButton_OY_AO.Checked = false;
            //    }
            //}
            //if (radioButton_OY_AO.Checked)
            //{
            //    if (radioButton_OY_AO.Checked == true)
            //    {
            //        radioButton_MP.Checked = false;
            //    }
            //}
        }

        public void FullNull()
        {
            for (int i = 0; i < 16; i++)
            {
                data_a[i, 0].Value = 0; //заполнение строк нулями
                data_b[i, 0].Value = 0;
                data_ra[i, 0].Value = 0;
                data_rb[i, 0].Value = 0;
                data_ra[i + 16, 0].Value = 0;
                data_rb[i + 16, 0].Value = 0;
                data[i, 0].Value = 0;
                data[i, 0].Value = 0;
                if (i < 4)
                    data_counter[i, 0].Value = 0;
            }
            data[16, 0].Value = 0;
        }

        private void start_Click(object sender, EventArgs e) // пуск
        {
            Model Program = new Model(A, B, C, D);
            groupBox2.Enabled = false;
            groupBox4.Enabled = false;
            data_a.ClearSelection();
            data_b.ClearSelection();
            Pr1 = new Model(Program);
            start.Enabled = false;
            is_start = true;
            Pr1.SetStart();
            data_a.Enabled = false;
            data_b.Enabled = false;
            checkBox_a0.Checked = true;
            if (radioButton_auto.Checked == true)
            {
                while (!Pr1.GetEnd())
                {
                    if (radioButton_MP.Checked == true)
                    {
                        Pr1.Microprogram2();
                        DoCheckBox(Pr1);
                    }
                    else
                    {
                        OA_and_YA();
                    }
                    Pr1.ShowValues(ref data_ra, ref data_rb, ref data, ref data_counter);
                }
                C = Pr1.GetC();
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
            if (radioButton_tact.Checked == true)
            {
                if (is_start)
                {
                    if (!Pr1.GetEnd())
                    {
                        if (radioButton_MP.Checked == true)
                        {
                            Pr1.Microprogram2();
                            DoCheckBox(Pr1);
                        }
                        else
                        {
                            OA_and_YA();
                        }
                        Pr1.ShowValues(ref data_ra, ref data_rb,
                            ref data, ref data_counter);
                    }
                    else
                    {
                        C = Pr1.GetC();
                        ShowResult();
                        is_start = false;
                        start.Enabled = false;
                        button_tact.Enabled = false;
                    }
                }
            }
        }

        private void ShowResult() //вывод результата в текстбокс на последнем такте
        {
            double res = 0;
            if (Pr1.GetPP() == 1)
            {
                textBox_c.Text = "Переполнение";
            }
            else //if (C != 0)
            {//заполняем массив 0 и 1 в цикле по всему регистру С
                for (int i = 16; i >= 0; i--)
                {
                    if (C % 2 == 0) { data[i, 0].Value = 0; mass[i] = 0; }
                    else { data[i, 0].Value = 1; mass[i] = 1; }
                    C = (UInt32)(C / 2);
                } //высчитываем значение в десятичном эквиваленте с 30 по 16 регистр
                for (int col = 1; col <= 15; col++)
                {
                    res += Math.Pow(2, 0 - (col)) * Double.Parse(data[col, 0].Value.ToString());
                }
                res = Math.Round(res, 6);
                textBox_c.Text = res.ToString();
                if (data[0, 0].Value.ToString() == "1")
                    textBox_c.Text = textBox_c.Text.Insert(0, "-");
            }
        }

        //заполнение чекбоксов на гса
        private void DoCheckBox(Model Program)
        {
            checkBox_a0.Checked = false;
            checkBox_a1.Checked = false;
            checkBox_a2.Checked = false;
            checkBox_a3.Checked = false;
            checkBox_a4.Checked = false;
            checkBox_a5.Checked = false;
            checkBox_a6.Checked = false;
            checkBox_a7.Checked = false;
            checkBox_a8.Checked = false;
            checkBox_a.Checked = false;
            switch (Program.GetState())
            {
                case 0:
                    checkBox_a0.Checked = true;
                    break;
                case 1:
                    checkBox_a1.Checked = true;
                    break;
                case 2:
                    checkBox_a2.Checked = true;
                    break;
                case 3:
                    checkBox_a3.Checked = true;
                    break;
                case 4:
                    checkBox_a4.Checked = true;
                    break;
                case 5:
                    checkBox_a5.Checked = true;
                    break;
                case 6:
                    checkBox_a6.Checked = true;
                    break;
                case 7:
                    checkBox_a7.Checked = true;
                    break;
                case 8:
                    checkBox_a8.Checked = true;
                    break;
                case 9:
                    checkBox_a.Checked = true;
                    break;
            }
        }

        private void reset_Click(object sender, EventArgs e) //сброс
        {
            a = "";
            b = "";
            A = 0;
            B = 0;
            C = 0;
            D = 0;
            Ddd = 0;
            AM = 0;
            BM = 0;
            is_start = false;
            start.Text = "Пуск";
            start.Enabled = true;
            Pr1 = new Model();
            button_tact.Enabled = false;
            textBox_a.Text = "";
            textBox_b.Text = "";
            textBox_c.Text = "";
            data_a.Enabled = true;
            data_b.Enabled = true;
            mass = new int[32];
            checkBox_a0.Checked = true;
            checkBox_a1.Checked = false;
            checkBox_a2.Checked = false;
            checkBox_a3.Checked = false;
            checkBox_a4.Checked = false;
            checkBox_a5.Checked = false;
            checkBox_a6.Checked = false;
            checkBox_a7.Checked = false;
            checkBox_a8.Checked = false;
            checkBox_a.Checked = false;
            checkBoxa0.Checked = true;
            checkBoxa1.Checked = false;
            checkBoxa2.Checked = false;
            checkBoxa3.Checked = false;
            checkBoxa4.Checked = false;
            checkBoxa5.Checked = false;
            checkBoxa6.Checked = false;
            checkBoxa7.Checked = false;
            checkBoxa8.Checked = false;
            checkBox_x0.Checked = false;
            checkBox_x1.Checked = false;
            checkBox_x2.Checked = false;
            checkBox_x3.Checked = false;
            checkBox_x4.Checked = false;
            checkBox_x5.Checked = false;
            checkBox_x6.Checked = false;
            checkBox_y1.Checked = false;
            checkBox_y2.Checked = false;
            checkBox_y3.Checked = false;
            checkBox_y4.Checked = false;
            checkBox_y5.Checked = false;
            checkBox_y6.Checked = false;
            checkBox_y7.Checked = false;
            checkBox_y8.Checked = false;
            checkBox_y9.Checked = false;
            checkBox_y10.Checked = false;
            checkBox_y11.Checked = false;
            checkBox_y12.Checked = false;
            checkBox_y13.Checked = false;
            checkBox_y14.Checked = false;
            checkBox_y15.Checked = false;
            checkBox_y16.Checked = false;
            checkBox_y17.Checked = false;
            checkBox_yk.Checked = false;
            checkBox_Q0.Checked = false;
            checkBox_Q1.Checked = false;
            checkBox_Q2.Checked = false;
            checkBox_Q3.Checked = false;
            checkBox_D0.Checked = false;
            checkBox_D1.Checked = false;
            checkBox_D2.Checked = false;
            checkBox_D3.Checked = false;
            groupBox2.Enabled = true;
            groupBox4.Enabled = true;
            FullNull();
        }

        private void data_a_CellClick(object sender, DataGridViewCellEventArgs e)
        //изменение значения А
        {
            A = 0;
            a = "";
            string mark1 = ""; //знак операнда 1
            if (data_a.Rows[0].Cells[e.ColumnIndex].Value.ToString() == "0")
                data_a.Rows[0].Cells[e.ColumnIndex].Value = 1;
            else data_a.Rows[0].Cells[e.ColumnIndex].Value = 0;
            for (int i = 0; i < 16; i++)
            {
                a += data_a[i, 0].Value.ToString();
            }
            if (data_a.Rows[0].Cells[0].Value.ToString() == "1")
                mark1 = "-";
            get_decimal_values(ref A, a, mark1, textBox_a);
        }

        private void data_b_CellClick(object sender, DataGridViewCellEventArgs e)
        //изменение значения В
        {
            B = 0;
            b = "";
            string mark2 = ""; //знак операнда 2
            if (data_b.Rows[0].Cells[e.ColumnIndex].Value.ToString() == "0")
                data_b.Rows[0].Cells[e.ColumnIndex].Value = 1;
            else data_b.Rows[0].Cells[e.ColumnIndex].Value = 0;
            for (int i = 0; i < 16; i++) //переносим 0 и 1 в строку
            {
                b += data_b[i, 0].Value.ToString();
            }
            if (data_b.Rows[0].Cells[0].Value.ToString() == "1") //записываем в переменную -
                mark2 = "-";
            get_decimal_values(ref B, b, mark2, textBox_b);
        }

        private void get_decimal_values(ref uint T, string t, string mark, TextBox name)
        //получаем десятичные значения
        {
            double res = 0;
            for (int col = 1; col < 15; col++) //переводим значение в десятичную из двоичной сч
            {
                res += Math.Pow(2, 0 - col) * Double.Parse(t[col].ToString());
            } //преобразуем в uint строковое представление числа из системы счисления 2
            T = Convert.ToUInt16(t, 2);
            res = Math.Round(res, 5);
            name.Text = mark + res.ToString("0.#####");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            data_ra.ClearSelection();
            data_rb.ClearSelection();
            data_a.ClearSelection();
            data_b.ClearSelection();
            data.ClearSelection();
        }

        //Вывод A на форму
        private void stateChanged(int a_in)
        {
            checkBox_a0.Checked = false;
            checkBox_a1.Checked = false;
            checkBox_a2.Checked = false;
            checkBox_a3.Checked = false;
            checkBox_a4.Checked = false;
            checkBox_a5.Checked = false;
            checkBox_a6.Checked = false;
            checkBox_a7.Checked = false;
            checkBox_a8.Checked = false;
            checkBox_a.Checked = false;
            checkBoxa0.Checked = false;
            checkBoxa1.Checked = false;
            checkBoxa2.Checked = false;
            checkBoxa3.Checked = false;
            checkBoxa4.Checked = false;
            checkBoxa5.Checked = false;
            checkBoxa6.Checked = false;
            checkBoxa7.Checked = false;
            checkBoxa8.Checked = false;
            switch (a_in)
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

        //Вывод Y на форму
        private void showY(byte[] Y)
        {
            for (int i = 0; i < Y.Length; i++)
            {
                switch (i)
                {
                    case 0:
                        checkBox_yk.Checked = (Y[i] != 0);
                        break;
                    case 1:
                        checkBox_y1.Checked = (Y[i] != 0);
                        break;
                    case 2:
                        checkBox_y2.Checked = (Y[i] != 0);
                        break;
                    case 3:
                        checkBox_y3.Checked = (Y[i] != 0);
                        break;
                    case 4:
                        checkBox_y4.Checked = (Y[i] != 0);
                        break;
                    case 5:
                        checkBox_y5.Checked = (Y[i] != 0);
                        break;
                    case 6:
                        checkBox_y6.Checked = (Y[i] != 0);
                        break;
                    case 7:
                        checkBox_y7.Checked = (Y[i] != 0);
                        break;
                    case 8:
                        checkBox_y8.Checked = (Y[i] != 0);
                        break;
                    case 9:
                        checkBox_y9.Checked = (Y[i] != 0);
                        break;
                    case 10:
                        checkBox_y10.Checked = (Y[i] != 0);
                        break;
                    case 11:
                        checkBox_y11.Checked = (Y[i] != 0);
                        break;
                    case 12:
                        checkBox_y12.Checked = (Y[i] != 0);
                        break;
                    case 13:
                        checkBox_y13.Checked = (Y[i] != 0);
                        break;
                    case 14:
                        checkBox_y14.Checked = (Y[i] != 0);
                        break;
                    case 15:
                        checkBox_y15.Checked = (Y[i] != 0);
                        break;
                    case 16:
                        checkBox_y16.Checked = (Y[i] != 0);
                        break;
                    case 17:
                        checkBox_y17.Checked = (Y[i] != 0);
                        break;
                }
            }
        }

        // Вывод X на форму
        private void showX(bool[] X)
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

        private void OA_and_YA()
        {
            //показ Q
            show_Q(Pr1.Get_Dt());
            //память логических условий
            Pr1.Logic_Memory_Cond(Pr1.Get_X());
            //комбинационная схема Y
            Pr1.KSY(Pr1.Get_X());
            showY(Pr1.Get_Y());
            //операционный автомат (х)
            Pr1.OA(Pr1.Get_Y());
            showX(Pr1.Get_X());
            //д триггер
            Pr1.KSD(Pr1.Get_X());
            show_D(Pr1.Get_Dt());
            //a на форму
            stateChanged(Pr1.Get_Dt());
        }

        //Вывод Q на форму
        private void show_Q(int Dtr)
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
        private void show_D(int Dtr)
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

    }
}
