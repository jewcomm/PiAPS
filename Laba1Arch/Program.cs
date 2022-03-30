/*
 * c - емкость ресурса = 1
 * s - размер ресурса = 1
 * v - цена ресурса = 0
 * pr - приоритет запроса = 0
 * wt - приемлимое время ожидания ресурса = не ограничено
 * tu - необходимая продолжительность обслуживания ресурса = целое число m > 0
 * pbu - вероятность отказа исп. ресурса = 0 
 * pbi - вероятность отказа св. ресурса = 0
 * Из ждущих выбирается запрос с наименьшим временем использования ресурса
 */

using System;
using System.IO;
using System.Timers;
using System.Threading;

namespace ResRegV1cons
{
    class ResAreBusy : Exception { }
    class ResIdInvalid : Exception { }
    class UnRecommended : Exception { }
    class ResIsBusy : Exception { }
    class ResWasFree : Exception { }
    static class SetUp
    {
        public static string Path; //путь к файлу, сохраняющему модель

        private static void ClearModel()
        {
            Console.WriteLine("Укажите количество ресурсов:");
            try
            {
                int size = Convert.ToInt32(Console.ReadLine());
                Model.vRes_s = new string[size];
                Model.tRes_s = new int[size];
                for (int i = 0; i < Model.vRes_s.Length; i++) { Model.vRes_s[i] = "F"; Model.tRes_s[i] = 0; }
            }
            catch
            {
                Console.WriteLine("Введено некорректное число!");
                ClearModel();
            }
        }

        private static void GetModel()
        {
            Console.WriteLine("Обновить файл?");
            if (Console.ReadLine().ToUpper() == "Y") ClearModel();
            else
            {
                Model.vRes_s = File.ReadAllLines(Path);
                Model.tRes_s = new int[Model.vRes_s.Length];
            }
        }
        public static bool On()
        {
            try
            {
                if (File.Exists(Directory.GetCurrentDirectory() + @"\Resmod00"))
                {
                    Console.WriteLine("Использовать существующий стандартный файл Resmod00?");
                    if (Console.ReadLine().ToUpper() == "Y")
                    {
                        Path = Directory.GetCurrentDirectory() + @"\Resmod00";
                        GetModel();
                        return true;
                    }
                }
                else
                {
                    Console.WriteLine("Создать стандартный файл?");
                    if (Console.ReadLine().ToUpper() == "Y")
                    {
                        Path = Directory.GetCurrentDirectory() + @"\Resmod00";
                        ClearModel();
                        return true;
                    }
                };
                Console.WriteLine("Введите полный адрес нестандартного файла:");
                Path = Console.ReadLine();
                if (File.Exists(Path))
                {
                    GetModel();
                    return true;
                }
                else
                {
                    ClearModel();
                    return true;
                }
            }
            catch (IOException) { Console.WriteLine("Файл не открылся."); return false; }
            catch (Exception) { Console.WriteLine("Ошибка ввода-вывода."); return false; }
        }
    }
    static class Model
    {
        private static System.Timers.Timer aTimer;

        public static string[] vRes_s;//Модель набора ресурсов

        public static int[] tRes_s; // время использования ресурсов

        public static void Diverse()
        {
            int nMinRes = -1;
            int min = Int32.MaxValue;
            for (int i = 0; i < vRes_s.Length; i++)
            {
                if (vRes_s[i] == "B" && tRes_s[i] < min)
                {
                    nMinRes = i + 1;
                    min = tRes_s[i];
                }
            }
            if(nMinRes == -1)
            {
                Console.WriteLine("Все ресурсы свободны");
                return;
            }
            else
            {
                Console.WriteLine($"Найден ресурс с наименьшим временем использования ресурса: {nMinRes}");
                Console.WriteLine("Освобождаем этот ресурс");
                Free(nMinRes.ToString());
            }

        }

        public static void Occupy(string cn)
        {
            AwaitResponse();
            if ((Convert.ToInt16(cn) > vRes_s.Length) | (Convert.ToInt16(cn) < 0)) throw new ResIdInvalid();
            if (vRes_s[Convert.ToInt16(cn) - 1] == "B") throw new ResIsBusy();
            vRes_s[Convert.ToInt16(cn) - 1] = "B";
            tRes_s[Convert.ToInt16(cn) - 1] = 0;
        }
        public static void Free(string cn)
        {
            AwaitResponse();
            if ((Convert.ToInt16(cn) > vRes_s.Length) | (Convert.ToInt16(cn) < 0)) throw new ResIdInvalid();
            if (vRes_s[Convert.ToInt16(cn) - 1] == "F") throw new ResWasFree();
            Console.WriteLine($"Время использования освобожденного ресурса: {tRes_s[Convert.ToInt16(cn) - 1]}");
            vRes_s[Convert.ToInt16(cn) - 1] = "F";
            tRes_s[Convert.ToInt16(cn) - 1] = 0;
        }
        public static string Request()
        {
            AwaitResponse();
            for (int i = 0; i < vRes_s.Length; i++)
            {
                if (vRes_s[i] == "F") return Convert.ToString(i + 1);
            }
            throw new ResAreBusy(); ;
        }

        private static void AwaitResponse()
        {
            Console.WriteLine("Ожидание...");
            Thread.Sleep(1000);
        }

        public static void SetTimer()
        {
            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimerEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private static void OnTimerEvent(Object source, ElapsedEventArgs e)
        {
            for (int i = 0; i < tRes_s.Length; i++) if(vRes_s[i] == "B") tRes_s[i]++;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string Command;
            while (!SetUp.On()) ;
            Model.SetTimer();
            do
            {
                File.WriteAllLines(SetUp.Path, Model.vRes_s);//сохранение модели
                Console.WriteLine("Введите команду:");
                Command = Console.ReadLine();
                Command = Command.ToUpper();
                try
                {
                    if (Command == "REQUEST") Console.WriteLine(Model.Request());
                    if (Command == "OCCUPY")
                    {
                        Console.WriteLine("Введите номер ресурса:");
                        Model.Occupy(Console.ReadLine());
                        Console.WriteLine("Ресурс стал занятым.");
                    };
                    if (Command == "FREE")
                    {
                        Console.WriteLine("Введите номер ресурса:");
                        Model.Free(Console.ReadLine());
                        Console.WriteLine("Ресурс освобождён.");
                    };
                    if (Command == "DIVERSE")
                    {
                        Model.Diverse();
                    };
                }
                catch (OverflowException) { Console.WriteLine("Такого ресурса нет."); }
                catch (FormatException) { Console.WriteLine("Такого ресурса нет."); }
                catch (ResIdInvalid) { Console.WriteLine("Такого ресурса нет."); }
                catch (ResWasFree) { Console.WriteLine("Ресурс был свободен."); }
                catch (ResAreBusy) { Console.WriteLine("Все ресурсы заняты."); }
                catch (ResIsBusy) { Console.WriteLine("ресурс уже занят."); }
            }
            while (Command != "");
        }
    }
}
