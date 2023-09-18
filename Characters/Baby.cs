using HelpMama.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace HelpMama.Characters
{
    internal class Baby : Character
    {
        public Baby(string url, int speed, double posX = 0, double posY = 0, double height = 50, double width = 50, int lives = 1, bool isLive = true) : base(url, speed, posX, posY, height, width, lives, isLive)
        {
        }
    }
}
