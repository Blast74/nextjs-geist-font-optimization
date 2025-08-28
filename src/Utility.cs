using System;
using System.Collections.Generic;
using System.Linq;

namespace YourNamespace
{
    public class Utility
    {
        /// <summary>
        /// Scales a value from one range to another range
        /// </summary>
        /// <param name="value">The value to scale</param>
        /// <param name="domainMin">Minimum value of the input domain</param>
        /// <param name="domainMax">Maximum value of the input domain</param>
        /// <param name="rangeMin">Minimum value of the output range</param>
        /// <param name="rangeMax">Maximum value of the output range</param>
        /// <returns>Scaled value</returns>
        public double ScaleValue(double value, double domainMin, double domainMax, double rangeMin, double rangeMax)
        {
            try
            {
                if (Math.Abs(domainMax - domainMin) < double.Epsilon)
                    return rangeMin;

                double ratio = (value - domainMin) / (domainMax - domainMin);
                return rangeMin + ratio * (rangeMax - rangeMin);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error scaling value: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Inverse scaling - converts from canvas coordinates back to data coordinates
        /// </summary>
        /// <param name="canvasValue">Canvas coordinate value</param>
        /// <param name="canvasMin">Minimum canvas coordinate</param>
        /// <param name="canvasMax">Maximum canvas coordinate</param>
        /// <param name="dataMin">Minimum data value</param>
        /// <param name="dataMax">Maximum data value</param>
        /// <returns>Data coordinate value</returns>
        public double InverseScaleValue(double canvasValue, double canvasMin, double canvasMax, double dataMin, double dataMax)
        {
            try
            {
                if (Math.Abs(canvasMax - canvasMin) < double.Epsilon)
                    return dataMin;

                double ratio = (canvasValue - canvasMin) / (canvasMax - canvasMin);
                return dataMin + ratio * (dataMax - dataMin);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error inverse scaling value: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Clamps a value between minimum and maximum bounds
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <param name="min">Minimum bound</param>
        /// <param name="max">Maximum bound</param>
        /// <returns>Clamped value</returns>
        public double Clamp(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        /// <summary>
        /// Generates a Gaussian peak for chromatography simulation
        /// </summary>
        /// <param name="timePoints">Array of time points</param>
        /// <param name="retentionTime">Peak retention time</param>
        /// <param name="peakWidth">Peak width (sigma)</param>
        /// <param name="intensity">Peak intensity</param>
        /// <returns>Array of intensity values</returns>
        public double[] GenerateGaussianPeak(double[] timePoints, double retentionTime, double peakWidth, double intensity)
        {
            try
            {
                double[] peakValues = new double[timePoints.Length];
                double variance = peakWidth * peakWidth;
                double normalizationFactor = intensity / (peakWidth * Math.Sqrt(2 * Math.PI));

                for (int i = 0; i < timePoints.Length; i++)
                {
                    double timeDiff = timePoints[i] - retentionTime;
                    peakValues[i] = normalizationFactor * Math.Exp(-(timeDiff * timeDiff) / (2 * variance));
                }

                return peakValues;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error generating Gaussian peak: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Combines multiple chromatographic peaks into a single chromatogram
        /// </summary>
        /// <param name="timePoints">Array of time points</param>
        /// <param name="retentionTimes">Array of retention times for each peak</param>
        /// <param name="peakWidths">Array of peak widths</param>
        /// <param name="intensities">Array of peak intensities</param>
        /// <returns>Combined chromatogram</returns>
        public double[] GenerateChromatogram(double[] timePoints, double[] retentionTimes, double[] peakWidths, double[] intensities)
        {
            try
            {
                if (retentionTimes.Length != peakWidths.Length || retentionTimes.Length != intensities.Length)
                    throw new ArgumentException("All peak parameter arrays must have the same length");

                double[] chromatogram = new double[timePoints.Length];

                for (int peakIndex = 0; peakIndex < retentionTimes.Length; peakIndex++)
                {
                    double[] peak = GenerateGaussianPeak(timePoints, retentionTimes[peakIndex], 
                                                        peakWidths[peakIndex], intensities[peakIndex]);
                    
                    for (int i = 0; i < chromatogram.Length; i++)
                    {
                        chromatogram[i] += peak[i];
                    }
                }

                return chromatogram;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error generating chromatogram: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Quick sort implementation for sorting arrays (from original QuickBasic code)
        /// </summary>
        /// <param name="array">Array to sort</param>
        /// <param name="indices">Optional array to track original indices</param>
        public void QuickSort(double[] array, int[] indices = null)
        {
            try
            {
                if (indices == null)
                {
                    indices = new int[array.Length];
                    for (int i = 0; i < array.Length; i++)
                        indices[i] = i;
                }

                QuickSortRecursive(array, indices, 0, array.Length - 1);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error in QuickSort: {ex.Message}", ex);
            }
        }

        private void QuickSortRecursive(double[] array, int[] indices, int low, int high)
        {
            if (low < high)
            {
                int pivotIndex = Partition(array, indices, low, high);
                QuickSortRecursive(array, indices, low, pivotIndex - 1);
                QuickSortRecursive(array, indices, pivotIndex + 1, high);
            }
        }

        private int Partition(double[] array, int[] indices, int low, int high)
        {
            double pivot = array[high];
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                if (array[j] <= pivot)
                {
                    i++;
                    Swap(array, i, j);
                    Swap(indices, i, j);
                }
            }

            Swap(array, i + 1, high);
            Swap(indices, i + 1, high);
            return i + 1;
        }

        private void Swap(double[] array, int i, int j)
        {
            double temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        private void Swap(int[] array, int i, int j)
        {
            int temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }

        /// <summary>
        /// Calculates the distance between two points
        /// </summary>
        /// <param name="x1">X coordinate of first point</param>
        /// <param name="y1">Y coordinate of first point</param>
        /// <param name="x2">X coordinate of second point</param>
        /// <param name="y2">Y coordinate of second point</param>
        /// <returns>Distance between points</returns>
        public double CalculateDistance(double x1, double y1, double x2, double y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Interpolates between two values
        /// </summary>
        /// <param name="value1">First value</param>
        /// <param name="value2">Second value</param>
        /// <param name="factor">Interpolation factor (0-1)</param>
        /// <returns>Interpolated value</returns>
        public double Interpolate(double value1, double value2, double factor)
        {
            factor = Clamp(factor, 0.0, 1.0);
            return value1 + factor * (value2 - value1);
        }

        /// <summary>
        /// Bilinear interpolation for 2D data
        /// </summary>
        /// <param name="x">X coordinate</param>
        /// <param name="y">Y coordinate</param>
        /// <param name="x1">X coordinate of point 1</param>
        /// <param name="y1">Y coordinate of point 1</param>
        /// <param name="x2">X coordinate of point 2</param>
        /// <param name="y2">Y coordinate of point 2</param>
        /// <param name="value11">Value at (x1, y1)</param>
        /// <param name="value12">Value at (x1, y2)</param>
        /// <param name="value21">Value at (x2, y1)</param>
        /// <param name="value22">Value at (x2, y2)</param>
        /// <returns>Interpolated value</returns>
        public double BilinearInterpolation(double x, double y, double x1, double y1, double x2, double y2,
                                          double value11, double value12, double value21, double value22)
        {
            try
            {
                if (Math.Abs(x2 - x1) < double.Epsilon || Math.Abs(y2 - y1) < double.Epsilon)
                    return value11;

                double fx = (x - x1) / (x2 - x1);
                double fy = (y - y1) / (y2 - y1);

                double interpolatedValue1 = Interpolate(value11, value21, fx);
                double interpolatedValue2 = Interpolate(value12, value22, fx);

                return Interpolate(interpolatedValue1, interpolatedValue2, fy);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error in bilinear interpolation: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validates that a numeric value is within acceptable bounds
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <param name="min">Minimum acceptable value</param>
        /// <param name="max">Maximum acceptable value</param>
        /// <param name="parameterName">Name of the parameter for error messages</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool ValidateNumericInput(double value, double min, double max, string parameterName)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new ArgumentException($"{parameterName} must be a valid number");
            }

            if (value < min || value > max)
            {
                throw new ArgumentOutOfRangeException(parameterName, 
                    $"{parameterName} must be between {min} and {max}");
            }

            return true;
        }

        /// <summary>
        /// Converts variable names to their corresponding type codes (from original QuickBasic code)
        /// </summary>
        /// <param name="variableName">Variable name string</param>
        /// <returns>Variable type code</returns>
        public int GetVariableTypeCode(string variableName)
        {
            if (string.IsNullOrEmpty(variableName))
                return 0;

            string upperName = variableName.ToUpper();

            if (upperName.Contains("TEMP") || upperName == "T")
                return 1; // Temperature

            if (upperName == "PH")
                return 2; // pH

            if (upperName.Contains("GRADIENT") && upperName.Contains("TIME") || upperName == "TGRAD")
                return 3; // Gradient time

            if (upperName.Contains("FLOW") && upperName.Contains("RATE") || upperName == "FRATE")
                return 4; // Flow rate

            if (upperName.Contains("IONIC") || upperName.Contains("BUFFER"))
                return 5; // Ionic strength

            if (upperName.Contains("GRADIENT") && upperName.Contains("SLOPE") || upperName == "GS")
                return 6; // Gradient slope

            if (upperName == "TGS" || upperName.Contains("TEMP") && upperName.Contains("GRAD"))
                return 7; // Temperature gradient slope

            if (upperName.Contains("FLOW") && upperName.Contains("GRAD"))
                return 8; // Flow rate gradient slope

            if (upperName == "%B" || upperName.Contains("ISOCRATIC"))
                return 9; // Percent B

            return 0; // Unknown
        }

        /// <summary>
        /// Gets the display name for a variable type
        /// </summary>
        /// <param name="variableType">Variable type code</param>
        /// <returns>Display name</returns>
        public string GetVariableDisplayName(int variableType)
        {
            switch (variableType)
            {
                case 1: return "T(°C)";
                case 2: return "pH";
                case 3: return "tG(min)";
                case 4: return "f-rate(ml/min)";
                case 5: return "ionic strength";
                case 6: return "Gs(1/min)";
                case 7: return "TgS(°C/min)";
                case 8: return "f-rate-Gs(ml/min²)";
                case 9: return "%B";
                default: return "Unknown";
            }
        }
    }
}
