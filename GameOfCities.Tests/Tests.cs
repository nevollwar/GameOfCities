using Microsoft.VisualStudio.TestTools.UnitTesting;
using GameOfCities.Logic;

namespace GameOfCities.Tests
{
    [TestClass]
    public class GameCoreTests
    {
        // Тест 1: Проверяем, что игра правильно определяет следующую букву
        [TestMethod]
        public void PlayerMove_Moscow_ShouldSetNextLetterToA()
        {
            GameCore core = new GameCore();
            bool result = core.PlayerTurn("Москва", out string error);
            Assert.IsTrue(result, "Ход должен быть успешным");
            Assert.AreEqual('А', core.CurrentLetter, "После Москвы должна быть буква А");
        }

        // Тест 2: Проверяем правило "Ь" (мягкого знака)
        [TestMethod]
        public void PlayerMove_Perm_ShouldSetNextLetterToM()
        {
            GameCore core = new GameCore();
            core.PlayerTurn("Пермь", out string error);
            Assert.AreEqual('М', core.CurrentLetter, "После Перми буква должна быть М (пропуск Ь)");
        }

        // Тест 3: Проверка на ошибку (не та буква)
        [TestMethod]
        public void PlayerMove_WrongLetter_ShouldReturnFalse()
        {
            GameCore core = new GameCore();
            core.PlayerTurn("Москва", out string error);
            bool result = core.PlayerTurn("Берлин", out string errorMessage);
            Assert.IsFalse(result, "Ход на неправильную букву не должен быть принят");
            Assert.IsFalse(string.IsNullOrEmpty(errorMessage), "Должно вернуться сообщение об ошибке");
        }
    }
}