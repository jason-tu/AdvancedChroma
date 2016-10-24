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
        public double stepDivisor = 255;

        public bool fadingOut = false;
        

        public StarlightKey(ColoreColor first, ColoreColor second, int keyX, int keyY, Random rand)
        {
            this.first = first;
            this.second = second;

            stepDivisor = rand.Next(80, 160);

            redStep = ((double)first.R - (double)second.R) / stepDivisor;
            greenStep = ((double)first.G - (double)second.G) / stepDivisor;
            blueStep = ((double)first.B - (double)second.B) / stepDivisor;

            this.keyX = keyX;
            this.keyY = keyY;
        }

        public void reset(Random rand)
        {

            this.keyX = rand.Next(0, Constants.MaxRows);
            this.keyY = rand.Next(0, Constants.MaxColumns);

            stepDivisor = rand.Next(80, 160);
            fadingOut = false;
        }
        
    }
}
