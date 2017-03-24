using System;

namespace Long_arithmetic
{
    struct MplbLongArithm
    {
        private string Value; // поле, которое содержит число
        private int length; // поле, которое содержит кол-во разрядов числа

        public string Number // свойство, которое возвращает либо задаёт число, которе хранится в данной структуре
        {
            get
            {
                return Value;
            }

            set
            {
                if (IsValid(value))
                {
                    this.Value = value;
                    this.length = value.Length;
                }
                else
                {
                    this.Value = "NaN";
                    this.length = -1;
                }
            }
        }

        public string Module // свойство, которое возвращает модуль числа, которе хранится в этой структуре
        {
            get
            {
                return (this.Value[0] == '-') ? this.Value.Remove(0, 1) : this.Value;
            }
        }

        public bool Sign // свойство, которое возвращает знак числа, которое хранится в этой структуре
        {
            get
            {
                return (this[0] == '-') ? true : false ;
            }
        }

        public int Length // свойство, которое возвращает кол-во разрядов числа
        {
            get
            {
                return length;
            }
        }

        public char this[int ind] // индексация структуры; возвращает значение указанного разряда числа
        {
            get
            {
                return this.Value[ind];
            }
        }

        public MplbLongArithm(string in_num) // конструктор, который заполняет число входящей в него строкой
        {
            if (IsValid(in_num))
            {
                this.Value = in_num;
                this.length = in_num.Length;
            }
            else
            {
                this.Value = "NaN";
                this.length = -1;
            }
        }

        public MplbLongArithm(MplbLongArithm number) // конструктор, который принимает дилнное число и делает текущий экземпляр структуры = вх. аргументу
        {
            this = number;
        }

        public static MplbLongArithm operator +(MplbLongArithm left, MplbLongArithm right) // перегрузка оператора сложения; реализация сложения длинных чисел
        {
            if ((left.Value != "NaN" && right.Value != "NaN") || (left.Length >= 0 && right.Length >= 0))
            {
                MplbLongArithm left_mod = new MplbLongArithm(left.Module),
                right_mod = new MplbLongArithm(right.Module);

                if (!left.Sign && !right.Sign)
                {
                    string left_val = left.Value, right_val = right.Value;
                    SetWithZeroes(ref left_val, ref right_val);

                    string result = "";
                    byte carry = 0, temp = 0;
                    for (int i = left_val.Length - 1; i >= 0; i--)
                    {
                        temp = (byte)(left_val[i] - 48 + right_val[i] - 48 + carry);
                        carry = (temp < 10) ? (byte)0 : (byte)1;
                        result += (temp < 10) ? Convert.ToString(temp) : Convert.ToString(temp - 10);
                    }

                    if (carry > 0)
                        result += 1;

                    result = ReverseString(result);
                    return new MplbLongArithm(result);
                }

                if (left.Sign && !right.Sign)
                    return right_mod - left_mod;

                if (!left.Sign && right.Sign)
                    return left_mod - right_mod;

                return !(left_mod + right_mod);
            }
            else
                return new MplbLongArithm("E");
        }

        public static MplbLongArithm operator -(MplbLongArithm left, MplbLongArithm right) // перегрузка оператора вычитания; реализация вычитания длинных чисел
        {
            if ((left.Value != "NaN" && right.Value != "NaN") || (left.Length >= 0 && right.Length >= 0))
            {
                if (left == right)
                    return new MplbLongArithm("0");

                string delta = "";
                MplbLongArithm result,
                        left_mod = new MplbLongArithm(left.Module),
                        right_mod = new MplbLongArithm(right.Module);

                if ((left.Sign && right.Sign) ||
                    (!left.Sign && !right.Sign))
                {
                    string greater_mod = (left > right) ? left.Module : right.Module,
                        less_mod = (left > right) ? right.Module : left.Module;
                    SetWithZeroes(ref greater_mod, ref less_mod);
                    
                    for (int i = greater_mod.Length - 1; i >= 0; i--)
                    {
                        if (greater_mod[i] >= less_mod[i])
                            delta += (char)((greater_mod[i] - less_mod[i]) + 48);
                        else
                        {
                            delta += (char)(((greater_mod[i] + 10) - less_mod[i]) + 48);
                            int k = 1;
                            while (greater_mod[i - k] == '0')
                            {
                                greater_mod = greater_mod.Remove(i - k, 1);
                                greater_mod = greater_mod.Insert(i - k, "9");
                                k++;
                            }
                            char temp = (char)(greater_mod[i - k] - 1);
                            greater_mod = greater_mod.Remove(i - k, 1);
                            greater_mod = greater_mod.Insert(i - k, temp.ToString());
                        }
                    }

                    delta = ReverseString(delta);

                    result = new MplbLongArithm(delta);
                    return ((left_mod > right_mod && !left.Sign) ||
                    (left_mod < right_mod && left.Sign)) ? result : !result;
                }

                return (!left.Sign && right.Sign) ? left_mod + right_mod : !(left_mod + right_mod);
            }
            else
                return new MplbLongArithm("E");
        }

        public static MplbLongArithm operator *(MplbLongArithm left, MplbLongArithm right) // перегрузка оператора умножениея; реализация умножения длинных чисел
        {
            if ((left.Value != "NaN" && right.Value != "NaN") || (left.Length >= 0 && right.Length >= 0))
            {
                string min = (left > right) ? right.Module : left.Module,
                    max = (left > right) ? left.Module : right.Module;

                string[] terms = new string[min.Length];
                for (int i = 0; i < terms.Length; i++)
                    terms[i] = "";
                
                for (int i = min.Length - 1; i >= 0; i--)
                {
                    string prod = "00";
                    for (int j = max.Length - 1; j >= 0; j--)
                    {
                        prod = Convert.ToString(((min[i] - 48) * (max[j] - 48)) + (prod[0] - 48));

                        if (prod.Length == 1)
                            prod = prod.Insert(0, "0");

                        terms[min.Length - 1 - i] += prod[1];
                    }

                    terms[min.Length - 1 - i] += prod[0];
                    terms[min.Length - 1 - i] = ReverseString(terms[min.Length - 1 - i]);
                    terms[min.Length - 1 - i] += new string('0', min.Length - 1 - i);
                }

                MplbLongArithm result = new MplbLongArithm(terms[0]);
                for (int i = 1; i < min.Length; i++)
                    result += new MplbLongArithm(terms[i]);

                if (left.Sign && !right.Sign || !left.Sign && right.Sign)
                    return !result;

                return result;
            }
            else
                return new MplbLongArithm("E");
        }

        public static bool operator >(MplbLongArithm left, MplbLongArithm right) // перегрузка оператора > для сравнения 2-х длинных чисел
        {
            if ((left.Value == "NaN" && right.Value == "NaN") ||
                (left.Value == "NaN" && right.Value != "NaN") ||
                (left.Sign && !right.Sign))
                return false;

            if ((left.Value != "NaN" && right.Value == "NaN")||
                (!left.Sign && right.Sign))
                return true;

            if ((!left.Sign && !right.Sign) ||
                (left.Sign && right.Sign))
            {
                string left_val = left.Module, rigth_val = right.Module;
                SetWithZeroes(ref left_val, ref rigth_val);
                for (int i = 0; i < left_val.Length; i++)
                    if (left_val[i] > rigth_val[i])
                        return true;
                    else if (left_val[i] < rigth_val[i])
                        return false;
            }

            return false;
        }

        public static bool operator <(MplbLongArithm left, MplbLongArithm right) // перегрузка оператора < для сравнения 2-х длинных чисел
        {
            if (left > right) return false;

            if (left.Value == right.Value) return false;

            return true;
        }

        public static bool operator ==(MplbLongArithm left, MplbLongArithm right) // перегрузка оператора == для сравнения 2-х длинных чисел
        {
            string left_val = left.Value, right_val = right.Value;
            SetWithZeroes(ref left_val, ref right_val);
            for (int i = 0; i < left_val.Length; i++)
                if (left_val[i] != right_val[i])
                    return false;
            return true;
        }

        public static bool operator !=(MplbLongArithm left, MplbLongArithm right) // перегрузка оператора != для сравнения 2-х длинных чисел
        {
            if (left == right)
                return false;
            return true;
        }

        public static MplbLongArithm operator !(MplbLongArithm right) // перегрузка оператора !; возвращает длинное число, противоположное данному длинному числу
        {
            MplbLongArithm result = new MplbLongArithm(right);
            if (result[0] == '-')
                result.Value = result.Value.Remove(0, 1);
            else
                result.Value = result.Value.Insert(0, "-");

            return result;
        }

        public static string ReverseString(string s) // статический метод, который переворачивает строку
        {
            string result = "";
            for (int i = s.Length - 1; i >= 0; i--)
                result += s[i];
            return result;
        }

        public static bool IsValid(string s) // статический метод, который проверяет, содержится ли в строке только число
        {
            for (int i = (s[0] == '-') ? 1 : 0; i < s.Length; i++)
                if ((s[i] > 57) || (s[i] < 48))
                    return false;
            return true;
        }

        public static void SetWithZeroes(ref string a, ref string b) // статический метод, который добавляет нули в
                                                                    //начало числа с меньшим кол-вом разрядов,
                                                                    //пока длины чисел не станут одинаковыми
        {
                string temp = new string('0', Math.Abs(a.Length - b.Length));
                if (a.Length > b.Length)
                    b = b.Insert(0, temp);
                else
                    a = a.Insert(0, temp);
        }

        public static void DeleteZeroes(ref string a) // статический метод удаляет лишние нули в начале числа
        {
            if (ContainsChar(a, '0') == a.Length)
                a = "0";
            else
            {
                int i = (a[0] == '-') ? 1 : 0, j = i, k = 0;

                while (i < a.Length && a[i] == '0')
                {
                    k++;
                    i++;
                }

                a = a.Remove(j, k);
            }
        }

        public static uint ContainsChar(string s, char ch) // статический метод, который возвращает кол-во вхождений символа в строку
        {
            uint res = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ch)
                    res++;
            }

            return res;
        }

        public override string ToString() // переопределение ToString, возвращает число
        {
            string temp = this.Value;
            DeleteZeroes(ref temp);
            return temp;
        }
    }
}
