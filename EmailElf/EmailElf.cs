using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailElf
{
    public class Elf
    {
        static EmailElfWindow _EEW = new EmailElfWindow();

        public static void Summon()
        {
            _EEW.Show();
        }

        public static void Withdraw()
        {
            _EEW.Hide();
        }

        public static void DTD()
        {
            if (_EEW is null) return;
            _EEW.Close();
            _EEW = null;
        }

    }
}
