using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;

namespace prac1_new
{
    class World
    {
        public Random rnd_red = new Random();
        public Random rnd_green = new Random(86);

        bool red_get = false;
        bool green_get = false;

        public int iter = 0;

        static public Point rp = new Point(650, 275);//точка старта красного и финиша зеленого(левый верхний угол)
        static public Point gp = new Point(275, 100);//точка старта зеленого и финаша красного(левый верхний угол)

        public Robot red = new Robot(rp, gp, "red");
        public Robot green = new Robot(gp, rp, "green");

        public List<Point> red_path = new List<Point>();
        public List<Point> green_path = new List<Point>();
        public List<int> distance = new List<int>();

        Brush red_brush = new SolidBrush(Color.Red);
        Brush green_brush = new SolidBrush(Color.Green);
        Brush test_brush = new SolidBrush(Color.Black);

        Pen pen = new Pen(Color.Black, 3);
        Pen penr = new Pen(Color.Red, 3);
        Pen peng = new Pen(Color.Green, 3);

        Point p1 = new Point(50, 350);
        Point p2 = new Point(750, 350);
        Point p3 = new Point(750, 200);
        Point p4 = new Point(450, 200);
        Point p5 = new Point(450, 50);
        Point p6 = new Point(250, 50);
        Point p7 = new Point(250, 200);
        Point p8 = new Point(50, 200);

        public struct data//структура с данными о шаге
        {
            public int x1;//Х красного
            public int y1;//У красного 
            public int x2;//Х зеленого
            public int y2;//У зеленого
            public float distance;//дистанция между центрами роботов
            public int red_to_goal;//расстояние до цели красного
            public int green_to_goal;//расстояние до цели зеленого
            public float f;//функция пригодности

        }

        public List<data[]> iteration_data = new List<data[]>();//лист массивов итерация, для хранения данных всех итераций

        public data[] algorithm_data = new data[12];// данные всех шагов в отдельной итерации

        public List<data[]> population_data = new List<data[]>();//данные о популяции после мутации

        public data[] best_to_mutate = new data[12];//массив лучших точек для мутации

        public void draw_map(Graphics g)
        {
            g.Clear(Color.White);

            g.DrawLine(pen, p1, p2);
            g.DrawLine(pen, p2, p3);
            g.DrawLine(pen, p3, p4);
            g.DrawLine(pen, p4, p5);
            g.DrawLine(pen, p5, p6);
            g.DrawLine(pen, p6, p7);
            g.DrawLine(pen, p7, p8);
            g.DrawLine(pen, p8, p1);


        }
        public void draw(Graphics g)
        {


            draw_map(g);


            foreach (Edge i in red.edge_list)
            {
                g.DrawLine(penr, i.start, i.finish);
            }

            foreach (Edge i in green.edge_list)
            {
                g.DrawLine(peng, i.start, i.finish);
            }

            g.FillEllipse(test_brush, red.x - 3, red.y - 3, 6, 6);//центры
            g.FillEllipse(test_brush, green.x - 3, green.y - 3, 6, 6);//центры
        }

        public void move()
        {
            if (!red.outofmoves)
            {
                red.destination(rnd_red);
            }

            if (!green.outofmoves)
            {
                green.destination(rnd_green);
            }

        }

        public void get_data(int iteration, string writePath)
        {
            for (int i = 0; i < red.list.Count; i++)//добавление в таблицу точек зеленого
            {
                algorithm_data[i].x1 = red.list[i].X;
                algorithm_data[i].y1 = red.list[i].Y;
            }
            for (int i = 0; i < green.list.Count; i++)//добавление в таблицу точек зеленого
            {
                algorithm_data[i].x2 = green.list[i].X;
                algorithm_data[i].y2 = green.list[i].Y;
            }
            for (int i = 0; i < 12; i++)
            {
                algorithm_data[i].distance = (int)Math.Pow(Math.Pow((algorithm_data[i].x1 - algorithm_data[i].x2), 2) + Math.Pow((algorithm_data[i].y1 - algorithm_data[i].y2), 2), 0.5);
                algorithm_data[i].red_to_goal = (int)Math.Pow(Math.Pow((algorithm_data[i].x1 - red.finish_x), 2) + Math.Pow((algorithm_data[i].y1 - red.finish_y), 2), 0.5);
                algorithm_data[i].green_to_goal = (int)Math.Pow(Math.Pow((algorithm_data[i].x2 - green.finish_x), 2) + Math.Pow((algorithm_data[i].y2 - green.finish_y), 2), 0.5);

                int p = crossing((int)red.x, (int)red.y, algorithm_data[i].x1, algorithm_data[i].y1, (int)green.x, (int)green.y, algorithm_data[i].x2, algorithm_data[i].y2);

                algorithm_data[i].f = ((algorithm_data[i].distance / 65) * (200 / (float)(algorithm_data[i].red_to_goal + algorithm_data[i].green_to_goal + 1))) * p;//изменил функцию принадлежности
            }

            float sum = 0;
            float best_sum = 0;
            foreach (data i in algorithm_data)
            {
                sum += i.f;
            }
            foreach (data i in best_to_mutate)
            {
                best_sum += i.f;
            }
            if (sum > best_sum)
            {
                best_to_mutate = algorithm_data;
            }
            sum = 0; best_sum = 0;


            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
                if (iter != iteration)
                {
                    sw.WriteLine();
                    sw.WriteLine("Итерация " + Convert.ToString(iteration));//добавить номер итерации
                    sw.WriteLine();
                    iter = iteration;
                }
                foreach (data i in algorithm_data)
                {
                    sw.WriteLine("x1= " + i.x1 + " " + "y1= " + i.y1 + " " + "x2= " + i.x2 + " " + "y2= " + i.y2 + " " + "dist= " + i.distance + " " + "red_goal= " + i.red_to_goal + " " + "green_goal= " + i.green_to_goal + " " + "f= " + i.f);
                }
            }
        }

        public void restart()//перезапуск мира для новой итерации
        {
            red.restart(rp, gp);
            green.restart(gp, rp);

        }

        public void get_best(Graphics g)// выбор лучших точек для мутации
        {
            //выбор лучших точек
            foreach (data[] i in iteration_data)
            {
                for (int j = 0; j < i.Count(); j++)
                {
                    if (i[j].f > best_to_mutate[j].f)
                        best_to_mutate[j] = i[j];
                }
            }

            float rf = (float)Math.Pow(Math.Pow(best_to_mutate.Last().x1 - red.finish_x, 2) + Math.Pow(best_to_mutate.Last().y1 - red.finish_y, 2), 0.5);
            float gf = (float)Math.Pow(Math.Pow(best_to_mutate.Last().x2 - green.finish_x, 2) + Math.Pow(best_to_mutate.Last().y2 - green.finish_y, 2), 0.5);
            if (rf < 50)
            {
                best_to_mutate[11].x1 = red.finish_x;
                best_to_mutate[11].y1 = red.finish_y;
            }

            if (gf < 50)
            {
                best_to_mutate[11].x2 = green.finish_x;
                best_to_mutate[11].y2 = green.finish_y;

            }

            string writePath = @"D:\Soft\master_degree\prac1_new\logs\log.txt";//ПК

            using (StreamWriter sw = new StreamWriter(writePath, true, System.Text.Encoding.Default))
            {
                foreach (data i in best_to_mutate)
                {
                    sw.WriteLine("x1= " + i.x1 + " " + "y1= " + i.y1 + " " + "x2= " + i.x2 + " " + "y2= " + i.y2 + " " + "dist= " + i.distance + " " + "red_goal= " + i.red_to_goal + " " + "green_goal= " + i.green_to_goal + " " + "f= " + i.f);
                }
                sw.WriteLine();
            }

            ///// отрисовка лучшей траектории, собранной из 10 случайных траекторий
            draw_map(g);
            red.edge_list.Clear();
            green.edge_list.Clear();
            for (int i = 0; i < best_to_mutate.Count(); i++)
            {
                if (i == best_to_mutate.Count() - 1)
                    continue;
                red.edge_list.Add(new Edge(new Point(best_to_mutate[i].x1, best_to_mutate[i].y1), new Point(best_to_mutate[i + 1].x1, best_to_mutate[i + 1].y1)));
                green.edge_list.Add(new Edge(new Point(best_to_mutate[i].x2, best_to_mutate[i].y2), new Point(best_to_mutate[i + 1].x2, best_to_mutate[i + 1].y2)));


            }

            foreach (Edge i in red.edge_list)
            {
                g.DrawLine(penr, i.start, i.finish);
            }

            foreach (Edge i in green.edge_list)
            {
                g.DrawLine(peng, i.start, i.finish);
            }

            foreach (data i in best_to_mutate)
            {
                if (i.distance <= 65)
                {
                    g.FillEllipse(test_brush, i.x1 + 3, i.y1 - 3, 6, 6);
                    g.FillEllipse(test_brush, i.x2 - 3, i.y2 - 3, 6, 6);
                    g.DrawLine(pen, i.x1, i.y1, i.x2, i.y2);
                }
            }
        }

        public void mutation(data[] to_mutate, Random rnd, int c, int iter)//метод мутации, C - это величина смещения
        {
            iteration_data.Clear();

            for (int j = 0; j < iter; j++)
            {
                data[] temp_data = new data[best_to_mutate.Count()];
                temp_data = best_to_mutate;
                for (int i = 0; i < best_to_mutate.Count(); i++)
                {
                    if (i == 0)
                        continue;

                    int dist1 = (int)Math.Pow(Math.Pow(temp_data[i].x1 - temp_data[i - 1].x1, 2) + Math.Pow(temp_data[i].y1 - temp_data[i - 1].y1, 2), 0.5);
                    int dist2 = (int)Math.Pow(Math.Pow(temp_data[i].x2 - temp_data[i - 1].x2, 2) + Math.Pow(temp_data[i].y2 - temp_data[i - 1].y2, 2), 0.5);


                    if (temp_data[i].distance >= 65 & red_get)
                    {

                    }
                    else
                    {
                        if (temp_data[i].red_to_goal < 2 * c)
                        {
                            temp_data[i].x1 = red.finish_x;
                            temp_data[i].y1 = red.finish_y;
                            red_get = true;
                        }
                        else
                        {
                            if (dist1 < 50)
                            {
                                int kx1 = rnd.Next(0, c);
                                temp_data[i].x1 -= kx1;
                                if (temp_data[i].y1 <= 230)
                                    temp_data[i].y1 += (int)Math.Pow(Math.Pow(c, 2) - Math.Pow(kx1, 2), 0.5);
                                else
                                    temp_data[i].y1 -= (int)Math.Pow(Math.Pow(c, 2) - Math.Pow(kx1, 2), 0.5);
                            }

                            else
                            {
                                double curr_alpha = Math.Atan2((temp_data[i].y1 - temp_data[i - 1].y1), (temp_data[i - 1].x1 - temp_data[i].x1));
                                double degree = -curr_alpha - rnd.Next(-c, c) * Math.PI / 180;

                                temp_data[i].x1 = (int)((double)temp_data[i - 1].x1 - Math.Cos(degree) * 50);
                                temp_data[i].y1 = (int)((double)temp_data[i - 1].y1 - Math.Sin(degree) * 50);
                            }


                            }
                        }

                        if (temp_data[i].distance >= 65 & green_get)
                        {

                        }
                        else
                        {
                            if (temp_data[i].green_to_goal < 2 * c)
                            {
                                temp_data[i].x2 = green.finish_x;
                                temp_data[i].y2 = green.finish_y;
                                green_get = true;
                            }
                            else
                            {
                                if (dist2 < 50)
                                {
                                    int kx2 = rnd.Next(0, c);
                                    temp_data[i].x2 += kx2;
                                    if (temp_data[i].y2 >= 290)
                                        temp_data[i].y2 -= (int)Math.Pow(Math.Pow(c, 0.5) - Math.Pow(kx2, 0.5), 2);
                                    else
                                        temp_data[i].y2 += (int)Math.Pow(Math.Pow(c, 0.5) - Math.Pow(kx2, 0.5), 2);

                                }
                                else
                                {
                                    double curr_alpha = Math.Atan2((temp_data[i].y2 - temp_data[i - 1].y2), (temp_data[i].x2 - temp_data[i - 1].x2));
                                    double degree = curr_alpha + rnd.Next(-c, c) * Math.PI / 180;

                                    temp_data[i].x2 = (int)((double)temp_data[i - 1].x2 + Math.Cos(degree) * 50);
                                    temp_data[i].y2 = (int)((double)temp_data[i - 1].y2 + Math.Sin(degree) * 50);

                                }

                            }
                        }

                        temp_data[i].distance = (int)Math.Pow(Math.Pow((temp_data[i].x1 - temp_data[i].x2), 2) + Math.Pow((temp_data[i].y1 - temp_data[i].y2), 2), 0.5);
                        temp_data[i].red_to_goal = (int)Math.Pow(Math.Pow((temp_data[i].x1 - red.finish_x), 2) + Math.Pow((temp_data[i].y1 - red.finish_y), 2), 0.5);
                        temp_data[i].green_to_goal = (int)Math.Pow(Math.Pow((temp_data[i].x2 - green.finish_x), 2) + Math.Pow((temp_data[i].y2 - green.finish_y), 2), 0.5);

                        int p = crossing((int)red.x, (int)red.y, temp_data[i].x1, temp_data[i].y1, (int)green.x, (int)green.y, temp_data[i].x2, temp_data[i].y2);

                        temp_data[i].f = ((temp_data[i].distance / 65) * (200 / (float)(temp_data[i].red_to_goal + temp_data[i].green_to_goal + 1))) * p;
                    }
                    iteration_data.Add(temp_data);
                }


            }

            int crossing(int xr, int yr, int x1, int y1, int xg, int yg, int x2, int y2)//метод проверки пересечения ребер, r-red, g-green,1-red,2-green
            {
                float zn = (y2 - yg) * (x1 - xr) - (x2 - xg) * (y1 - yr);//знаменатель рассчета
                float ua = ((x2 - xg) * (yr - yg) - (y2 - yg) * (xr - xg)) / zn;
                float ub = ((x1 - xr) * (yr - yg) - (y1 - yr) * (xr - xg)) / zn;

                if (0 < ua & ua < 1 & 0 < ub & ub < 1) return 0; else return 1;
            }



        }
    }
