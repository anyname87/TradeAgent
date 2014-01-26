using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using System.Diagnostics;

namespace Anyname_Projects.lib
{
    public class TradeWay
    {
    }

    // Реализация алгоритма Дейкстры. Содержит матрицу смежности в виде массивов вершин и ребер
    class DekstraAlgorim
    {

        public Point[] Points { get; private set; } // массив вершин
        public Rebro[] Rebra { get; private set; } // массив ребер
        public Point BeginPoint { get; private set; } // вершина графа

        public DekstraAlgorim(Point[] pointsOfgrath, Rebro[] rebraOfgrath)
        {
            // конструктор класса
            Points = pointsOfgrath; 
            Rebra = rebraOfgrath;
        }

        // Запуск алгоритма расчета
        public void AlgoritmRun(Point beginp)
        {

            if (!Points.Any() || !Rebra.Any())
                throw new DekstraException("Массив вершин или ребер не задан!");
            else
            {
                BeginPoint = beginp; // вершина графа
                OneStep(beginp); // первый проход для вершины графа
                foreach (Point point in Points)
                {
                    /* Получаем следующую точку с наименьшим весом.
                     * Как правило, ей всегда оказывается "ближайший" сосед 
                     * предыдущей относительной точки. Если таких нет, 
                     * значит построение маршрута закончено
                     */ 
                    Point anotherP = GetAnotherUncheckedPoint(); 
                    if (anotherP != null)
                        OneStep(anotherP); // Ищем следующего "ближайшего соседа"
                    else
                        break;

                }
            }

        }

        // Метод, делающий один шаг алгоритма. Принимает на вход вершину (относительную точку)
        public void OneStep(Point beginpoint)
        {
            /* Начинаем перебирать соседние связанные вершины нашей, 
             * переданной на вход функции, относительной вершины
             */
            foreach (Point nextp in Pred(beginpoint))
            {
                // Если для соседняя вершина еще не обрабатывалась как относительная точка, то продолжаем
                if (nextp.IsChecked == false) //не отмечена
                {
                    /* Подсчитываем "массу" точки используя значение по-умолчанию (9999999) и
                     * "длину ребра" между "соседом" и относительной точкой
                     */
                    float newmetka = beginpoint.Weight + GetMyRebro(nextp, beginpoint).Weight;

                    /* Ищем точку, которая ранее уже рассматривалась, 
                     * как соседняя текущей относительной точки, 
                     * имеющую "массу" меньше, чем мы только что подсчитали.
                     * Другими словами смотрим, есть ли у кого путь короче текущего.
                     */
                    var more_point = this.Points.Where(item => ((item.Weight < newmetka) && (item.predPoint == beginpoint)));

                    /* Если "масса" текущего соседа (по-умолчанию она у всех ровна 9999999, кроме вершины, равной 0) 
                     * больше только что подсчитанной, и соседей имеющих путь короче не нашлось...
                     */
                    if ((nextp.Weight > newmetka) && !more_point.Any())
                    {
                        /* Обнуляем соседей, вернув им значение "массы" по-умолчанию.
                         * Убиваем ссылку "предыдущей вершины" на текущую относительную точку.
                         */
                        var set_null = this.Points.Where(item => (item.predPoint == beginpoint));
                        if (set_null.Any())
                        {
                            foreach (Point p in set_null)
                            {
                                p.Weight = 9999999;
                                p.predPoint = new Point();
                            }
                            
                        }
                        /* Задаем текущему соседу новое значение "массы" 
                         * и устаналиваем сылку на текущую относительную точку
                         */
                        nextp.Weight = newmetka;
                        nextp.predPoint = beginpoint;
                    }
                }
            }
            // Помечаем относительную точку, как исследованную
            beginpoint.IsChecked = true; //вычеркиваем
        }

        // Поиск соседей для вершины. Для неориентированного графа ищутся все соседи.
        private IEnumerable<Point> Pred(Point currpoint)
        {
            IEnumerable<Point> firstpoints = from ff in Rebra where ff.FirstPoint == currpoint select ff.SecondPoint;
            IEnumerable<Point> secondpoints = from sp in Rebra where sp.SecondPoint == currpoint select sp.FirstPoint;
            IEnumerable<Point> totalpoints = firstpoints.Concat<Point>(secondpoints);
            return totalpoints;
        }

        // Получаем ребро, соединяющее 2 входные точки
        private Rebro GetMyRebro(Point a, Point b)
        {//ищем ребро по 2 точкам
            IEnumerable<Rebro> myr = from reb in Rebra where (reb.FirstPoint == a & reb.SecondPoint == b) || (reb.SecondPoint == a & reb.FirstPoint == b) select reb;
            if (myr.Count() > 1 || myr.Count() == 0)
            {
                throw new DekstraException("Не найдено ребро между соседями!");
            }
            else
            {
                return myr.First();
            }
        }

        // Получаем очередную неотмеченную вершину, "ближайшую" к заданной.
        private Point GetAnotherUncheckedPoint()
        {
            IEnumerable<Point> pointsuncheck = from p in Points where p.IsChecked == false select p;
            if (pointsuncheck.Count() != 0)
            {
                float minVal = pointsuncheck.First().Weight;
                Point minPoint = pointsuncheck.First();
                foreach (Point p in pointsuncheck)
                {
                    if (p.Weight < minVal)
                    {
                        minVal = p.Weight;
                        minPoint = p;
                    }
                }
                return minPoint;
            }
            else
            {
                return null;
            }
        }

        public List<Point> MinPath1(Point end)
        {
            List<Point> listOfpoints = new List<Point>();
            Point tempp = new Point();
            tempp = end;
            while (tempp != this.BeginPoint)
            {
                listOfpoints.Add(tempp);
                tempp = tempp.predPoint;
            }

            return listOfpoints;
        }
    }
    // Класс, реализующий ребро
    class Rebro
    {
        public Point FirstPoint { get; private set; }
        public Point SecondPoint { get; private set; }
        public float Weight { get; private set; }

        public Rebro(Point first, Point second, float valueOfWeight)
        {
            FirstPoint = first;
            SecondPoint = second;
            Weight = valueOfWeight;
        }
    }


    // Класс, реализующий вершину графа
    class Point
    {
        public float Weight { get; set; }
        public string Name { get; private set; }
        public bool IsChecked { get; set; }
        public Point predPoint { get; set; }
        public object SomeObj { get; set; }
        public Point(int value, bool ischecked)
        {
            Weight = value;
            IsChecked = ischecked;
            predPoint = new Point();
        }
        public Point(int value, bool ischecked, string name)
        {
            Weight = value;
            IsChecked = ischecked;
            Name = name;
            predPoint = new Point();
        }
        public Point()
        {
        }
    }
    // класс координатных точек от клиента
    public class Geopoints
    {
        public string id { get; set; }
        public double lon { get; set; }
        public double lat { get; set; }
        public float weight { get; private set; }

        public Geopoints(string id, double lon, double lat)
        {
            this.id = id;
            this.lon = lon;
            this.lat = lat;
        }

        public Geopoints(string id, double lon, double lat, float weight)
        {
            this.id = id;
            this.lon = lon;
            this.lat = lat;
            this.weight = weight;
        }

        public Geopoints()
        {
        }
    }
    // для печати графа
    static class PrintGrath
    {
        public static List<Point> PrintAllMinPaths(DekstraAlgorim dekstra)
        {
            var list_points = dekstra.Points.ToList();
            list_points = list_points.OrderBy(x => x.Weight).ToList();
            return list_points;
        }
    }
    
    internal class DekstraException : ApplicationException
    {
        public DekstraException(string message)
            : base(message)
        {
        }
    }
}