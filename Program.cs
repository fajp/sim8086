using System;

namespace emu8086
{
    class Program
    {
        //Funkcja sprawdzająca czy podana wartość jest liczbą heksadecymalną
        public static bool OnlyHexInString(string test)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(test, @"\A\b[0-9a-fA-F]+\b\Z");
        }
        //Funkcja konwertująca 4-znakową liczbę heksadecymalną na format [xxxx]
        public static string HexToRegHex(string hexValue)
        {
            if (hexValue.Length > 3)
            {
                return hexValue;
            }
            else if (hexValue.Length == 3)
            {
                return "0" + hexValue;
            }
            else if (hexValue.Length == 2)
            {
                return "00" + hexValue;
            }
            else if (hexValue.Length == 1)
            {
                return "000" + hexValue;
            }
            else
            {
                return hexValue;
            }
        }
        //Konwerter 4-znakowych liczb heksadecymalnych na decymalne
        public static void ConvertToDec(string input, ref int exit)
        {
            if (OnlyHexInString(input) == true && input.Length < 5)
            {
                exit = Convert.ToInt32(input, 16);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Podana wartość nie jest liczbą heksadecymalną lub zawiera zbyt dużo bajtów!");
                Console.WriteLine();
            }
        }
        //Konwerter 4-znakowych liczb heksadecymalnych na decymalne dla dolnego rejestru
        public static void ConvertToDecL(string input, ref int exit)
        {
            if (OnlyHexInString(input) == true && input.Length < 3)
            {
                input = HexToRegHex(input);
                string temp = HexToRegHex(exit.ToString("X"));
                temp = "" + temp[0] + temp[1] + input[2] + input[3];
                exit = Convert.ToInt32(temp, 16);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Podana wartość nie jest liczbą heksadecymalną lub zawiera zbyt dużo bajtów!");
                Console.WriteLine();
            }
        }
        //Konwerter 4-znakowych liczb heksadecymalnych na decymalne dla górnego rejestru
        public static void ConvertToDecH(string input, ref int exit)
        {
            if (OnlyHexInString(input) == true && input.Length < 3)
            {
                input = HexToRegHex(input);
                string temp = HexToRegHex(exit.ToString("X"));
                temp = "" + input[2] + input[3] + temp[2] + temp[3];
                exit = Convert.ToInt32(temp, 16);
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Podana wartość nie jest liczbą heksadecymalną lub zawiera zbyt dużo bajtów!");
                Console.WriteLine();
            }
        }
        public static void  Mov(ref int exit, int input, string mode) //Symulacja komendy MOV dla rejestrów górnych (h) i dolnych (l)
        {
            if (mode.Equals("hh", StringComparison.OrdinalIgnoreCase))
            {
                string hexExit = HexToRegHex(exit.ToString("X"));
                string hexInput = HexToRegHex(input.ToString("X"));
                string temp = "" + hexInput[0] + hexInput[1] + hexExit[2] + hexExit[3];
                exit = Convert.ToInt32(temp, 16);
            }
            else if (mode.Equals("ll", StringComparison.OrdinalIgnoreCase))
            {
                string hexExit = HexToRegHex(exit.ToString("X"));
                string hexInput = HexToRegHex(input.ToString("X"));
                string temp = "" + hexExit[0] + hexExit[1] + hexInput[2] + hexInput[3];
                exit = Convert.ToInt32(temp, 16);
            }
            else if (mode.Equals("hl", StringComparison.OrdinalIgnoreCase))
            {
                string hexExit = HexToRegHex(exit.ToString("X"));
                string hexInput = HexToRegHex(input.ToString("X"));
                string temp = "" + hexInput[2] + hexInput[3] + hexExit[2] + hexExit[3];
                exit = Convert.ToInt32(temp, 16);
            }
            else if (mode.Equals("lh", StringComparison.OrdinalIgnoreCase))
            {
                string hexExit = HexToRegHex(exit.ToString("X"));
                string hexInput = HexToRegHex(input.ToString("X"));
                string temp = "" + hexExit[0] + hexExit[1] + hexInput[0] + hexInput[1];
                exit = Convert.ToInt32(temp, 16);
            }
        }
        public static void XCHG(ref int exit, ref int input, string mode)
        {
            if (mode.Equals("xx", StringComparison.OrdinalIgnoreCase))
            {
                int temp = input;
                input = exit;
                exit = temp;
            }
            else if (mode.Equals("hh", StringComparison.OrdinalIgnoreCase))
            {
                int temp = input;
                Mov(ref input, exit, "hh");
                Mov(ref exit, temp, "hh");
            }
            else if (mode.Equals("hl", StringComparison.OrdinalIgnoreCase))
            {
                int temp = input;
                Mov(ref input, exit, "lh");
                Mov(ref exit, temp, "hl");
            }
            else if (mode.Equals("ll", StringComparison.OrdinalIgnoreCase))
            {
                int temp = input;
                Mov(ref input, exit, "ll");
                Mov(ref exit, temp, "ll");
            }
        }
        static void Main(string[] args)
        {
            Console.WriteLine("Symulator Intel 8086\n" +
            "Autor: Filip Piechnik, nr albumu: 12895\n\n" +
            "Aby wyświetlić dostępne komendy wpisz 'h'\n" +
            "Aby zakończyć działanie programu wpisz 'q'\n");

            int ax, bx, cx, dx; //Zmienne przechowujące wartość rejestrów w systemie dziesiętnym
            ax = bx = cx = dx = 0;

            string input = null;

            //Pętla programu
            do
            {

                input = Console.ReadLine(); //Pobieranie komendy od użytkownika
                string[] Input = input.Split(' '); //Rozdzielenie pobranego stringa na tabelę substringów oddzielonych spacją

                if (Input[0].Equals("h", StringComparison.OrdinalIgnoreCase)) //Wyświetla listę dostępnych komend
                {
                    Console.WriteLine("\nDostępne komendy:\n" +
                        "   q - zakończenie działania programu\n" +
                        "   r - wyświetlenie aktualnego stanu rejestrów\n" +
                        "   [rejestr] [wartość] - przypisanie wartości (HEX) do rejestru\n" +
                        "   mov [rejestr docelowy] [rejestr źródłowy] - skopiowanie wartości rejestrów\n" +
                        "   xchg [rejestr docelowy] [rejestr źródłowy] - zamiana wartości rejestrów\n" +
                        "   dostępne rejestry: AH, AL, AX, BH, BL, BX, CH, CL, CX, DH, DL, DX\n");
                }
                else if (Input[0].Equals("r", StringComparison.OrdinalIgnoreCase)) //Wyświetla stan rejestrów
                {
                    Console.WriteLine();
                    Console.WriteLine("AX: " + HexToRegHex(ax.ToString("X")));
                    Console.WriteLine("BX: " + HexToRegHex(bx.ToString("X")));
                    Console.WriteLine("CX: " + HexToRegHex(cx.ToString("X")));
                    Console.WriteLine("DX: " + HexToRegHex(dx.ToString("X")));
                    Console.WriteLine();
                }
                else if (Input.Length == 2) //Komendy przypisywania wartości do poszczególnych rejestrów
                {
                    if (Input[0].Equals("ax", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDec(Input[1], ref ax);
                    }
                    else if (Input[0].Equals("al", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDecL(Input[1], ref ax);
                    }
                    else if (Input[0].Equals("ah", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDecH(Input[1], ref ax);
                    }
                    else if (Input[0].Equals("bx", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDec(Input[1], ref bx);
                    }
                    else if (Input[0].Equals("bl", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDecL(Input[1], ref bx);
                    }
                    else if (Input[0].Equals("bh", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDecH(Input[1], ref bx);
                    }
                    else if (Input[0].Equals("cx", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDec(Input[1], ref cx);
                    }
                    else if (Input[0].Equals("cl", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDecL(Input[1], ref cx);
                    }
                    else if (Input[0].Equals("ch", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDecH(Input[1], ref cx);
                    }
                    else if (Input[0].Equals("dx", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDec(Input[1], ref dx);
                    }
                    else if (Input[0].Equals("dl", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDecL(Input[1], ref dx);
                    }
                    else if (Input[0].Equals("dh", StringComparison.OrdinalIgnoreCase))
                    {
                        ConvertToDecH(Input[1], ref dx);
                    }
                    else
                    {
                        Console.WriteLine();
                    }
                }
                else if (Input.Length == 3 && Input[0].Equals("mov", StringComparison.OrdinalIgnoreCase))
                //Komeda mov dla pełnych rejestrów
                {
                    if (Input[1].Equals("ax", StringComparison.OrdinalIgnoreCase)) //Kopiowanie wartości do AX
                    {
                        if (Input[2].Equals("bx", StringComparison.OrdinalIgnoreCase))
                        {
                            ax = bx;
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("cx", StringComparison.OrdinalIgnoreCase))
                        {
                            ax = cx;
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dx", StringComparison.OrdinalIgnoreCase))
                        {
                            ax = dx;
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }
                    else if (Input[1].Equals("bx", StringComparison.OrdinalIgnoreCase)) //Kopiowanie wartości do BX
                    {
                        if (Input[2].Equals("ax", StringComparison.OrdinalIgnoreCase))
                        {
                            bx = ax;
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("cx", StringComparison.OrdinalIgnoreCase))
                        {
                            bx = cx;
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dx", StringComparison.OrdinalIgnoreCase))
                        {
                            bx = dx;
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }
                    else if (Input[1].Equals("cx", StringComparison.OrdinalIgnoreCase)) //Kopiowanie wartości do CX
                    {
                        if (Input[2].Equals("ax", StringComparison.OrdinalIgnoreCase))
                        {
                            cx = ax;
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bx", StringComparison.OrdinalIgnoreCase))
                        {
                            cx = bx;
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dx", StringComparison.OrdinalIgnoreCase))
                        {
                            cx = dx;
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }
                    else if (Input[1].Equals("dx", StringComparison.OrdinalIgnoreCase)) //Kopiowanie wartości do DX
                    {
                        if (Input[2].Equals("ax", StringComparison.OrdinalIgnoreCase))
                        {
                            dx = ax;
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bx", StringComparison.OrdinalIgnoreCase))
                        {
                            dx = bx;
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("cx", StringComparison.OrdinalIgnoreCase))
                        {
                            dx = cx;
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }
                    // Komenda MOV dla podrejestrów H i L
                    else if (Input[1].Equals("ah", StringComparison.OrdinalIgnoreCase)) //Dla rejestru AH
                    {
                        if (Input[2].Equals("al", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, ax, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, bx, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, bx, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, cx, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, cx, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, dx, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, dx, "hl");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }
                    else if (Input[1].Equals("al", StringComparison.OrdinalIgnoreCase)) //Dla rejestru AL
                    {
                        if (Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, ax, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, bx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, bx, "ll");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, cx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, cx, "ll");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, dx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref ax, dx, "ll");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }
                    else if (Input[1].Equals("bh", StringComparison.OrdinalIgnoreCase)) //Dla rejestru BH
                    {
                        if (Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, bx, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, ax, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("al", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, ax, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, cx, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, cx, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, dx, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, dx, "hl");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }
                    else if (Input[1].Equals("bl", StringComparison.OrdinalIgnoreCase)) //Dla rejestru BL
                    {
                        if (Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, bx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, ax, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("al", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, ax, "ll");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, cx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, cx, "ll");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, dx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref bx, dx, "ll");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }
                    else if (Input[1].Equals("ch", StringComparison.OrdinalIgnoreCase)) //Dla rejestru CH
                    {
                        if (Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, cx, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, ax, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("al", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, ax, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, bx, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, bx, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, dx, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, dx, "hl");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }
                    else if (Input[1].Equals("cl", StringComparison.OrdinalIgnoreCase)) //Dla rejestru CL
                    {
                        if (Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, cx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, ax, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("al", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, ax, "ll");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, bx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, bx, "ll");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, dx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref cx, dx, "ll");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }
                    else if (Input[1].Equals("dh", StringComparison.OrdinalIgnoreCase)) //Dla rejestru DH
                    {
                        if (Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, dx, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, ax, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("al", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, ax, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, bx, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, bx, "hl");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, cx, "hh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, cx, "hl");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }
                    else if (Input[1].Equals("dl", StringComparison.OrdinalIgnoreCase)) //Dla rejestru DL
                    {
                        if (Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, dx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, ax, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("al", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, ax, "ll");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, bx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, bx, "ll");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, cx, "lh");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        {
                            Mov(ref dx, cx, "ll");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }
                    }

                }
                else if (Input.Length == 3 && Input[0].Equals("xchg", StringComparison.OrdinalIgnoreCase))
                {
                    //XCHG dla AX i BX
                    if ((Input[1].Equals("ax", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bx", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("bx", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ax", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref bx, "xx");
                        Console.WriteLine();
                        /*if (Input[2].Equals("bx", StringComparison.OrdinalIgnoreCase))
                        {
                            XCHG(ref ax, ref bx, "xx");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("cx", StringComparison.OrdinalIgnoreCase))
                        {
                            XCHG(ref ax, ref cx, "xx");
                            Console.WriteLine();
                        }
                        else if (Input[2].Equals("dx", StringComparison.OrdinalIgnoreCase))
                        {
                            XCHG(ref ax, ref dx, "xx");
                            Console.WriteLine();
                        }
                        else
                        {
                            Console.WriteLine("Nieprawidłowy rejestr!");
                            Console.WriteLine();
                        }*/
                    }
                    //XCHG dla AX i CX
                    else if ((Input[1].Equals("ax", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("cx", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("cx", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ax", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref cx, "xx");
                        Console.WriteLine();
                    }
                    //XCHG dla AX i DX
                    else if ((Input[1].Equals("ax", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dx", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dx", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ax", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref dx, "xx");
                        Console.WriteLine();
                    }
                    //XCHG dla BX i CX
                    else if ((Input[1].Equals("bx", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("cx", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("cx", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bx", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref bx, ref cx, "xx");
                        Console.WriteLine();
                    }
                    //XCHG dla BX i DX
                    else if ((Input[1].Equals("bx", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dx", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dx", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bx", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref bx, ref dx, "xx");
                        Console.WriteLine();
                    }
                    //XCHG dla CX i DX
                    else if ((Input[1].Equals("cx", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dx", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dx", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("cx", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref cx, ref dx, "xx");
                        Console.WriteLine();
                    }
                    //XCHG dla AH i AL
                    else if ((Input[1].Equals("ah", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("al", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("al", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref ax, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla AH i BH
                    else if ((Input[1].Equals("ah", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("bh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref bx, "hh");
                        Console.WriteLine();
                    }
                    //XCHG dla AH i BL
                    else if ((Input[1].Equals("ah", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("bl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref bx, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla AH i CH
                    else if ((Input[1].Equals("ah", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("ch", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref cx, "hh");
                        Console.WriteLine();
                    }
                    //XCHG dla AH i CL
                    else if ((Input[1].Equals("ah", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("cl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref cx, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla AH i DH
                    else if ((Input[1].Equals("ah", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref dx, "hh");
                        Console.WriteLine();
                    }
                    //XCHG dla AH i DL
                    else if ((Input[1].Equals("ah", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ah", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref dx, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla AL i BH
                    else if ((Input[1].Equals("al", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("bh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("al", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref bx, ref ax, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla AL i BL
                    else if ((Input[1].Equals("al", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("bl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("al", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref bx, "ll");
                        Console.WriteLine();
                    }
                    //XCHG dla AL i CH
                    else if ((Input[1].Equals("al", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("ch", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("al", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref cx, ref ax, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla AL i CL
                    else if ((Input[1].Equals("al", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("cl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("al", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref cx, "ll");
                        Console.WriteLine();
                    }
                    //XCHG dla AL i DH
                    else if ((Input[1].Equals("al", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("al", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref dx, ref ax, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla AL i DL
                    else if ((Input[1].Equals("al", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("al", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref ax, ref dx, "ll");
                        Console.WriteLine();
                    }
                    //XCHG dla BH i BL
                    else if ((Input[1].Equals("bh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("bl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref bx, ref bx, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla BH i CH
                    else if ((Input[1].Equals("bh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("ch", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref bx, ref cx, "hh");
                        Console.WriteLine();
                    }
                    //XCHG dla BH i CL
                    else if ((Input[1].Equals("bh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("cl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref bx, ref cx, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla BH i DH
                    else if ((Input[1].Equals("bh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref bx, ref dx, "hh");
                        Console.WriteLine();
                    }
                    //XCHG dla BH i DL
                    else if ((Input[1].Equals("bh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bh", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref bx, ref dx, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla BL i CH
                    else if ((Input[1].Equals("bl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("ch", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref cx, ref bx, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla BL i CL
                    else if ((Input[1].Equals("bl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("cl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref bx, ref cx, "ll");
                        Console.WriteLine();
                    }
                    //XCHG dla BL i DH
                    else if ((Input[1].Equals("bl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("bl", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref dx, ref bx, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla CH i CL
                    else if ((Input[1].Equals("ch", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("cl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("cl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref cx, ref cx, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla CH i DH
                    else if ((Input[1].Equals("ch", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref cx, ref dx, "hh");
                        Console.WriteLine();
                    }
                    //XCHG dla CH i DL
                    else if ((Input[1].Equals("ch", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("ch", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref cx, ref dx, "hl");
                        Console.WriteLine();
                    }
                    //XCHG dla DH i DL
                    else if ((Input[1].Equals("dh", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dl", StringComparison.OrdinalIgnoreCase))
                        || (Input[1].Equals("dl", StringComparison.OrdinalIgnoreCase) && Input[2].Equals("dh", StringComparison.OrdinalIgnoreCase)))
                    {
                        XCHG(ref dx, ref dx, "hl");
                        Console.WriteLine();
                    }
                    else
                    {
                        Console.WriteLine("Nieprawidłowy rejestr!");
                        Console.WriteLine();
                    }
                }
                else if (input.Equals("q", StringComparison.OrdinalIgnoreCase) == false) //Nieznana komenda
                {
                    Console.WriteLine("Nieznana komenda!");
                    Console.WriteLine();
                }

            } while (input.Equals("q", StringComparison.OrdinalIgnoreCase) == false); //Zatrzymanie programu
            
        }
        
    }
}
