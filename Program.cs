using System.Runtime.Serialization.Formatters.Binary;

namespace Lab9
{
    internal class Program
    {
        static bool isFileSelected;
        static string selectedFileDirectory;
        static string selectedFileName;

        [Serializable]
        public class CreditInfo // класс, содержащий информацию о кредитах
        {
            public string bankName { get; set; }
            public double creditAmount { get; set; }
            public double monthlyFee { get; set; }
            public bool isDebt { get; set; }

            public CreditInfo()
            {
                bankName = string.Empty;
                creditAmount = -1;
                monthlyFee = -1;
                isDebt = false;
            }

            public CreditInfo(string bankName, double creditAmount, double monthlyFee, bool isDebt)
            {
                this.bankName = bankName;
                this.creditAmount = creditAmount;
                this.monthlyFee = monthlyFee;
                this.isDebt = isDebt;
            }
        }

        [Serializable]
        public class CreditHistory // класс, содержащий информацию о кредитных историях заемщиков
        {
            public string surname { get; set; }
            public string name { get; set; }
            public string patronymic { get; set; }
            public CreditInfo[] credits { get; set; }

            public CreditHistory()
            {
                surname = string.Empty;
                name = string.Empty;
                patronymic = string.Empty;
                credits = null;
            }
            public CreditHistory(string surname, string name, string patronymic, CreditInfo[] credits)
            {
                this.surname = surname;
                this.name = name;
                this.patronymic = patronymic;
                this.credits = credits;
            }
        }

        static void writeFile(CreditHistory[] creditHistories, string name) // функция записи файл
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(selectedFileDirectory + "\\" + name, FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, creditHistories);
            }
        }

        static CreditHistory inputHistory() // функция ввода информации о заемщике
        {
            Console.Write("Введите фамилию заемщика: ");
            string surname = Console.ReadLine();
            Console.Write("Введите имя заёмщика: ");
            string name = Console.ReadLine();
            Console.Write("Введите отчество заемщика: ");
            string patronymic = Console.ReadLine();
            int creditsCount;
            Console.Write("Количество кредитов у заемщика: ");
            while (!int.TryParse(Console.ReadLine(), out creditsCount) || creditsCount > 3 || creditsCount < 0)
            {
                Console.Write("Некорректный ввод!\nКредитов не может быть больше 3\nКоличество кредитов у заемщика: ");
            }
            CreditInfo[] credits = new CreditInfo[creditsCount];
            for (int i = 0; i < creditsCount; i++)
            {
                Console.Write("Введите имя банка заёмщика: ");
                string bankName = Console.ReadLine();
                double creditAmount;
                Console.Write("Введите сумму, взятую в кредит: ");
                while (!double.TryParse(Console.ReadLine(), out creditAmount))
                {
                    Console.Write("Некорректный ввод!\nВведите сумму, взятую в кредит: ");
                }
                double monthlyFee;
                Console.Write("Введите сумму ежемесячного платежа по кредиту: ");
                while (!double.TryParse(Console.ReadLine(), out monthlyFee))
                {
                    Console.Write("Некорректный ввод!\nВведите сумму ежемесячного платежа по кредиту: ");
                }
                bool isDebt;
                Console.Write("Есть ли просрочка по данному кредиту: ");
                while (!bool.TryParse(Console.ReadLine(), out isDebt))
                {
                    Console.Write("Некорректный ввод!\nНеобходимо ввести true или false\nЕсть ли просрочка по данному кредиту: ");
                }
                CreditInfo credit = new CreditInfo(bankName, creditAmount, monthlyFee, isDebt);
                credits[i] = credit;
            }
            CreditHistory history = new CreditHistory(surname, name, patronymic, credits);
            return history;
        }


        static CreditHistory[] readFile() // функция, считывающая данные из файла
        {
            CreditHistory[] ch;
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream(selectedFileDirectory + "\\" + selectedFileName, FileMode.Open))
            {
                if (fs.Length != 0)
                    ch = (CreditHistory[])formatter.Deserialize(fs);
                else
                    ch = new CreditHistory[0];
            }
            return ch;
        }

        static void createRecord() //  функция, создающая запись
        {
            if (isFileSelected)
            {
                CreditHistory addingCreditHistory = inputHistory();
                CreditHistory[] creditHistory = readFile();
                Array.Resize(ref creditHistory, creditHistory.Length + 1);
                creditHistory[creditHistory.Length - 1] = addingCreditHistory;
                writeFile(creditHistory, selectedFileName);
                Console.WriteLine("Запись успешно создана\n");
            }
            else
                Console.WriteLine("Файл не выбран\n");
        }

        static void editRecord() // функция, редактирующая запись с выбранным номером
        {
            if (isFileSelected)
            {
                CreditHistory[] creditHistory = readFile();
                int number;
                Console.Write("Введите номер записи для редактирования или 0 для отмены редактирования: ");
                while (!int.TryParse(Console.ReadLine(), out number) || number > creditHistory.Length || number < 0)
                {
                    Console.Write("Некорректный ввод!\nВведите номер записи для редактирования или 0 для отмены редактирования: ");
                }
                number--;
                if (number != -1)
                {
                    CreditHistory addingCreditHistory = inputHistory();
                    creditHistory[number] = addingCreditHistory;
                    writeFile(creditHistory, selectedFileName);
                    Console.WriteLine("Запись успешно отредактирована\n");
                }
                else
                    Console.WriteLine("Отмена операции редактирования\n");
            }
            else
                Console.WriteLine("Файл не выбран\n");
        }

        static void deleteRecord() // функция, удаляющая запись с выбранным номером
        {
            if (isFileSelected)
            {
                CreditHistory[] creditHistory = readFile();
                int number;
                Console.Write("Введите номер записи для удаления или 0 для отмены удаления: ");
                while (!int.TryParse(Console.ReadLine(), out number) || number > creditHistory.Length || number < 0)
                {
                    Console.Write("Некорректный ввод!\nВведите номер записи для удаления или 0 для отмены удаления: ");
                }
                number--;
                if (number != -1)
                {
                    CreditHistory[] newHistory = new CreditHistory[creditHistory.Length - 1];
                    Array.Copy(creditHistory, 0, newHistory, 0, number);
                    Array.Copy(creditHistory, number + 1, newHistory, number, newHistory.Length - number);
                    writeFile(newHistory, selectedFileName);
                    Console.WriteLine("Запись успешно удалена\n");
                }
                else
                    Console.WriteLine("Отмена операции удаления\n");
            }
            else
                Console.WriteLine("Файл не выбран\n");
        }

        static void printFile() // функция, выводящая информацию о всех кредитных историях
        {
            CreditHistory[] creditHistory = readFile();
            if (creditHistory.Length != 0)
            {
                for (int i = 0; i < creditHistory.Length; i++)
                {
                    string output = $"{i + 1}. Фамилия: {creditHistory[i].surname} | Имя: {creditHistory[i].name} | Отчество: {creditHistory[i].patronymic}\n";
                    for (int j = 0; j < creditHistory[i].credits.Length; j++)
                    {
                        output += $"   Кредит {j + 1}: Банк: {creditHistory[i].credits[j].bankName} | Сумма кредита: {creditHistory[i].credits[j].creditAmount} | " +
                            $"Ежемесячный платеж: {creditHistory[i].credits[j].monthlyFee} | Есть ли просрочка: {creditHistory[i].credits[j].isDebt}\n";
                    }
                    Console.WriteLine(output);
                }
            }
            else
                Console.WriteLine("Файл пуст\n");
        }

        static void printTextFile()
        {
            string[] ras = selectedFileName.Split('.');
            if (ras[ras.Length - 1] == "txt")
            {
                string[] text = File.ReadAllLines(selectedFileDirectory + "\\" + selectedFileName);
                for (int i = 0; i < text.Length; i++)
                {
                    Console.WriteLine(text[i]);
                }
                Console.WriteLine();
            }
            else
                Console.WriteLine("Файл данных считывается другим пунктом меню");
        }

        static void firstTask()
        {
            if (isFileSelected)
            {
                Console.Write("Введите имя банка: ");
                string bankName = Console.ReadLine();
                CreditHistory[] creditHistory = readFile();
                CreditHistory[] newCreditHistory = new CreditHistory[0];
                for (int i = 0; i < creditHistory.Length; i++)
                {
                    for (int j = 0; j < creditHistory[i].credits.Length; j++)
                    {
                        if (creditHistory[i].credits[j].bankName == bankName)
                        {
                            Array.Resize(ref newCreditHistory, newCreditHistory.Length + 1);
                            newCreditHistory[newCreditHistory.Length - 1] = creditHistory[i];
                            break;
                        }
                    }
                }
                writeFile(newCreditHistory, "1 task.dat");
                Console.WriteLine($"Задание 1 выполнено, результат помещён в текстовый файл 1 task.dat");
            }
            else
                Console.WriteLine("Файл не выбран\n");
        }

        static void secondTask()
        {
            if (isFileSelected)
            {
                CreditHistory[] creditHistory = readFile();
                string output = "";
                for (int i = 0; i < creditHistory.Length; i++)
                {
                    bool hasDebt = false;
                    CreditInfo[] credits = creditHistory[i].credits;
                    for (int j = 0; j < credits.Length; j++)
                    {
                        if (credits[j].isDebt)
                        {
                            hasDebt = true;
                            break;
                        }
                    }
                    if (hasDebt)
                        output += creditHistory[i].surname + " ";
                }
                File.WriteAllText(selectedFileDirectory + "\\" + "2 task.txt", output);
                Console.WriteLine($"Задание 2 выполнено, результат помещён в текстовый файл 2 task.txt");
            }
            else
                Console.WriteLine("Файл не выбран\n");
        }

        static void Main(string[] args)
        {
            /*
            В файле хранится информация о кредитных историях: фамилия, имя, отчество заемщика,
            кредитная история: список не более чем из 3 кредитов, с указанием названия банка, суммы кредита,
            ежемесячного платежа и отметки об имеющейся просрочке.
            В новый файл переписать информацию о заемщиках, взявших кредит в заданном банке.
            Вывести в текстовый файл фамилии заемщиков, у которых имеется просрочка хотя бы по одному платежу.
            */
            string mainMenuText = "1) Выбрать файл\n" +
                                  "2) Создать новый файл\n" +
                                  "3) Просмотреть выбраный файл данных\n" +
                                  "4) Просмотреть выбраный текстовый файл\n" +
                                  "5) Редактировать выбранный файл\n" +
                                  "6) Удалить выбранный файл\n" +
                                  "7) Задание 1 (Переписать информацию о заемщиках, взявших кредит в заданном банке)\n" +
                                  "8) Задание 2 (Вывести в текстовый файл фамилии заемщиков, у которых имеется просрочка хотя бы по одному платежу)\n" +
                                  "9) Выход\n" +
                                  "Ваш выбор: ";
            string editMenuText = "1) Добавить запись\n" +
                                  "2) Редактировать запись\n" +
                                  "3) Удалить запись\n" +
                                  "4) Назад\n" +
                                  "Ваш выбор: ";
            isFileSelected = false;
            selectedFileDirectory = string.Empty;
            selectedFileName = string.Empty;
            bool exitProgram = false;
            while (!exitProgram) // цикл, работающий, пока не выбран пункт "выход"
            {
                Console.Write(mainMenuText);
                string mainMenuChoice = Console.ReadLine();
                Console.WriteLine();
                switch (mainMenuChoice)
                {
                    case "1":
                        Console.Write("Введите имя файла: ");
                        string fileName = Console.ReadLine();
                        FileInfo fileInfo = new FileInfo(@"..\..\..\" + fileName);
                        if (fileInfo.Exists) // ищем файл в папке лабораторной работы
                        {
                            isFileSelected = true;
                            selectedFileDirectory = fileInfo.DirectoryName;
                            selectedFileName = fileInfo.Name;
                            Console.WriteLine($"Файл {selectedFileName} успешно выбран\n");
                        }
                        else
                            Console.WriteLine($"Файл с именем {fileName} не найдем\n");
                        break;

                    case "2":
                        Console.Write("Введите имя файла: ");
                        string newFileName = Console.ReadLine();
                        File.Create(@"..\..\..\" + newFileName + ".dat").Close(); // сохраняем получившийся файл
                        Console.WriteLine($"Файл {newFileName}.dat успешно создан\n");
                        break;

                    case "3":
                        if (isFileSelected)
                        {
                            string[] ras = selectedFileName.Split('.');
                            if (ras[ras.Length - 1] == "dat")
                            {
                                Console.WriteLine("Содержимое файла:");
                                printFile();
                            }
                            else
                                Console.WriteLine("Текстовый файл считывается другим пунктом меню");
                        }
                        else
                            Console.WriteLine("Файл не выбран\n");
                        break;

                    case "4":
                        if (isFileSelected)
                        {
                            Console.WriteLine("Содержимое файла:");
                            printTextFile();
                        }
                        else
                            Console.WriteLine("Файл не выбран\n");
                        break;

                    case "5":
                        bool exitEditMenu = false;
                        while (!exitEditMenu)
                        {
                            Console.Write(editMenuText);
                            string editMenuChoice = Console.ReadLine();
                            switch (editMenuChoice)
                            {
                                case "1":
                                    createRecord();
                                    break;

                                case "2":
                                    editRecord();
                                    break;

                                case "3":
                                    deleteRecord();
                                    break;

                                case "4":
                                    exitEditMenu = true;
                                    break;

                                default:
                                    Console.WriteLine("Команда не распознана");
                                    break;
                            }
                        }
                        break;

                    case "6":
                        if (isFileSelected)
                        {
                            File.Delete(selectedFileDirectory + "\\" + selectedFileName);
                            Console.WriteLine($"Файл {selectedFileName} успешно удален");
                            isFileSelected = false;
                            selectedFileDirectory = string.Empty;
                            selectedFileName = string.Empty;
                        }
                        else
                            Console.WriteLine("Файл не выбран\n");
                        break;

                    case "7":
                        firstTask();
                        break;

                    case "8":
                        secondTask();
                        break;

                    case "9":
                        exitProgram = true;
                        break;

                    default:
                        Console.WriteLine("Команда не распознана, повторите ввод");
                        break;
                }
            }
        }
    }
}