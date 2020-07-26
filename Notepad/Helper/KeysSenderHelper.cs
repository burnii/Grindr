using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Notepad
{
    public class KeysSenderHelper
    {
        private static Dictionary<char, Keys> KeysByChar { get; set; } = new Dictionary<char, Keys>()
        {
            {'a', Keys.A},
            {'b', Keys.B},
            {'c', Keys.C},
            {'d', Keys.D},
            {'e', Keys.E},
            {'f', Keys.F},
            {'g', Keys.G},
            {'h', Keys.H},
            {'i', Keys.I},
            {'j', Keys.J},
            {'k', Keys.K},
            {'l', Keys.L},
            {'m', Keys.M},
            {'n', Keys.N},
            {'o', Keys.O},
            {'p', Keys.P},
            {'q', Keys.Q},
            {'r', Keys.R},
            {'s', Keys.S},
            {'t', Keys.T},
            {'u', Keys.U},
            {'v', Keys.V},
            {'w', Keys.W},
            {'x', Keys.X},
            {'y', Keys.Y},
            {'z', Keys.Z},
            {'0', Keys.D0},
            {'1', Keys.D1},
            {'2', Keys.D2},
            {'3', Keys.D3},
            {'4', Keys.D4},
            {'5', Keys.D5},
            {'6', Keys.D6},
            {'7', Keys.D7},
            {'8', Keys.D8},
            {'9', Keys.D9},
            {'.', Keys.OemPeriod}

        };

        public static void SendString(InputController inputController, string value)
        {
            foreach (var c in value)
            {
          
                //if (KeysByChar.TryGetValue(c, out var key))
                //{
                //    inputController.TapKey(key);
                //}
                //else if (c == '@')
                //{
                //    inputController.PressKey(Keys.Alt);
                //    inputController.PressKey(Keys.Control);
                //    inputController.TapKey(Keys.E);
                //    inputController.ReleaseKey(Keys.Alt);
                //    inputController.ReleaseKey(Keys.Control);

                //}
                //else
                //{
                //    throw new Exception($"Could not find a valid key for '{c}'. Only a-z (lower case) and 0-9 are supported");
                //}
                //Thread.Sleep(1000);
            }
        }

    }
}
