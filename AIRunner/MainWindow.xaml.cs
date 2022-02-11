using AIRunner.Page;
using NetworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace AIRunner
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int Direction       = 0;
        int round           = 1;
        int moves           = 0;
        DispatcherTimer     timer;

        Network network = null;

        public MainWindow()
        {
            InitializeComponent();

            network = new Network(4, 6, 2, 4);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Game game = new Game();
            MainFrame.Navigate(game);

            game.Spawn();

            double reward = 0;
            List<double> list = new List<double>();
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(0.3);
            timer.Tick += (o, r) => 
            {
                game.BotStep();

                bool PlayerRun = true;
                if (game.IsGameOver())
                {
                    AddToStatusList(round, moves);
                    round++;
                    moves = 0;
                    game.Spawn();

                    MainFrame.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 0, 0));

                    reward = 0.01;
                    PlayerRun = false;

                }

                if (reward > 0 && list.Count > 0)
                {
                    list[Direction] = reward;
                    network.BackPropagation(network.GetError(list));
                    network.UpdateWeights();

                    reward = 0;
                    list.Clear();
                }


                if (PlayerRun)
                {
                    List<double> Input = game.GetWorld();
                    list = network.ForwardPropagation(Input);

                    for (int i = 0; i < Input.Count; i++)
                        ((TextBlock)((Grid)InputNeurons.Children[i]).Children[1]).Text = Math.Round(Input[i], 2).ToString();

                    for (int i = 0; i < list.Count; i++)
                        ((TextBlock)((Grid)((StackPanel)OutNeurons.Children[i]).Children[0]).Children[1]).Text = Math.Round(list[i], 2).ToString();

                    for (int i = 0; i < list.Count; i++)
                        if (list[i] == list.Max())
                            Direction = i;

                    moves++;
                    game.AIRunnerStep(Direction);
                    game.Print();

                    MainFrame.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 162, 0));

                    if (Input[Direction] == 1)
                        reward = 0.01;
                    else
                        reward = 0.8;
                }

                Round.Text  = "Раунд: " + round;
                Moves.Text   = "Ходов: " + moves;

                Arrow.Angle = Direction * 90;
            };
            timer.Start();
        }


        void AddToStatusList(int round, int moves)
        {
            StackPanel temp = new StackPanel();
            temp.Orientation = Orientation.Horizontal;

            TextBlock Round = new TextBlock();
            Round.Text = "Раунд: " + round;
            temp.Children.Add(Round);

            TextBlock Moves = new TextBlock();
            Moves.Text = "Ходов: " + moves;
            Moves.Margin = new Thickness(20, 0, 0, 0);
            temp.Children.Add(Moves);

            StatusList.Items.Add(temp);
            StatusList.ScrollIntoView(temp);
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    Direction = 0;
                    break;

                case Key.D:
                    Direction = 1;
                    break;

                case Key.S:
                    Direction = 2;
                    break;

                case Key.A:
                    Direction = 3;
                    break;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (timer == default)
                return;

            timer.Interval = TimeSpan.FromMilliseconds((int)e.NewValue);
            SliderVal.Text = e.NewValue.ToString();
        }
    }
}
