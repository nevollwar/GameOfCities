using System;
using System.Collections.Generic;
using System.Linq;

namespace GameOfCities.Logic
{
    /// <summary>
    /// Представляет центральный компонент игровой логики (ядро), 
    /// обеспечивающий соблюдение правил игры, управление состоянием и ходами участников.
    /// </summary>
    public class GameCore
    {
        /// <summary>
        /// Коллекция, содержащая полный перечень допустимых названий городов.
        /// </summary>
        private readonly List<string> allCities;

        /// <summary>
        /// Коллекция уникальных названий городов, уже задействованных в текущей игровой сессии.
        /// </summary>
        private readonly HashSet<string> usedCities;

        /// <summary>
        /// Генератор псевдослучайных чисел для обеспечения вариативности ответов компьютера.
        /// </summary>
        private readonly Random random = new Random();

        /// <summary>
        /// Возвращает текущий целевой символ, на который должен начинаться следующий город.
        /// </summary>
        public char CurrentLetter { get; private set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GameCore"/>, 
        /// выполняя загрузку базы данных городов и инициализацию игровых структур.
        /// </summary>
        public GameCore()
        {
            var repository = new CityRepository();
            allCities = repository.GetCities();
            usedCities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Выполняет обработку хода пользователя, включая валидацию введенного значения 
        /// на соответствие правилам игры и лексической базе.
        /// </summary>
        /// <param name="city">Название города, введенное пользователем.</param>
        /// <param name="errorMessage">Выходной параметр, содержащий описание ошибки при неудачной валидации.</param>
        /// <returns>Возвращает true, если ход признан корректным, иначе — false.</returns>
        public bool PlayerTurn(string city, out string errorMessage)
        {
            errorMessage = "";
            city = city.Trim();

            // Проверка на наличие пустой строки или некорректного ввода
            if (string.IsNullOrWhiteSpace(city))
            {
                errorMessage = "Поле ввода не может быть пустым.";
                return false;
            }

            // Верификация существования города в эталонной базе данных
            if (!allCities.Any(c => c.Equals(city, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = "Указанный город отсутствует в базе данных.";
                return false;
            }

            // Контроль уникальности (исключение повторного использования названий)
            if (usedCities.Contains(city))
            {
                errorMessage = "Данный город уже использовался в текущей сессии.";
                return false;
            }

            // Валидация первой буквы на соответствие правилам последовательности
            if (CurrentLetter != '\0' && !city.StartsWith(CurrentLetter.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = $"Нарушение правил: город должен начинаться на букву '{CurrentLetter}'.";
                return false;
            }

            // Фиксация успешного хода
            usedCities.Add(city);
            CurrentLetter = GetNextValidLetter(city);
            return true;
        }

        /// <summary>
        /// Реализует алгоритм хода автоматизированного оппонента (компьютера) 
        /// с применением механизма случайного выбора из доступных вариантов.
        /// </summary>
        /// <returns>Возвращает название выбранного города или null, если доступные варианты исчерпаны.</returns>
        public string ComputerTurn()
        {
            // Фильтрация базы городов по текущей букве и исключение уже использованных
            var possibleCities = allCities
                .Where(c => c.StartsWith(CurrentLetter.ToString(), StringComparison.OrdinalIgnoreCase)
                    && !usedCities.Contains(c))
                .ToList();

            if (possibleCities.Count > 0)
            {
                // Выбор случайного элемента для обеспечения нелинейности игрового процесса
                int index = random.Next(possibleCities.Count);
                string foundCity = possibleCities[index];

                usedCities.Add(foundCity);
                CurrentLetter = GetNextValidLetter(foundCity);
                return foundCity;
            }

            return null; // Состояние поражения компьютера
        }

        /// <summary>
        /// Определяет следующую игровую букву на основе анализа окончания слова, 
        /// исключая недопустимые символы согласно правилам (ь, ы, ъ, й).
        /// </summary>
        /// <param name="city">Город, на основе которого вычисляется буква.</param>
        /// <returns>Символ в верхнем регистре, предназначенный для следующего хода.</returns>
        private char GetNextValidLetter(string city)
        {
            // Буквы, которые мы точно пропускаем
            char[] badLetters = { 'ь', 'ы', 'ъ', 'й' };
            string lowerCity = city.ToLower();

            // Идем с конца слова к началу
            for (int i = lowerCity.Length - 1; i >= 0; i--)
            {
                char letter = lowerCity[i];

                // 1. Проверяем: является ли символ БУКВОЙ? 
                // (Это пропустит скобки, пробелы, цифры и тире)
                if (char.IsLetter(letter))
                {
                    // 2. Если это буква, проверяем, не входит ли она в список запрещенных
                    if (!badLetters.Contains(letter))
                    {
                        return char.ToUpper(letter);
                    }
                }
            }

            // Если вдруг в строке вообще не оказалось букв (чего быть не должно)
            return 'А';
        }
    }
}