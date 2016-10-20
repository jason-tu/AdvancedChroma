using Corale.Colore.Core;
using Corale.Colore.Razer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ColoreColor = Corale.Colore.Core.Color;

namespace AdvancedChroma
{
    class Starlight
    {

        public void TransitionStarlight(ColoreColor first, ColoreColor second, bool back, int rest, int duration)
        {
            Random rand = new Random();
            int randRow = rand.Next(0, Constants.MaxRows);
            int randCol = rand.Next(0, Constants.MaxColumns);
            int randRow1 = rand.Next(0, Constants.MaxRows);
            int randCol1 = rand.Next(0, Constants.MaxColumns);

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
                Console.Write(randRow + "|" + randCol + "|" + Constants.MaxRows + "|" + Constants.MaxColumns + "\n");
                Chroma.Instance.Keyboard[randRow, randCol] =
                            new ColoreColor((byte)tempRed, (byte)tempGreen, (byte)tempBlue);
                Chroma.Instance.Keyboard[randRow1, randCol1] =
                            new ColoreColor((byte)tempRed, (byte)tempGreen, (byte)tempBlue);

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

                Chroma.Instance.Keyboard[randRow, randCol] =
                     new ColoreColor((byte)tempRed, (byte)tempGreen, (byte)tempBlue);
                Chroma.Instance.Keyboard[randRow1, randCol1] =
                            new ColoreColor((byte)tempRed, (byte)tempGreen, (byte)tempBlue);
                Thread.Sleep(sleepTime);
            }

        }
    }
}
