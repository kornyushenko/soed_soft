using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

/*
Короче, задача какая: 
приложение(на интерфейс посрать, взять какие нибудь из библиотек простые) - 
выдает заданное(задаваемое в настройках) количество цифр по очереди(с задаваемым временным интервалом). 
Потом пользователь вбивает цифры по памяти. - и приложение показывает два ряда цифр рядом, чтобы можно было сравнить. вот и все )
*/

namespace SoedTestMem
{
    public partial class MainPage : ContentPage
    {
        private Button _startButton;
        private Button _checkButton;
        private Entry _timeEntry;
        private Label _timeLabel;
        private Entry _numbersCountEntry;
        private Label _numbersCountLabel;
        private Label _numberLabel;
        private Entry _repeatEntry;
        private Label _repeatLabel;
        private Label _generatedLabel;
        private bool _isRun = false;
        private int _runsLeft = -1;
        private System.Timers.Timer _timer;
        private List<int> _generatedList = new List<int>();
        private Random _random = new Random();
        private AbsoluteLayout _layout;

        public MainPage()
        {
            InitializeComponent();
            CreateWidgets();

            _random = new Random(DateTime.Now.Millisecond);

            _timer = new System.Timers.Timer();
            _timer.Elapsed += _timer_Elapsed;
        }

        public void CreateWidgets()
        {
            var layout = new AbsoluteLayout();
            _layout = layout;
            var yOffset = 25;
            var xOffset = 25;
            var xstep = 25;
            var ystep = 25;
            var startButtonW = 100;
            var startButtonH = 90;

            _startButton = new Button() { Text = "Start" };
            AbsoluteLayout.SetLayoutBounds(_startButton, new Rectangle(xOffset, yOffset, startButtonW, startButtonH));
            _startButton.Clicked += _startButton_Clicked;

            var timeLabelW = 100;
            var timeLabelH = 40;
            var currentX = startButtonW + xOffset + xstep;
            _timeLabel = new Label() { Text = "Time Interval, seconds:" };
            AbsoluteLayout.SetLayoutBounds(_timeLabel, new Rectangle(currentX, yOffset, timeLabelW, timeLabelH));
            _timeEntry = new Entry() { Keyboard = Keyboard.Numeric, MaxLength = 5, Text = "1" };
            AbsoluteLayout.SetLayoutBounds(_timeEntry, new Rectangle(currentX, yOffset + 5 + timeLabelH, timeLabelW, timeLabelH));

            currentX = startButtonW + xOffset + xstep + timeLabelW + xstep;
            _numbersCountLabel = new Label() { Text = "Numbers count:" };
            AbsoluteLayout.SetLayoutBounds(_numbersCountLabel, new Rectangle(currentX, yOffset, timeLabelW, timeLabelH));
            _numbersCountEntry = new Entry() { Keyboard = Keyboard.Numeric, MaxLength = 10, Text = "5" };
            AbsoluteLayout.SetLayoutBounds(_numbersCountEntry, new Rectangle(currentX, yOffset + 5 + timeLabelH, timeLabelW, timeLabelH));

            var currentY = startButtonH + ystep;
            var numberH = 250;
            _numberLabel = new Label();
            AbsoluteLayout.SetLayoutBounds(_numberLabel, new Rectangle(xOffset + startButtonW + xstep, yOffset + startButtonH, 200, numberH));
            _numberLabel.FontSize = 196;

            currentX = xOffset;
            _repeatLabel = new Label() { Text = "Repeat numbers then press Check button:" };
            AbsoluteLayout.SetLayoutBounds(_repeatLabel, new Rectangle(currentX, yOffset + startButtonH + numberH, timeLabelW * 4, timeLabelH));
            _repeatEntry = new Entry() { Keyboard = Keyboard.Numeric };
            AbsoluteLayout.SetLayoutBounds(_repeatEntry, new Rectangle(currentX + 105, yOffset + startButtonH + numberH + timeLabelH, 600, timeLabelH));
            _checkButton = new Button() { Text = "Check" };
            _checkButton.Clicked += _checkButton_Clicked;
            AbsoluteLayout.SetLayoutBounds(_checkButton, new Rectangle(currentX, yOffset + startButtonH + numberH + timeLabelH - 5, 100, 50));

            _generatedLabel = new Label();
            AbsoluteLayout.SetLayoutBounds(_generatedLabel, new Rectangle(currentX + 50 + timeLabelH + xstep, yOffset + startButtonH + numberH + timeLabelH + timeLabelH + ystep, 600, timeLabelH));

            layout.Children.Add(_startButton);
            layout.Children.Add(_timeLabel);
            layout.Children.Add(_timeEntry);
            layout.Children.Add(_numbersCountLabel);
            layout.Children.Add(_numbersCountEntry);
            layout.Children.Add(_numberLabel);

            Content = layout;
        }

        private void ShowGenerated()
        {
            _layout.Children.Add(_generatedLabel);
            var sb = new StringBuilder();
            foreach (var c in _generatedList)
            {
                sb.Append(c.ToString());
            }
            _generatedLabel.Text = sb.ToString();
            if (_generatedLabel.Text == _repeatEntry.Text)
            {
                _repeatEntry.TextColor = Color.Green;
            }
            else
            {
                _repeatEntry.TextColor = Color.Red;
            }
        }

        private void ShowRepeat()
        {
            _repeatEntry.Text = "";
            _repeatEntry.MaxLength = _generatedList.Count;
            _layout.Children.Add(_repeatLabel);
            _layout.Children.Add(_repeatEntry);
            _layout.Children.Add(_checkButton);
        }

        private void HideRepeat()
        {
            _repeatEntry.TextColor = Color.Black;
            _layout.Children.Remove(_generatedLabel);
            _layout.Children.Remove(_repeatLabel);
            _layout.Children.Remove(_repeatEntry);
            _layout.Children.Remove(_checkButton);
        }

        private void _checkButton_Clicked(object sender, EventArgs e)
        {
            ShowGenerated();
        }

        private void _startButton_Clicked(object sender, EventArgs e)
        {
            if (!_isRun)
            {
                float interval = -1;
                if (!float.TryParse(_timeEntry.Text, System.Globalization.NumberStyles.Float, null, out interval))
                {
                    _timeEntry.TextColor = Color.Red;
                }
                else
                {
                    _timeEntry.TextColor = Color.Black;
                }

                int count = -1;
                if (!int.TryParse(_numbersCountEntry.Text, out count) || count <= 0)
                {
                    _numbersCountEntry.TextColor = Color.Red;
                }
                else
                {
                    _numbersCountEntry.TextColor = Color.Black;
                }

                if (interval > 0 && count > 0)
                {
                    _repeatEntry.Text = "";
                    _runsLeft = count;
                    _timer.Interval = interval * 1000;
                    _isRun = true;
                    _generatedList.Clear();
                    HideRepeat();
                    NextNumber();
                    _timer.Start();
                }
            }
        }

        private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (_isRun)
            {
                NextNumber();
            }
        }

        private void NextNumber()
        {
            --_runsLeft;

            if (_runsLeft >= 0)
            {
                var n = _random.Next(0, 10);
                if (_generatedList.Count == 0)
                {
                    _generatedList.Add(n);
                }
                else
                {
                    while (_generatedList[_generatedList.Count - 1] == n)
                    {
                        n = _random.Next(0, 10);
                    }
                    _generatedList.Add(n);
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    _numberLabel.Text = n.ToString();
                });
            }
            
            if (_runsLeft == -1)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ShowRepeat();
                    _numberLabel.Text = "";
                });
                _isRun = false;
            }
        }
    }
}
