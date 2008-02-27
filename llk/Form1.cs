using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace llk
{
    public partial class Form1 : Form
    {
        private int hozCount = 18; //总数16*9=144=12*12
        private int VerCount = 11;
        private int tileWidth = 31;
        private int tileHeight = 34;
        private bool _GameStart = false;
        private MapTitle _selTile = null;
        private int _imageCount=12;

        private int[,] map1 =
            {
                {-1,-1,-1,-1,-1},
                {-1,-1, 2, 2,-1},
                {-1, 1, 1, 1,-1},
                {-1, 2, 1, 2,-1},
                {-1,-1, 1,-1,-1},
                {-1,-1,-1,-1,-1}
            };

        private MapTitle[,] MapTiles = null;
        private int[] MapArray = null;

        private void RandomMapArray()
        {
            int count= (hozCount-2) * (VerCount-2);
            MapArray=new int[count];

            for (int i = 0; i < count; i++)
                MapArray[i] = i + 1;

            Random rnd = new Random((int)System.DateTime.Now.Millisecond);
            for (int i = count; i > 0; i--)
            {
                int j = rnd.Next(i);
                int temp = MapArray[j];
                MapArray[j] = MapArray[i - 1];
                MapArray[i - 1] = temp;
            }
        }


        private void InitTiles()
        {
            MapTiles = new MapTitle[VerCount, hozCount];

            //预留边界
            for (int i = 0; i < hozCount; i++)
            {
                MapTiles[0,i] = new MapTitle(0,i, -1);
                MapTiles[VerCount-1,i] = new MapTitle(VerCount-1,i, -1);
            }
            for (int j = 0; j < VerCount; j++)
            {
                MapTiles[j, 0] = new MapTitle(j, 0, -1);
                MapTiles[j, hozCount - 1] = new MapTitle(j, hozCount - 1, -1);
            }

            //初始化内部数据

            for (int i = 1; i < VerCount-1; i++)
                for (int j = 1; j < hozCount-1; j++)
                    MapTiles[i, j] = new MapTitle(i, j, MapArray[(i - 1) * (hozCount - 2) + (j - 1)] % _imageCount+1, OffsetY + (j - 1) * tileWidth,OffsetX + (i - 1) * tileHeight);

            ShowData();
        }

        private void ShowData()
        {
            if (listView1.Columns.Count == 0)
            {
                for (int i = 0; i < hozCount; i++)
                {
                    ColumnHeader _header = new ColumnHeader();
                    _header.Text = i.ToString();
                    _header.Width = 40;

                    listView1.Columns.Add(_header);
                }
            }

            listView1.Items.Clear();

            for (int i = 0; i < VerCount; i++)
            {
                ListViewItem item = new ListViewItem();
                int k=0;
                item.Text = MapTiles[i, k].ImageID.ToString();
                for (int j = 1; j < hozCount; j++)
                    item.SubItems.Add((MapTiles[i, j].ImageID).ToString());

                listView1.Items.Add(item);
            }
        }

        private void DrawImage()
        {
            for(int i=1;i<VerCount-1;i++)
                for (int j = 1; j < hozCount - 1; j++)
                {
                    MapTitle mt=MapTiles[i,j];
                    if (mt.ImageID != -1)
                    {
                        int imageId = mt.ImageID ;
                        string fileName = Application.StartupPath + @"/Image/" + imageId.ToString() + ".bmp";
                        Bitmap bmp = new Bitmap(fileName);

                        Graphics g = Graphics.FromHwnd(panel1.Handle);

                        g.DrawImage(bmp, mt.OffsetX, mt.OffsetY);
                    }
                }
        }

        private void HideTile(MapTitle tile)
        {
            Rectangle rect = new Rectangle(tile.OffsetX, tile.OffsetY, tileWidth, tileHeight);

            Graphics g = Graphics.FromHwnd(panel1.Handle);

            g.FillRectangle(new SolidBrush(Color.Black), rect);
        }

        private void DrawTile(MapTitle tile)
        {
            int imageId = tile.ImageID;
            string fileName = Application.StartupPath + @"/Image/" + imageId.ToString() + ".bmp";
            Bitmap bmp = new Bitmap(fileName);

            Graphics g = Graphics.FromHwnd(panel1.Handle);

            g.DrawImage(bmp, tile.OffsetX, tile.OffsetY);
        }

        private void InitGameData()
        {
            RandomMapArray();
            InitTiles();
        }
        private int _offsetX=20;

        public int OffsetX
        {
            get { return _offsetX; }
            set { _offsetX = value; }
        }

        private int _offsetY=40;

        public int OffsetY
        {
            get { return _offsetY; }
            set { _offsetY = value; }
        }


        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获得垂直或者水平方向的大点和小点
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="p1_temp">返回大点</param>
        /// <param name="p2_temp">返回小点</param>
        private void GetMaxMinPt(Point pt1, Point pt2, ref Point p1_temp, ref Point p2_temp)
        {
            if (pt1.Y == pt2.Y)
            {
                p1_temp.X = pt1.X > pt2.X ? pt1.X : pt2.X;
                p1_temp.Y = pt1.Y;

                p2_temp.Y = pt1.Y;
                p2_temp.X = pt1.X > pt2.X ? pt2.X : pt1.X;


            }
            else
                if (pt1.X == pt2.X)
                {
                    p1_temp.X = pt1.X;
                    p1_temp.Y = pt1.Y > pt2.Y ? pt1.Y : pt2.Y;

                    p2_temp.X = pt1.X;
                    p2_temp.Y = pt1.Y > pt2.Y ? pt2.Y : pt1.Y;
                }
        }


        //是否在一条直线上
        private bool IsSameLine(Point pt1, Point pt2)
        {
            return pt1.X == pt2.X || pt1.Y == pt2.Y;
        }


        //查看是否可以直接联通
        private bool CheckOneLine(Point pt1, Point pt2)
        {
            if (IsSameLine(pt1, pt2))
            {
                Point p1_temp = new Point();  //p1_temp存储右边或者下面的大点
                Point p2_temp = new Point();  //p2_temp存储左边或者上面的小点


                //纵向
                if (pt1.Y == pt2.Y)
                {
                    GetMaxMinPt(pt1, pt2, ref p1_temp, ref p2_temp);
                    //如果是直接相邻，直接联通
                    if (p1_temp.X - p2_temp.X == 1)
                        return true;
                    //如果中间相隔几个
                    for (int i = p2_temp.X + 1; i < p1_temp.X; i++)
                        if (MapTiles[i, p1_temp.Y].ImageID != -1)
                            return false;

                    return true; 
                }
                else
                {
                    GetMaxMinPt(pt1, pt2, ref p1_temp, ref p2_temp);


                    //判定直接相邻
                    if (p1_temp.Y - p2_temp.Y == 1)
                        return true;

                    //如果中间相隔几个
                    for (int i = p2_temp.Y + 1; i < p1_temp.Y; i++)
                    {
                        if (MapTiles[p2_temp.X, i].ImageID != -1)
                            return false;
                    }

                    return true;
                }
            }
            else
                return false;
        }


        //查看是否超出数组边界
        private bool isPointOutBorder(Point pt)
        {
            return pt.X == -1 || pt.Y == -1 || pt.X == VerCount || pt.Y == hozCount;
        }


        //获得两个点的相对位置
        private int GetDirection(Point pt1, Point pt2)
        {
            if (pt1.X < pt2.X)
            {
                if (pt1.Y < pt2.Y)
                    return 1;
                else
                    return 2;
            }
            else
            {
                if (pt1.Y < pt2.Y)
                    return 3;
                else
                    return 4;
            }
        }
        /// <summary>
        /// 版本1、只查询pt1在pt2的左上方的情况
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        private bool CheckOneCorner(Point pt1, Point pt2)
        {
            int Direction = 1; //1-左上方 2-左下方 3-右上方 4-右下方
            bool bLink = false;
            Direction = GetDirection(pt1, pt2);

            switch (Direction)
            {
                case 1:
                    bLink = CheckLeftTop(pt1, pt2);
                    break;
                case 2:
                    bLink = CheckLeftBottom(pt1, pt2);
                    break;
                case 3:
                    bLink = CheckRightTop(pt1, pt2);
                    break;
                case 4:
                    bLink = CheckRightBottom(pt1, pt2);
                    break;
                default:
                    bLink = false;
                    break;
            }

            return bLink;
        }

        private bool CheckRightBottom(Point pt1, Point pt2)
        {           
            return CheckLeftTop(pt2, pt1);

        }

        private bool CheckRightTop(Point pt1, Point pt2)
        {          
            return CheckLeftBottom(pt2, pt1);

        }

        private bool CheckLeftBottom(Point pt1, Point pt2)
        {           
            Point pt1_temp = new Point(pt2.X,pt1.Y);
            Point pt2_temp = new Point(pt1.X,pt2.Y);

            if ((CheckOneLine(pt1_temp, pt1)
                &&
                MapTiles[pt1_temp.X, pt1_temp.Y].ImageID == -1 
                && CheckOneLine(pt1_temp,pt2)
                )
                || 
                (CheckOneLine(pt2, pt2_temp)
                &&
                MapTiles[pt2_temp.X, pt2_temp.Y].ImageID == -1 
                && 
                CheckOneLine(pt2_temp,pt1)
                )
                )
                return true;
            else
                return false;
        }

        /// <summary>
        /// 查看是否可以可以单角联通
        /// </summary>
        /// <param name="pt1">左上方点</param>
        /// <param name="pt2">右下方点</param>
        /// <returns></returns>
        private bool CheckLeftTop(Point pt1, Point pt2)
        {           
            Point pt1_temp = new Point(pt1.X, pt2.Y);
            Point pt2_temp = new Point(pt2.X, pt1.Y);

            if ((CheckOneLine(pt1, pt1_temp) && MapTiles[pt1_temp.X, pt1_temp.Y].ImageID == -1 && CheckOneLine(pt1_temp,pt2) ) || (CheckOneLine(pt2_temp, pt2) && MapTiles[pt2_temp.X, pt2_temp.Y].ImageID == -1 && CheckOneLine(pt1,pt2_temp)))
                return true;
            else
                return false;
        }

        private bool CheckTwoCorner(Point pt1, Point pt2)
        {
            Point pt_temp = new Point();

            //左侧检查
            pt_temp.X = pt1.X - 1;
            pt_temp.Y = pt1.Y;

            while (!isPointOutBorder(pt_temp) && MapTiles[pt_temp.X,pt_temp.Y].ImageID==-1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.X -= 1;
            }

            //右侧检查
            pt_temp.X = pt1.X + 1;
            pt_temp.Y = pt1.Y;

            while (!isPointOutBorder(pt_temp) && MapTiles[pt_temp.X, pt_temp.Y].ImageID == -1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.X += 1;
            }

            //上方检查
            pt_temp.X = pt1.X;
            pt_temp.Y = pt1.Y - 1;

            while (!isPointOutBorder(pt_temp) && MapTiles[pt_temp.X, pt_temp.Y].ImageID == -1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.Y -= 1;
            }

            //下方检查
            pt_temp.X = pt1.X;
            pt_temp.Y = pt1.Y + 1;


            //if (isPointOutBorder(pt_temp))
            //    return false;
            while (!isPointOutBorder(pt_temp) && MapTiles[pt_temp.X, pt_temp.Y].ImageID == -1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.Y += 1;

                //if (isPointOutBorder(pt_temp))
                //    return false;
            }

            return false;
        }

        private bool CanLink(Point pt1, Point pt2)
        {
            if (CheckOneLine(pt1, pt2))
                return true;
            else
                if (CheckOneCorner(pt1, pt2))
                    return true;
                else
                    if (CheckTwoCorner(pt1, pt2))
                        return true;

            return false;
        }


        /// <summary>
        /// 测试自动消除
        /// </summary>
        private void AutoLink()
        {
            if (CalcSolution() == 2)
            {
                DrawPanelImage();
                _GameStart = false;
                MessageBox.Show("Greate Job!", "Notice");
                return;
            }
            bool needShuff = false;
            for (int i = 1; i < VerCount - 1; i++)
            {
                for (int j = 1; j < hozCount - 1; j++)
                {
                    if (MapTiles[i, j].ImageID != -1)
                    {
                        //tileNum++;
                        for (int k = i; k < VerCount - 1; k++)
                            for (int l = 1; l < hozCount - 1; l++)
                            {
                                if (k == i && j == l) continue;

                                if (isSameTile(MapTiles[i, j], MapTiles[k, l]))
                                {
                                    if (MapTiles[k, l].ImageID != -1)
                                    {
                                        if (CanLink(MapTiles[i, j], MapTiles[k, l]))
                                        {
                                            HideTile(MapTiles[i, j]);
                                            HideTile(MapTiles[k, j]);

                                            MapTiles[i, j].ImageID = -1;
                                            MapTiles[k, l].ImageID = -1;
                                            if (CalcSolution() == 1)
                                            {
                                                needShuff = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                    }

                    if (needShuff)
                        break;
                }

                if (needShuff)
                    break;
            }

            if (needShuff)
            {
                AgainShuffleGame();
                DrawPanelImage();
                AutoLink();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            AutoLink();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            _GameStart = true;
            InitGameData();
            DrawImage();

            foreach(int j in MapArray)
                listBox1.Items.Add(j);
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(e.X.ToString() + "--" + e.Y.ToString());
            if (!_GameStart)
                return;

            Point pt = new Point(e.X, e.Y);

            int x = (pt.X - OffsetY) / tileWidth+1;
            int y = (pt.Y - OffsetX) / tileHeight+1;

            

            Point SelPoint = new Point(y, x);

            if (isPointOutBorder(SelPoint))
            {
                _selTile = null;
                return;
            }

            if (_selTile == null)
            {
                _selTile = MapTiles[y, x];
                label1.Text = "Selected Tile:" + y.ToString() + "," + x.ToString()+":"+_selTile.ImageID.ToString();
                return ;
            }
            else
            {
                MapTitle tile = MapTiles[y, x];
                label2.Text = "Selected Tile 2:" + y.ToString() + "," + x.ToString()+":"+tile.ImageID.ToString();
                if (isSameTile(_selTile, tile))
                {
                    //如果可以连接
                    if (CanLink(new Point(_selTile.X, _selTile.Y), new Point(tile.X, tile.Y)))
                    {
                        HideTile(_selTile);
                        HideTile(tile);

                        MapTiles[_selTile.X, _selTile.Y].ImageID = -1;
                        MapTiles[tile.X, tile.Y].ImageID = -1;

                        ShowData();
                        int i=CalcSolution();
                        if (i == 2)
                        {
                            MessageBox.Show("Congruation! You do greate job!");
                            _GameStart = false;
                        }

                        if (i == 1)
                        {
                            AgainShuffleGame();
                        }
                        _selTile = null;
                    }
                    else
                        _selTile = null;
                }
                else
                {
                    _selTile = null;
                }
            }
        }


        /// <summary>
        /// 查看是否是一个图形
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        private bool isSameTile(MapTitle t1, MapTitle t2)
        {
            return t1.ImageID == t2.ImageID && !(t1.X == t2.X && t2.Y == t1.Y);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            if (_GameStart && MapTiles != null)
            {
                for (int i = 1; i < VerCount - 1; i++)
                    for (int j = 1; j < hozCount - 1; j++)
                    {
                        MapTitle mt = MapTiles[i, j];
                        if (mt.ImageID != -1)
                        {
                            int imageId = (mt.ImageID % _imageCount) + 1;
                            string fileName = Application.StartupPath + @"/Image/" + imageId.ToString() + ".bmp";
                            Bitmap bmp = new Bitmap(fileName);

                            //Graphics g = Graphics.FromHwnd(panel1.Handle);

                            g.DrawImage(bmp, mt.OffsetX, mt.OffsetY);
                        }
                    }
            }
        }

        private bool CanLink(MapTitle t1, MapTitle t2)
        {
            return CanLink(new Point(t1.X, t1.Y), new Point(t2.X, t2.Y));
        }
        /// <summary>
        /// test if the current game situation have solution.
        /// return value:
        ///   0: yes,have solution.
        ///   1: no,need to shuffle the cards.
        ///   2: the cards are all deleted,this level is clear.
        /// </summary>
        /// <returns></returns>
        private int CalcSolution()
        {
            int tileNum=0;
            for(int i=1;i<VerCount-1;i++)
                for(int j=1;j<hozCount-1;j++)
                    if (MapTiles[i, j].ImageID != -1)
                    {
                        tileNum++;
                        for (int k = i; k < VerCount - 1; k++)
                            for (int l = 1; l < hozCount - 1; l++)
                            {
                                if (k == i && j == l) continue;

                                if (isSameTile(MapTiles[i, j], MapTiles[k, l]))
                                {
                                    if (MapTiles[k, l].ImageID != -1)
                                    {
                                        if (CanLink(MapTiles[i, j], MapTiles[k, l]))
                                            return 0;
                                    }
                                }
                            }                                
                    }

            if (tileNum > 0)
                return 1;

            return 2;

        }

        private void RandomArray(int[] array)
        {
            int count = array.Length;
            Random rnd = new Random((int)System.DateTime.Now.Millisecond);
            for (int i = count; i > 0; i--)
            {
                int j = rnd.Next(i);
                int temp = array[j];
                array[j] = array[i - 1];
                array[i - 1] = temp;
            }
        }

        /// <summary>
        /// 重新打乱数组，保证有解
        /// </summary>
        private void AgainShuffleGame()
        {
            List<int> a1 = new List<int>();

            for (int i = 1; i < VerCount - 1; i++)
                for (int j = 1; j < hozCount - 1; j++)
                    //if (MapTiles[i, j].ImageID != -1)
                        a1.Add(MapTiles[i, j].ImageID);


            int[] array_temp = a1.ToArray();

            RandomArray(array_temp);

            int m = 1;
            for (int i = 1; i < VerCount - 1; i++)
                for (int j = 1; j < hozCount - 1; j++)
                {
                    //if (MapTiles[i, j].ImageID != -1)
                    //{
                    //    MapTiles[i, j].ImageID = array_temp[m];
                    //    m++;
                    //}
                    MapTiles[i, j] = new MapTitle(i, j, array_temp[(i - 1) * (hozCount - 2) + (j - 1)] , OffsetY + (j - 1) * tileWidth, OffsetX + (i - 1) * tileHeight);

                }

            ShowData();
            panel1.Invalidate();
            if (CalcSolution() == 1)
            {
                AgainShuffleGame();
            }
            else
                if (CalcSolution() == 0)
                {
                    DrawPanelImage();   
                }
                        
        }

        private void DrawPanelImage()
        {
            Graphics g = Graphics.FromHwnd(panel1.Handle);

            Rectangle rect = panel1.ClientRectangle;

            g.FillRectangle(new SolidBrush(Color.Black), rect);

            DrawImage();
        }

        private void CalcHinted(object sender, EventArgs e)
        {
            for (int i = 1; i < VerCount - 1; i++)
                for (int j = 1; j < hozCount - 1; j++)
                    if (MapTiles[i, j].ImageID != -1)
                    {                        
                        for (int k = i; k < VerCount - 1; k++)
                            for (int l = 1; l < hozCount - 1; l++)
                            {
                                if (k == i && j == l) continue;

                                if (MapTiles[k, l].ImageID != -1)
                                {
                                    if(isSameTile(MapTiles[i,j],MapTiles[k,l])){
                                        if (CanLink(MapTiles[i, j], MapTiles[k, l]))
                                        {
                                            int delay = 30;
                                            HideTile(MapTiles[i, j]);
                                            System.Threading.Thread.Sleep(delay);
                                            DrawTile(MapTiles[i, j]);

                                            HideTile(MapTiles[k, l]);
                                            System.Threading.Thread.Sleep(delay);
                                            DrawTile(MapTiles[k, l]);
                                            //MessageBox.Show(i.ToString() + "," + j.ToString() + "-" + k.ToString() + "," + l.ToString(), "提示");
                                            return;
                                        }
                                    }
                                }
                            }
                    }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ShowData();
        }
    }
}