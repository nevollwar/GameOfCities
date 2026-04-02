using System;
using System.Collections.Generic;
using System.Linq;

namespace GameOfCities.Logic
{
    public class GameCore
    {
        private readonly List<string> _allCities;
        private readonly HashSet<string> _usedCities;

        public char CurrentLetter { get; private set; }

        public GameCore()
        {
            var repository = new CityRepository();
            _allCities = repository.GetCities();

            _usedCities = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        }

        public bool PlayerTurn(string city, out string errorMessage)
        {
            errorMessage = "";
            city = city.Trim();

            // Проверка 1: Существует ли такой город?
            if (!_allCities.Any(c => c.Equals(city, StringComparison.OrdinalIgnoreCase)))
            {
                errorMessage = "Я не знаю такого города!😢";
                return false;
            }

            // Проверка 2: Не называли ли его раньше
            if (_usedCities.Contains(city))
            {
                errorMessage = "Этот город уже называли!😢";
                return false;
            }

            // Проверка 3: Та ли это буква? (пропускаем для первого хода)
            if (CurrentLetter != '\0' && !city.StartsWith(CurrentLetter.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = $"Нужно назвать город на букву '{CurrentLetter}'!";
                return false;
            }

            _usedCities.Add(city);
            CurrentLetter = GetNextValidLetter(city);
            return true;
        }

        public string ComputerTurn()
        {
            string foundCity = _allCities
                .FirstOrDefault(c => c.StartsWith(CurrentLetter.ToString(), StringComparison.OrdinalIgnoreCase) && !_usedCities.Contains(c));

            if (foundCity != null)
            {
                _usedCities.Add(foundCity);
                CurrentLetter = GetNextValidLetter(foundCity);
            }

            return foundCity;
        }

        private char GetNextValidLetter(string city)
        {
            char[] badLetters = { 'ь', 'ы', 'ъ', 'й' };

            string lowerCity  = city.ToLower();

            for(int i = lowerCity.Length - 1; i >= 0; i--)
            {
                char letter = lowerCity[i];
                if(!badLetters.Contains(letter))
                {
                    return char.ToUpper(letter);
                }
            }

            return 'А';
        }
    }
}
