using System;
using System.Drawing;
using System.Windows.Forms;
using GameOfCities.Logic;

namespace GameOfCities.UI
{
    /// <summary>
    /// Представляет графический интерфейс пользователя и обрабатывает взаимодействие с игроком.
    /// </summary>
    public partial class GameOfCities : Form
    {
        // Поля управления визуальными компонентами
        private Panel pnlMenu;
        private Panel pnlGame;
        private Panel pnlContent;
        private Panel pnlInput;

        private ListBox lstHistory;
        private TextBox txtInput;
        private Button btnSend;
        private Label lblStatus;
        private Label lblError;
        private ProgressBar pbTime;
        private System.Windows.Forms.Timer gameTimer;

        // Поля состояния игровой сессии
        private int timeLeft;
        private int maxTime;
        private GameCore game;

        /// <summary>
        /// Инициализирует форму, настраивает параметры окна и вызывает начальный экран меню.
        /// </summary>
        public GameOfCities()
        {
            this.Text = "Игра в города v1.0";
            this.Size = new Size(500, 700);
            this.MinimumSize = new Size(500, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = true;

            // Подписка на событие изменения размера для динамического центрирования элементов
            this.Resize += (s, e) => CenterMenuElements();

            InitializeMenu();
        }

        /// <summary>
        /// Выполняет динамический пересчет координат центральной панели меню 
        /// для обеспечения корректного отображения при масштабировании окна.
        /// </summary>
        private void CenterMenuElements()
        {
            if (pnlContent != null && pnlMenu != null)
            {
                pnlContent.Left = (pnlMenu.Width - pnlContent.Width) / 2;
                pnlContent.Top = (pnlMenu.Height - pnlContent.Height) / 2;
            }
        }

        /// <summary>
        /// Формирует графические элементы главного меню приложения.
        /// </summary>
        private void InitializeMenu()
        {
            this.Controls.Clear();
            pnlMenu = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 30, 30) };
            this.Controls.Add(pnlMenu);

            pnlContent = new Panel { Width = 400, Height = 500, BackColor = Color.Transparent };
            pnlMenu.Controls.Add(pnlContent);

            Label lblLogo = new Label
            {
                Text = "ГОРОДА",
                Font = new Font("Segoe UI", 48, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Top,
                Height = 100,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnStart = CreateMenuButton("ИГРАТЬ", 150, Color.FromArgb(0, 122, 204));
            Button btnHelp = CreateMenuButton("ПОМОЩЬ", 220, Color.FromArgb(60, 60, 60));
            Button btnDev = CreateMenuButton("ОБ АВТОРЕ", 290, Color.FromArgb(60, 60, 60));
            Button btnExit = CreateMenuButton("ВЫХОД", 360, Color.Brown);

            btnStart.Click += (s, e) => ShowDifficultySelection();
            btnHelp.Click += (s, e) => MessageBox.Show("Правила игры:\n\n" +
    "1. Называйте реально существующие города мира.\n" +
    "2. Ваш город должен начинаться на последнюю букву города оппонента.\n" +
    "3. Если город заканчивается на буквы Ь, Ы, Ъ или Й, то следующему игроку нужно называть город на букву, стоящую перед ними.\n" +
    "4. Повторять города в течение одной партии нельзя.\n" +
    "5. Вы проигрываете, если у вас закончится время на таймере.",
    "Инструкция пользователя");
            btnDev.Click += (s, e) => MessageBox.Show("Курсовая работа по дисциплине:\n" +
    "«Основы программирования»\n\n" +
    "Тема: Игровое приложение «Игра в города»\n\n" +
    "Разработчик: Ясенков Н.\n" +
    "Группа: Б.ПИН.ИИ.25.16\n" +
    "Преподаватель: Веревка А.А.\n\n" +
    "2026 год",
    "Сведения о разработчике");
            btnExit.Click += (s, e) => Application.Exit();

            pnlContent.Controls.AddRange(new Control[] { lblLogo, btnStart, btnHelp, btnDev, btnExit });
            CenterMenuElements();
        }

        /// <summary>
        /// Отображает интерфейс выбора уровня сложности (ограничения по времени).
        /// </summary>
        private void ShowDifficultySelection()
        {
            pnlContent.Controls.Clear();
            Label lblTitle = new Label
            {
                Text = "ВЫБОР СЛОЖНОСТИ",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.Gray,
                Dock = DockStyle.Top,
                Height = 50,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnEasy = CreateMenuButton("ЛЕГКО (60 сек)", 70, Color.Green);
            Button btnMed = CreateMenuButton("СРЕДНЕ (30 сек)", 140, Color.Orange);
            Button btnHard = CreateMenuButton("СЛОЖНО (15 сек)", 210, Color.Red);
            Button btnBack = CreateMenuButton("НАЗАД", 280, Color.DimGray);

            btnEasy.Click += (s, e) => StartGame(60);
            btnMed.Click += (s, e) => StartGame(30);
            btnHard.Click += (s, e) => StartGame(15);
            btnBack.Click += (s, e) => InitializeMenu();

            pnlContent.Controls.AddRange(new Control[] { lblTitle, btnEasy, btnMed, btnHard, btnBack });
        }

        /// <summary>
        /// Осуществляет переход от меню к игровому процессу с заданными параметрами.
        /// </summary>
        /// <param name="seconds">Ограничение времени на один ход в секундах.</param>
        private void StartGame(int seconds)
        {
            maxTime = seconds;
            timeLeft = seconds;
            this.Controls.Clear();
            SetupGameUI();

            try
            {
                game = new GameCore();
                lstHistory.Items.Add("Система: Сессия начата.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Критическая ошибка: " + ex.Message);
            }
        }

        /// <summary>
        /// Выполняет компоновку и настройку элементов управления игрового экрана.
        /// </summary>
        private void SetupGameUI()
        {
            pnlGame = new Panel { Dock = DockStyle.Fill, Padding = new Padding(30), BackColor = Color.FromArgb(30, 30, 30) };
            this.Controls.Add(pnlGame);

            Label lblTitleText = new Label { Text = "ГОРОДА", Font = new Font("Segoe UI", 24, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 60 };

            Panel pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 220 };
            lblStatus = new Label { Text = "Ожидание ввода...", ForeColor = Color.Gray, Dock = DockStyle.Top, Height = 30, Font = new Font("Segoe UI", 10, FontStyle.Italic) };
            pnlInput = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.FromArgb(60, 60, 60) };
            txtInput = new TextBox { Dock = DockStyle.Fill, BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 20) };
            btnSend = new Button { Text = "ОТВЕТИТЬ", Dock = DockStyle.Right, Width = 150, BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 12, FontStyle.Bold) };

            pnlInput.Controls.Add(txtInput);
            pnlInput.Controls.Add(btnSend);

            lblError = new Label { Text = "", ForeColor = Color.Tomato, Dock = DockStyle.Top, Height = 40, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
            pbTime = new ProgressBar { Dock = DockStyle.Top, Height = 15, Maximum = maxTime, Value = maxTime };
            Button btnSurrender = new Button { Text = "СДАТЬСЯ", Dock = DockStyle.Bottom, Height = 50, BackColor = Color.FromArgb(80, 40, 40), ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

            pnlBottom.Controls.AddRange(new Control[] { lblError, pbTime, pnlInput, lblStatus, btnSurrender });

            lstHistory = new ListBox { Dock = DockStyle.Fill, BackColor = Color.FromArgb(45, 45, 48), ForeColor = Color.LightGray, BorderStyle = BorderStyle.None, Font = new Font("Segoe UI", 16) };

            pnlGame.Controls.Add(lstHistory);
            pnlGame.Controls.Add(pnlBottom);
            pnlGame.Controls.Add(lblTitleText);

            gameTimer = new System.Windows.Forms.Timer { Interval = 1000 };
            gameTimer.Tick += GameTimer_Tick;
            btnSend.Click += btnMove_Click;
            txtInput.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnSend.PerformClick(); };
            btnSurrender.Click += (s, e) => GameOver("Вы признали поражение.");
        }

        /// <summary>
        /// Инкапсулирует логику создания стилизованных кнопок для интерфейса меню.
        /// </summary>
        private Button CreateMenuButton(string text, int y, Color color)
        {
            return new Button
            {
                Text = text,
                Location = new Point(50, y),
                Size = new Size(300, 55),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        /// <summary>
        /// Обработчик события таймера; выполняет декремент времени и проверку условия проигрыша.
        /// </summary>
        private void GameTimer_Tick(object sender, EventArgs e)
        {
            if (timeLeft > 0) { timeLeft--; pbTime.Value = timeLeft; }
            else GameOver("Время истекло!");
        }

        /// <summary>
        /// Обрабатывает событие нажатия кнопки хода, инициируя цепочку валидации и ответного хода оппонента.
        /// </summary>
        private void btnMove_Click(object sender, EventArgs e)
        {
            string city = txtInput.Text.Trim();
            if (game.PlayerTurn(city, out string error))
            {
                AddMoveToList("ИГРОК", city);
                timeLeft = maxTime; pbTime.Value = maxTime; gameTimer.Start();

                string compCity = game.ComputerTurn();
                if (compCity != null) AddMoveToList("РОБОТ", compCity);
                else WinGame();
            }
            else lblError.Text = "⚠ " + error;
        }

        /// <summary>
        /// Обновляет визуальный лог событий, добавляя в него информацию о совершенном ходе.
        /// </summary>
        private void AddMoveToList(string senderName, string cityName)
        {
            string formatted = char.ToUpper(cityName[0]) + cityName.Substring(1).ToLower();
            lstHistory.Items.Add($"{senderName}: {formatted}");
            lstHistory.TopIndex = lstHistory.Items.Count - 1;
            txtInput.Clear(); lblError.Text = ""; txtInput.Focus();
        }

        private void GameOver(string m) { gameTimer.Stop(); MessageBox.Show(m); ResetToMenu(); }
        private void WinGame() { gameTimer.Stop(); MessageBox.Show("Победа!"); ResetToMenu(); }
        private void ResetToMenu() { this.Controls.Clear(); InitializeMenu(); }
    }
}