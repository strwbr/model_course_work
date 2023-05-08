using System;
using System.Windows.Forms;

namespace model_course_work
{
    // Класс, содержащий основную логику программы
    class Model
    {
        private uint A; // Делимое
        private uint B; // Делитель
        private uint AM; // Доп. регистр делимого
        private uint BM; // Доп. регистр делителя
        private uint D; // Остаток
        private int CR; // Счетчик
        private bool start; // Флаг начала работы
        private bool[] states;

        public uint C { get; private set; } // Частное
        public bool Stop { get; private set; } // Флаг завершения работы
        public int State { get; private set; } // Состояние
        public byte PP { get; private set; } // Признак переполнения
        public bool[] X { get; private set; } // Вектор X-в
        public bool[] Y { get; private set; } // Вектор У-в
        public int Dt { get; private set; } // Код состояния (kA)

        // Условия для ПЛУ
        private bool _X2;
        private bool _X3;
        private bool _X5;
        // Конструктор по умолчанию
        public Model() { }
        // Конструктор с параметрами А - делимое, В - делитель
        public Model(uint a, uint b)
        {
            A = a;
            B = b;

            C = AM = BM = D = 0;
            PP = 0;
            CR = State = Dt = 0;
            start = Stop = false;
            X = new bool[7];
            Y = new bool[18];
            states = new bool[9];
            for (int i = 0; i < 18; i++)
                Y[i] = false;
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

        // Запуск выполнения моделирования
        public void Run() => start = true;

        // Методы для X-в
        // Пуск
        private bool X0() => start;
        // BM = 0 - проверка деления на 0
        private bool X1() => BM == 0;
        // АМ = 0 - проверка, что делимое равно 0
        private bool X2() => AM == 0;
        // AM(31)
        private bool X3() => (AM & 0x80000000) != 0; // 1000 0000 0000 0000 0000 0000 0000 0000
        // CR = 0 - проверка окончания цикла
        private bool X4() => CR == 0;
        // С(0)
        private bool X5() => (C & 1) == 1; // 0000 ... 0001 = 1
        //A(15) xor B(15)
        private bool X6() => (A & 0x8000) != (B & 0x8000); //0000 0000 0000 0000 1000 0000 0000 0000

        // Методы вычисления Y-в
        // АМ(29:15) := А(14:0)
        private void Y1() => AM = (A & 0x7FFF) << 15; // 0000 0000 0000 0000 0111 1111 1111 1111
        // ВМ(29:15) := В(14:0)
        private void Y2() => BM = (B & 0x7FFF) << 15; // 0000 0000 0000 0000 0111 1111 1111 1111
        // АМ(14:0) := 0 
        private void Y3() => AM = AM & 0xFFFF8000; // 1111 1111 1111 1111 1000 0000 0000 0000
        // ВМ(14:0) := 0
        private void Y4() => BM = BM & 0xFFFF8000; // 1111 1111 1111 1111 1000 0000 0000 0000
        //AM := AM + 11.!BM(29:0)+1
        private void Y5() => AM += (~BM | 0xC0000000) + 1; //1100 0000 0000 0000 0000 0000 0000 0000
        //AM := AM + BM(29:0)
        private void Y6() => AM += BM & 0x3FFFFFFF; // 0011 1111 1111 1111 1111 1111 1111 1111
        // D: = AM
        private void Y7() => D = AM;
        // BM := R1(0.BM)
        private void Y8() => BM = BM >> 1;
        //C := 0
        private void Y9() => C = 0;
        // CR := 0
        private void Y10() => CR = 0;
        // C := L1(C.1)
        private void Y11() => C = (C << 1) | 1;
        // C := L1(C.0)
        private void Y12() => C = C << 1;
        // AM := D
        private void Y13() => AM = D;
        // CR := CR-1
        private void Y14()
        {
            if (CR == 0)
                CR = 15;
            else CR--;
        }
        // С(16:1) := С(16:1)+1
        private void Y15() => C = (C & 0xFFFFFFFE) + 2; // 1111 1111 1111 1111 1111 1111 1111 1110
        //С(16) := 1
        private void Y16() => C = C | 0x10000; // 0000 0000 0000 0001 0000 0000 0000 0000
        //ПП := 1
        private void Y17() => PP = 1;

        private void Yk() => Stop = true;

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

        // Дешифратор
        public void Decoder()
        {
            for (int i = 0; i < states.Length; i++)
            {
                states[i] = false;
            }
            states[State] = true;
        }

        // Память состояний
        public void StateMemory()
        {
            State = Dt;
        }

        // Память логических условий
        public void LogicCondMemory(bool[] X)
        {
            _X3 = X[3];
            _X2 = X[2];
            _X5 = X[5];
        }

        // Комбинационная схема векторов Y
        public void KSY()
        {
            bool[] A = states;
            // Вычисление операций У, который должны быть выполнены в этом такте
            Y[0] = ((A[6] & X[4] & !_X5 & !X[6]) | (A[7] & !X[6]) | (A[8])); // yk
            Y[1] = (A[0] & X[0]);
            Y[2] = Y[1];
            Y[3] = Y[1];
            Y[4] = Y[1];
            Y[5] = ((A[1] & !X[1] & !_X2) | (A[3]) | (A[6] & !X[4]));
            Y[6] = (A[2] & _X3);
            Y[7] = ((A[2] & _X3) | (A[5]));
            Y[8] = Y[7];
            Y[9] = ((A[2] & _X3) | (A[1] & !X[1] & _X2));
            Y[10] = Y[6];
            Y[11] = (A[4] & !_X3);
            Y[12] = (A[4] & _X3);
            Y[13] = Y[12];
            Y[14] = A[5];
            Y[15] = (A[6] & X[4] & _X5);
            Y[16] = ((A[6] & X[4] & !_X5 & X[6]) | (A[7] & X[6]));
            Y[17] = ((A[1] & X[1]) | (A[2] & !_X3));
        }

        // Комбинационная схема векторов D
        public void KSD()
        {
            bool[] A = states;
            // Вычисление кода следующего состояния
            Dt = ((A[0] & X[0]) | (A[2] & _X3) | (A[4]) | (A[6] & X[4] & _X5)) ? 1 : 0;
            Dt += ((A[1] & !X[1] & !_X2) | (A[2] & _X3) | (A[5]) | (A[6] & X[4] & _X5)) ? 2 : 0;
            Dt += ((A[3]) | (A[6] & !X[4]) | (A[4]) | (A[5]) | (A[6] & X[4] & _X5)) ? 4 : 0;
            Dt += ((A[1] & !X[1] & _X2) | (A[1] & X[1]) | (A[2] & !_X3) | (A[6] & X[4] & !_X5 & X[6]) | (A[7] & X[6])) ? 8 : 0;
        }

        // Операционный автомат
        public void OA(/*bool[] Y*/)
        {
            // Проход по вектору операций Y
            for (int i = 0; i < Y.Length; i++)
            {
                // Поиск операции Y, которая должна выполнится
                if (Y[i])
                {
                    // Вызов соответствующего метода выполнения
                    switch (i)
                    {
                        case 0: Yk(); break;
                        case 1: Y1(); break;
                        case 2: Y2(); break;
                        case 3: Y3(); break;
                        case 4: Y4(); break;
                        case 5: Y5(); break;
                        case 6: Y6(); break;
                        case 7: Y7(); break;
                        case 8: Y8(); break;
                        case 9: Y9(); break;
                        case 10: Y10(); break;
                        case 11: Y11(); break;
                        case 12: Y12(); break;
                        case 13: Y13(); break;
                        case 14: Y14(); break;
                        case 15: Y15(); break;
                        case 16: Y16(); break;
                        case 17: Y17(); break;
                    }
                }
            }
            // Вычисление условий Х для следующего такта
            X[0] = X0();
            X[1] = X1();
            X[2] = X2();
            X[3] = X3();
            X[4] = X4();
            X[5] = X5();
            X[6] = X6();
        }

        // Вывод значений операндов на визуальный слой
        public void ViewOperands(ref DataGridView table_AM, ref DataGridView table_BM, ref DataGridView table_C, ref DataGridView table_CR)
        {
            // Инициализация временных переменных для операндов
            uint tempC = C, tempAM = AM, tempBM = BM;
            int tempCR = CR;
            // Перевод в двоичную СЧ и заполнение элементов формы
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
        // Геттеры для x для ПЛУ
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

        // Комбинационная схема векторов Y
        //public void KSY(bool[] vectorX)
        //{
        //    byte[] NotX = new byte[7];
        //    byte[] X = new byte[7];
        //    byte[] A = new byte[9];

        //    byte tempX2 = Convert.ToByte(_X2);
        //    byte tempX3 = Convert.ToByte(_X2);
        //    byte tempX5 = Convert.ToByte(_X2);

        //    byte NotX2 = (byte)(1 - tempX2);
        //    byte NotX3 = (byte)(1 - tempX3);
        //    byte NotX5 = ((byte)(1 - tempX5));

        //    for(int i = 0; i < vectorX.Length; i++)
        //    {
        //        X[i] = Convert.ToByte(vectorX[i]);
        //        NotX[i] = (byte)(1 - X[i]);
        //    }

        //    //if (_X3) { tempX3 = 1; NotX3 = 0; }
        //    //else { tempX3 = 0; NotX3 = 1; }

        //    //if (_X2) { tempX2 = 1; NotX2 = 0; }
        //    //else { tempX2 = 0; NotX2 = 1; }

        //    //if (_X5) { tempX5 = 1; NotX5 = 0; }
        //    //else { tempX5 = 0; NotX5 = 1; }

        //    A[Dt] = 1;

        //    //for (int i = 0; i < 7; i++) //по всем иксам, 
        //    //{
        //    //    if (vectorX[i]) { X[i] = 1; NotX[i] = 0; }
        //    //    else { X[i] = 0; NotX[i] = 1; }
        //    //}

        //    Y[0] = (byte)((A[6] & X[4] & NotX5 & NotX[6]) | (A[7] & NotX[6]) | (A[8])); //yk
        //    Y[1] = (byte)(A[0] & X[0]);
        //    Y[2] = Y[1];
        //    Y[3] = Y[1];
        //    Y[4] = Y[1];
        //    Y[5] = (byte)((A[1] & NotX[1] & NotX2) | (A[3]) | (A[6] & NotX[4]));
        //    Y[6] = (byte)(A[2] & tempX3);
        //    Y[7] = (byte)((A[2] & tempX3) | (A[5]));
        //    Y[8] = Y[7];
        //    Y[9] = (byte)((A[2] & tempX3) | (A[1] & NotX[1] & tempX2));
        //    Y[10] = Y[6];
        //    Y[11] = (byte)(A[4] & NotX3);
        //    Y[12] = (byte)(A[4] & tempX3);
        //    Y[13] = Y[12];
        //    Y[14] = A[5];
        //    Y[15] = (byte)(A[6] & X[4] & tempX5);
        //    Y[16] = (byte)((A[6] & X[4] & NotX5 & X[6]) | (A[7] & X[6]));
        //    Y[17] = (byte)((A[1] & X[1]) | (A[2] & NotX3));
        //}

        //// Комбинационная схема векторов D
        //public void KSD(bool[] vectorX)
        //{
        //    byte[] NotX = new byte[7];
        //    byte[] X = new byte[7];
        //    byte[] A = new byte[9];
        //    byte Not2, Not3, Not5;
        //    byte X2, X3, X5;

        //    if (_X2) { X2 = 1; Not2 = 0; }
        //    else { X2 = 0; Not2 = 1; }

        //    if (_X3) { X3 = 1; Not3 = 0; }
        //    else { X3 = 0; Not3 = 1; }

        //    if (_X5) { X5 = 1; Not5 = 0; }
        //    else { X5 = 0; Not5 = 1; }

        //    for (int i = 0; i < 7; i++)
        //    {
        //        if (vectorX[i]) { X[i] = 1; NotX[i] = 0; }
        //        else { X[i] = 0; NotX[i] = 1; }
        //    }

        //    A[Dt] = 1;

        //    Dt = (byte)((A[0] & X[0]) | (A[2] & X3) | (A[4]) | (A[6] & X[4] & X5));

        //    Dt += (byte)((A[1] & NotX[1] & Not2) | (A[2] & X3) | (A[5]) | (A[6] & X[4] & X5)) << 1;

        //    Dt += (byte)((A[3]) | (A[6] & NotX[4]) | (A[4]) | (A[5]) | (A[6] & X[4] & X5)) << 2;

        //    Dt += (byte)((A[1] & NotX[1] & X2) | (A[1] & X[1]) | (A[2] & Not3) |
        //        (A[6] & X[4] & Not5 & X[6]) | (A[7] & X[6])) << 3;

        //}
    }
}
