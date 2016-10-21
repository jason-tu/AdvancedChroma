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
        ColoreColor defaultColorReactive;
        ColoreColor targetColorReactive;
        int restReactive;
        int durationReactive;

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
                defaultColorReactive = new ColoreColor(Convert.ToByte(defaultColorRedReactive.Text), Convert.ToByte(defaultColorGreenReactive.Text), Convert.ToByte(defaultColorBlueReactive.Text));
                targetColorReactive = new ColoreColor(Convert.ToByte(targetColorRedReactive.Text), Convert.ToByte(targetColorGreenReactive.Text), Convert.ToByte(targetColorBlueReactive.Text));
                restReactive = (int)(Convert.ToDouble(restTextBoxReactive.Text) * 1000);
                Chroma.Instance.SetAll(defaultColorReactive);
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
            ColoreColor defaultColor = new ColoreColor((byte)0, (byte)0, (byte)0);
            //Chroma.Instance.Initialize();
            Chroma.Instance.Keyboard.SetAll(defaultColor);

            int maxKeys = 4;
            int minKeys = 1;

            int targetRed = 255;
            int targetGreen = 51;
            int targetBlue = 153;
            ColoreColor targetColor = new ColoreColor((byte)targetRed, (byte)targetGreen, (byte)targetBlue);

            List<StarlightKey> starlightKeys = new List<StarlightKey>();
            for (int i = minKeys; i < maxKeys; i++)
            {
                // TODO: Make sure no duplicate keys.
                starlightKeys.Add(new StarlightKey(defaultColor, targetColor, rand.Next(0, Constants.MaxRows), rand.Next(0, Constants.MaxColumns)));
            }

            while (true)
            {

                foreach (StarlightKey sk in starlightKeys)
                {
                    sk.work();
                    //Thread.Sleep(0);
                }

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

            Transition(defaultColorReactive, targetColorReactive, true, restReactive, durationReactive);

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

