using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace dailyplannerApp
{
    class Program
    {
       static int index = 0;
       static int? select = null;
       static private List<string> menuItems = new List<string>() {
                "Посмотреть записи",
                "Посмотреть просроченные записи",
                "Сортировать записи",
                "Добавить запись",
                "Удалить запись",
                "Выход"
            };


        static void Main(string[] args)
        {
            Console.CursorVisible = false;
            //Запись из бд    

            Getdb();
           
            while (true)
            {

                MenuSelection(menuItems);

                switch (select)
                {

                    case 0:
                        DailyPlanner.ShowRecodrs();
                        PressEnter();
                        break;

                    case 1:
                        DailyPlanner.ExpiredRecords();
                        PressEnter();

                        break;
                    case 2:
                        DailyPlanner.SortRecords();
                        PressEnter();
                        break;
                    case 3:
                        DailyPlanner.AddRecord();
                        PressEnter();
                        break;
                    case 4:
                        DailyPlanner.RemoveRecord();
                        PressEnter();
                        break;
                    case 5:
                        Environment.Exit(0);
                        break;



                }

            }





        }

        //Сам планировщик
        static class DailyPlanner
        {
            static DateTime today = DateTime.Today;
            static public List<Record> listRecord = new List<Record>();

            static async public void AddRecord()
            {


                Console.WriteLine("Введите название задачи");

                string name = ValidateName(Console.ReadLine());


                Console.WriteLine("Введите дату");
                Console.WriteLine("Введите год");
                int year = ValidateYear(Console.ReadLine());

                Console.WriteLine("Введите Месяц");
                int month = ValidateMonth(Console.ReadLine());
                Console.WriteLine("Введите Число");
                int day = ValidateDay(Console.ReadLine());

                //Время
                Console.WriteLine("Введите время(часы)");
                int hour = ValidateHour(Console.ReadLine());
                Console.WriteLine("Введите время(минуты)");
                int minutes = ValidateMinutes(Console.ReadLine());
                Console.WriteLine("Введите краткое описание задачи");
                string desc = Console.ReadLine();
                try
                {
                    DateTime date = new DateTime(year, month, day, hour, minutes, 0);
                    Record record = new Record(name, date, desc);

                    listRecord.Add(record);
                    await Writedb(record);
                    
                    

                }
                catch
                {
                    Console.WriteLine("Произошла ошибка,запись не добавленна");
                }

               


            }

            static async public void RemoveRecord()
            {
                Console.WriteLine("Укажите индетификатор записи");

                try
                {
                    int id = ValidateToInt(Console.ReadLine());
                    var itemRemove = listRecord.Where(el => el.Id == id).Single();
                    listRecord.Remove(itemRemove);
                    
                        await Removedb(id);
                   
                   
                    Console.WriteLine($"Запись с ID = {id} успешно удалена");
                }
                catch
                {
                    Console.WriteLine("Запись c данным ID не найдена");
                }

              


            }

            static public void ShowRecodrs()
            {

                if (listRecord.Count == 0)
                    Console.WriteLine("Записи не найдены");
                else
                {
                    Console.WriteLine("Все задачи");
                    foreach (Record obj in listRecord)
                    {
                        obj.Show();
                    }
                }
               

            }

            static public void ExpiredRecords()
            {
                var records = listRecord.Where(u => u.Date < today);
                if (records.Count() == 0)
                {
                    Console.WriteLine("Записи не обнаружены");
                }
                else
                {
                    Console.WriteLine("Просроченные записи");
                    foreach (var el in records)
                    {
                        el.Show();
                    }
                }


            }

            static public void SortRecords()
            {

                if (listRecord.Count == 0)
                    Console.WriteLine("Записи не найдены");
                else
                {
                    bool isMenu = true;
                    List<string> fields = new List<string>() {
                        "Id",
                        "Дата",
                        "Выход"
                    };

                    Console.Clear();

                    while (isMenu)
                    {

                        Console.WriteLine("Выберите поле,по которому сортировать");
                        MenuSelection(fields);
                        switch (select)
                        {
                            case 0:
                               
                                SortParam("id");
                                PressEnter();
                                break;
                            case 1:
                                
                                SortParam("date");
                                PressEnter();
                                
                                break;
                            case 2:
                                Console.Clear();
                                isMenu = false;
                                break;
                        }
                    }
                }
              


            }
            static void SortParam(string str)
            {
                index = 0;

                List<string> param = new List<string>
                {
                    "По возрастанию",
                    "По убыванию",
                };
                bool temp = true;
                while (temp)
                {
                    MenuSelection(param);
                    Console.Clear();
                    switch (select)
                    {
                        case 0:
                            Console.WriteLine("Отсортированный список");

                            if (str == "id")
                            {
                                var sortlist = listRecord.OrderBy(u => u.Id);
                                foreach (var el in sortlist)
                                {
                                    el.Show();
                                }
                            }
                            else
                            {

                                var sortlist = listRecord.OrderBy(u => u.Date);
                                foreach (var el in sortlist)
                                {
                                    el.Show();
                                }
                            }
                            temp = false;
                            Console.WriteLine("Нажмите Enter для продолжения");
                            Console.Read();
                            Console.Clear();
                            break;
                        case 1:

                            Console.WriteLine("Отсортированный список");
                            if (str == "id")
                            {
                                var sortlist = listRecord.OrderByDescending(u => u.Id);
                                foreach (var el in sortlist)
                                {
                                    el.Show();
                                }
                            }
                            else
                            {

                                var sortlist = listRecord.OrderByDescending(u => u.Date);
                                foreach (var el in sortlist)
                                {
                                    el.Show();
                                }
                            }
                            temp = false;
                           
                            break;

                    }
                }
            }

        }


        //Класс записи в плане
        public class Record
        {

            public int Id { get;private set; }
            public string Name { get; private set; }
            public DateTime Date { get; private set; }
            public string Desc { get; private set; }


            public void Show()
            {
                Console.WriteLine($"Id:{Id} Имя:{Name} Дата:{Date}  Описание: {Desc} ");
            }
            public Record(string name, DateTime date, string desc)
            {

                Name = name;
                Date = date;
                Desc = desc;

            }
            public Record()
            {

            }

        }
        //Класс для работы с БД
        public class ApplicationContext : DbContext
        {
            public DbSet<Record> Records { get; set; }

            public ApplicationContext()
            {
                Database.EnsureCreated();
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=plannerdb;Trusted_Connection=True;");
            }
        }

        //Отдельные функции
        //Запись записи в БД
        static async public Task Writedb(Record el)
        {
            using ApplicationContext db = new ApplicationContext();
            db.Add(el);
           await db.SaveChangesAsync();

        }
        //Удаление записи из БД
        static async public Task Removedb(int id)
        {
           using ApplicationContext db = new ApplicationContext();
            var records = await db.Records.ToListAsync();
            foreach (var el in records)
            {
                if (el.Id == id)
                {
                    db.Remove(el);
                }
            }

           await db.SaveChangesAsync();


        }
        //Загрузка данных из БД
        static public void Getdb()
        {
            Console.WriteLine("Загрузка данных из бд");
            using ApplicationContext db = new ApplicationContext();
            var records = db.Records.ToList();
            foreach (var el in records)
            {
                DailyPlanner.listRecord.Add(el);
            }
            Console.Clear();
            Console.WriteLine("Загрузка завершена");
            Thread.Sleep(400);
            Console.Clear();
        }
        //Нажатие Enter
        static public void PressEnter()
        {
            Console.WriteLine("Для продолжения нажмите Enter");
            Console.ReadLine();
            Console.Clear();
        }
       //Валидация имени
        static public string ValidateName(string a)
        {
            if (String.IsNullOrEmpty(a))
            {
                Console.WriteLine("Вы не ввели имя.Повторите попытку:");
                return ValidateName(Console.ReadLine());

            }
            bool isExist = false;

            foreach (var el in DailyPlanner.listRecord)
            {
                if (a == el.Name)
                {
                    isExist = true;
                }
            }
            if (isExist)
            {
                Console.WriteLine("Запись с таким именем уже существуе.Повторите попытку.");
                return ValidateName(Console.ReadLine());
            }
            else
            {
                return a;
            }
        }
        //Преобразование string to int
        static public int ValidateToInt(string str)
        {
            bool success = Int32.TryParse(str, out int b);
            if (success)
            {

                return b;
            }
            else
            {
                Console.WriteLine("Некорректный формат данных. Попробуйе ещё раз:");

                return ValidateToInt(Console.ReadLine());
            }
        }
        //Валидация года
        static public int ValidateYear(string str)
        {
            bool success = Int32.TryParse(str, out int b);
            if (success)
            {

                return b;
            }
            else
            {
                Console.WriteLine("Некорректный формат данных. Попробуйе ещё раз:");

                return ValidateYear(Console.ReadLine());
            }
        }
        //Валидация месяца
        static public int ValidateMonth(string str)
        {
            bool success = Int32.TryParse(str, out int b);
            if (success)
            {
                if (b < 0 | b > 12)
                {
                    Console.WriteLine("Вы ввели несуществующий месяц");
                    return ValidateMonth(Console.ReadLine());
                }
                else
                {
                    return b;
                }

            }
            else
            {
                Console.WriteLine("Некорректный формат данных. Попробуйе ещё раз:");

                return ValidateMonth(Console.ReadLine());
            }

        }
        //Валидация дня
        static public int ValidateDay(string str)
        {
            bool success = Int32.TryParse(str, out int b);
            if (success)
            {
                if (b <= 0 | b > 31)
                {
                    Console.WriteLine("Вы ввели несуществующий день");
                    return ValidateDay(Console.ReadLine());
                }
                else
                {
                    return b;
                }

            }
            else
            {
                Console.WriteLine("Некорректный формат данных. Попробуйе ещё раз:");

                return ValidateDay(Console.ReadLine());
            }

        }
        //Валидация часов
        static public int ValidateHour(string str)
        {
            bool success = Int32.TryParse(str, out int b);
            if (success)
            {
                if (0 > b | b > 24)
                {
                    Console.WriteLine("Часы должны принимать значение от 0 до 24");
                    return ValidateHour(Console.ReadLine());
                }
                else
                {
                    return b;
                }

            }
            else
            {
                Console.WriteLine("Некорректный формат данных. Попробуйе ещё раз:");

                return ValidateHour(Console.ReadLine());
            }

        }
        //Валидация минут
        static public int ValidateMinutes(string str)
        {
            bool success = Int32.TryParse(str, out int b);
            if (success)
            {
                if (0 > b | b > 59)
                {
                    Console.WriteLine("Минуты должны принимать значение от 0 до 59");
                    return ValidateMinutes(Console.ReadLine());
                }
                else
                {
                    return b;
                }

            }
            else
            {
                Console.WriteLine("Некорректный формат данных. Попробуйе ещё раз:");

                return ValidateMinutes(Console.ReadLine());
            }

        }
        //Функция меню
        static public void MenuSelection(List<string> items)
        {
            select = null;

            foreach (string str in items)
            {
                if (items.IndexOf(str) == index)
                {
                    Console.BackgroundColor = ConsoleColor.Gray;
                    Console.ForegroundColor = ConsoleColor.Black;

                    Console.WriteLine(str);
                }
                else
                {
                    Console.WriteLine(str);
                }
                Console.ResetColor();
            }

            ConsoleKeyInfo ckey = Console.ReadKey();
            if (ckey.Key == ConsoleKey.DownArrow)
            {

                if (index == items.Count - 1)
                {
                    index = 0;
                }
                else
                {
                    index++;
                }


            }
            else if (ckey.Key == ConsoleKey.UpArrow)
            {

                if (index <= 0)
                {
                    index = items.Count - 1;
                }
                else
                {
                    index--;
                }

            }
            else if (ckey.Key == ConsoleKey.Enter)
            {
                select = items.IndexOf(items[index]);

            }

            Console.Clear();
        }

    }
}
