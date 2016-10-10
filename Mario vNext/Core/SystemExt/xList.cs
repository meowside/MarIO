﻿using Mario_vNext.Core.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Mario_vNext.Core.SystemExt
{
    class xList<T> : List<T>, ICore
    {
        public xList() { }

        public xList(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                this.Add(item);
            }
        }

        public void AddAll(params T[] stuff)
        {
            foreach (var item in stuff)
            {
                this.Add(item);
            }
        }

        public object DeepCopy()
        {
            xList<T> retValue = new xList<T>();

            foreach (T item in this.ToList())
            {
                retValue.Add((T)(item as ICore).DeepCopy());
            }

            return (xList<T>)retValue;
        }

        public void Render(int x, int y, byte[] imageBuffer, bool[] imageBufferKey)
        {
            var temp = this.ToList().Where(item => Finder((I3Dimensional)item, x, y)).Select(item => item).ToList();

            while (true)
            {
                if (temp.Count == 0) return;

                int tempHeight = temp.Max(item => ((I3Dimensional)item).Z);
                var toRender = temp.Where(item => ((I3Dimensional)item).Z == tempHeight).Select(item => item).ToList();
                
                foreach (ICore item in toRender)
                {
                    item.Render(x, y, imageBuffer, imageBufferKey);
                }

                temp.RemoveAll(item => toRender.FirstOrDefault(item2 => ReferenceEquals(item, item2)) != null);
            }
        }

        private bool Finder(I3Dimensional obj, int x, int y)
        {
            return (obj.X + obj.width >= x && obj.X < x + Shared.RenderWidth && obj.Y + obj.height >= y && obj.Y < y + Shared.RenderHeight);
        }

        private bool FindBiggerZ(I3Dimensional item1, I3Dimensional item2)
        {
            return (item1.Z > item2.Z);
        }
    }
}