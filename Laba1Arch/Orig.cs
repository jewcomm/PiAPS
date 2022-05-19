using System.IO;
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
                     Model.vRes_s = new string[Convert.ToInt32(Console.ReadLine())];
                     for (int i = 0; i < Model.vRes_s.Length; i++) Model.vRes_s[i] = "F";                }
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
public static string[] vRes_s;//Модель набора ресурсов
public static void Occupy(string cn)
            {
                if ((Convert.ToInt16(cn) > vRes_s.Length) | (Convert.ToInt16(cn) < 0)) throw new ResIdInvalid();
                if(vRes_s[Convert.ToInt16(cn) - 1] == "B")throw new ResIsBusy();
                vRes_s[Convert.ToInt16(cn) - 1] = "B";
            }
public static void Free(string cn)
            {
                if ((Convert.ToInt16(cn) > vRes_s.Length) | (Convert.ToInt16(cn) < 0)) throw new ResIdInvalid();
                if (vRes_s[Convert.ToInt16(cn) - 1] == "F") throw new ResWasFree();
                vRes_s[Convert.ToInt16(cn) - 1] = "F";
            }
public static string Request()
            {
                for(int i=0;i<vRes_s.Length;i++)
                {
                    if(vRes_s[i] == "F") return Convert.ToString(i + 1);
                }
                throw new ResAreBusy(); ;
            }
}
class Program
{
    static void Main(string[] args)
    { 
		string Command;
        while (!SetUp.On()); 
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
			}
			catch (OverflowException) { Console.WriteLine("Такого ресурса нет."); }
			catch (FormatException) { Console.WriteLine("Такого ресурса нет."); }
			catch (ResIdInvalid) { Console.WriteLine("Такого ресурса нет."); }
			catch (ResWasFree) { Console.WriteLine("Ресурс был свободен."); }
			catch (ResAreBusy) { Console.WriteLine("Все ресурсы заняты."); }
			catch (ResIsBusy) { Console.WriteLine("ресурс уже занят."); }
} 
while(Command != "");
        }
    }
}
