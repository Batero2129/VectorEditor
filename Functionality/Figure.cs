﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;



namespace GraphicEditor.Functionality
{
    public abstract class Figure
    {
        public FigureType FigureType { get; protected set; } 
        public Point AnchorPoint { get; set; }
        public int StrokeWidth { get => GetStrokeWidth(); set => SetStrokeWidth(value); }
        public SolidColorBrush LineColor { get => GetLineColor(); set => SetLineColor(value); }
        public SolidColorBrush Fill { get => GetFill(); set => SetFill(value); }



        protected abstract int GetStrokeWidth(); //all
        protected abstract void SetStrokeWidth(int value); //all
        protected abstract SolidColorBrush GetFill(); //all
        protected abstract void SetFill(SolidColorBrush brush); //all
        protected abstract SolidColorBrush GetLineColor(); //all
        protected abstract void SetLineColor(SolidColorBrush colorBrush); //all
        public abstract void ShowOutline(); //all
        public abstract void HideOutline(); //all
        public abstract void MoveMarker(Point position); //all
        public abstract void ExecuteRelize(Point position); //all
        public abstract bool SelectMarker(Point poin); //all
        public abstract bool SelectLine(Point point); //all
        public abstract void DeselectFigure(); //all
        public abstract List<Rectangle> GetMarkers(); //all
        public abstract Polyline GetShape(); //all
        public virtual Rectangle ExecuteDoubleClick(Point position) //line
        {
            return null; 
        }



    }
}
