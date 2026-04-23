using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameOfCities.Logic;

namespace GameOfCities.Tests
{
    [TestClass]
    public class GameCoreTests
    {
        /// <summary>
        /// Выполняет верификацию базового алгоритма определения целевой буквы после успешного хода.
        /// Ожидается, что после ввода города "Москва" целевым символом для следующего хода станет 'А'.
        /// </summary>
        [TestMethod]
        public void PlayerMove_Moscow_ShouldSetNextLetterToA()
        {
            GameCore core = new GameCore();
            bool result = core.PlayerTurn("Москва", out string error);
            Assert.IsTrue(result, "Ход должен быть успешным");
            Assert.AreEqual('А', core.CurrentLetter, "После Москвы должна быть буква А");
        }

        /// <summary>
        /// Проверяет корректность реализации правила обработки окончаний на исключаемые символы.
        /// Верифицирует алгоритм пропуска буквы 'Ь' и перехода к предшествующему валидному символу 'М'.
        /// </summary>
        [TestMethod]
        public void PlayerMove_Perm_ShouldSetNextLetterToM()
        {
            GameCore core = new GameCore();
            core.PlayerTurn("Пермь", out string error);
            Assert.AreEqual('М', core.CurrentLetter, "После Перми буква должна быть М (пропуск Ь)");
        }

        /// <summary>
        /// Тестирует механизм валидации начального символа вводимого города на соответствие правилам последовательности.
        /// Ожидается отклонение хода при несоответствии первой буквы текущему игровому состоянию.
        /// </summary>
        [TestMethod]
        public void PlayerMove_WrongLetter_ShouldReturnFalse()
        {
            GameCore core = new GameCore();
            core.PlayerTurn("Москва", out string error);
            bool result = core.PlayerTurn("Берлин", out string errorMessage);
            Assert.IsFalse(result, "Ход на неправильную букву не должен быть принят");
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage), "Должно вернуться сообщение об ошибке");
        }

        /// <summary>
        /// Проверяет устойчивость системы к попыткам повторного использования названий городов.
        /// Верифицирует соблюдение правила уникальности слов в рамках одной игровой сессии.
        /// </summary>
        [TestMethod]
        public void PlayerTurn_DuplicateCity_ShouldReturnFalse()
        {
            GameCore core = new GameCore();
            core.PlayerTurn("Москва", out _);

            bool result = core.PlayerTurn("Москва", out string error);

            Assert.IsFalse(result);
            Assert.AreEqual("Данный город уже использовался в текущей сессии.", error);
        }

        /// <summary>
        /// Тестирует реакцию системы на ввод строковых данных, отсутствующих в эталонной лексической базе.
        /// Ожидается отказ в совершении хода с выводом соответствующего уведомления.
        /// </summary>
        [TestMethod]
        public void PlayerTurn_UnknownCity_ShouldReturnFalse()
        {
            GameCore core = new GameCore();
            bool result = core.PlayerTurn("НесуществующийГородХУЗ", out string error);

            Assert.IsFalse(result);
            Assert.AreEqual("Указанный город отсутствует в базе данных.", error);
        }

        /// <summary>
        /// Верифицирует работу алгоритма при обнаружении названий со скобками и спецсимволами.
        /// Проверяет корректность игнорирования небуквенных знаков при вычислении следующей буквы.
        /// </summary>
        [TestMethod]
        public void PlayerTurn_CityWithBrackets_ShouldIgnoreSpecialCharacters()
        {
            GameCore core = new GameCore();
            // Пример из практики: Олбани (файет кантри) -> должна определиться буква 'И'
            core.PlayerTurn("Олбани (файет кантри)", out _);

            Assert.AreEqual('И', core.CurrentLetter, "Алгоритм должен игнорировать скобки и искать последнюю БУКВУ.");
        }

        /// <summary>
        /// Проверяет функциональность автоматизированного хода компьютерного оппонента.
        /// Верифицирует, что бот находит город на заданную букву и обновляет состояние игры.
        /// </summary>
        [TestMethod]
        public void ComputerTurn_ShouldFindValidCity()
        {
            GameCore core = new GameCore();
            // Игрок задает букву 'А'
            core.PlayerTurn("Москва", out _);

            string botCity = core.ComputerTurn();

            Assert.IsNotNull(botCity, "Компьютер должен найти город в базе.");
            Assert.IsTrue(botCity.StartsWith("А", StringComparison.OrdinalIgnoreCase), "Город бота должен начинаться на правильную букву.");
        }
    }
}