using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColoreColor = Corale.Colore.Core.Color;
using System.Threading;
using Corale.Colore.Razer.Keyboard.Effects;
using Corale.Colore.Core;
using Corale.Colore.Razer.Keyboard;

namespace AdvancedChroma
{
    class StarlightKey
    {
        ColoreColor first;
        ColoreColor second;
        double redStep;
        double greenStep;
        double blueStep;

        double currRed;
        double currGreen;
        double currBlue;

        int keyX;
        int keyY;

        bool fadingOut = false;

        int stepsTaken = 0;

        public StarlightKey(ColoreColor first, ColoreColor second, int keyX, int keyY)
        {
            this.first = first;
            this.second = second;
            redStep = ((double)first.R - (double)second.R) / 255;
            greenStep = ((double)first.G - (double)second.G) / 255;
            blueStep = ((double)first.B - (double)second.B) / 255;

            Console.Write(blueStep);

            currRed = first.R;
            currGreen = first.G;
            currBlue = first.B;
        }

        public void work1()
        {
            if (!fadingOut)
            {

                currRed -= redStep;
                currGreen -= greenStep;
                currBlue -= blueStep;

                Chroma.Instance.Keyboard[(int)keyX, (int)keyY] =
                                new ColoreColor((byte)currRed, (byte)currGreen, (byte)currBlue);

                stepsTaken++;

                if (stepsTaken == 255)
                {
                    fadingOut = true;
                }
                fadingOut = true;
            }
            else
            {
                    // TODO: reset this instance of the Key.
                    fadingOut = false;
                    Random rand = new Random();
                    keyX = rand.Next(0, Constants.MaxRows);
                    keyY = rand.Next(0, Constants.MaxColumns);
            }
        }

        public void work()
        {
            if (!fadingOut)
            {
                currRed -= redStep;
                currGreen -= greenStep;
                currBlue -= blueStep;

                Chroma.Instance.Keyboard[(int)keyX, (int)keyY] =
                                new ColoreColor((byte)currRed, (byte)currGreen, (byte)currBlue);

                stepsTaken++;

                if (stepsTaken == 255)
                {
                    fadingOut = true;
                }
            }
            else
            {
                currRed += redStep;
                currGreen += greenStep;
                currBlue += blueStep;

                Chroma.Instance.Keyboard[(int)keyX, (int)keyY] =
                                new ColoreColor((byte)currRed, (byte)currGreen, (byte)currBlue);

                stepsTaken--;

                if (stepsTaken == 0)
                {
                    // TODO: reset this instance of the Key.
                    fadingOut = false;
                    Random rand = new Random();
                    keyX = rand.Next(0, Constants.MaxRows);
                    keyY = rand.Next(0, Constants.MaxColumns);
                }
            }
        }
    }
}
