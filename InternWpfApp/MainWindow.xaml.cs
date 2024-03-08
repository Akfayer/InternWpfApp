using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Diagnostics;

namespace InternWpfApp
{
    /// <summary>
    /// Програмний код для обробки великої кількості чисел у файлі
    /// </summary>
    public partial class MainWindow : Window
    {
        int[] numbers; // Масив для зберігання чисел з файлу
        int[] incrSeq; // Найдовша зростаюча послідовність чисел
        int[] decrSeq; // Найдовша спадаюча послідовність чисел
        double average; // Середнє арифметичне чисел з файлу

        List<Task> tasks = new List<Task>(); // Список завдань для асинхронного виконання

        public MainWindow()
        {
            InitializeComponent();
        }

        // Обробник події для натискання кнопки "Обрати файл" для асинхронного зчитування файлу
        private async void Find_File_Button_Async_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";
            if (openFileDialog.ShowDialog() == true)
            {
                File_Path.Text = Path.GetFullPath(openFileDialog.FileName);
                // Асинхронне зчитування чисел з файлу та їх конвертація в масив
                numbers = await Task.Run(() => File.ReadAllLines(openFileDialog.FileName).Select(int.Parse).ToArray());
                CountDataButton.IsEnabled = true; // Активація кнопки для обчислення даних
                // Додавання завдань для знаходження середнього, найдовшої зростаючої та спадаючої послідовностей
                tasks.Add(Task.Run(() => average = numbers.Average()));
                tasks.Add(Task.Run(() => incrSeq = FindLongestIncreasingSequence(numbers)));
                tasks.Add(Task.Run(() => decrSeq = FindLongestDecreasingSequence(numbers)));
            }
        }

        // Обробник події для натискання кнопки "Знайти дані"
        private async void CountDataButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.WhenAll(tasks); // Очікування завершення всіх асинхронних завдань

            Array.Sort(numbers); // Сортування масиву чисел

            PrintInfo(); // Виведення результатів на екран        
        }

        // Метод для виведення інформації на екран
        void PrintInfo()
        {
            // Виведення найдовшої зростаючої послідовності
            foreach (var number in incrSeq)
            {
                Increasing_Seq.Text += number.ToString() + "; ";
            }
            // Виведення найдовшої спадаючої послідовності
            foreach (var number in decrSeq)
            {
                Decreasing_Seq.Text += number.ToString() + "; ";
            }
            // Виведення мінімального та максимального чисел
            Min.Text = numbers[0].ToString();
            Max.Text = numbers[numbers.Length - 1].ToString();
            // Виведення медіани та середнього арифметичного
            Median.Text = FindMedian(numbers).ToString();
            Average.Text = average.ToString();
        }

        // Метод для знаходження медіани
        double FindMedian(int[] seq)
        {
            if (seq.Length % 2 == 0)
            {
                return (seq[seq.Length / 2 - 1] + seq[seq.Length / 2]) / 2.0;
            }
            else
            {
                return seq[seq.Length / 2];
            }
        }

        // Метод для знаходження найдовшої зростаючої послідовності
        int[] FindLongestIncreasingSequence(int[] seq)
        {
            int elemCount = 0;
            int curSeqLength = 1;
            int skipCount = 1;
            int lengthOfSeq = 0;

            for (int i = 0; i < seq.Length - 1; i++)
            {
                if (seq[i] < seq[i + 1])
                {
                    curSeqLength++;
                    if (curSeqLength > lengthOfSeq)
                    {
                        lengthOfSeq = curSeqLength;
                        skipCount = elemCount;
                    }
                }
                else
                {
                    elemCount = i + 1;
                    curSeqLength = 1;
                }
            }
            return seq.Skip(skipCount).Take(lengthOfSeq).ToArray();
        }

        // Метод для знаходження найдовшої спадаючої послідовності
        int[] FindLongestDecreasingSequence(int[] seq)
        {
            int elemCount = 0;
            int curSeqLength = 1;
            int skipCount = 1;
            int lengthOfSeq = 0;

            for (int i = 0; i < seq.Length - 1; i++)
            {
                if (seq[i] > seq[i + 1])
                {
                    curSeqLength++;
                    if (curSeqLength > lengthOfSeq)
                    {
                        lengthOfSeq = curSeqLength;
                        skipCount = elemCount;
                    }
                }
                else
                {
                    elemCount = i + 1;
                    curSeqLength = 1;
                }
            }
            return seq.Skip(skipCount).Take(lengthOfSeq).ToArray();
        }
    }
}
