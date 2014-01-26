$(document).ready(function () {

    var map, markers, way;
    
    var controls = new Array();

    var redCircleStyle = {
        externalGraphic: '/Content/img/red-circle.png',
        graphicWidth: 25,
        graphicHeight: 25,
        graphicYOffset: -25,
    };
    var tradeAgentStyle = {
        externalGraphic: '/Content/img/tradeagent.png',
        graphicWidth: 40,
        graphicHeight: 40,
        graphicYOffset: -28,
    };

    // Спрятать/Показать маршрут (функционал не используется)
    /*
    $("input:radio").on('click', function (e) {
        if ($(this).val() == 'show') {
            way.setVisibility(true);
            markers.setVisibility(false);
        } else {
            way.setVisibility(false);
            markers.setVisibility(true);
        }
    });*/

    $("#clear").on('click', function (e) {
        markers && map.removeLayer(markers);
        controls['point'].deactivate();
        map.removeControl(controls['point']);

        $("#weight").text('');
        $('#clear').attr('disabled', 'disabled');
        $('#clear').fadeOut();
        $('#but').attr('disabled', 'disabled');

        way && map.removeLayer(way);
        addWayLayer();

        addMarkerLayer();
        addControlPoint();
    });

    $("#but").on('click', function (e) {
        var points = new Array();
        for (var i in markers.features) {
            points.push({ "id": markers.features[i].geometry.getVertices()[0].id, "lon": markers.features[i].geometry.getVertices()[0].x, "lat": markers.features[i].geometry.getVertices()[0].y });
        }
        if (points.length > 2) {
            jQuery.ajax({
                url: "/Home", //Адрес подгружаемой страницы
                type: "POST", //Тип запроса, 
                contentType: 'application/json',
                dataType: "JSON", //Тип данных
                data: JSON.stringify({ points: points }),
                traditional: true,
                success: function (response) { //Если все нормально
                    showWay(response);
                   // $("input:radio").removeAttr("disabled");
                },
                error: function (response) { //Если ошибка
                    //$("input:radio").attr("disabled", "disabled");
                    alert("Во время обработки запроса произошла ошибка");
                }
            });
        } else {
            alert("Слишком мало координат для расчета.");
        }
    });

    function init() {
        // Создаем карту, использовав в качестве контейнера элемент с id='map'
        map = new OpenLayers.Map('map');

        // Создаем слой с типом OSM и именем "OSM"
        var layer = new OpenLayers.Layer.OSM('OSM');

        // Добавляем слой на карту	
        map.addLayer(layer);

        // Москва 37.60, 55.694 долгота и ширина
        map.moveTo(
                 // Пересчитать координаты точки
            toProjectEPSG(new OpenLayers.LonLat(37.60, 55.694), map),
            12
        );

        // Создаем слой с маркерами
        addMarkerLayer();
        // Добавляем контроллер
        addControlPoint();
    }

    function showWay(way_points) {
        // Создаем слой с линиями маршрута
        way && map.removeLayer(way);
        addWayLayer();

        var points = Array();

        for (var i in way_points) {
            var lonlat = new OpenLayers.LonLat(way_points[i]['lon'], way_points[i]['lat']);
            var point = new OpenLayers.Geometry.Point(lonlat.lon, lonlat.lat);
            $("#weight").text(Math.round(way_points[i]['weight']/1000) + " км");
            //addPoint(point, i); // функция не используется
            points.push(point);
        }
        var line = new OpenLayers.Geometry.LineString(points);

        var style = {
            strokeColor: '#ff0000',
            strokeOpacity: 0.5,
            strokeWidth: 3
        };

        var lineFeature = new OpenLayers.Feature.Vector(line, null, style);
        way.addFeatures([lineFeature]);
    };

    // После первой поставленной точки, стили для последующей будут изменены
    function addNewPoint() {
        if (markers.features.length > 3)
            $('#but').removeAttr('disabled');
        else if (markers.features.length > 0) {
            $('#clear').removeAttr('disabled');
            $('#clear').fadeIn();
        } else {
            $('#but').attr('disabled', 'disabled');
            $('#clear').attr('disabled', 'disabled');
        }
        markers.style = redCircleStyle;
    }

    function addMarkerLayer() {
        // Создаем слой с маркерами, задаем стиль точки
        markers = new OpenLayers.Layer.Vector("Спрятать маршрут", {
            renderers: OpenLayers.Layer.Vector.prototype.renderers,
            style: tradeAgentStyle
        });
        map.addLayer(markers);
    }

    function addWayLayer() {
        // Создаем слой с линиями маршрута
        way = new OpenLayers.Layer.Vector("Отобразить маршрут", {
            renderers: OpenLayers.Layer.Vector.prototype.renderers
        });
        map.addLayer(way);
    }

    function addControlPoint() {
        // Контроллер
        controls['point'] = new OpenLayers.Control.DrawFeature(markers, OpenLayers.Handler.Point, {
            featureAdded: addNewPoint
        });
        // Добавляем контроллер
        map.addControl(controls['point']);

        // Делаем контроллер активным 
        controls['point'].activate();
    }

    function to4326(obj, map) {
        //из системы координат карты (EPSG:900913)
        //в систему координат EPSG:4326
        obj.transform(map.getProjectionObject(), 'EPSG:4326');
        return obj;
    }

    function toProjectEPSG(obj, map) {
        //из системы координат EPSG:4326
        //в систему координат карты (EPSG:900913)
        obj.transform('EPSG:4326', map.getProjectionObject());
        return obj;
    }

    // Добавление маркера при генерации пути (в текущий момент в нем нет необходимости)
    /*
    function addPoint(point, i) {
        var title;
        if (i == 0)
            title = "Торговый Агент"
        else
            title = "Пункт назначения #" + i;
        way.addFeatures(new OpenLayers.Feature.Vector(point, null, {
            label: title,
            name: title,
            //рисуем картинку
            externalGraphic: 'http://docs.gurtam.com/ru/hosting/_media/icons/event.png',
            //вот такой ширины
            graphicWidth: 24,
            //вот такой вышины *
            graphicHeight: 24,
            //сместив картинку на 8 пикселей влево 
            //относительно координат геометрии
            graphicXOffset: -8,
            //и на 16 пикселей вверх
            graphicYOffset: -16,
            //с базовой точкой текста подписи посередине-сверху текста
            labelAlign: 'ct',
            //сдвинув текст на 5 пикселей вниз
            labelYOffset: '-5',
            fontColor: "#000000",
            fontSize: "10px",
            fontFamily: "Courier New, monospace",
            fontWeight: "bold"
        }));
    }*/

    init();
});