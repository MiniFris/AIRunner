using System;
using System.Collections.Generic;
using System.Windows.Controls;

namespace AIRunner.Page
{

    public class Bot
    {
        public int[] Pos { get; set; }
    }

    public class AIRunner
    {
        public int[] Pos { get; set; }
    }


    /// <summary>
    /// Логика взаимодействия для Game.xaml
    /// </summary>
    public partial class Game
    {
        Random rand = new Random();

        public Bot bot      = null;
        public AIRunner core   = null;

        int[] PoleSize      = null;
        int Space           = 1;

        int Width = 400;
        int Height = 400;


        public Game()
        {
            InitializeComponent();
        }

        public void Spawn()
        {
            PoleSize = new int[] { (int)(Width / (AIRunnerOBJ.Width + Space)), (int)(Height / (AIRunnerOBJ.Height + Space)) };

            bot         = new Bot(); ;
            core        = new AIRunner();

            bot.Pos     = new int[] { rand.Next(0, PoleSize[0]), rand.Next(0, PoleSize[1]) };
            core.Pos    = new int[] { rand.Next(0, PoleSize[0]), rand.Next(0, PoleSize[1]) };
        }

        public void BotStep()
        {
            int x = core.Pos[0] - bot.Pos[0];
            int y = core.Pos[1] - bot.Pos[1];

            if (x < 0) x *= -1;
            if (y < 0) y *= -1;

            if (x > y)
            {
                if (bot.Pos[0] < core.Pos[0]) bot.Pos[0]++;
                else if (bot.Pos[0] > core.Pos[0]) bot.Pos[0]--;
            }
            else
            {
                if (bot.Pos[1] < core.Pos[1]) bot.Pos[1]++;
                else if (bot.Pos[1] > core.Pos[1]) bot.Pos[1]--;
            }
        }

        public void Print()
        {
            Canvas.SetLeft(AIRunnerOBJ, (core.Pos[0] * (AIRunnerOBJ.Width + Space)));
            Canvas.SetTop(AIRunnerOBJ, (core.Pos[1] * (AIRunnerOBJ.Height + Space)));

            Canvas.SetLeft(BotOBJ, (bot.Pos[0] * (BotOBJ.Width + Space)));
            Canvas.SetTop(BotOBJ, (bot.Pos[1] * (BotOBJ.Height + Space)));
        }

        public void AIRunnerStep(int Direction)
        {
            int[] NewPos = new int[] { core.Pos[0], core.Pos[1] };
            switch(Direction)
            {
                case 0:
                    NewPos[1] -= 1;
                    break;

                case 1:
                    NewPos[0] += 1;
                    break;

                case 2:
                    NewPos[1] += 1;
                    break;

                case 3:
                    NewPos[0] -= 1;
                    break;
            }


            if (NewPos[0] < 0 || NewPos[0] >= PoleSize[0])
                NewPos[0] = core.Pos[0];

            if(NewPos[1] < 0 || NewPos[1] >= PoleSize[1])
                NewPos[1] = core.Pos[1];

            core.Pos = new int[] { NewPos[0], NewPos[1] };
        }


        public List<double> GetWorld()
        {
            List<double> Temp = new List<double>() { 0.1, 0.1, 0.1, 0.1 };

            if (core.Pos[0] - 3 < 0) Temp[3] = 1;
            if (core.Pos[0] + 3 >= PoleSize[0]) Temp[1] = 1;

            if (core.Pos[1] - 3 < 0) Temp[0] = 1;
            if (core.Pos[1] + 3 >= PoleSize[1]) Temp[2] = 1;


            if (core.Pos[0] - 3 <= bot.Pos[0] && core.Pos[0] > bot.Pos[0] && core.Pos[1] == bot.Pos[1]) Temp[3] = 1;
            if (core.Pos[0] + 3 >= bot.Pos[0] && core.Pos[0] < bot.Pos[0] && core.Pos[1] == bot.Pos[1]) Temp[1] = 1;

            if (core.Pos[1] - 3 <= bot.Pos[1] && core.Pos[1] > bot.Pos[1] && core.Pos[0] == bot.Pos[0]) Temp[0] = 1;
            if (core.Pos[1] + 3 >= bot.Pos[1] && core.Pos[1] < bot.Pos[1] && core.Pos[0] == bot.Pos[0]) Temp[2] = 1;

            return Temp;
        }


        public bool IsGameOver()
        {
            if (bot.Pos[0] == core.Pos[0] && bot.Pos[1] == core.Pos[1])
                return true;

            return false;
        }
    }
}
