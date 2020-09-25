using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace prac1_new
{
    class Robot
    {
        public float x;//стартовое значение Х
        public float y;//стартовое значени У
        public int speed = 2;
        public string color;

        public int c = 50;//длина шага

        public float temp_x;
        public float temp_y;

        public float next_x;
        public float next_y;

        public int finish_x;//точка назначения Х
        public int finish_y;//точка назначения У

        public bool get = false;//достижение финиша
        public bool outofmoves = false;//максимальное число вершин достигнуто

        int kx;
        float ky;

        public List<Point> list = new List<Point>();

        public List<Edge> edge_list = new List<Edge>();

        public Robot(Point start, Point finish, string color)
        {
            this.color = color;
            this.x = start.X;
            this.y = start.Y;
            this.finish_x = finish.X;
            this.finish_y = finish.Y;

            this.temp_x = start.X;
            this.temp_y = start.Y;

            next_x = x;
            next_y = y;

            list.Add(start);
        }

        public void move()
        {
            double c = Math.Pow(Math.Pow(next_x - temp_x, 2) + Math.Pow(next_x - temp_y, 2), 0.5);//Гипотенуза треугольника движения(вектор по которому едет робот)
            double A = Math.Atan2(y - next_y, x - next_x) / Math.PI * 180;

            x += speed * ((next_x - temp_x) / (float)c);
            y += speed * ((next_y - temp_y) / (float)c);
        }

        public void destination(Random rnd)
        {
            if (Math.Abs(finish_x - x) <= 10 & Math.Abs(finish_y - y) <= 10)
                get = true;
            else
            {
                if ((Math.Pow(Math.Pow(x, 2) + Math.Pow(y, 2), 0.5)<=50))
                {
                    temp_x = x;
                    temp_y = y;
                    next_x = finish_x;
                    next_y = finish_y;
                    edge_list.Add(new Edge(list.Last(), new Point((int)next_x, (int)next_y)));
                    list.Add(new Point(finish_x, finish_y));
                }
                else
                {
                    temp_x = x;
                    temp_y = y;
                    kx= rnd.Next(0, 51);
                    double degree = rnd.Next(-90, 90) * Math.PI / 180;
                    if (finish_x > x)
                    {
                        next_x = x + (float)Math.Cos(degree) * c;
                    }
                    else
                    {
                        next_x = x - (float)Math.Cos(degree) * c;
                    }
                    ky = (float)Math.Pow((int)Math.Pow(c, 2) + (int)Math.Pow(kx, 2), 0.5);
                    if (rnd.Next(0, 100) > 50)
                    {
                        next_y = y + (float)Math.Sin(degree) * c;
                    }
                    else
                    {
                        next_y = y - (float)Math.Sin(degree) * c;
                    }
                        
                     if (next_x > 50+30 & next_x < 250 + 30)//первый сегмент, от левого края, до ниши
                     {
                        if(next_y>320)
                        {

                            next_y = 320 - rnd.Next(0, c);
                           float k_x = (float)Math.Pow(Math.Abs((int)Math.Pow(c, 2) - (int)Math.Pow(Math.Abs(y - next_y), 2)), 0.5);
                            if (finish_x > x)
                            {
                                next_x = x + k_x;
                            }
                            else
                            {
                                next_x = x - k_x;
                            }
                        }
                        if (next_y < 230)
                        {
                            next_y = 230 + rnd.Next(0, c);
                            float k_x = (float)Math.Pow(Math.Abs((int)Math.Pow(c, 2) - (int)Math.Pow(Math.Abs(y - next_y), 2)), 0.5);
                            if (finish_x > x)
                            {
                                next_x = x + k_x;
                            }
                            else
                            {
                                next_x = x - k_x;
                            }
                        }
                     }

                     if ((next_x > 250 + 30 & next_x < 450 - 30))//второй сегмент
                     {
                         if (next_y > 320)
                        {
                            next_y = 320- rnd.Next(0,50);
                            float k_x = (float)Math.Pow(Math.Abs((int)Math.Pow(c, 2) - (int)Math.Pow(Math.Abs(y - next_y), 2)), 0.5);
                            if (finish_x > x)
                            {
                                next_x = x + k_x;
                                if (next_x > 420 & next_y<200)
                                {
                                    next_x = 420;
                                    next_y= y+ (float)Math.Pow(Math.Abs((int)Math.Pow(c, 2) - (int)Math.Pow(Math.Abs(x - next_x), 2)), 0.5);
                                }
                            }
                            else
                            {
                                next_x = x - k_x;
                                if (next_x < 280 & next_y<200)
                                {
                                    next_x = 280;
                                    next_y = y + (float)Math.Pow(Math.Abs((int)Math.Pow(c, 2) - (int)Math.Pow(Math.Abs(x - next_x), 2)), 0.5);
                                }
                            }

                           
                        }
                             
                         if (next_y < 80)
                        {
                            next_y = 80 + rnd.Next(0, c);
                            float k_x = (float)Math.Pow(Math.Abs((int)Math.Pow(c, 2) - (int)Math.Pow(Math.Abs(y - next_y), 2)), 0.5);
                            if (finish_x > x)
                            {
                                next_x = x + k_x;
                            }
                            else
                            {
                                next_x = x - k_x;
                            }
                        }
                             
                     }

                     if (next_x > 450 - 30 & next_x < 630)//третий сегмент, от ниши, до правого края
                     {
                        if (next_y > 320)
                        {
                            next_y = 320 - rnd.Next(0, c);
                            float k_x = (float)Math.Pow(Math.Abs((int)Math.Pow(c, 2) - (int)Math.Pow(Math.Abs(y - next_y), 2)), 0.5);
                            if (finish_x > x)
                            {
                                next_x = x + k_x;
                            }
                            else
                            {
                                next_x = x - k_x;
                            }
                        }
                        if (next_y < 230)
                        {
                            next_y = 230 + rnd.Next(0, c);
                            float k_x = (float)Math.Pow(Math.Abs((int)Math.Pow(c, 2) - (int)Math.Pow(Math.Abs(y - next_y), 2)), 0.5);
                            if (finish_x > x)
                            {
                                next_x = x + k_x;
                            }
                            else
                            {
                                next_x = x - k_x;
                            }
                        }
                    }
                    if (Math.Pow(Math.Abs((int)Math.Pow(next_x - x, 2) + (int)Math.Pow(Math.Abs(y - next_y), 2)), 0.5) > c)
                        destination(rnd);
                    else
                    {
                        edge_list.Add(new Edge(list.Last(), new Point((int)next_x, (int)next_y)));
                        list.Add(new Point((int)next_x, (int)next_y));
                        kx = 0;
                        ky = 0;
                        x = next_x;
                        y = next_y;
                        if (list.Count == 12)
                        {
                            outofmoves = true;
                        }
                    }

                }

            }            
                    
        }

        public void restart(Point start,Point finish)
        {
            list.Clear();
            this.x = start.X;
            this.y = start.Y;
            this.finish_x = finish.X;
            this.finish_y = finish.Y;

            this.temp_x = start.X;
            this.temp_y = start.Y;

            next_x = x;
            next_y = y;

            this.get = false;
            this.outofmoves = false;

            list.Add(start);

        }
    }
}
