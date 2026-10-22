using System;
using System.Collections.Generic;
using System.Threading;

namespace ThreadsProcessingApp
{
    internal class Program
    {
        // Основний масив на 1000 елементів
        private static int[] fullArray = new int[1000];

        // Списки для парних та непарних чисел
        private static List<int> evenList = new List<int>();
        private static List<int> oddList = new List<int>();

        // Поточний індекс елемента в масиві, який береться на обробку
        private static int currentIndex = 0;

        // Об'єкти для блокування (синхронізації) спільних ресурсів
        private static readonly object indexLock = new object();
        private static readonly object evenLock = new object();
        private static readonly object oddLock = new object();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine("=== Багатопотокова обробка масиву чисел ===");
            Console.WriteLine();

            // 1. Заповнення масиву випадковими числами від 1 до 10
            Random random = new Random();
            for (int i = 0; i < fullArray.Length; i++)
            {
                fullArray[i] = random.Next(1, 11); // від 1 до 10 включно
            }

            Console.WriteLine($"Масив fullArray успішно ініціалізовано ({fullArray.Length} елементів).");
            Console.WriteLine("Запуск 3-х потоків для сортування на парні та непарні...");
            Console.WriteLine();

            // 2. Створення 3-х робочих потоків
            Thread thread1 = new Thread(ProcessArray) { Name = "Потік 1" };
            Thread thread2 = new Thread(ProcessArray) { Name = "Потік 2" };
            Thread thread3 = new Thread(ProcessArray) { Name = "Потік 3" };

            // 3. Запуск потоків
            thread1.Start();
            thread2.Start();
            thread3.Start();

            // 4. Очікуємо завершення роботи всіх трьох потоків
            thread1.Join();
            thread2.Join();
            thread3.Join();

            Console.WriteLine("Усі потоки завершили свою роботу!");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine($"Загальна кількість оброблених елементів: {currentIndex}");
            Console.WriteLine($"Кількість парних чисел (evenList): {evenList.Count}");
            Console.WriteLine($"Кількість непарних чисел (oddList): {oddList.Count}");
            Console.WriteLine($"Перевірка суми: {evenList.Count} + {oddList.Count} = {evenList.Count + oddList.Count}");
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();

            // 5. Виведення списку парних елементів
            Console.WriteLine($"--- Список парних елементів (evenList, всього {evenList.Count}) ---");
            PrintList(evenList);
            Console.WriteLine();

            // 6. Виведення списку непарних елементів
            Console.WriteLine($"--- Список непарних елементів (oddList, всього {oddList.Count}) ---");
            PrintList(oddList);
        }

        /// <summary>
        /// Метод, який паралельно виконується кожним із 3-х потоків
        /// </summary>
        private static void ProcessArray()
        {
            while (true)
            {
                int currentNumber;

                // Блокування доступу до лічильника індексу, щоб два потоки не взяли один і той самий елемент
                lock (indexLock)
                {
                    if (currentIndex >= fullArray.Length)
                    {
                        break; // Усі елементи масиву вже розібрано
                    }

                    currentNumber = fullArray[currentIndex];
                    currentIndex++;
                }

                // Перевірка на парність та безпечне додавання у відповідний список
                if (currentNumber % 2 == 0)
                {
                    lock (evenLock)
                    {
                        evenList.Add(currentNumber);
                    }
                }
                else
                {
                    lock (oddLock)
                    {
                        oddList.Add(currentNumber);
                    }
                }
            }
        }

        /// <summary>
        /// Допоміжний метод для зручного виведення списку на екран
        /// </summary>
        private static void PrintList(List<int> list)
        {
            Console.WriteLine(string.Join(", ", list));
        }
    }
}
