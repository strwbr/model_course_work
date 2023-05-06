using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace model_course_work
{
    class Model
    {
        private uint A; // Делимое
        private uint B; // Делитель
        private uint AM; // Доп. регистр делимого
        private uint BM; // Доп. регистр делителя
        private uint D; // Остаток
        private int CR; // Счетчик
        private bool Start; // Флаг начала работы

        public uint C { get; private set; } // Частное
        public bool Stop { get; private set; } // Флаг завершения работы
        public int State { get; private set; } // Состояние
        public byte PP { get; private set; } // Признак переполнения
        public bool[] X { get; private set; } // Вектор X-в
        public byte[] Y { get; private set; } // Вектор У-в
        public int Dt { get; private set; } // Код состояния (kA)

        // Условия для ПЛУ
        private bool _X2;
        private bool _X3;
        private bool _X5;

        public Model() { }

        public Model(uint a, uint b)
        {
            A = a;
            B = b;

            C = AM = BM = D = 0;
            //C = 0;
            PP = 0;
            //AM = 0;
            //BM = 0;
            //D = 0;
            CR = State = Dt = 0;
            Start = Stop = false;
            X = new bool[7];
            Y = new byte[18];
            //Start = false;
            //State = 0;
            //Dt = 0;
            for (int i = 0; i < 18; i++)
                Y[i] = 0;
            for (int i = 0; i < 7; i++)
                X[i] = false;
            X[0] = true;
            _X2 = X2();
            _X3 = X3();
            _X5 = X5();
            X[2] = _X2;
            X[3] = _X3;
            X[5] = _X5;
        }

        public void Run()
        {
            Start = true;
        }

        // Методы для X-в
        // Пуск
        private bool X0()
        {
            return Start;
        }
        // BM = 0 - проверка деления на 0
        private bool X1()
        {
            return BM == 0;
        }
        // АМ = 0 - проверка, что делимое равно 0
        private bool X2()
        {
            return AM == 0;
        }
        // AM(31)
        private bool X3()
        {
            return (AM & 0x80000000) != 0; // 1000 0000 0000 0000 0000 0000 0000 0000
        }
        // CR = 0 - проверка окончания цикла
        private bool X4()
        {
            return CR == 0;
        }
        // С(0)
        private bool X5()
        {
            return (C & 1) == 1; // 0000 ... 0001 = 1
        }
        //A(15) xor B(15)
        private bool X6()
        {
            return (A & 0x8000) != (B & 0x8000); //0000 0000 0000 0000 1000 0000 0000 0000
        }

        // Методы вычисления Y-в
        // АМ(29:15) := А(14:0)
        private void Y1()
        {
            AM = (A & 0x7FFF) << 15; // 0000 0000 0000 0000 0111 1111 1111 1111
        }
        // ВМ(29:15) := В(14:0)
        private void Y2()
        {
            BM = (B & 0x7FFF) << 15; // 0000 0000 0000 0000 0111 1111 1111 1111
        }
        // АМ(14:0) := 0 
        private void Y3()
        {
            AM = AM & 0xFFFF8000; // 1111 1111 1111 1111 1000 0000 0000 0000
        }
        // ВМ(14:0) := 0
        private void Y4()
        {
            BM = BM & 0xFFFF8000; // 1111 1111 1111 1111 1000 0000 0000 0000
        }
        //AM := AM + 11.неBM(29:0)+1
        private void Y5()
        {
            AM += (~BM | 0xC0000000) + 1; //1100 0000 0000 0000 0000 0000 0000 0000
        }
        //AM := AM + BM(29:0)
        private void Y6()
        {
            AM += BM & 0x3FFFFFFF; // 0011 1111 1111 1111 1111 1111 1111 1111
        }
        // D: = AM
        private void Y7()
        {
            D = AM;
        }
        // BM := R1(0.BM)
        private void Y8()
        {
            BM = BM >> 1; // Сдвиг вправо на 1 бит (0 припишется справа автоматически)
        }
        //C := 0
        private void Y9()
        {
            C = 0;
        }
        // CR := 0
        private void Y10()
        {
            CR = 0;
        }
        // C := L1(C.1)
        private void Y11()
        {
            C = (C << 1) | 1;
        }
        // C := L1(C.0)
        private void Y12()
        {
            C = C << 1;
        }
        // AM := D
        private void Y13()
        {
            AM = D;
        }
        // CR := CR-1
        private void Y14()
        {
            //Counter();
            if (CR == 0)
                CR = 15;
            else CR--;
        }
        // С(16:1) := С(16:1)+1
        private void Y15()
        {
            //C += 0x2;
            C = (C & 0xFFFFFFFE) + 2; // 1111 1111 1111 1111 1111 1111 1111 1110
        }
        //С(16) := 1
        private void Y16() //С(16) := 1
        {
            C = C | 0x10000; // 0000 0000 0000 0001 0000 0000 0000 0000
        }
        //ПП := 1
        private void Y17() //PP := 1
        {
            PP = 1;
        }

        private void Yk()
        {
            Stop = true;
        }

        // Режим микропрограммы
        public void Microprogram()
        {
            switch (State)
            {
                case 0:
                    if (!X0())
                    {
                        State = 0;
                        break;
                    }
                    if (X0())
                    {
                        Y1();
                        Y2();
                        Y3();
                        Y4();
                        State = 1;
                        break;
                    }
                    break;
                case 1:
                    if (!X1() && !X2())
                    {
                        Y5();
                        State = 2;
                        break;
                    }
                    if (!X1() && X2())
                    {
                        Y9();
                        State = 8;
                        break;
                    }
                    if (X1())
                    {
                        Y17();
                        State = 8;
                        break;
                    }
                    break;
                case 2:
                    if (X3())
                    {
                        Y6();
                        Y7();
                        Y8();
                        Y9();
                        Y10();
                        State = 3;
                        break;
                    }
                    if (!X3())
                    {
                        Y17();
                        State = 8;
                        break;
                    }
                    break;
                case 3:
                    Y5();
                    State = 4;
                    break;
                case 4:
                    if (!X3())
                    {
                        Y11();
                        State = 5;
                        break;
                    }
                    if (X3())
                    {
                        Y12();
                        Y13();
                        State = 5;
                        break;
                    }
                    break;
                case 5:
                    Y7();
                    Y8();
                    Y14();
                    State = 6;
                    break;
                case 6:
                    if (X4() && !X5() && !X6())
                    {
                        Yk();
                        State = 9;
                        break;
                    }
                    if (!X4())
                    {
                        Y5();
                        State = 4;
                        break;
                    }
                    if (X4() && X5())
                    {
                        Y15();
                        State = 7;
                        break;
                    }
                    if (X4() && !X5() && X6())
                    {
                        Y16();
                        State = 8;
                        break;
                    }
                    break;
                case 7:
                    if (!X6())
                    {
                        Yk();
                        State = 9;
                        break;
                    }
                    if (X6())
                    {
                        Y16();
                        State = 8;
                        break;
                    }
                    break;
                case 8:
                    Yk();
                    State = 9;
                    break;
            }
        }

        //Память логических условий
        public void Logic_Memory_Cond(bool[] X)
        {
            _X3 = X[3];
            _X2 = X[2];
            _X5 = X[5];
        }

        // Комбинационная схема векторов Y
        public void KSY(bool[] Xx)
        {
            byte[] NotX = new byte[7];
            byte[] X = new byte[7];
            byte[] A = new byte[9];
            byte Not2, Not3, Not5;
            byte X2;
            byte X3;
            byte X5;

            if (_X3) { X3 = 1; Not3 = 0; }
            else { X3 = 0; Not3 = 1; }

            if (_X2) { X2 = 1; Not2 = 0; }
            else { X2 = 0; Not2 = 1; }

            if (_X5) { X5 = 1; Not5 = 0; }
            else { X5 = 0; Not5 = 1; }

            A[Dt] = 1;

            for (int i = 0; i < 7; i++) //по всем иксам, 
            {
                if (Xx[i]) { X[i] = 1; NotX[i] = 0; }
                else { X[i] = 0; NotX[i] = 1; }
            }

            Y[0] = (byte)((A[6] & X[4] & Not5 & NotX[6]) | (A[7] & NotX[6]) | (A[8])); //yk
            Y[1] = (byte)(A[0] & X[0]);
            Y[2] = Y[1];
            Y[3] = Y[1];
            Y[4] = Y[1];
            Y[5] = (byte)((A[1] & NotX[1] & Not2) | (A[3]) | (A[6] & NotX[4]));
            Y[6] = (byte)(A[2] & X3);
            Y[7] = (byte)((A[2] & X3) | (A[5]));
            Y[8] = Y[7];
            Y[9] = (byte)((A[2] & X3) | (A[1] & NotX[1] & X2));
            Y[10] = Y[6];
            Y[11] = (byte)(A[4] & Not3);
            Y[12] = (byte)(A[4] & X3);
            Y[13] = Y[12];
            Y[14] = A[5];
            Y[15] = (byte)(A[6] & X[4] & X5);
            Y[16] = (byte)((A[6] & X[4] & Not5 & X[6]) | (A[7] & X[6]));
            Y[17] = (byte)((A[1] & X[1]) | (A[2] & Not3));
        }

        // Операционный автомат
        public void OA(byte[] Y)
        {
            for (int i = 0; i < 18; i++)
            {
                if (Y[i] != 0)
                {
                    switch (i)
                    {
                        case 0:
                            Yk();
                            break;
                        case 1:
                            Y1();
                            break;
                        case 2:
                            Y2();
                            break;
                        case 3:
                            Y3();
                            break;
                        case 4:
                            Y4();
                            break;
                        case 5:
                            Y5();
                            break;
                        case 6:
                            Y6();
                            break;
                        case 7:
                            Y7();
                            break;
                        case 8:
                            Y8();
                            break;
                        case 9:
                            Y9();
                            break;
                        case 10:
                            Y10();
                            break;
                        case 11:
                            Y11();
                            break;
                        case 12:
                            Y12();
                            break;
                        case 13:
                            Y13();
                            break;
                        case 14:
                            Y14();
                            break;
                        case 15:
                            Y15();
                            break;
                        case 16:
                            Y16();
                            break;
                        case 17:
                            Y17();
                            break;
                    }
                }
            }
            X[0] = X0();
            X[1] = X1();
            X[2] = X2();
            X[3] = X3();
            X[4] = X4();
            X[5] = X5();
            X[6] = X6();
        }

        // Комбинационная схема векторов D
        public void KSD(bool[] Xx)
        {
            byte[] NotX = new byte[7];
            byte[] X = new byte[7];
            byte[] A = new byte[9];
            byte Not2;
            byte Not3;
            byte Not5;
            byte X2;
            byte X3;
            byte X5;

            if (_X2) { X2 = 1; Not2 = 0; }
            else { X2 = 0; Not2 = 1; }

            if (_X3) { X3 = 1; Not3 = 0; }
            else { X3 = 0; Not3 = 1; }

            if (_X5) { X5 = 1; Not5 = 0; }
            else { X5 = 0; Not5 = 1; }

            for (int i = 0; i < 7; i++)
            {
                if (Xx[i]) { X[i] = 1; NotX[i] = 0; }
                else { X[i] = 0; NotX[i] = 1; }
            }

            A[Dt] = 1;

            Dt = (byte)((A[0] & X[0]) | (A[2] & X3) | (A[4]) | (A[6] & X[4] & X5));

            Dt += (byte)((A[1] & NotX[1] & Not2) | (A[2] & X3) | (A[5]) | (A[6] & X[4] & X5)) << 1;

            Dt += (byte)((A[3]) | (A[6] & NotX[4]) | (A[4]) | (A[5]) | (A[6] & X[4] & X5)) << 2;

            Dt += (byte)((A[1] & NotX[1] & X2) | (A[1] & X[1]) | (A[2] & Not3) |
                (A[6] & X[4] & Not5 & X[6]) | (A[7] & X[6])) << 3;

        }

        // метод, выводит значение операндов на визуальный слой
        public void ShowValues(ref DataGridView table_AM, ref DataGridView table_BM, ref DataGridView table_C, ref DataGridView table_CR)
        {
            uint tempC = C, tempAM = AM, tempBM = BM;
            int tempCR = CR;

            for (int i = 16; i >= 0; i--)
            {
                table_C[i, 0].Value = tempC % 2;
                tempC = (UInt32)(tempC / 2);
            }
            for (int i = 31; i >= 0; i--)
            {
                table_AM[i, 0].Value = tempAM % 2;
                table_BM[i, 0].Value = tempBM % 2;
                tempAM = (UInt32)(tempAM / 2);
                tempBM = (UInt32)(tempBM / 2);
            }
            for (int i = 3; i >= 0; i--)
            {
                table_CR[i, 0].Value = tempCR % 2;
                tempCR = (Int32)(tempCR / 2);
            }
        }

        public bool Get_X2() { return _X2; }
        public bool Get_X3() { return _X3; }
        public bool Get_X5() { return _X5; }


        /*        public void Counter()
        {
            if (CR == 0) 
                CR = 15;
            else CR--;

            //CR = (CR != 0) ? CR - 1 : 15;
        }*/
    }
}
