﻿using Mario.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mario.Data.Objects
{
    class Core_objects : ICore, ICoordinated, I2Dimensional, IGraphics
    {
        public string name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }
        public int width { get; set; }
        public int height { get; set; }

        public int AnimationState { get; set; }

        public int jumpheight = 0;     // Number of Blocks
        public int jumplength = 0;     // In ms (miliseconds)

        public Material mesh { get; set; }
        public Collider collider = new Collider();

        public object Copy()
        {
            return this.MemberwiseClone();
        }

        public void AddTo(List<object> destination)
        {
            destination.Add(this);
        }

        public object DeepCopy()
        {
            Core_objects retValue = (Core_objects)this.MemberwiseClone();

            retValue.mesh = (Material)this.mesh.DeepCopy();

            return retValue;
        }

        public void Render(int x, int y)
        {
            //mesh.Render(destination, destinationColor, frameWidth, frameHeight, this.X - x, this.Y - y);
        }

        Color ICore.Render(int x, int y)
        {
            throw new NotImplementedException();
        }
    }
}
