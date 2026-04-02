using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
namespace GameOfCities.Logic
{
    public class CityRepository
    {
        private readonly string _filePath;

        public CityRepository()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            _filePath = Path.Combine(folder, "cities.txt");
        } 

        public List<string> GetCities()
        {
            if (!File.Exists(_filePath))
            {
                throw new FileNotFoundException($"Файл по пути {_filePath} не найден.");
            }

            string[] allLines = File.ReadAllLines(_filePath);

            List<string> cleanCities = allLines
                .Where (line => !string.IsNullOrWhiteSpace (line))
                .Select (line => line.Trim())
                .ToList();
            return cleanCities;
        }
    }
}
