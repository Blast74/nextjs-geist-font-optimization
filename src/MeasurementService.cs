using System;
using System.Collections.Generic;

namespace YourNamespace
{
    public class MeasurementService
    {
        private DataModel _dataModel;

        public MeasurementService(DataModel dataModel)
        {
            _dataModel = dataModel;
        }

        public CalculationResults ComputeParameters()
        {
            try
            {
                // Validate input parameters
                if (_dataModel.Parameters.ColumnLength <= 0 || 
                    _dataModel.Parameters.ColumnDiameter <= 0 || 
                    _dataModel.Parameters.FlowRate <= 0)
                {
                    throw new ArgumentException("Invalid parameter values");
                }

                var results = new CalculationResults();
                
                // Calculate mobile phase volume
                double radius = _dataModel.Parameters.ColumnDiameter / 2.0;
                results.MobilePhaseVolume = Math.PI * radius * radius * _dataModel.Parameters.ColumnLength * (2.0 / 3.0);
                
                // Calculate t0 (dead time)
                results.DeadTime = results.MobilePhaseVolume / _dataModel.Parameters.FlowRate;
                
                // Calculate linear velocity
                results.LinearVelocity = _dataModel.Parameters.ColumnLength / results.DeadTime;

                return results;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error in parameter computation: {ex.Message}", ex);
            }
        }

        public void ComputeLinearisation(int variableType, double range1, double range2, 
            double[] values1, double[] values2, out double[] coefficientsA, out double[] coefficientsB)
        {
            try
            {
                int componentCount = values1.Length;
                coefficientsA = new double[componentCount];
                coefficientsB = new double[componentCount];

                double[] transformedRange1 = new double[componentCount];
                double[] transformedRange2 = new double[componentCount];

                // Transform ranges based on variable type
                for (int i = 0; i < componentCount; i++)
                {
                    switch (variableType)
                    {
                        case 1: // Temperature
                            transformedRange1[i] = 1.0 / (range1 + 273.15); // Convert to Kelvin and invert
                            transformedRange2[i] = 1.0 / (range2 + 273.15);
                            break;
                        case 2: // pH
                            transformedRange1[i] = Math.Pow(10, -range1) / (Math.Pow(10, -range1) + Math.Pow(10, -_dataModel.Parameters.PKa[i]));
                            transformedRange2[i] = Math.Pow(10, -range2) / (Math.Pow(10, -range2) + Math.Pow(10, -_dataModel.Parameters.PKa[i]));
                            break;
                        default: // Linear variables (gradient time, flow rate, etc.)
                            transformedRange1[i] = range1;
                            transformedRange2[i] = range2;
                            break;
                    }

                    // Calculate linear regression coefficients
                    coefficientsA[i] = (values1[i] - values2[i]) / (transformedRange1[i] - transformedRange2[i]);
                    coefficientsB[i] = values1[i] - coefficientsA[i] * transformedRange1[i];
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error in linearisation computation: {ex.Message}", ex);
            }
        }

        public void ComputeRetentionFactors(int componentCount, double xxx, double yyy, double zzz, 
            out double[] retentionFactors, out double[] retentionTimes, out double[] peakWidths)
        {
            try
            {
                retentionFactors = new double[componentCount];
                retentionTimes = new double[componentCount];
                peakWidths = new double[componentCount];

                // Transform variables based on type
                double transformedX = TransformVariable(xxx, _dataModel.Parameters.VariableTypeX);
                double transformedY = TransformVariable(yyy, _dataModel.Parameters.VariableTypeY);
                double transformedZ = TransformVariable(zzz, _dataModel.Parameters.VariableTypeZ);

                for (int i = 0; i < componentCount; i++)
                {
                    // Calculate retention factor based on number of variables
                    double logK = CalculateLogRetentionFactor(i, transformedX, transformedY, transformedZ);
                    
                    // Prevent overflow
                    if (logK > Math.Log(1E32))
                        logK = Math.Log(1E32);
                    
                    retentionFactors[i] = Math.Exp(logK);
                    
                    // Calculate retention time
                    retentionTimes[i] = _dataModel.Parameters.DeadTimeExperimental * retentionFactors[i] + _dataModel.Parameters.DeadTimeExperimental;
                    
                    // Calculate peak width (sigma)
                    peakWidths[i] = Math.Sqrt(2.0 / _dataModel.Parameters.PlateNumber) * retentionTimes[i] / 2.5;
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error in retention factor computation: {ex.Message}", ex);
            }
        }

        private double TransformVariable(double value, int variableType)
        {
            switch (variableType)
            {
                case 1: // Temperature
                    return 1.0 / (value + 273.15);
                case 2: // pH
                    return 1.0 / (1.0 + Math.Pow(10, -_dataModel.Parameters.PKa[0] + value));
                default: // Linear variables
                    return value;
            }
        }

        private double CalculateLogRetentionFactor(int componentIndex, double x, double y, double z)
        {
            // This would use the linearisation coefficients calculated earlier
            // Simplified implementation - in reality this would use the complex multi-dimensional interpolation
            // from the original QuickBasic code
            
            switch (_dataModel.Parameters.NumberOfVariables)
            {
                case 1:
                    return _dataModel.Parameters.CoefficientsA1[componentIndex] * x + _dataModel.Parameters.CoefficientsB1[componentIndex];
                case 2:
                    return (_dataModel.Parameters.CoefficientsAA1[componentIndex] * y + _dataModel.Parameters.CoefficientsAB1[componentIndex]) * x +
                           _dataModel.Parameters.CoefficientsBA1[componentIndex] * y + _dataModel.Parameters.CoefficientsB1[componentIndex];
                case 3:
                    // Complex 3D interpolation - simplified version
                    return (((_dataModel.Parameters.CoefficientsAAA[componentIndex] * z + _dataModel.Parameters.CoefficientsAAB[componentIndex]) * y +
                            (_dataModel.Parameters.CoefficientsBAA[componentIndex] * z + _dataModel.Parameters.CoefficientsBAB[componentIndex])) * x +
                           ((_dataModel.Parameters.CoefficientsABA[componentIndex] * z + _dataModel.Parameters.CoefficientsABB[componentIndex]) * y +
                            (_dataModel.Parameters.CoefficientsB1[componentIndex] * z + _dataModel.Parameters.CoefficientsB1[componentIndex])));
                default:
                    return 0.0;
            }
        }

        public double CalculateResolution(double[] retentionFactors, int plateNumber)
        {
            try
            {
                // Find the two closest retention factors (critical pair)
                double minDifference = double.MaxValue;
                int component1 = 0, component2 = 1;

                for (int i = 0; i < retentionFactors.Length; i++)
                {
                    for (int j = i + 1; j < retentionFactors.Length; j++)
                    {
                        double difference = Math.Abs(retentionFactors[i] - retentionFactors[j]);
                        if (difference < minDifference)
                        {
                            minDifference = difference;
                            component1 = i;
                            component2 = j;
                        }
                    }
                }

                // Calculate resolution using the standard formula
                double k1 = retentionFactors[component1];
                double k2 = retentionFactors[component2];
                
                if (k1 > k2)
                {
                    double temp = k1;
                    k1 = k2;
                    k2 = temp;
                }

                double alpha = k2 / k1;
                double resolution = (Math.Sqrt(plateNumber) / 2.0) * (alpha - 1.0) * (k1 / (k1 + k2 + 2.0));
                
                return resolution * 2.5; // Adjustment factor from original code
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error in resolution calculation: {ex.Message}", ex);
            }
        }
    }
}
