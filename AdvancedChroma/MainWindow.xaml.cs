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

namespace AdvancedChroma
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Thread runningEffect;
        Boolean reactiveEnabled = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            try {
                runningEffect.Abort();
                reactiveEnabled = false;
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
                reactiveEnabled = true;
            }
            try {
                Thread.CurrentThread.IsBackground = true;
                runningEffect.Start();
            } catch (Exception e1)
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

        private void mouseClick(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (reactiveEnabled)
                {
                    // Currently clicking is only limited to the Window. Need to find a way to get click even if outside window.
                    Chroma.Instance.Keyboard.SetAll(new ColoreColor(0, 255, 0));
                    int i1 = 255;
                    for (int i = 255; i > 0; i--)
                    {
                        Chroma.Instance.Keyboard.SetAll(new ColoreColor(i, i1, 0));
                        Thread.Sleep(1);
                        i1++;
                    }
                    i1 = 0;
                    for (int i = 255; i > 0; i--)
                    {
                        Chroma.Instance.Keyboard.SetAll(new ColoreColor(i1, i, 0));
                        Thread.Sleep(1);
                        i1++;
                    }
                }
            }
        }
    }
}
