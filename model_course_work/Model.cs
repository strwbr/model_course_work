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
        uint A; // Делимое
        uint B; // Делитель
        uint AM; // Доп. регистр делимого
        uint BM; // Доп. регистр делителя
        uint C; // Частное
        uint Ddd; // Остаток
        int count; // Счетчик
        bool start; // Флаг начала
        bool end; // Флаг завершения
        int state; // Состояние
        int PP; // Признак переполнения
        bool[] X = new bool[7]; // Вектор X-в
        // Условия для ПЛУ
        bool _X2;
        bool _X3;
        bool _X5;
        byte[] Y = new byte[18]; // Вектор У-в
        int Dt; // Код состояния (kA)

        public void SetStart() { start = true; }

        public bool GetEnd() { return end; }

        public int GetState() { return state; }

        public uint GetC() { return C; }

        public void Counter() //вычитание счётчика
        {
            if (count == 0) count = 15;
            else count--;
        }

        public Model(uint a, uint b, uint c, int d)
        {
            A = a;
            B = b;
            C = c;

            PP = 0;
            AM = 0;
            BM = 0;
            Ddd = 0;
            count = 0;
            end = false;
            start = false;
            state = 0;
            Dt = 0;
            for (int i = 0; i < 18; i++)
                Y[i] = 0;
            for (int i = 0; i < 7; i++)
                X[i] = false;
            X[0] = true;
            _X2 = X2();
            _X3 = X3();
            X[2] = _X2;
            X[3] = _X3;
        }

        public Model(Model temp) //конструктор с параметром
        {
            A = temp.A;
            B = temp.B;
            C = temp.C;
            AM = temp.AM;
            BM = temp.BM;
            Ddd = temp.Ddd;
            PP = temp.PP;
            count = 0;
            end = false;
            start = false;
            state = 0;
            for (int i = 0; i < 18; i++)
                Y[i] = temp.Y[i];
            for (int i = 0; i < 7; i++)
                X[i] = temp.X[i];
            _X3 = temp._X3;
            _X2 = temp._X2;
            Dt = temp.Dt;
        }

        public Model() { }

        // Методы для X-в
        // Пуск
        private bool X0()
        {
            return start;
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
            return (AM & 0x80000000) != 0; // 0x80000000 = 1000 0000 0000 0000 0000 0000 0000 0000
            //return (AM >> 31) == 1;
        }
        // CR = 0 - проверка окончания цикла
        private bool X4()
        {
            return count == 0;
            //if (count == 0) return true;
            //else return false;
        }
        // С(0)
        private bool X5()
        {
            //return ((C << 31) >> 31) == 1;
            return (C & 1) == 1;
        }
        //A(15) xor B(15)
        private bool X6()
        {
            return (A & 0x8000) != (B & 0x8000); // 0x8000 = 0000 0000 0000 0000 1000 0000 0000 0000
            //return (A >> 15) != (B >> 15);
        }

        // Методы вычисления Y-в
        // АМ(29:15) := А(14:0)
        private void Y1()
        {
            AM = (A & 0x7FFF) << 15;
        }
        // ВМ(29:15) := В(14:0)
        private void Y2() //BM(31;15)=B(14;0)
        {
            BM = (B & 0x7FFF) << 15; // 0x7FFF = 0000 0000 0000 0000 0111 1111 1111 1111
        }
        // АМ(14:0) 
        private void Y3()
        {
            // В АМ(14:0) уже изначально нули
        }
        // ВМ(14:0) 
        private void Y4()
        {
            // В ВМ(14:0) уже изначально нули
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
            Ddd = AM;
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
            count = 0;
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
            AM = Ddd;
        }
        // CR := CR-1
        private void Y14()   
        {
            Counter();
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
            end = true;
        }

        public int GetPP()
        {
            return PP;
        }


        //режим микропрограммы
        public void Microprogram2()
        {
            switch (state)
            {
                case 0:
                    if (!X0())
                    {
                        state = 0;
                        break;
                    }
                    if (X0())
                    {
                        Y1();
                        Y2();
                        Y3();
                        Y4();
                        state = 1;
                        break;
                    }
                    break;
                case 1:
                    if (!X1() && !X2())
                    {
                        Y5();
                        state = 2;
                        break;
                    }
                    if (!X1() && X2())
                    {
                        Y9();
                        state = 8;
                        break;
                    }
                    if (X1())
                    {
                        Y17();
                        state = 8;
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
                        state = 3;
                        break;
                    }
                    if (!X3())
                    {
                        Y17();
                        state = 8;
                        break;
                    }
                    break;
                case 3:
                    Y5();
                    state = 4;
                    break;
                case 4:
                    if (!X3())
                    {
                        Y11();
                        state = 5;
                        break;
                    }
                    if (X3())
                    {
                        Y12();
                        Y13();
                        state = 5;
                        break;
                    }
                    break;
                case 5:
                    Y7();
                    Y8();
                    Y14();
                    state = 6;
                    break;
                case 6:
                    if (X4() && !X5() && !X6())
                    {
                        Yk();
                        state = 9;
                        break;
                    }
                    if (!X4())
                    {
                        Y5();
                        state = 4;
                        break;
                    }
                    if (X4() && X5())
                    {
                        Y15();
                        state = 7;
                        break;
                    }
                    if (X4() && !X5() && X6())
                    {
                        Y16();
                        state = 8;
                        break;
                    }
                    break;
                case 7:
                    if (!X6())
                    {
                        Yk();
                        state = 9;
                        break;
                    }
                    if (X6())
                    {
                        Y16();
                        state = 8;
                        break;
                    }
                    break;
                case 8:
                    Yk();
                    state = 9;
                    break;
            }
        }

        //метод, выводит значение операндов на визуальный слой
        public void ShowValues(ref DataGridView data1, ref DataGridView data2,
            ref DataGridView data3, ref DataGridView data4)
        {
            uint C1 = C;
            uint A1 = AM;
            uint B1 = BM;
            int count1 = count;
            for (int i = 16; i >= 0; i--)
            {
                if (C1 % 2 == 0) { data3[i, 0].Value = 0; }
                else { data3[i, 0].Value = 1; }
                C1 = (UInt32)(C1 / 2);
            }
            for (int i = 31; i >= 0; i--)
            {
                if (A1 % 2 == 0) { data1[i, 0].Value = 0; }
                else { data1[i, 0].Value = 1; }
                A1 = (UInt32)(A1 / 2);
            }
            for (int i = 31; i >= 0; i--)
            {
                if (B1 % 2 == 0) { data2[i, 0].Value = 0; }
                else { data2[i, 0].Value = 1; }
                B1 = (UInt32)(B1 / 2);
            }
            for (int i = 3; i >= 0; i--)
            {
                if (count1 % 2 == 0) { data4[i, 0].Value = 0; }
                else { data4[i, 0].Value = 1; }
                count1 = (Int32)(count1 / 2);
            }
        }

        public int Get_Dt() { return Dt; }

        public byte[] Get_Y() { return Y; }

        public bool[] Get_X() { return X; }

        public bool Get_X3() { return _X3; }

        public bool Get_X2() { return _X2; }
        public bool Get_X5() { return _X5; }

        //Память логических условий
        public void Logic_Memory_Cond(bool[] X)
        {
            _X3 = X[3];
            _X2 = X[2];
            _X5 = X[5];
        }

        //Комбинационная схема векторов Y
        public void KSY(bool[] Xx)
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

        //Операционный автомат
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

        //Комбинационная схема векторов D
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
    }
}
