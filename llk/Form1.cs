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
        private int hozCount = 20;
        private int VerCount = 12;
        private int tileWidth = 31;
        private int tileHeight = 34;

        private int[,] map =
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
                    MapTiles[i, j] = new MapTitle(i, j, MapArray[(i - 1) * (hozCount-2) + (j - 1)], OffsetX + (i - 1) * tileWidth, OffsetY + (j - 1) * tileHeight);

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
                    item.SubItems.Add((MapTiles[i, j].ImageID%36+1).ToString());

                listView1.Items.Add(item);
            }
        }
        private void InitGameData()
        {
            RandomMapArray();
            InitTiles();
        }
        private int _offsetX;

        public int OffsetX
        {
            get { return _offsetX; }
            set { _offsetX = value; }
        }

        private int _offsetY;

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
                        if (map[i, p1_temp.Y] != -1)
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
                        if (map[p2_temp.X, i] != -1)
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
                map[pt1_temp.X, pt1_temp.Y] == -1 
                && CheckOneLine(pt1_temp,pt2)
                )
                || 
                (CheckOneLine(pt2, pt2_temp)
                &&
                map[pt2_temp.X, pt2_temp.Y] == -1 
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

            if ((CheckOneLine(pt1, pt1_temp) && map[pt1_temp.X, pt1_temp.Y] == -1 && CheckOneLine(pt1_temp,pt2) ) || (CheckOneLine(pt2_temp, pt2) && map[pt2_temp.X, pt2_temp.Y] == -1 && CheckOneLine(pt1,pt2_temp)))
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

            while (!isPointOutBorder(pt_temp) && map[pt_temp.X,pt_temp.Y]==-1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.X -= 1;
            }

            //右侧检查
            pt_temp.X = pt1.X + 1;
            pt_temp.Y = pt1.Y;

            while (!isPointOutBorder(pt_temp) && map[pt_temp.X, pt_temp.Y] == -1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.X += 1;
            }

            //上方检查
            pt_temp.X = pt1.X;
            pt_temp.Y = pt1.Y - 1;

            while (!isPointOutBorder(pt_temp) && map[pt_temp.X, pt_temp.Y] == -1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.Y -= 1;
            }

            //下方检查
            pt_temp.X = pt1.X;
            pt_temp.Y = pt1.Y + 1;


            if (isPointOutBorder(pt_temp))
                return false;
            while (!isPointOutBorder(pt_temp) && map[pt_temp.X, pt_temp.Y] == -1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.Y += 1;

                if (isPointOutBorder(pt_temp))
                    return false;
            }

            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string msgOK = "直接联通！";
            string msgOK_1 = "单角联通！";
            string msgOK_2 = "双角联通！";
            Point pt1 = new Point(3, 1);
            Point pt2 = new Point(1, 2);
            if (CheckOneLine(pt1, pt2))
                MessageBox.Show(msgOK);
            else
                if (CheckOneCorner(pt1, pt2))
                    MessageBox.Show(msgOK_1);
                else
                    if (CheckTwoCorner(pt1, pt2))
                        MessageBox.Show(msgOK_2);
                    else
                        MessageBox.Show("不能联通");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            InitGameData();

            foreach(int j in MapArray)
                listBox1.Items.Add(j);
        }
    }
}