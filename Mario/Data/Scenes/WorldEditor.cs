﻿using Mario.Core;
using Mario.Data.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Mario.Data.Scenes
{
    class WorldEditor
    {
        World map;
        Block _newBlock = new Block();

        xList<xList<Block>> undo = new xList<xList<Block>>();

        TextBlock posX = new TextBlock(1, 1);
        TextBlock posY = new TextBlock(1, 7);
        TextBlock posZ = new TextBlock(1, 13);

        Camera cam = new Camera();
        
        private int Z = 1;
        private int selected = 0;

        bool EnterPressed = false;
        bool Changed = false;

        BaseHiararchy core = new BaseHiararchy();

        public void Start()
        {
            Console.WriteLine("1) NEW");
            Console.WriteLine("2) LOAD");

            switch (int.Parse(Console.ReadLine()))
            {
                case 1:
                    New();
                    break;

                case 2:
                    Load();
                    break;
            }
        }

        private void New()
        {
            Console.Write("Width (number of blocks):  ");
            int w = int.Parse(Console.ReadLine());

            Console.Write("Height (number of blocks):  ");
            int h = int.Parse(Console.ReadLine());

            Console.Write("World Name:  ");
            string n = Console.ReadLine();

            EnterPressed = true;

            map = new World(w * 16, h * 16, n);

            Init();
        }

        private void Load()
        {
            string[] files = Directory.GetFiles(Environment.CurrentDirectory + @"\Data\Worlds");

            Console.WriteLine(string.Format("\n\n"));

            if (files.Length != 0)
            {
                int count = 0;

                foreach (string s in files)
                {
                    Console.WriteLine(string.Format("{0}) {1}", count++, s));
                }
                
                int select = int.Parse(Console.ReadLine());

                map = WorldLoader.Load(files[select]);

                Init();
            }

            else
            {
                Console.WriteLine("NO WORLDS FOUND, PRESS ENTER TO CONTINUE");
                Console.ReadLine();

                New();
            }
        }

        private void Init()
        {
            _newBlock.Init(selected);

            posX.Text("X " + _newBlock.X.ToString());
            posY.Text("Y " + _newBlock.Y.ToString());
            posZ.Text("Z " + Z.ToString());
            
            //core.background = map.background;
            //core.middleground.Add(map.middleground);
            //core.foreground.Add(map.foreground);
            
            core.UI.Add(posX);
            core.UI.Add(posY);
            core.UI.Add(posZ);

            xRectangle border = new xRectangle(-8, -8, map.width, map.height);

            //core.exclusive.Add(border);

            //core.exclusive.Add(_newBlock);
            
            cam.Init(core, -(Camera.RENDER_WIDTH / 2 - 8), -(Camera.RENDER_HEIGHT / 2 - 8));

            Thread keychecker = new Thread(() => KeyPress());
            keychecker.Start();
        }

        private void LayerSort(List<Block> layer)
        {
            for(int i = 0; i < layer.Count - 1; i++)
            {
                for(int j = 0; j < layer.Count - 1; j++)
                {
                    if(layer[j].X > layer[j + 1].X)
                    {

                        Block temp = (Block)layer[j].DeepCopy();
                        layer[j] = (Block)layer[j + 1].DeepCopy();
                        layer[j + 1] = (Block)temp.DeepCopy();
                    }
                }
            }

            for (int i = 0; i < layer.Count - 1; i++)
            {
                for (int j = 0; j < layer.Count - 1; j++)
                {
                    if(layer[j].X == layer[j + 1].X)
                        if (layer[j].Y > layer[j + 1].Y)
                        {
                            Block temp = (Block)layer[j].DeepCopy();
                            layer[j] = (Block)layer[j + 1].DeepCopy();
                            layer[j + 1] = (Block)temp.DeepCopy();
                        }
                }
            }
        }

        private void BlockFinder(List<Block> layer, int X, int Y, int Z)
        {
            Block temp = (Block)layer.Find(b => b.X == X && b.Y == Y && b.Z == Z);

            if (temp != null) layer.Remove(temp);
        }

        private void Fill(List<Block> layer)
        {
            int Xoffset = 0;
            int Yoffset = 0;
            int type = _newBlock.Type;

            while(true)
            {
                BlockFinder(layer, (int)Xoffset, (int)Yoffset, Z);
                layer.Add(new Block(Xoffset, Yoffset, type));
                Yoffset += _newBlock.mesh.height;

                if (Xoffset + _newBlock.mesh.width >= map.width && Yoffset + _newBlock.mesh.height > map.height)
                {
                    return;
                }

                if (Yoffset + _newBlock.mesh.height > map.height)
                {
                    Yoffset = 0;
                    Xoffset += _newBlock.mesh.width;
                }
            }
        }

        private bool Save(World w)
        {
            BinaryWriter bw;

            try
            {
                bw = new BinaryWriter(new FileStream("Data\\Worlds\\" + map.Level + ".WORLD", FileMode.Create));
            }
            catch (IOException e)
            {
                MessageBox.Show(e.Message + "\n Cannot create file.");
                return false;
            }

            try
            {
                bw.Write(w.Level);
                bw.Write(w.width);
                bw.Write(w.height);

                bw.Write(w.PlayerSpawnX);
                bw.Write(w.PlayerSpawnY);

                bw.Write(w.model.Count);

                foreach (Block item in w.model)
                {
                    bw.Write((int)item.X);
                    bw.Write((int)item.Y);
                    bw.Write((int)item.Z);
                    bw.Write(item.Type);
                }
            }

            catch (IOException e)
            {
                MessageBox.Show(e.Message + "\n Cannot write to file.");
                return false;
            }

            bw.Close();

            return true;
        }

        //
        //  Controls
        //

        [DllImport("user32.dll")]
        public static extern ushort GetKeyState(short nVirtKey);

        public const ushort keyDownBit = 0x80;

        public bool IsKeyPressed(ConsoleKey key)
        {
            return ((GetKeyState((short)key) & keyDownBit) == keyDownBit);
        }
        
        private void KeyPress()
        {
            
            while (true)
            {
                Thread.Sleep(100);
                

                if (IsKeyPressed(ConsoleKey.W))
                {
                    if(_newBlock.Y > 0)
                    {
                        _newBlock.Y -= 16;
                        posY.Text("Y " + _newBlock.Y.ToString());

                        cam.Yoffset -= 16;
                    }
                }


                if (IsKeyPressed(ConsoleKey.A))
                {
                    if(_newBlock.X > 0)
                    {
                        _newBlock.X -= 16;
                        posX.Text("X " + _newBlock.X.ToString());
                        
                        cam.Xoffset -= 16;
                    }
                }

                if (IsKeyPressed(ConsoleKey.D))
                {
                    if(_newBlock.X + _newBlock.mesh.width < map.width)
                    {
                        _newBlock.X += 16;
                        posX.Text("X " + _newBlock.X.ToString());
                        
                        cam.Xoffset += 16;
                    }
                }

                if (IsKeyPressed(ConsoleKey.S))
                {
                    if(_newBlock.Y + _newBlock.mesh.height < map.height)
                    {
                        _newBlock.Y += 16;
                        posY.Text("Y " + _newBlock.Y.ToString());

                        cam.Yoffset += 16;
                    }
                }

                if (IsKeyPressed(ConsoleKey.Enter))
                {

                    if (!EnterPressed)
                    {
                        undo.Add((xList<Block>)map.model.DeepCopy());

                        Block temp = new Block();

                        temp = (Block)_newBlock.DeepCopy();
                        
                        BlockFinder(map.model, (int)_newBlock.X, (int)_newBlock.Y, (int)_newBlock.Z);
                        map.model.Add(temp);
                        LayerSort(map.model);

                        EnterPressed = true;
                        
                        _newBlock.Init(selected);
                    }
                }

                else
                {
                    EnterPressed = false;
                }

                if (IsKeyPressed(ConsoleKey.Delete) || IsKeyPressed(ConsoleKey.Backspace))
                {
                    undo.Add((xList<Block>)map.model.DeepCopy());
                    
                    BlockFinder(map.model, (int)_newBlock.X, (int)_newBlock.Y, (int)_newBlock.Z);
                }

                if (IsKeyPressed(ConsoleKey.Q))
                {
                    if (selected > 0 && !Changed)
                    {
                        selected--;
                        _newBlock.Init(selected);
                        Changed = true;
                    }
                }

                else if (IsKeyPressed(ConsoleKey.E))
                {
                    if (selected < 21 && !Changed)
                    {
                        selected++;
                        _newBlock.Init(selected);
                        Changed = true;
                    }
                }

                else if (IsKeyPressed(ConsoleKey.PageUp))
                {
                    if (Z < 2 && !Changed)
                    {
                        Z++;
                        posZ.Text("Z " + Z.ToString());
                        Changed = true;
                    }
                }

                else if (IsKeyPressed(ConsoleKey.PageDown))
                {
                    if (Z > 0 && !Changed)
                    {
                        Z--;
                        posZ.Text("Z " + Z.ToString());
                        Changed = true;
                    }
                }

                else if (IsKeyPressed(ConsoleKey.F))
                {
                    undo.Add((xList<Block>)map.model.DeepCopy());
                    
                    Fill(map.model);
                }

                else if (IsKeyPressed(ConsoleKey.End))
                {
                    World temp = new World();
                    temp = (World)map.DeepCopy();

                    Save(temp);
                }

                else if (IsKeyPressed(ConsoleKey.Z))
                {
                    if(undo.Count > 0)
                    {
                        map.model = (xList<Block>)undo[undo.Count - 1].DeepCopy();
                        
                        undo.Remove(undo[undo.Count - 1]);
                    }
                }

                else Changed = false;
            }
        }
    }
}
