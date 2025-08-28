using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace YourNamespace
{
    public class GraphicsRenderer
    {
        private Canvas _canvas;
        private DataModel _dataModel;
        private Utility _utility;

        public GraphicsRenderer(Canvas canvas, DataModel dataModel)
        {
            _canvas = canvas;
            _dataModel = dataModel;
            _utility = new Utility();
        }

        public void ClearCanvas()
        {
            _canvas.Children.Clear();
        }

        public void DrawMeasurementFrame(double x1, double y1, double x2, double y2, double currentZ)
        {
            try
            {
                // Convert data coordinates to canvas coordinates
                double canvasX1 = _utility.ScaleValue(x1, _dataModel.Parameters.XMin, _dataModel.Parameters.XMax, 0, _canvas.ActualWidth);
                double canvasY1 = _utility.ScaleValue(y1, _dataModel.Parameters.YMin, _dataModel.Parameters.YMax, _canvas.ActualHeight, 0);
                double canvasX2 = _utility.ScaleValue(x2, _dataModel.Parameters.XMin, _dataModel.Parameters.XMax, 0, _canvas.ActualWidth);
                double canvasY2 = _utility.ScaleValue(y2, _dataModel.Parameters.YMin, _dataModel.Parameters.YMax, _canvas.ActualHeight, 0);

                // Determine color based on Z value
                Brush frameBrush;
                Brush circleBrush;
                
                if (Math.Abs(currentZ - _dataModel.Parameters.ZMin) < 0.001 || 
                    Math.Abs(currentZ - _dataModel.Parameters.ZMax) < 0.001)
                {
                    frameBrush = Brushes.White;
                    circleBrush = Brushes.White;
                }
                else
                {
                    frameBrush = Brushes.Gray;
                    circleBrush = Brushes.Gray;
                }

                // Draw corner circles
                DrawCircle(canvasX1, canvasY1, 4, circleBrush, true);
                DrawCircle(canvasX2, canvasY1, 4, circleBrush, true);
                DrawCircle(canvasX1, canvasY2, 4, circleBrush, true);
                DrawCircle(canvasX2, canvasY2, 4, circleBrush, true);

                // Draw frame rectangle
                Rectangle frame = new Rectangle
                {
                    Width = Math.Abs(canvasX2 - canvasX1),
                    Height = Math.Abs(canvasY2 - canvasY1),
                    Stroke = frameBrush,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent
                };

                Canvas.SetLeft(frame, Math.Min(canvasX1, canvasX2));
                Canvas.SetTop(frame, Math.Min(canvasY1, canvasY2));
                _canvas.Children.Add(frame);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error drawing measurement frame: {ex.Message}", ex);
            }
        }

        public void DrawTargetMarker(double x, double y)
        {
            try
            {
                // Convert data coordinates to canvas coordinates
                double canvasX = _utility.ScaleValue(x, _dataModel.Parameters.XMin, _dataModel.Parameters.XMax, 0, _canvas.ActualWidth);
                double canvasY = _utility.ScaleValue(y, _dataModel.Parameters.YMin, _dataModel.Parameters.YMax, _canvas.ActualHeight, 0);

                // Ensure marker stays within canvas bounds
                canvasX = Math.Max(5, Math.Min(_canvas.ActualWidth - 5, canvasX));
                canvasY = Math.Max(5, Math.Min(_canvas.ActualHeight - 5, canvasY));

                // Draw target background square
                Rectangle targetBg = new Rectangle
                {
                    Width = 8,
                    Height = 8,
                    Fill = Brushes.White,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                Canvas.SetLeft(targetBg, canvasX - 4);
                Canvas.SetTop(targetBg, canvasY - 4);
                _canvas.Children.Add(targetBg);

                // Draw center circle
                DrawCircle(canvasX, canvasY, 2, Brushes.Black, true);

                // Draw crosshairs
                Line horizontalLine1 = new Line
                {
                    X1 = canvasX - 4, Y1 = canvasY,
                    X2 = canvasX - 2, Y2 = canvasY,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                Line horizontalLine2 = new Line
                {
                    X1 = canvasX + 2, Y1 = canvasY,
                    X2 = canvasX + 4, Y2 = canvasY,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                Line verticalLine1 = new Line
                {
                    X1 = canvasX, Y1 = canvasY - 4,
                    X2 = canvasX, Y2 = canvasY - 2,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                Line verticalLine2 = new Line
                {
                    X1 = canvasX, Y1 = canvasY + 2,
                    X2 = canvasX, Y2 = canvasY + 4,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };

                _canvas.Children.Add(horizontalLine1);
                _canvas.Children.Add(horizontalLine2);
                _canvas.Children.Add(verticalLine1);
                _canvas.Children.Add(verticalLine2);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error drawing target marker: {ex.Message}", ex);
            }
        }

        public void DrawCrosshairs(double x, double y)
        {
            try
            {
                double canvasX = _utility.ScaleValue(x, _dataModel.Parameters.XMin, _dataModel.Parameters.XMax, 0, _canvas.ActualWidth);
                double canvasY = _utility.ScaleValue(y, _dataModel.Parameters.YMin, _dataModel.Parameters.YMax, _canvas.ActualHeight, 0);

                // Draw horizontal crosshair
                Line horizontalCrosshair = new Line
                {
                    X1 = 0, Y1 = canvasY,
                    X2 = _canvas.ActualWidth, Y2 = canvasY,
                    Stroke = Brushes.LightBlue,
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 5, 5 }
                };

                // Draw vertical crosshair
                Line verticalCrosshair = new Line
                {
                    X1 = canvasX, Y1 = 0,
                    X2 = canvasX, Y2 = _canvas.ActualHeight,
                    Stroke = Brushes.Yellow,
                    StrokeThickness = 1,
                    StrokeDashArray = new DoubleCollection { 5, 5 }
                };

                _canvas.Children.Add(horizontalCrosshair);
                _canvas.Children.Add(verticalCrosshair);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error drawing crosshairs: {ex.Message}", ex);
            }
        }

        public void DrawSpectrum(double[] timePoints, double[] intensities, double[] retentionTimes)
        {
            try
            {
                if (timePoints.Length != intensities.Length)
                    throw new ArgumentException("Time points and intensities arrays must have the same length");

                // Find min/max values for scaling
                double minTime = double.MaxValue;
                double maxTime = double.MinValue;
                double minIntensity = 0;
                double maxIntensity = double.MinValue;

                foreach (double time in timePoints)
                {
                    if (time < minTime) minTime = time;
                    if (time > maxTime) maxTime = time;
                }

                foreach (double intensity in intensities)
                {
                    if (intensity > maxIntensity) maxIntensity = intensity;
                }

                // Draw spectrum curve
                Polyline spectrumCurve = new Polyline
                {
                    Stroke = Brushes.Red,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent
                };

                for (int i = 0; i < timePoints.Length; i++)
                {
                    double canvasX = _utility.ScaleValue(timePoints[i], minTime, maxTime, 0, _canvas.ActualWidth);
                    double canvasY = _utility.ScaleValue(intensities[i], minIntensity, maxIntensity, _canvas.ActualHeight, 0);
                    spectrumCurve.Points.Add(new Point(canvasX, canvasY));
                }

                _canvas.Children.Add(spectrumCurve);

                // Draw retention time markers
                DrawRetentionTimeMarkers(retentionTimes, minTime, maxTime, maxIntensity);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error drawing spectrum: {ex.Message}", ex);
            }
        }

        private void DrawRetentionTimeMarkers(double[] retentionTimes, double minTime, double maxTime, double maxIntensity)
        {
            for (int i = 0; i < retentionTimes.Length; i++)
            {
                if (retentionTimes[i] >= minTime && retentionTimes[i] <= maxTime)
                {
                    double canvasX = _utility.ScaleValue(retentionTimes[i], minTime, maxTime, 0, _canvas.ActualWidth);
                    
                    // Determine color based on peak overlap
                    Brush markerBrush = Brushes.Blue;
                    for (int j = 0; j < retentionTimes.Length; j++)
                    {
                        if (i != j && Math.Abs(retentionTimes[i] - retentionTimes[j]) < 0.1)
                        {
                            markerBrush = Brushes.Orange; // Overlapping peaks
                            break;
                        }
                    }

                    // Draw vertical line from bottom to peak
                    Line retentionMarker = new Line
                    {
                        X1 = canvasX, Y1 = _canvas.ActualHeight,
                        X2 = canvasX, Y2 = 0,
                        Stroke = markerBrush,
                        StrokeThickness = 2,
                        StrokeDashArray = new DoubleCollection { 3, 3 }
                    };

                    _canvas.Children.Add(retentionMarker);
                }
            }
        }

        public void DrawScale(double minX, double maxX, double minY, double maxY)
        {
            try
            {
                // Draw X-axis scale
                for (double x = Math.Ceiling(minX); x <= Math.Floor(maxX); x++)
                {
                    double canvasX = _utility.ScaleValue(x, minX, maxX, 0, _canvas.ActualWidth);
                    
                    if (canvasX >= 0 && canvasX <= _canvas.ActualWidth)
                    {
                        Line tickMark = new Line
                        {
                            X1 = canvasX, Y1 = _canvas.ActualHeight - 5,
                            X2 = canvasX, Y2 = _canvas.ActualHeight,
                            Stroke = Brushes.Black,
                            StrokeThickness = 1
                        };
                        _canvas.Children.Add(tickMark);

                        // Add scale label
                        TextBlock label = new TextBlock
                        {
                            Text = x.ToString("F0"),
                            FontSize = 10,
                            Foreground = Brushes.Black
                        };
                        Canvas.SetLeft(label, canvasX - 10);
                        Canvas.SetTop(label, _canvas.ActualHeight - 20);
                        _canvas.Children.Add(label);
                    }
                }

                // Draw Y-axis scale
                for (double y = Math.Ceiling(minY); y <= Math.Floor(maxY); y++)
                {
                    double canvasY = _utility.ScaleValue(y, minY, maxY, _canvas.ActualHeight, 0);
                    
                    if (canvasY >= 0 && canvasY <= _canvas.ActualHeight)
                    {
                        Line tickMark = new Line
                        {
                            X1 = 0, Y1 = canvasY,
                            X2 = 5, Y2 = canvasY,
                            Stroke = Brushes.Black,
                            StrokeThickness = 1
                        };
                        _canvas.Children.Add(tickMark);

                        // Add scale label
                        TextBlock label = new TextBlock
                        {
                            Text = y.ToString("F0"),
                            FontSize = 10,
                            Foreground = Brushes.Black
                        };
                        Canvas.SetLeft(label, 10);
                        Canvas.SetTop(label, canvasY - 7);
                        _canvas.Children.Add(label);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error drawing scale: {ex.Message}", ex);
            }
        }

        public void DrawResolutionMap(double[,] resolutionData, double deltaR, double maxResolution)
        {
            try
            {
                int width = resolutionData.GetLength(0);
                int height = resolutionData.GetLength(1);

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        double resolution = resolutionData[i, j];
                        
                        if (resolution > 0)
                        {
                            // Map resolution to color
                            Brush pixelBrush = GetResolutionColor(resolution, deltaR, maxResolution);
                            
                            Rectangle pixel = new Rectangle
                            {
                                Width = _canvas.ActualWidth / width,
                                Height = _canvas.ActualHeight / height,
                                Fill = pixelBrush
                            };

                            Canvas.SetLeft(pixel, i * _canvas.ActualWidth / width);
                            Canvas.SetTop(pixel, j * _canvas.ActualHeight / height);
                            _canvas.Children.Add(pixel);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error drawing resolution map: {ex.Message}", ex);
            }
        }

        private Brush GetResolutionColor(double resolution, double deltaR, double maxResolution)
        {
            // Color mapping based on resolution levels (similar to original QuickBasic code)
            if (resolution >= maxResolution - deltaR) return Brushes.Red;
            if (resolution >= maxResolution - 2 * deltaR) return Brushes.LightBlue;
            if (resolution >= maxResolution - 3 * deltaR) return Brushes.Magenta;
            if (resolution >= maxResolution - 4 * deltaR) return Brushes.LightGreen;
            if (resolution >= maxResolution - 5 * deltaR) return Brushes.Blue;
            if (resolution >= maxResolution - 6 * deltaR) return Brushes.Cyan;
            if (resolution >= maxResolution - 7 * deltaR) return Brushes.DarkBlue;
            if (resolution >= maxResolution - 8 * deltaR) return Brushes.LightGray;
            if (resolution >= maxResolution - 9 * deltaR) return Brushes.Brown;
            if (resolution >= maxResolution - 10 * deltaR) return Brushes.Gray;
            return Brushes.DarkGray;
        }

        private void DrawCircle(double centerX, double centerY, double radius, Brush brush, bool filled)
        {
            Ellipse circle = new Ellipse
            {
                Width = radius * 2,
                Height = radius * 2,
                Stroke = brush,
                StrokeThickness = 1
            };

            if (filled)
                circle.Fill = brush;
            else
                circle.Fill = Brushes.Transparent;

            Canvas.SetLeft(circle, centerX - radius);
            Canvas.SetTop(circle, centerY - radius);
            _canvas.Children.Add(circle);
        }
    }
}
