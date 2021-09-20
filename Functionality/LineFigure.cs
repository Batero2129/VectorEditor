﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GraphicEditor.Functionality
{
    class LineFigure : Figure
    {
        private List<MarkerPoint> markers;
        private Polyline polyline;
        private MarkerPoint selectedMarker;

        public LineFigure(Polyline line)
        {
            CreatePolyline();
            DefinePolyline(line);
            DefineMarkerPoints();
        }
        public LineFigure(Point firstPoint, Point secondPoint)
        {
            CreatePolyline(firstPoint, secondPoint);
            DefineMarkerPoints();
        }
        public LineFigure (SLFigure sLFigure)
        {
            CreatePolyline();
            DefinePolyline(sLFigure.Polyline.ParsePolylineFromArray());
            polyline.StrokeThickness = Convert.ToInt32(sLFigure.LineStrokeThinkness);
            DefineMarkerPoints();
            LineColor = new SolidColorBrush
            {
                Color = (Color)ColorConverter.ConvertFromString(sLFigure.LineColor)
            };
        }


        private void CreatePolyline()
        {
            FigureType = FigureType.Line;
            markers = new List<MarkerPoint>();
            polyline = new Polyline();
            polyline.StrokeEndLineCap = PenLineCap.Round;
            polyline.StrokeLineJoin = PenLineJoin.Round;
        }
        private void CreatePolyline(Point firstPoint, Point secondPoint)
        {
            FigureType = FigureType.Line;
            markers = new List<MarkerPoint>();
            polyline = new Polyline();
            polyline.Fill = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            polyline.StrokeEndLineCap = PenLineCap.Square;
            polyline.StrokeLineJoin = PenLineJoin.Round;
            polyline.Points.Add(firstPoint);
            polyline.Points.Add(secondPoint);
        }
        private void RefreshMarkerPoints()
        {
            for (int i = 0; i < markers.Count; i++)
            {
                markers[i].SetMarkerSize(StrokeWidth);
                markers[i].Move(polyline.Points[i]);
            }
        }
        private void DefinePolyline(Polyline line)
        {
            foreach (var point in line.Points)
            {
                polyline.Points.Add(point);
            }
        }
        private void DefineMarkerPoints()
        {
            foreach (var point in polyline.Points)
            {
                markers.Add(new MarkerPoint(point));
            }
            polyline.StrokeThickness = StrokeWidth;
            polyline.Visibility = Visibility.Visible;
            polyline.Stroke = Brushes.Black;
        }
        protected override int GetStrokeWidth()
        {
            return (int)polyline.StrokeThickness;
        }
        protected override void SetStrokeWidth(int value)
        {
            polyline.StrokeThickness = value;
            RefreshMarkerPoints();
        }
        protected override SolidColorBrush GetFill()
        {
            MessageBox.Show("Невозможно получить Кисть заливки!!!");
            return null;
        }
        protected override void SetFill(SolidColorBrush brush)
        {
            MessageBox.Show("Невозможно залить Линию!!!");
        }
        protected override SolidColorBrush GetLineColor()
        {
            SolidColorBrush brush = (SolidColorBrush)polyline.Stroke;
            return brush;
        }
        protected override void SetLineColor(SolidColorBrush colorBrush)
        {
            polyline.Stroke = colorBrush;
        }
        internal Polyline GetPolyline()
        {
            return polyline;
        }
        internal List<MarkerPoint> GetMarkerPoints()
        {
            return markers;
        }
        public override void ShowOutline()
        {
            foreach (var marker in markers)
            {
                marker.Show();
            }
        }
        public override void HideOutline()
        {
            foreach (var marker in markers)
            {
                marker.Hide();
            }
        }
        public override bool SelectMarker(Point point)
        {
            foreach (var marker in markers)
            {
                bool result = marker.Point.ItInsideCircle(point, StrokeWidth);
                if (result)
                {
                    selectedMarker = marker;
                    ShowOutline();
                    return true;
                }
                else
                {
                    selectedMarker = null;
                }
            }
            return false;
        }
        public override void MoveMarker(Point position)
        {
            for (int i = 0; i < markers.Count; i++)
            {
                if (markers[i].Equals(selectedMarker))
                {
                    markers[i].Move(position);
                    polyline.Points[i] = position;
                }
            }
        }
        public override void ExecuteRelize(Point position)
        {
            if (selectedMarker != null)
            {
                for (int i = 0; i < polyline.Points.Count; i++)
                {
                    if (selectedMarker.Point.ItInsideCircle(polyline.Points[i], StrokeWidth) && !selectedMarker.Equals(markers[i]))
                    {
                        selectedMarker.Hide();
                        markers.Remove(selectedMarker);
                        polyline.Points.RemoveAt(i);
                        selectedMarker = null;
                        break;
                    }
                }
                if (polyline.Points.Count == 1)
                {
                    polyline.Points.Clear();
                    polyline.Visibility = Visibility.Hidden;
                    markers[0].Hide();
                    markers.Clear();
                }
            }
        }
        public override Rectangle ExecuteDoubleClick(Point position)
        {
            for (int i = 0; i <polyline.Points.Count-1; i++)
            {
                Point A = polyline.Points[i];
                Point B = polyline.Points[i + 1];
                if(position.ItIntersect(A,B, StrokeWidth))
                {
                    polyline.Points.Insert(i+1, position);
                    MarkerPoint marker = new MarkerPoint(position);
                    marker.SetMarkerSize(StrokeWidth);
                    markers.Insert(i+1, marker);
                    marker.Show();
                    //canvas.Children.Add(marker.Marker);
                    return marker.Marker;
                }
            }
            return null;
        }
        public override bool SelectLine(Point point)
        {
            for (int i = 0; i < markers.Count - 1; i++)
            {
                if (point.AbsAngleBetweenPoints(markers[i].Point, markers[i + 1].Point) > 60)
                {
                    double A = point.Length(markers[i + 1].Point);
                    double B = point.Length(markers[i].Point);
                    double C = markers[i].Point.Length(markers[i + 1].Point);
                    double p = (A + B + C) / 2;
                    double S = Math.Sqrt(p * (p - A) * (p - B) * (p - C));
                    double h = 2 * S / C;
                    if (h <= 10 + StrokeWidth)
                    {
                        return true;
                    }
                }

            }
            return false;
        }
        public override void DeselectFigure()
        {
            HideOutline();
            selectedMarker = null;
        }
        public override List<Rectangle> GetMarkers()
        {
            List<Rectangle> rects = new List<Rectangle>();
            foreach (var marker in markers)
            {
                rects.Add(marker.Marker);
            }
            return rects;
        }
        public override Polyline GetShape()
        {
            return polyline;
        }
    }
}