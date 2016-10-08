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
                //Chroma.Instance.Uninitialize();
            }
            catch
            {

            }

            if (((Button)sender).Name == "starlight")
            {
                Chroma.Instance.Initialize();
                _runningEffect = new Thread(Starlight);
            }
            else if (((Button)sender).Name == "reactive")
            {
                Chroma.Instance.Initialize();
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
            catch (Exception e1)
            {
                Console.Write(e1);
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

            Transition(defaultColorReactive, targetColorReactive, true, restReactive, durationReactive);

            _isRunning = false;
        }

        private static void Transition(ColoreColor first, ColoreColor second, bool back, int rest, int duration)
        {
            try {             //calculate
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
            } catch (Corale.Colore.Razer.NativeCallException e1)
            {

            }
        }
    }
}
