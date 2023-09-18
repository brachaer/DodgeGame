using HelpMama.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace HelpMama.Characters
{
    internal class Mom : Character
    {
        public Mom(string url, int speed, double posX = 0, double posY = 0, double height = 100, double width = 100, int lives = 3) : base(url, speed, posX, posY, height, width, lives)
        {
        }
    }
}
