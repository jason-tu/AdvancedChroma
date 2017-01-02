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
using System.Collections;
using System.ComponentModel;

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
        static ColoreColor defaultColor;
        static ColoreColor targetColor;
        int restReactive;
        int durationReactive;
        public static List<StarlightKey> starlightKeys;
        static int numberOfKeys;
        static int starlightDuration;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += OnWindowClosing;
            Chroma.Instance.Initialize();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            _runningEffect?.Abort();
            try
            {
                Unsubscribe();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
            }
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
                defaultColor = new ColoreColor(Convert.ToByte(defaultColorRedStarlight.Text), Convert.ToByte(defaultColorGreenStarlight.Text), Convert.ToByte(defaultColorBlueStarlight.Text));
                targetColor = new ColoreColor(Convert.ToByte(starColorRedStarlight.Text), Convert.ToByte(starColorGreenStarlight.Text), Convert.ToByte(starColorBlueStarlight.Text));
                numberOfKeys = Convert.ToInt32(starlightNumberOfKeys.Text);
                starlightDuration = Convert.ToInt32(starlightDurationBox.Text);
                _runningEffect = new Thread(Starlight);
            }
            else if (((Button)sender).Name == "reactive")
            {
                defaultColor = new ColoreColor(Convert.ToByte(defaultColorRedReactive.Text), Convert.ToByte(defaultColorGreenReactive.Text), Convert.ToByte(defaultColorBlueReactive.Text));
                targetColor = new ColoreColor(Convert.ToByte(targetColorRedReactive.Text), Convert.ToByte(targetColorGreenReactive.Text), Convert.ToByte(targetColorBlueReactive.Text));
                restReactive = (int)(Convert.ToDouble(restTextBoxReactive.Text) * 1000);
                // TODO: Add in some sort of gate... Rapid clicking has the below method causing an exception. Corale.Colore.Razer.NativeCallException
                Chroma.Instance.SetAll(defaultColor);
                durationReactive = (int)(Convert.ToDouble(durationTextBoxReactive.Text) * 1000);
                
                Subscribe();
            }
            try
            {
                Thread.CurrentThread.IsBackground = true;
                _runningEffect.Start();
            }
            catch
            {
            }
        }

        public static void Starlight()
        {
            Random rand = new Random();
           // Chroma.Instance.Initialize();
            Chroma.Instance.Keyboard.SetAll(defaultColor);

            int maxKeys = numberOfKeys;

            starlightKeys = new List<StarlightKey>();
            for (int i = 0; i < maxKeys; i++)
            {
                StarlightKey toAdd = new StarlightKey(defaultColor, targetColor, rand.Next(0, Constants.MaxRows), rand.Next(0, Constants.MaxColumns));
                while (true)
                {
                    if (starlightKeys.Contains(toAdd)) toAdd = new StarlightKey(defaultColor, targetColor, rand.Next(0, Constants.MaxRows), rand.Next(0, Constants.MaxColumns));
                    else starlightKeys.Add(toAdd); break;
                }
            }

            while (true)
            {
                foreach (StarlightKey sk in starlightKeys)
                {
                    TransitionStarlight(sk);
                }
               Thread.Sleep(starlightDuration);
            }
        }

        private static void TransitionStarlight(StarlightKey sk)
        {
            //Transition to second color
            if (!sk.fadingOut)
            {

                sk.currRed -= sk.redStep;
                sk.currGreen -= sk.greenStep;
                sk.currBlue -= sk.blueStep;

                sk.currRed = checkRange(sk.currRed);
                sk.currGreen = checkRange(sk.currGreen);
                sk.currBlue = checkRange(sk.currBlue);

                Chroma.Instance.Keyboard[(int)sk.keyX, (int)sk.keyY] =
                            new ColoreColor((byte)sk.currRed, (byte)sk.currGreen, (byte)sk.currBlue);

                sk.step++;
                if (sk.step == sk.stepDivisor)
                {
                    Chroma.Instance.Keyboard[(int)sk.keyX, (int)sk.keyY] = sk.second;

                    sk.fadingOut = true;
                }

            }

            else // Is fading out.
            {
                sk.currRed += sk.redStep;
                sk.currGreen += sk.greenStep;
                sk.currBlue += sk.blueStep;

                sk.currRed = checkRange(sk.currRed);
                sk.currGreen = checkRange(sk.currGreen);
                sk.currBlue = checkRange(sk.currBlue);

                Chroma.Instance.Keyboard[(int)sk.keyX, (int)sk.keyY] =
                            new ColoreColor((byte)sk.currRed, (byte)sk.currGreen, (byte)sk.currBlue);

                sk.step--;
                if (sk.step == 0)
                {
                    Chroma.Instance.Keyboard[(int)sk.keyX, (int)sk.keyY] = sk.first;
                    sk.reset();
                }

            }

        }

        public static double checkRange(double color)
        {
            if (color < 0) return 0;
            else if (color > 255) return 255;
            else return color;
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

            Transition(defaultColor, targetColor, true, restReactive, durationReactive);

            _isRunning = false;
        }

        private static void Transition(ColoreColor first, ColoreColor second, bool back, int rest, int duration)
        {
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

