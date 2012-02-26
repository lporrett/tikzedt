﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;
using TikzEdt.Parser;

namespace TikzEdt
{
   /// <summary>
   /// The code here is not functional yet
   /// </summary>
   
   
    class RectangleTool : OverlayAdderTool
    {
        // point where the rectangle originates, in screen coordinates
        Point origin;

        // the rectangle to be shown on drawing
        Rectangle PreviewRect = new Rectangle();

        public RectangleTool()
        {
            PreviewRect.Visibility = Visibility.Collapsed;
            PreviewRect.Stroke = Brushes.Black;

        }

        public override void OnActivate()
        {
            base.OnActivate();
            overlay.canvas.Cursor = Cursors.Cross;
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            PreviewRect.Visibility = Visibility.Collapsed;
        }

        public override void OnLeftMouseButtonDown(OverlayShape item, Point p, MouseButtonEventArgs e)
        {
            // initiate drawing process
            Point mousep = overlay.Rasterizer.RasterizePixel(p);
            origin = new Point(mousep.X, overlay.Height - mousep.Y);
            
            Canvas.SetLeft(PreviewRect, origin.X);
            Canvas.SetTop(PreviewRect, origin.Y);
            PreviewRect.Width = PreviewRect.Height = 0;
            if (!overlay.canvas.Children.Contains(PreviewRect))
                overlay.canvas.Children.Add(PreviewRect);
            PreviewRect.Visibility = Visibility.Visible;

            if (!overlay.canvas.IsMouseCaptured)
                overlay.canvas.CaptureMouse();
        }
        public override void OnLeftMouseButtonUp(MouseButtonEventArgs e, Point p)
        {
            // add the rectangle
            if (PreviewRect.Visibility == Visibility.Visible)
            {
                if (!EnsureParseTreeExists())
                    return;

                Point firstpoint = overlay.ScreenToTikz(origin, true);
                Point secondpoint = overlay.Rasterizer.RasterizePixelToTikz(p);

                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    // both sides the same
                    double sidelength = Math.Max(Math.Abs(firstpoint.X-secondpoint.X), Math.Abs(firstpoint.Y-secondpoint.Y));
                    secondpoint = new Point(
                        firstpoint.X + Math.Sign(secondpoint.X-firstpoint.X) * sidelength,
                        firstpoint.Y + Math.Sign(secondpoint.Y-firstpoint.Y) * sidelength );
                }

                overlay.BeginUpdate();

                if (AddNewCurAddTo())
                {
                    Parser.Tikz_Coord tc1 = new Parser.Tikz_Coord();
                    tc1.type = Parser.Tikz_CoordType.Cartesian;
                    Parser.Tikz_Coord tc2 = new Parser.Tikz_Coord();
                    tc2.type = Parser.Tikz_CoordType.Cartesian;

                    curAddTo.AddChild(new Parser.Tikz_Something(" "));
                    curAddTo.AddChild(tc1);
                    curAddTo.AddChild(new Parser.Tikz_Something(" rectangle "));
                    curAddTo.AddChild(tc2);

                    tc1.SetAbsPos(firstpoint);
                    tc2.SetAbsPos(secondpoint);
                    
                    overlay.AddToDisplayTree(tc1);
                    overlay.AddToDisplayTree(tc2);

                    curAddTo.UpdateText();
                }

                overlay.EndUpdate();

                PreviewRect.Visibility = Visibility.Collapsed;
            }
        }



        public override void OnMouseMove(Point p, MouseEventArgs e)
        {
            base.OnMouseMove(p, e);

            //Point mousep = e.GetPosition(overlay.canvas);
            Point mousep = overlay.Rasterizer.RasterizePixel(p);
            mousep = new Point(mousep.X, overlay.canvas.ActualHeight - mousep.Y);
            if (PreviewRect.Visibility == Visibility.Visible)
            {
                // update the size of the selection rect
                double x = Math.Min(mousep.X, origin.X),
                       y = Math.Min(mousep.Y, origin.Y);
                //SelectionRect.RenderTransform = new TranslateTransform(x, y);
                Canvas.SetLeft(PreviewRect, x);
                Canvas.SetTop(PreviewRect, y);
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                {
                    // both sides the same
                    PreviewRect.Width = PreviewRect.Height = Math.Max(Math.Abs(mousep.X - origin.X), Math.Abs(mousep.Y - origin.Y));
                }
                else
                {
                    PreviewRect.Width = Math.Abs(mousep.X - origin.X);
                    PreviewRect.Height = Math.Abs(mousep.Y - origin.Y);
                }
            }
        }
    }

    class EllipseTool : OverlayAdderTool
    {
        // point where the rectangle originates, in screen coordinates
        Point origin;

        // the rectangle to be shown on drawing
        Ellipse PreviewEllipse = new Ellipse();

        public EllipseTool()
        {
            PreviewEllipse.Visibility = Visibility.Collapsed;
            PreviewEllipse.Stroke = Brushes.Black;

        }

        public override void OnActivate()
        {
            base.OnActivate();
            overlay.canvas.Cursor = Cursors.Cross;
        }
        public override void OnDeactivate()
        {
            base.OnDeactivate();
            PreviewEllipse.Visibility = Visibility.Collapsed;
        }

        public override void OnLeftMouseButtonDown(OverlayShape item, Point p, MouseButtonEventArgs e)
        {
            // initiate drawing process
            Point mousep = overlay.Rasterizer.RasterizePixel(p);
            origin = new Point(mousep.X, overlay.Height - mousep.Y);


            Canvas.SetLeft(PreviewEllipse, origin.X);
            Canvas.SetTop(PreviewEllipse, origin.Y);
            PreviewEllipse.Width = PreviewEllipse.Height = 0;
            if (!overlay.canvas.Children.Contains(PreviewEllipse))
                overlay.canvas.Children.Add(PreviewEllipse);
            PreviewEllipse.Visibility = Visibility.Visible;

            if (!overlay.canvas.IsMouseCaptured)
                overlay.canvas.CaptureMouse();
        }
        public override void OnLeftMouseButtonUp(MouseButtonEventArgs e, Point p)
        {
            // add the rectangle
            if (PreviewEllipse.Visibility == Visibility.Visible)
            {
                if (!EnsureParseTreeExists())
                    return;

                Point center = overlay.ScreenToTikz(origin, true);
                Point secondpoint = overlay.Rasterizer.RasterizePixelToTikz(p);

                double width = Math.Abs(center.X - secondpoint.X),   // actually half the width/height!
                       height = Math.Abs(center.Y - secondpoint.Y);
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    width = height = Math.Max(width, height);
                


                overlay.BeginUpdate();

                if (AddNewCurAddTo())
                {
                    Parser.Tikz_Coord tc1 = new Parser.Tikz_Coord();
                    tc1.type = Parser.Tikz_CoordType.Cartesian;

                    curAddTo.AddChild(new Parser.Tikz_Something(" "));
                    curAddTo.AddChild(tc1);
                    // since there might be a coordinate tranform, the width/height have to be transformed
                    // (note: rotates are not very well supported here) 
                    TikzMatrix M;
                    if (curAddTo.GetCurrentTransformAt(tc1, out M))
                    {
                        Point size_trans = M.Inverse().Transform(new Point(width, height), true);
                        width = size_trans.X;
                        height = size_trans.Y;
                    }
                    else
                    {
                        MainWindow.AddStatusLine("Warning: Couldn't compute tranform at insertion position. Coordinates might be wrong.", true);
                    }

                    width = Math.Round(width, (int)Properties.Settings.Default.RoundToDecimals);
                    height = Math.Round(height, (int)Properties.Settings.Default.RoundToDecimals);

                    if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control) && width == height)
                        curAddTo.AddChild(new Parser.Tikz_Something(" circle (" + width + ")" ));
                    else
                        curAddTo.AddChild(new Parser.Tikz_Something(" ellipse (" + width + " and " + height + ")"));

                    tc1.SetAbsPos(center);

                    overlay.AddToDisplayTree(tc1);

                    curAddTo.UpdateText();
                }

                overlay.EndUpdate();

                PreviewEllipse.Visibility = Visibility.Collapsed;
            }
        }



        public override void OnMouseMove(Point p, MouseEventArgs e)
        {
            base.OnMouseMove(p, e);

            //Point mousep = e.GetPosition(overlay.canvas);
            Point mousep = overlay.Rasterizer.RasterizePixel(p);
            mousep = new Point(mousep.X, overlay.canvas.ActualHeight - mousep.Y);
            if (PreviewEllipse.Visibility == Visibility.Visible)
            {
                double width = Math.Abs(mousep.X - origin.X),   // actually half the width/height!
                       height = Math.Abs(mousep.Y - origin.Y);
                if (Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
                    width = height = Math.Max(width, height);

                Canvas.SetLeft(PreviewEllipse, origin.X - width);
                Canvas.SetTop(PreviewEllipse, origin.Y - height);

                PreviewEllipse.Width = 2*width;
                PreviewEllipse.Height = 2*height;

            }
        }
    }
}