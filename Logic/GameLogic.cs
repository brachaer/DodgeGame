using HelpMama.Characters;
using HelpMama.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Devices.Core;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace HelpMama.Logic
{
    internal class GameLogic
    {
        public Image explosionImage { get; set; }
        Board Board;
        Random random;
        DispatcherTimer gameTimer;
        DispatcherTimer moveTimer;
        DispatcherTimer explodeTimer;
        bool moveLeft, moveRight, moveUp, moveDown, jump;
        double collissionPosX, collissionPosY;

        public GameLogic(Board board)
        {
            this.Board = board;
            gameTimer = new DispatcherTimer();
            gameTimer.Interval = new TimeSpan(1000);
            gameTimer.Tick += GameTimer_Tick;
            moveTimer = new DispatcherTimer();
            moveTimer.Interval = new TimeSpan(20);
            moveTimer.Tick += MoveTimer_Tick;
            explodeTimer = new DispatcherTimer();
            explodeTimer.Interval = new TimeSpan(10000);
            explodeTimer.Tick += ExplodeTimer_Tick;
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Window.Current.CoreWindow.KeyUp += CoreWindow_KeyUp;
            random = new Random();

        }

        public void StartGame()
        {
            gameTimer.Start();
            moveTimer.Start();
            NewGame();
        }

        public void StopGame()
        {
            gameTimer.Stop();
            moveTimer.Stop();
            Board.ClearBoard();
        }

        public void Pause()
        {
            gameTimer.Stop();
            moveTimer.Stop();
        }

        public void Resume()
        {
            gameTimer.Start();
            moveTimer.Start();
        }

        public async void NewGame()  //Play background music
        {
            Board.StopBgMusic();
            await Board.PlayBackgroundMusic();
        }

        private void MoveTimer_Tick(object sender, object e)
        {
            if (moveLeft == true && Board.Characters[0].PosX > 10)
            {
                Board.Characters[0].MoveLeft();
            }
            if (moveRight == true && Board.Characters[0].PosX + (Board.Characters[0].Width + 20) < 1520)
            {
                Board.Characters[0].MoveRight();
            }
            if (moveUp == true && Board.Characters[0].PosY > 5)
            {
                Board.Characters[0].MoveUp();
            }
            if (moveDown == true && Board.Characters[0].PosY + (Board.Characters[0].Width + 20) < 650)
            {
                Board.Characters[0].MoveDown();
            }
            if (jump == true)
            {
                Board.Characters[0].Jump();
            }
            
        }

        private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)  //arrow key method to move mom
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Left && Board.Characters[0].PosX > 10)
            {
                moveLeft = true;
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Right && Board.Characters[0].PosX + (Board.Characters[0].Width + 20) < 1520)
            {
                moveRight = true;
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Up && Board.Characters[0].PosY > 5)
            {
                moveUp = true;
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Down && Board.Characters[0].PosY + (Board.Characters[0].Width + 20) < 650)
            {
                moveDown = true;
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Shift)
            {
                jump = true;
            }
        }

        private void CoreWindow_KeyUp(CoreWindow sender, KeyEventArgs args)
        {
            if (args.VirtualKey == Windows.System.VirtualKey.Left)
            {
                moveLeft = false;
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Right)
            {
                moveRight = false;
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Up)
            {
                moveUp = false;
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Down)
            {
                moveDown = false;
            }
            if (args.VirtualKey == Windows.System.VirtualKey.Shift)
            {
                jump = false;
            }
        }

        private void GameTimer_Tick(object sender, object e) //gameTimer start the game
        {
            ChaseMom();
            BabyCollision();
            MomCollision();
            CleanMess();
        }
        
        private void ChaseMom() 
        {
            for (int i = 1; i < Board.Characters.Length; i++)
            {
                int randBaby = random.Next(11);
                if (Board.Characters[0].PosY < Board.Characters[i].PosY &&
                   (Board.Characters[i].PosY > 5) && randBaby != 0)
                {
                    Board.Characters[i].MoveUp();
                }
                else if (Board.Characters[0].PosY > Board.Characters[i].PosY &&
                        (Board.Characters[i].Width + 20) < 650 && randBaby != 0)
                {
                    Board.Characters[i].MoveDown();
                }
                if (Board.Characters[0].PosX > Board.Characters[i].PosX &&
                   (Board.Characters[0].Width + 20) < 1520 && randBaby != 0)
                {
                    Board.Characters[i].MoveRight();
                }
                else if (Board.Characters[0].PosX < Board.Characters[i].PosX &&
                        (Board.Characters[i].PosX > 10) && randBaby != 0)
                {
                    Board.Characters[i].MoveLeft();
                }
            }
        }
        
        private void BabyCollision()  
        {
            for (int i = 1; i < Board.Characters.Length; i++)
            {
                Rect rectI = new Rect(Board.Characters[i].PosX, Board.Characters[i].PosY,
                                      Board.Characters[i].Width, Board.Characters[i].Height);
                for (int j = 1; j < Board.Characters.Length; j++)
                {
                    if (i != j)
                    {
                        Rect rectJ = new Rect(Board.Characters[j].PosX, Board.Characters[j].PosY,
                                              Board.Characters[j].Width, Board.Characters[j].Height);
                        rectJ.Intersect(rectI);
                        if (!rectJ.IsEmpty)
                        {
                            collissionPosX = Board.Characters[j].PosX;
                            collissionPosY = Board.Characters[j].PosY;
                            DestroyCharachter(j);
                            CheckWin();
                        }
                    }
                }
            }
        }

        private void MomCollision()  
        {
            for (int i = 1; i < Board.Characters.Length; i++)
            {
                Rect rectI = new Rect(Board.Characters[i].PosX, Board.Characters[i].PosY,
                                      Board.Characters[i].Width, Board.Characters[i].Height);
                Rect rect0 = new Rect(Board.Characters[0].PosX, Board.Characters[0].PosY,
                                      Board.Characters[0].Width, Board.Characters[0].Height);
                rect0.Intersect(rectI);
                if (!rect0.IsEmpty)
                {
                    collissionPosX = Board.Characters[0].PosX;
                    collissionPosY = Board.Characters[0].PosY;
                    Explode();
                    Board.Characters[0].PosX = 100;
                    Board.Characters[0].PosY = 100;

                    if (Board.LivesLeft != 0)
                    {
                        Board.LivesLeft--;
                        Board.Characters[0].lives--;

                    }

                    if (Board.LivesLeft == 0)
                    {
                        StopGame();
                        DestroyCharachter(0);
                        Board.YouLooseMessage();
                    }
                }
            }
        }

        private void Explode() // visual explosion by collision
        {
            Image explosion = new Image();
            Uri uri = new Uri("ms-appx:///Assets/explosure.png");
            explosion.Source = new BitmapImage(uri);
            explosion.Height = 70;
            explosion.Width = 70;
            Canvas.SetLeft(explosion, collissionPosX);
            Canvas.SetTop(explosion, collissionPosY);
            explosionImage = explosion;
            Board.canvas.Children.Add(explosionImage);
            explodeTimer.Start();
        }

        private void ExplodeTimer_Tick(object sender, object e)   // removes visual explosion 
        {
            Board.canvas.Children.Remove(explosionImage);
            explodeTimer.Stop();
        }
      
        private void DestroyCharachter(int charachterNum)
        {
            Board.Characters[charachterNum].IsLive = false;
            Board.Characters[charachterNum].PosX = 2000;
            Board.Characters[charachterNum].PosY = 2500;
            Board.canvas.Children.Remove(Board.Characters[charachterNum].Image);
        }

        private void CheckWin()  //Getting the total amount of live babies
        {
            int BabyCount = 0;
            for (int i = 1; i < Board.Characters.Length; i++)
            {
                if (Board.Characters[i].IsLive == true)
                {
                    BabyCount++;
                }
            }
            if (BabyCount == 1)
            {
                StopGame();
                Board.YouWinMessage();
            }
        }

        private void CleanMess() // mom collission to clean baby fingerPrints
        {
            for (int i = 0; i < Board.fingerPrints.Length; i++)
            {
                if (Board.fingerPrints[i] != null)
                {
                    if ((Board.Characters[0].PosY > Board.fingerPrints[i].PosY - Board.fingerPrints[i].Height &&
                         Board.Characters[0].PosY < Board.fingerPrints[i].PosY + Board.fingerPrints[i].Height) &&
                         Board.Characters[0].PosX > Board.fingerPrints[i].PosX - Board.fingerPrints[i].Width &&
                         Board.Characters[0].PosX < Board.fingerPrints[i].PosX + Board.fingerPrints[i].Width)
                    {
                        Board.canvas.Children.Remove(Board.fingerPrints[i].Image);
                        Board.fingerPrints[i] = null;
                        Board.Characters[0].lives++;
                        Board.LivesLeft++;
                        Board.liveTB.Text = $"Lives: {Board.LivesLeft}";
                        break;

                    }

                }
            }
        }

    }


}





