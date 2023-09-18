using HelpMama.Characters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Foundation;
using Windows.Media.Devices;
using Windows.Security.Isolation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

namespace HelpMama.Logic
{
    internal class Board
    {
        public Canvas canvas { get; set; }
        public TextBox liveTB { get; set; }
        public Character[] Characters { get; set; }
        public FingerPrint[] fingerPrints { get; set; }
        public int LivesLeft { get; set; }
        int currentLives;
        GameLogic logic;
        Random random;
        Character[] savedCharacters;
        FingerPrint[] savedFingers;
        MediaElement backgroundSound;

        public Board(Canvas myCanvas)
        {
            this.canvas = myCanvas;
            Characters = new Character[11];
            savedCharacters = new Character[11];
            backgroundSound = new MediaElement();
            random = new Random();
            logic = new GameLogic(this);
            LivesLeft = 3;
            StartScreen();

        }

        public async Task<MediaElement> PlayBackgroundMusic()
        {
            var BgMusicElement = new MediaElement();
            var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Music");
            var file = await folder.GetFileAsync("BGmusic.mp3");
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            BgMusicElement.SetSource(stream, "");
            BgMusicElement.Play();
            BgMusicElement.Volume = 0.1;  
            backgroundSound = BgMusicElement;
            return BgMusicElement;
        }

        public void StopBgMusic() 
        {
            backgroundSound.Stop();
        }

        public void YouWinMessage()
        {
            ClearBoard();
            TextBlock winnerTB = new TextBlock();
            winnerTB.Text = "You Win!";
            winnerTB.HorizontalAlignment = HorizontalAlignment.Stretch;
            winnerTB.FontSize = 300;
            winnerTB.Height = 830;
            winnerTB.TextAlignment = TextAlignment.Center;
            winnerTB.Width = 1400;
            winnerTB.Foreground = new SolidColorBrush(Colors.Black);
            canvas.Children.Add(winnerTB);
            AddButtons();
        }

        public void YouLooseMessage()
        {
            ClearBoard();
            TextBlock looseTB = new TextBlock();
            looseTB.Text = "You Lost!";
            looseTB.Foreground = new SolidColorBrush(Colors.Black);
            looseTB.FontSize = 200;
            looseTB.TextAlignment = TextAlignment.Center;
            looseTB.Height = 830;
            looseTB.Width = 1400;
            looseTB.HorizontalAlignment = HorizontalAlignment.Stretch;
            canvas.Children.Add(looseTB);
            AddButtons();
        }

        public void ClearBoard()
        {
            canvas.Children.Clear();
            AddButtons();
        }

        private void StartScreen() // welcome screen and game instructions
        {
            Viewbox image = new Viewbox();
            Image imageControl = new Image() { Width = 1500, Height = 660 };
            Uri uri = new Uri("ms-appx:///Assets/Start.png");
            BitmapImage source = new BitmapImage(uri);
            imageControl.Source = source;
            image.Child = imageControl;
            canvas.Children.Add(image);
            AddButtons();
        }

        private void CreateCharacters()
        {
            Characters[0] = new Mom("ms-appx:///Assets/mom.png", 15, 100, 100); //pic source: <a href='https://pngtree.com/so/Cleaning'>Cleaning png from pngtree.com/</a>
            canvas.Children.Add(Characters[0].Image);
            Characters[0].lives = 3;
            for (int i = 1; i < Characters.Length; i++)
            {
                double posX = random.Next(400, 1000);
                double posY = random.Next(100, 500);
                int randBaby = random.Next(1, 5);
                Characters[i] = new Baby($"ms-appx:///Babies/{randBaby}.png", 5, posX, posY);
                canvas.Children.Add(Characters[i].Image);
            }
        }

        private void CreateBabyPrints()
        {
            fingerPrints = new FingerPrint[11];
            fingerPrints[0] = new FingerPrint("ms-appx:///Assets/babyFingers1.png", 0, 100, 150);
            fingerPrints[1] = new FingerPrint("ms-appx:///Assets/babyFingers2.png", 0, 100, 450);
            fingerPrints[2] = new FingerPrint("ms-appx:///Assets/babyFingers3.png", 0, 300, 50);
            fingerPrints[3] = new FingerPrint("ms-appx:///Assets/babyFingers4.png", 0, 400, 250);
            fingerPrints[4] = new FingerPrint("ms-appx:///Assets/babyFingers2.png", 0, 400, 550);
            fingerPrints[5] = new FingerPrint("ms-appx:///Assets/babyFingers1.png", 0, 700, 100);
            fingerPrints[6] = new FingerPrint("ms-appx:///Assets/babyFingers2.png", 0, 1100, 400);
            fingerPrints[7] = new FingerPrint("ms-appx:///Assets/babyFingers3.png", 0, 700, 470);
            fingerPrints[8] = new FingerPrint("ms-appx:///Assets/babyFingers4.png", 0, 1000, 50);
            fingerPrints[9] = new FingerPrint("ms-appx:///Assets/babyFingers1.png", 0, 1200, 300);
            fingerPrints[10] = new FingerPrint("ms-appx:///Assets/babyFingers3.png", 0, 1200, 70);
            foreach (FingerPrint item in fingerPrints)
            {
                canvas.Children.Add(item.Image);
            }
        }

        private void LivesTB() //Mom lives on screen
        {
            TextBox liveTB = new TextBox();
            this.liveTB = liveTB;
            liveTB.Height = 50;
            liveTB.Width = 100;
            liveTB.FontSize = 20;
            liveTB.Foreground = new SolidColorBrush(Colors.Black);
            Canvas.SetLeft(liveTB, 30);
            Canvas.SetTop(liveTB, 30);
            liveTB.Text = $"Lives: {LivesLeft}";
            canvas.Children.Add(liveTB);
        }

        private void AddButtons()
        {
            NewGameButton();
            PauseGameButton();
            LoadButton();
            ResumeButton();
            SaveButton();
        }

        private void NewGameButton()
        {
            Button btnNew = new Button();
            btnNew.Content = "New Game";
            btnNew.Height = 50;
            btnNew.Width = 180;
            btnNew.FontSize = 30;
            btnNew.Background = new SolidColorBrush(Colors.HotPink);
            btnNew.Foreground = new SolidColorBrush(Colors.Black);
            Canvas.SetLeft(btnNew, 40);
            Canvas.SetTop(btnNew, 700);
            canvas.Children.Add(btnNew);
            btnNew.Click += BtnNew_Click;
        }

        private void BtnNew_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            logic.StopGame();
            ClearBoard();
            CreateBabyPrints();
            CreateCharacters();
            logic.StartGame();
            LivesTB();
            LivesLeft = 3;
            Characters[0].lives = 3;
        }

        private void PauseGameButton()
        {
            Button btnPause = new Button();
            btnPause.Content = "Pause";
            btnPause.Height = 50;
            btnPause.Width = 180;
            btnPause.FontSize = 30;
            btnPause.Background = new SolidColorBrush(Colors.HotPink);
            btnPause.Foreground = new SolidColorBrush(Colors.Black);
            Canvas.SetLeft(btnPause, 330);
            Canvas.SetTop(btnPause, 700);
            canvas.Children.Add(btnPause);
            btnPause.Click += BtnPause_Click;
        }

        private void BtnPause_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            logic.Pause();
        }

        private void ResumeButton()
        {
            Button btnResume = new Button();
            btnResume.Content = "Resume";
            btnResume.Height = 50;
            btnResume.Width = 180;
            btnResume.FontSize = 30;
            btnResume.Background = new SolidColorBrush(Colors.HotPink);
            btnResume.Foreground = new SolidColorBrush(Colors.Black);
            Canvas.SetLeft(btnResume, 630);
            Canvas.SetTop(btnResume, 700);
            canvas.Children.Add(btnResume);
            btnResume.Click += BtnResume_Click;
        }

        private void BtnResume_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            logic.Resume();
        }

        private void SaveButton()
        {
            Button btnSave = new Button();
            btnSave.Content = "Save";
            btnSave.Height = 50;
            btnSave.Width = 180;
            btnSave.FontSize = 30;
            btnSave.Background = new SolidColorBrush(Colors.HotPink);
            btnSave.Foreground = new SolidColorBrush(Colors.Black);
            Canvas.SetLeft(btnSave, 930);
            Canvas.SetTop(btnSave, 700);
            canvas.Children.Add(btnSave);
            btnSave.Click += BtnSave_Click;
        }

        private void BtnSave_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            SaveCharacterPosition();
        }

        private void SaveCharacterPosition() // save character positions for save game
        {
            savedCharacters[0] = new Mom("ms-appx:///Assets/mom.png", 15, Characters[0].PosX, Characters[0].PosY);
            savedCharacters[0].lives = Characters[0].lives;
            for (int i = 1; i < Characters.Length; i++)
            {
                savedCharacters[i] = new Baby(Characters[i].ImageUrl, 3, Characters[i].PosX, Characters[i].PosY);
            }
            int currentLives = LivesLeft;
            FingerPrint[] savedFingers = new FingerPrint[fingerPrints.Length];
            this.savedFingers = savedFingers;
            for (int i = 0; i < savedFingers.Length; i++)
            {
                if (fingerPrints[i] == null)
                {
                    savedFingers[i] = null;
                }
                else
                {
                    savedFingers[i] = fingerPrints[i];
                    savedFingers[i].ImageUrl = fingerPrints[i].ImageUrl;
                    savedFingers[i].Image = fingerPrints[i].Image;
                    savedFingers[i].PosX = fingerPrints[i].PosX;
                    savedFingers[i].PosY = fingerPrints[i].PosY;
                }
            }
        }

        private void LoadButton()
        {
            Button btnLoad = new Button();
            btnLoad.Content = "Load";
            btnLoad.Height = 50;
            btnLoad.Width = 180;
            btnLoad.FontSize = 30;
            btnLoad.Background = new SolidColorBrush(Colors.HotPink);
            btnLoad.Foreground = new SolidColorBrush(Colors.Black);
            Canvas.SetLeft(btnLoad, 1230);
            Canvas.SetTop(btnLoad, 700);
            canvas.Children.Add(btnLoad);
            btnLoad.Click += BtnLoad_Click;
        }

        private void BtnLoad_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            LoadGame();
        }

        private void LoadGame() 
        {
            ClearBoard();
            logic.StopGame();
            Characters[0] = new Mom("ms-appx:///Assets/mom.png", 15, savedCharacters[0].PosX, savedCharacters[0].PosY);
            Characters[0].lives = savedCharacters[0].lives;
            canvas.Children.Add(Characters[0].Image);
            for (int i = 1; i < Characters.Length; i++)
            {
                Characters[i] = new Baby(savedCharacters[i].ImageUrl, 3, savedCharacters[i].PosX, savedCharacters[i].PosY);
                canvas.Children.Add(Characters[i].Image);
            }
            LivesLeft = currentLives;
            for (int i = 0; i < fingerPrints.Length; i++)
            {
                if (savedFingers[i] == null)
                {
                    fingerPrints[i] = null;
                }
                else
                {
                    fingerPrints[i] = savedFingers[i];
                    fingerPrints[i].ImageUrl = savedFingers[i].ImageUrl;
                    fingerPrints[i].Image = savedFingers[i].Image;
                    fingerPrints[i].PosX = savedFingers[i].PosX;
                    fingerPrints[i].PosY = savedFingers[i].PosY;
                    canvas.Children.Add(fingerPrints[i].Image);
                }
            }
            AddButtons();
            LivesTB();
            logic.StartGame();
        }
    }
}

