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
using Gma.System.MouseKeyHook;

namespace AdvancedChroma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread runningEffect;
        private IKeyboardMouseEvents m_GlobalHook;
        Boolean isRunning = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                runningEffect.Abort();
                Unsubscribe();
            }
            catch (NullReferenceException e1)
            {
                // First time.
            }
            if (((Button)sender).Name == "starlight")
            {
                runningEffect = new Thread(Starlight);
            }
            else if (((Button)sender).Name == "reactive")
            {
                Subscribe();
            }
            try
            {
                Thread.CurrentThread.IsBackground = true;
                runningEffect.Start();
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
                        Chroma.Instance.Keyboard[rand.Next(0, Constants.MaxRows), rand.Next(0, Constants.MaxColumns)] = new ColoreColor(rand.Next(1, 255), rand.Next(1, 255), rand.Next(1, 255));
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
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
        }

        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            if (!isRunning)
            {
                ThreadPool.QueueUserWorkItem(_ => React());
            }
        }

        public void Unsubscribe()
        {
            m_GlobalHook.MouseDownExt -= GlobalHookMouseDownExt;

            //It is recommened to dispose it
            m_GlobalHook.Dispose();
        }

        private void React()
        {
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

            double redStep = (defaultRed - newTargetRed) / 255;
            double greenStep = (defaultGreen - newTargetGreen) / 255;
            double blueStep = (defaultBlue - newTargetBlue) / 255;

            double displayedRed = defaultRed;
            double displayedGreen = defaultGreen;
            double displayedBlue = defaultBlue;
            for (int i = 0; i < 255; i++)
            {
                displayedRed -=  redStep;
                displayedGreen -=  greenStep;
                displayedBlue -= blueStep;

                if (defaultRed < newTargetRed && displayedRed > newTargetRed) displayedRed = newTargetRed;
                if (defaultGreen < newTargetGreen && displayedGreen > newTargetGreen) displayedGreen = newTargetGreen;
                if (defaultBlue < newTargetBlue && displayedBlue > newTargetBlue) displayedBlue = newTargetBlue;
                if (defaultRed > newTargetRed && displayedRed < newTargetRed) displayedRed = newTargetRed;
                if (defaultGreen > newTargetGreen && displayedGreen < newTargetGreen) displayedGreen = newTargetGreen;
                if (defaultBlue > newTargetBlue && displayedBlue < newTargetBlue) displayedBlue = newTargetBlue;

                Chroma.Instance.Keyboard.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));
                Chroma.Instance.Headset.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));
                Chroma.Instance.Mousepad.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));
                Chroma.Instance.Keypad.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));
                Chroma.Instance.Mouse.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));

                Thread.Sleep(1);
            }
            Thread.Sleep(500);
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

                Chroma.Instance.Keyboard.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));
                Chroma.Instance.Headset.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));
                Chroma.Instance.Mousepad.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));
                Chroma.Instance.Keypad.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));
                Chroma.Instance.Mouse.SetAll(new ColoreColor(displayedRed, displayedGreen, displayedBlue));

                Thread.Sleep(1);
            }
            isRunning = false;
        }
    }
}
