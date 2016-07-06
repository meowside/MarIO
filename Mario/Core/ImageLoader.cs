﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mario.Core
{
    class ImageLoader
    {
        public static Image Load(ObjectDatabase.Object type)
        {
            Image img = null;

            try
            {
                img = Image.FromFile(Environment.CurrentDirectory + ObjectDatabase.path[(int)type]);
            }
            catch
            {
                MessageBox.Show(Error.ErrorHandle((int)type), string.Format("Error 0x{0:X3}", (int)type, MessageBoxButtons.OK, MessageBoxIcon.Error));
                Environment.Exit(0);
            }

            return img;
        }
    }
}