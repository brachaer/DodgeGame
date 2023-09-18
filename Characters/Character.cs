using HelpMama.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace HelpMama.Characters
{
    internal class Character
    {
        public bool IsLive { get; set; }
        public Image Image { get; set; }
        public int Speed { get; set; }
        private string imageUrl;
        public string ImageUrl
        {
            get { return imageUrl; }

            set
            {
                // value => "ms-appx:///Assets/One.png";
                string url = value;
                imageUrl = url;
                Image.Source = new BitmapImage(new Uri(url));
                Image.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
        }
        public double PosX
        {
            get { return Canvas.GetLeft(Image); }
            set { Canvas.SetLeft(Image, value); }
        }
        public double PosY
        {
            get { return Canvas.GetTop(Image); }
            set { Canvas.SetTop(Image, value); }
        }
        public double Width { get { return Image.Width; } }
        public double Height { get { return Image.Height; } }
        public int lives { get; set; }
        static Random random;
        public Character(string url, int speed, double posX = 0.0, double posY = 0.0, double height = 70.0, double width = 70.0, int lives = 1, bool isLive = true)
        {
            Image = new Image();
            Image.Source = new BitmapImage(new Uri(url));
            Image.Width = width;
            Image.Height = height;
            Speed = speed;
            Canvas.SetLeft(Image, posX);
            Canvas.SetTop(Image, posY);
            imageUrl = url;
            this.lives = lives;
            IsLive = isLive;
        }
        public void MoveUp()
        {
            PosY -= Speed;
        }
        public void MoveDown()
        {
            PosY += Speed;
        }
        public void MoveLeft()
        {
            PosX -= Speed;
        }
        public void MoveRight()
        {
            PosX += Speed;
        }
        public void Jump()
        {
            random= new Random();
            PosX =random.NextDouble();
            PosY=random.NextDouble();
        }
    }
}
