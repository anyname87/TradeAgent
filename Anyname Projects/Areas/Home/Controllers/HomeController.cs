using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Anyname_Projects.lib;
//using System.Diagnostics;

namespace Anyname_Projects.Areas.Home.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/Index/
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Index(Geopoints[] points)
        {
            var count = points.Count(); // количество переданных координат
            var vertex = new Point[count]; // массив вершин графа

            for (int i = 0; i < count; i++)
                vertex[i] = new Point(9999999, false, points[i].id);

            vertex[0].Weight = 0; // Начальная точка, торговый агент, вершина графа

          //  var count_verge = (count * (count - 1)) / 2; // полный граф, в котором каждая пара различных вершин смежна, с n вершинами имеет n(n-1)/2 рёбер
          //  var arrVerge = new Rebro[count_verge];
            var listVerge = new List<Rebro>();

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < count; j++)
                {
                    // Создаем список ребер с учетом того, что у нас полный граф, ребра не дублируются
                    var anyPoint = listVerge.Where(item => ((item.SecondPoint == vertex[i]) && (item.FirstPoint == vertex[j])));
                    if ((i != j) && !anyPoint.Any())
                        listVerge.Add(new Rebro(vertex[i], vertex[j], Lenght(points[i].lat, points[i].lon, points[j].lat, points[j].lon)));
                }
            }
            // запускаем конструктор, запоминаем вершины и ребра
            var dekstra = new DekstraAlgorim(vertex, listVerge.ToArray());
            // запускаем процедуру расчета кратчайшего пути
            dekstra.AlgoritmRun(vertex[0]);
            // получаем упорядоченный массив вершин
            var shortPath = PrintGrath.PrintAllMinPaths(dekstra);

            // Создаем новый отсортированный массив координат
            count = shortPath.Count;
            Geopoints[] arrPath = new Geopoints[count];

            for (int i = 0; i < count; i++)
            {
                string id = shortPath[i].Name.ToString();
                double lon = points.Where(item => item.id == id).Select(item => item.lon).First();
                double lat = points.Where(item => item.id == id).Select(item => item.lat).First();
                float weight = shortPath[i].Weight;
                arrPath[i] = new Geopoints(id, lon, lat, weight);
            }
            var way = arrPath.ToArray();
            return Json(way);
        }

        /* Расчет длины ребра согласно стандартной математической 
         * формуле "Расстояние между двумя точками A1(x1;y1) и A2(x2;y2) 
         * в прямоугольной системе координат"
         */
        public float Lenght(double lat1, double lon1, double lat2, double lon2)
        {
            var f = Math.Sqrt(Math.Pow((lat2 - lat1), 2) + Math.Pow((lon2 - lon1), 2));
            return Convert.ToSingle(f);
        }
    }
}
