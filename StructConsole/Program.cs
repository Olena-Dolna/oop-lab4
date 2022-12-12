using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Dynamic;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace StructConsole
{
    struct Currency
    {
        public string CurrencyName;
        public double ExRate;

        public Currency (string currencyName, double exRate)
        {
            CurrencyName = currencyName;
            ExRate = exRate;
        }
    }
    struct Product
    {
        public string Name;
        public double PriceinCurrency;
        public Currency Cost;
        public int Quantity;
        public string Producer;
        public double Weight;

        public Product(string name, int quantity, string producer, double weight, double priceInCurrency, string currencyName, double exRate)
        {
            Name = name;
            PriceinCurrency = priceInCurrency;
            Cost = new Currency(currencyName, exRate);
            Quantity = quantity;
            Producer = producer;
            Weight = weight;
        }
        public double GetPriceInUAH()
        {
            return Math.Round(Cost.ExRate * PriceinCurrency, 2);
        }

        public double GetTotalPriceInUAH()
        {
            return GetPriceInUAH() * Quantity;
        }

        public double GetTotalWeight()
        {
            return GetPriceInUAH() * Weight;
        }
        static int GetValue()
        {
            int n;
            bool isCorrect = false;
            do
            {
                isCorrect = int.TryParse(Console.ReadLine(), out n);
                if (isCorrect == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Помилка введення значення! Будь ласка, повторіть введення ще раз!");
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                }
            } while (!isCorrect);
            return n;
        }

        internal class Program
        {
            static void Main(string[] args)
            {
                Console.OutputEncoding = Encoding.UTF8;
                System.Globalization.CultureInfo customCulture = (System.Globalization.CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
                customCulture.NumberFormat.NumberDecimalSeparator = ".";
                System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
                Console.Title = "Лабораторна робота N4";
                Console.SetWindowSize(100, 25);
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor = ConsoleColor.DarkBlue;

                Product[] arr = null;
                int ch;

                do
                {
                    Console.WriteLine("\nНатисніть будь-яку кнопку, щоб продовжити");
                    Console.ReadKey();
                    Console.Clear();
                    Menu();
                    ch = GetValue();
                    switch (ch)
                    {
                        case 1:
                            Console.Clear();
                            arr = ReadProductsArray();
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("\nДані успішно записано");
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            break;
                        case 2:
                            if (arr == null)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Помилка! Не знайдено даних про товари!");
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                break;
                            }
                            Console.Clear();
                            PrintProducts(arr);
                            break;
                        case 3:
                            if (arr == null)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Помилка! Не знайдено даних про товари!");
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                break;
                            }
                            Console.Clear();
                            Console.WriteLine($"Введіть номер товару (всього {arr.Length} товари(-ів))");
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            int note = GetValue() - 1;
                            if (note > arr.Length - 1 || note < 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Помилка введення значення! Будь ласка, повторіть введення ще раз!");
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                break;
                            }
                            else
                            {
                                Console.WriteLine();
                                PrintProduct(arr[note]);
                                break;
                            }
                        case 4:
                            if (arr == null)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Помилка! Не знайдено даних про товари!");
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                break;
                            }
                            Console.Clear();
                            GetProductsInfo(arr, out int min, out int max);
                            break;
                        case 5:
                            if (arr == null)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Помилка! Не знайдено даних про товари!");
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                break;
                            }
                            Console.Clear();
                            SortProductsByPrice(arr);
                            PrintProducts(arr);
                            break;
                        case 6:
                            if (arr == null)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Помилка! Не знайдено даних про товари!");
                                Console.ForegroundColor = ConsoleColor.DarkBlue;
                                break;
                            }
                            Console.Clear();
                            SortProductsByQuantity(arr);
                            PrintProducts(arr);
                            break;
                        case 0:
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Помилка введення значення! Будь ласка, повторіть введення ще раз!");
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                            break;
                    }
                } while (ch != 0);

            }
            static void Menu()
            {
                Console.WriteLine("1 - Ввести масив товарів");
                Console.WriteLine("2 - Вивести інформацію про товари");
                Console.WriteLine("3 - Вивести інформацію про конкретний товар");
                Console.WriteLine("4 - Інформація про найдешевший та найдорожчий товари");
                Console.WriteLine("5 - Сотрувати товари за зростанням ціни");
                Console.WriteLine("6 - Сортувати товари за кількістю на складі");
                Console.WriteLine("0 - Вийти");
            }
            readonly static string[] Fields =
            {
            "Назва товару",
            "Вартість одиниці товару у валюті",
            "Назва валюти",
            "Курс",
            "Кількість наявних товарів на складі",
            "Назва компанії-виробника",
            "Вага одиниці товару",
        };
            static Product[] ReadProductsArray()
            {
                Console.WriteLine("Введіть кількість елементів масиву: ");
                int k = GetValue();
                string name, producer, currencyName;
                int quantity;
                double weight, priceInCurrency, exRate;
                bool isCorrect;
                Product[] products = new Product[k];
                for (int i = 0; i < k; i++)
                {   
                    Console.WriteLine($"\nТовар [{i + 1}]");
                    Console.WriteLine($"{Fields[0]}: ");
                    name = Console.ReadLine();
                    Console.WriteLine($"{Fields[1]}: ");
                    do
                    {
                        isCorrect = double.TryParse(Console.ReadLine(), out priceInCurrency);
                        if (priceInCurrency <= 0)
                        {
                            isCorrect = false;
                        }
                        if (isCorrect == false)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Помилка введення значення! Будь ласка, повторіть введення ще раз!");
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                        }
                    }
                    while (!isCorrect);
                    Console.WriteLine($"{Fields[2]}: ");
                    currencyName = Console.ReadLine();
                    Console.WriteLine($"{Fields[3]}: ");
                    do
                    {
                        isCorrect = double.TryParse(Console.ReadLine(), out exRate);
                        if (exRate <= 0)
                        {
                            isCorrect = false;
                        }
                        if (isCorrect == false)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Помилка введення значення! Будь ласка, повторіть введення ще раз!");
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                        }
                    }
                    while (!isCorrect);
                    Console.WriteLine($"{Fields[4]}: ");
                    do
                    {
                        isCorrect = int.TryParse(Console.ReadLine(), out quantity);
                        if (quantity <= 0)
                        {
                            isCorrect = false;
                        }
                        if (isCorrect == false)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Помилка введення значення! Будь ласка, повторіть введення ще раз!");
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                        }
                    }
                    while (!isCorrect);
                    Console.WriteLine($"{Fields[5]}: ");
                    producer = Console.ReadLine();
                    Console.WriteLine($"{Fields[6]}: ");
                    do
                    {
                        isCorrect = double.TryParse(Console.ReadLine(), out weight);
                        if (weight <= 0)
                        {
                            isCorrect = false;
                        }
                        if (isCorrect == false)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Помилка введення значення! Будь ласка, повторіть введення ще раз!");
                            Console.ForegroundColor = ConsoleColor.DarkBlue;
                        }
                    }
                    while (!isCorrect);
                    products[i] = new Product(name, quantity, producer, weight, priceInCurrency, currencyName, exRate);
                }
                return products;
            }
            static void PrintProduct(Product el)
            {
                Console.WriteLine($"Назва товару: {el.Name}");
                Console.WriteLine($"Вартість одиниці товару у валюті: {el.PriceinCurrency} {el.Cost.CurrencyName}");
                Console.WriteLine($"Курс: {el.Cost.ExRate} UAH");
                Console.WriteLine($"Кількість наявних товарів на складі: {el.Quantity} шт.");
                Console.WriteLine($"Назва компанії-виробника: {el.Producer}");
                Console.WriteLine($"Вага одиниці товару: {el.Weight} кг");
            }
            static void PrintProducts(Product[] arr)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    Console.WriteLine($"\nТовар {i + 1}\n");
                    PrintProduct(arr[i]);
                    Console.WriteLine();
                }
            }
            static void GetProductsInfo(Product[] arr, out int outMin, out int outMax)
            {
                double min = arr[0].GetPriceInUAH(), max = arr[0].GetPriceInUAH();
                int MinIndex = 0, MaxIndex = 0;
                for (int i = 0; i < arr.Length; i++)
                {
                    double temp = arr[i].GetPriceInUAH();
                    if (temp < min)
                    {
                        min = temp;
                        MinIndex = i;
                    }
                    else if (temp > max)
                    {
                        max = temp;
                        MaxIndex = i;
                    }
                    
                }
                outMin = MinIndex;
                outMax = MaxIndex;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Найнижча ціна: {min} UAH\n");
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                PrintProduct(arr[MinIndex]);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n\nНайвища ціна: {max} UAH\n");
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                PrintProduct(arr[MaxIndex]);
            }
            static int CompareByPrice(Product a, Product b)
            {
                double aPrice = a.GetTotalPriceInUAH(), bPrice = b.GetPriceInUAH();
                if (aPrice > bPrice)
                {
                    return 1;
                }
                else if (bPrice > aPrice)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            static int CompareByQuantity(Product a, Product b)
            {
                int aQuantity = a.Quantity, bQuantity = b.Quantity;
                if (aQuantity > bQuantity)
                {
                    return 1;
                }
                else if (aQuantity < bQuantity)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            }
            static Product[] SortProductsByPrice(Product[] arr)
            {
                Array.Sort(arr, CompareByPrice);
                return arr;
            }
            static Product[] SortProductsByQuantity(Product[] arr)
            {
                Array.Sort (arr, CompareByQuantity);
                return arr;
            }

        }
    }
       
}
