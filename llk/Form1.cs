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
        private int hozCount = 5;
        private int VerCount = 6;

        private int[,] map =
            {
                {-1,-1,-1,-1,-1},
                {-1,-1, 2, 2,-1},
                {-1,-1, 1, 1,-1},
                {-1,-1,-1, 2,-1},
                {-1,-1, 1,-1,-1},
                {-1,-1,-1,-1,-1}
            };
        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ��ô�ֱ����ˮƽ����Ĵ���С��
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        /// <param name="p1_temp">���ش��</param>
        /// <param name="p2_temp">����С��</param>
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

                    p2_temp.X = pt1.Y;
                    p2_temp.Y = pt1.Y > pt2.Y ? pt2.Y : pt1.Y;
                }
        }


        //�Ƿ���һ��ֱ����
        private bool IsSameLine(Point pt1, Point pt2)
        {
            return pt1.X == pt2.X || pt1.Y == pt2.Y;
        }


        //�鿴�Ƿ����ֱ����ͨ
        private bool CheckOneLine(Point pt1, Point pt2)
        {
            if (IsSameLine(pt1, pt2))
            {
                Point p1_temp = new Point();  //p1_temp�洢�ұ߻�������Ĵ��
                Point p2_temp = new Point();  //p2_temp�洢��߻��������С��


                //����
                if (pt1.Y == pt2.Y)
                {
                    GetMaxMinPt(pt1, pt2, ref p1_temp, ref p2_temp);
                    //�����ֱ�����ڣ�ֱ����ͨ
                    if (p1_temp.X - p2_temp.X == 1)
                        return true;
                    //����м��������
                    for (int i = p2_temp.X + 1; i < p1_temp.X; i++)
                        if (map[i, p1_temp.Y] != -1)
                            return false;
                }
                else
                {
                    GetMaxMinPt(pt1, pt2, ref p1_temp, ref p2_temp);


                    //�ж�ֱ������
                    if (p1_temp.Y - p2_temp.Y == 1)
                        return true;

                    //����м��������
                    for (int i = p2_temp.Y + 1; i < p1_temp.Y; i++)
                        if (map[p2_temp.X, i] != -1)
                            return false;
                }
            }
            else
                return false;

            return true;

        }


        //�鿴�Ƿ񳬳�����߽�
        private bool isPointOutBorder(Point pt)
        {
            return pt.X == -1 || pt.Y == -1 || pt.X == hozCount || pt.Y == VerCount;
        }


        //�������������λ��
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

            return 0;
        }
        /// <summary>
        /// �汾1��ֻ��ѯpt1��pt2�����Ϸ������
        /// </summary>
        /// <param name="pt1"></param>
        /// <param name="pt2"></param>
        private bool CheckOneCorner(Point pt1, Point pt2)
        {
            int Direction = 1; //1-���Ϸ� 2-���·� 3-���Ϸ� 4-���·�
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
            //throw new Exception("The method or operation is not implemented.");
            if (IsSameLine(pt1, pt2))
                return CheckOneLine(pt1, pt2);

            //�Ҳ���
            if (CheckOneLine(new Point(pt1.X, pt2.Y), pt1))
                return true;

            //�·����
            if (CheckOneLine(new Point(pt2.X, pt1.Y), pt1))
                return true;

            return false;

        }

        private bool CheckRightTop(Point pt1, Point pt2)
        {
            if (IsSameLine(pt1, pt2))
                return CheckOneLine(pt1, pt2);

            //�Ҳ���
            if (CheckOneLine(new Point(pt2.Y, pt1.X), pt1))
                return true;

            //�Ϸ����
            if (CheckOneLine(new Point(pt2.X, pt1.Y), pt1))
                return true;

            return false;

        }

        private bool CheckLeftBottom(Point pt1, Point pt2)
        {
            if (IsSameLine(pt1, pt2))
                return CheckOneLine(pt1, pt2);

            //�Ҳ���
            bool canHorz = true;

            if (CheckOneLine(new Point(pt2.X, pt1.Y), pt2))
                return true;

            //�Ϸ����
            if (CheckOneLine(new Point(pt1.X, pt2.Y), pt2))
                return true;

            return false;
        }

        /// <summary>
        /// �鿴�Ƿ���Կ��Ե�����ͨ
        /// </summary>
        /// <param name="pt1">���Ϸ���</param>
        /// <param name="pt2">���·���</param>
        /// <returns></returns>
        private bool CheckLeftTop(Point pt1, Point pt2)
        {

            if (IsSameLine(pt1, pt2))
                return CheckOneLine(pt1, pt2);

            //���Ҳ���
            //Point pt_temp = new Point();
            
            bool canHorz=true;
            for (int i = pt1.X + 1; i < pt2.X; i++)
                if (map[i, pt1.Y] != -1)
                {
                    canHorz = false;
                    break;
                }
            if (canHorz)
            {
                if (CheckOneLine(new Point(pt2.X, pt1.Y), pt2))
                    return true;
            }

            //���·����
            bool canVert = true;
            for (int j = pt1.Y + 1; j < pt2.Y; j++)
                if (map[pt1.X, j] == -1)
                {
                    canVert = false;
                    break;
                }

            if(canVert)
                if (CheckOneLine(new Point(pt1.X, pt2.Y), pt2))
                    return true;

            return false;

        }

        private bool CheckTwoCorner(Point pt1, Point pt2)
        {
            Point pt_temp = new Point();

            //�����
            pt_temp.X = pt1.X - 1;
            pt_temp.Y = pt1.Y;

            while (!isPointOutBorder(pt_temp) && map[pt_temp.X,pt_temp.Y]==-1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.X -= 1;
            }

            //�Ҳ���
            pt_temp.X = pt1.X + 1;
            pt_temp.Y = pt1.Y;

            while (!isPointOutBorder(pt_temp) && map[pt_temp.X, pt_temp.Y] == -1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.X += 1;
            }

            //�Ϸ����
            pt_temp.X = pt1.X;
            pt_temp.Y = pt1.Y - 1;

            while (!isPointOutBorder(pt_temp) && map[pt_temp.X, pt_temp.Y] == -1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.Y -= 1;
            }

            //�·����
            pt_temp.X = pt1.X;
            pt_temp.Y = pt1.Y + 1;

            while (!isPointOutBorder(pt_temp) && map[pt_temp.X, pt_temp.Y] == -1)
            {
                if (CheckOneCorner(pt_temp, pt2))
                    return true;

                pt_temp.Y += 1;
            }

            return false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string msgOK = "ֱ����ͨ��";
            string msgOK_1 = "������ͨ��";
            string msgOK_2 = "˫����ͨ��";
            Point pt1 = new Point(1, 2);
            Point pt2 = new Point(3, 3);
            if (CheckOneLine(pt1, pt2))
                MessageBox.Show(msgOK);
            else
                if (CheckOneCorner(pt1, pt2))
                    MessageBox.Show(msgOK_1);
                else
                    if (CheckTwoCorner(pt1, pt2))
                        MessageBox.Show(msgOK_2);
                    else
                        MessageBox.Show("������ͨ");
        }
    }
}