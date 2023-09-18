using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace HelpMama.Characters
{
    internal class FingerPrint : Character
    {
        public FingerPrint(string url, int speed=0, double posX = 0, double posY = 0, double height = 80, double width = 80, int lives = 0, bool isLive = true) : base(url, speed, posX, posY, height, width, lives, isLive)
        {
        }
    }
}
