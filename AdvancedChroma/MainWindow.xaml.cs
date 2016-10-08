using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Corale.Colore.Core;
using Corale.Colore.Razer.Keyboard;
using ColoreColor = Corale.Colore.Core.Color;
using System.Threading;
using Corale.Colore.Razer.Keyboard.Effects;
using Gma.System.MouseKeyHook;
using Duration = Corale.Colore.Razer.Keyboard.Effects.Duration;

namespace AdvancedChroma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread _runningEffect;
        private IKeyboardMouseEvents _mGlobalHook;
        Boolean _isRunning = false;

        public MainWindow()
        {
            InitializeComponent();
            Chroma.Instance.Initialize();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _runningEffect?.Abort();
                Unsubscribe();



            }
            catch
            {

            }

            if (((Button)sender).Name == "starlight")
            {
                _runningEffect = new Thread(Starlight);
            }
            else if (((Button)sender).Name == "reactive")
            {
                Subscribe();
            }
            try
            {
                Thread.CurrentThread.IsBackground = true;
                _runningEffect.Start();
            }
            catch (Exception e1)
            {

            }
        }

        public static void Starlight()
        {

            


            Random rand = new Random();
            while (true)
            {
                // Select random key, set it to random color etc.
                for (int i = 0; i < rand.Next(1, 5); i++)
                {
                    try
                    {
                        Chroma.Instance.Keyboard[rand.Next(0, Constants.MaxRows), rand.Next(0, Constants.MaxColumns)] =
                            new ColoreColor(rand.Next(1, 255), rand.Next(1, 255), rand.Next(1, 255));
                    }
                    catch (Exception e)
                    {

                    }
                }
                Thread.Sleep(500);

                //Keyboard.Instance.SetAll(new Color(0, 255, 0));
                Chroma.Instance.Keyboard.SetAll(new ColoreColor(0, 0, 0));
            }







        }

        public void Subscribe()
        {
            // Note: for the application hook, use the Hook.AppEvents() instead
            _mGlobalHook = Hook.GlobalEvents();
            _mGlobalHook.MouseDownExt += GlobalHookMouseDownExt;
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            if (!_isRunning)
            {
                ThreadPool.QueueUserWorkItem(_ => React());
            }
        }

        public void Unsubscribe()
        {
            _mGlobalHook.MouseDownExt -= GlobalHookMouseDownExt;

            //It is recommened to dispose it
            _mGlobalHook.Dispose();
        }

        private void React()
        {
            if (_isRunning) return;


            _isRunning = true;
            
            Transition(ColoreColor.Red, ColoreColor.Blue, true, 500, 1000);



            _isRunning = false;
            //Old code below

            /*
            Chroma.Instance.Initialize();
            double defaultRed = 0;
            double defaultGreen = 0;
            double defaultBlue = 255;

            double newTargetRed = 255;
            double newTargetGreen = 0;
            double newTargetBlue = 0;

            ColoreColor defaultColor = new ColoreColor(defaultRed, defaultGreen, defaultBlue);
            ColoreColor reactColor = new ColoreColor(newTargetRed, newTargetGreen, newTargetBlue);
            isRunning = true;
            Chroma.Instance.Keyboard.SetAll(defaultColor);
            Chroma.Instance.Headset.SetAll(defaultColor);
            Chroma.Instance.Mousepad.SetAll(defaultColor);
            Chroma.Instance.Keypad.SetAll(defaultColor);
            Chroma.Instance.Mouse.SetAll(defaultColor);

            double redStep = (defaultRed - newTargetRed)/255;
            double greenStep = (defaultGreen - newTargetGreen)/255;
            double blueStep = (defaultBlue - newTargetBlue)/255;

            double displayedRed = defaultRed;
            double displayedGreen = defaultGreen;
            double displayedBlue = defaultBlue;
            for (int i = 0; i < 255; i++)
            {
                displayedRed -= redStep;
                displayedGreen -= greenStep;
                displayedBlue -= blueStep;

                if (defaultRed < newTargetRed && displayedRed > newTargetRed) displayedRed = newTargetRed;
                if (defaultGreen < newTargetGreen && displayedGreen > newTargetGreen) displayedGreen = newTargetGreen;
                if (defaultBlue < newTargetBlue && displayedBlue > newTargetBlue) displayedBlue = newTargetBlue;
                if (defaultRed > newTargetRed && displayedRed < newTargetRed) displayedRed = newTargetRed;
                if (defaultGreen > newTargetGreen && displayedGreen < newTargetGreen) displayedGreen = newTargetGreen;
                if (defaultBlue > newTargetBlue && displayedBlue < newTargetBlue) displayedBlue = newTargetBlue;

                Chroma.Instance.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));


                Thread.Sleep(1);
            }
            Chroma.Instance.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));


            for (int i = 0; i < 255; i++)
            {
                displayedRed += redStep;
                displayedGreen += greenStep;
                displayedBlue += blueStep;

                if (defaultRed < newTargetRed && displayedRed < defaultRed) displayedRed = defaultRed;
                if (defaultGreen < newTargetGreen && displayedGreen < defaultGreen) displayedGreen = defaultGreen;
                if (defaultBlue < newTargetBlue && displayedBlue < defaultBlue) displayedBlue = defaultBlue;
                if (defaultRed > newTargetRed && displayedRed > defaultRed) displayedRed = defaultRed;
                if (defaultGreen > newTargetGreen && displayedGreen > defaultGreen) displayedGreen = defaultGreen;
                if (defaultBlue > newTargetBlue && displayedBlue > defaultBlue) displayedBlue = defaultBlue;


                Chroma.Instance.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));

                Thread.Sleep(1);
            }
            isRunning = false;
            */
        }

        private static void Transition(ColoreColor first, ColoreColor second, bool back, int rest, int duration)
        {

            //Set first color



            //calculate
            var redStep = (first.R - second.R) / 255;
            var greenStep = (first.G - second.G) / 255;
            var blueStep = (first.B - second.B) / 255;
            int sleepTime = duration / 255;

            //set current color
            int tempRed = first.R;
            int tempGreen = first.G;
            int tempBlue = first.B;

            //Transition to second color
            for (int i = 0; i < 255; i++)
            {

                tempRed -= redStep;
                tempGreen -= greenStep;
                tempBlue -= blueStep;



                Chroma.Instance.SetAll(new ColoreColor((byte)tempRed, (byte)tempGreen, (byte)tempBlue));

                Thread.Sleep(sleepTime);


            }
            //Color rests
            
            Thread.Sleep(rest);

            //leave animation if bool is false
            if (!back)
            {

                return;
            }



            //Transition to first color



            for (int i = 0; i < 255; i++)
            {

                tempRed += redStep;
                tempGreen += greenStep;
                tempBlue += blueStep;

                Chroma.Instance.SetAll(new ColoreColor((byte)tempRed, (byte)tempGreen, (byte)tempBlue));

                Thread.Sleep(sleepTime);
            }

        }


    }
}
