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
        public ColoreColor first;
        public ColoreColor second;
        public double redStep;
        public double greenStep;
        public double blueStep;

        public double currRed;
        public double currGreen;
        public double currBlue;

        public int keyX;
        public int keyY;

        public int step = 0;

        public bool fadingOut = false;

        public StarlightKey(ColoreColor first, ColoreColor second, int keyX, int keyY)
        {
            this.first = first;
            this.second = second;
            redStep = ((double)first.R - (double)second.R) / 255;
            greenStep = ((double)first.G - (double)second.G) / 255;
            blueStep = ((double)first.B - (double)second.B) / 255;
        }

        public void reset()
        {
            Random rand = new Random();
            keyX = rand.Next(0, Constants.MaxRows);
            keyY = rand.Next(0, Constants.MaxColumns);
        }
        
    }
}
