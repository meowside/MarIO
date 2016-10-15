﻿using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace Mario_vNext.Core
{
    static class Shared
    {
        public static int RenderWidth = 640;
        public static int RenderHeight = 360;

        public static bool IsEscapeSequence(char letter)
        {
            if (letter == '\0')
                return true;

            if (letter == '\a')
                return true;

            if (letter == '\b')
                return true;

            if (letter == '\f')
                return true;

            if (letter == '\n')
                return true;

            if (letter == '\r')
                return true;

            if (letter == '\t')
                return true;

            if (letter == '\v')
                return true;

            if (letter == '\'')
                return true;

            if (letter == '\"')
                return true;

            return false;
        }

        public static PrivateFontCollection pfc = new PrivateFontCollection();

        public static void LoadFont()
        {
            int fontLength = Properties.Resources.Pixel_Millennium.Length;
            byte[] fontdata = Properties.Resources.Pixel_Millennium;
            System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);
            Marshal.Copy(fontdata, 0, data, fontLength);
            
            pfc.AddMemoryFont(data, fontLength);
        }
    }
}
