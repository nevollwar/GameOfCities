using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace GameOfCities.Logic
{
    /// <summary>
    /// Обеспечивает доступ к внешним данным, отвечая за чтение и первичную обработку списка городов.
    /// </summary>
    public class CityRepository
    {
        private readonly string filePath;

        /// <summary>
        /// Инициализирует репозиторий, формируя абсолютный путь к ресурсному файлу.
        /// </summary>
        public CityRepository()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            filePath = Path.Combine(folder, "cities.txt");
        }

        /// <summary>
        /// Извлекает список городов из текстового файла, выполняя фильтрацию пустых строк 
        /// и удаление лишних символов пробела.
        /// </summary>
        /// <returns>Список нормализованных названий городов.</returns>
        /// <exception cref="FileNotFoundException">Генерируется при отсутствии ресурсного файла.</exception>
        public List<string> GetCities()
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Ресурс не найден: {filePath}");
            }

            // Чтение данных и применение LINQ для нормализации коллекции
            string[] allLines = File.ReadAllLines(filePath);

            List<string> cleanCities = allLines
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Trim())
                .ToList();

            return cleanCities;
        }
    }
}