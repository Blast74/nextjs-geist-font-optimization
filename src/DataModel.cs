using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YourNamespace
{
    public class DataModel : INotifyPropertyChanged
    {
        public Parameters Parameters { get; set; }
        public CalculationResults Results { get; set; }

        public DataModel()
        {
            Parameters = new Parameters();
            Results = new CalculationResults();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Parameters : INotifyPropertyChanged
    {
        private double _columnLength = 25.0;
        private double _columnDiameter = 0.46;
        private double _flowRate = 1.0;
        private double _particleDiameter = 5.0;
        private int _plateNumber = 32000;
        private double _flowRateReference = 1.0;
        private double _deadTimeExperimental = 0.0;
        private int _numberOfVariables = 3;
        private int _numberOfComponents = 13;

        // Column parameters
        public double ColumnLength
        {
            get => _columnLength;
            set
            {
                if (value <= 0) throw new ArgumentException("Column length must be positive");
                _columnLength = value;
                OnPropertyChanged();
            }
        }

        public double ColumnDiameter
        {
            get => _columnDiameter;
            set
            {
                if (value <= 0) throw new ArgumentException("Column diameter must be positive");
                _columnDiameter = value;
                OnPropertyChanged();
            }
        }

        public double FlowRate
        {
            get => _flowRate;
            set
            {
                if (value <= 0) throw new ArgumentException("Flow rate must be positive");
                _flowRate = value;
                OnPropertyChanged();
            }
        }

        public double ParticleDiameter
        {
            get => _particleDiameter;
            set
            {
                if (value <= 0) throw new ArgumentException("Particle diameter must be positive");
                _particleDiameter = value;
                OnPropertyChanged();
            }
        }

        public int PlateNumber
        {
            get => _plateNumber;
            set
            {
                if (value <= 0) throw new ArgumentException("Plate number must be positive");
                _plateNumber = value;
                OnPropertyChanged();
            }
        }

        public double FlowRateReference
        {
            get => _flowRateReference;
            set
            {
                if (value <= 0) throw new ArgumentException("Reference flow rate must be positive");
                _flowRateReference = value;
                OnPropertyChanged();
            }
        }

        public double DeadTimeExperimental
        {
            get => _deadTimeExperimental;
            set
            {
                if (value < 0) throw new ArgumentException("Dead time cannot be negative");
                _deadTimeExperimental = value;
                OnPropertyChanged();
            }
        }

        // Variable configuration
        public int NumberOfVariables
        {
            get => _numberOfVariables;
            set
            {
                if (value < 1 || value > 3) throw new ArgumentException("Number of variables must be 1, 2, or 3");
                _numberOfVariables = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfComponents
        {
            get => _numberOfComponents;
            set
            {
                if (value <= 0) throw new ArgumentException("Number of components must be positive");
                _numberOfComponents = value;
                OnPropertyChanged();
            }
        }

        // Variable types (1=Temperature, 2=pH, 3=Gradient time, etc.)
        public int VariableTypeX { get; set; } = 3; // Gradient time
        public int VariableTypeY { get; set; } = 1; // Temperature
        public int VariableTypeZ { get; set; } = 9; // %B

        // Domain limits
        public double XMin { get; set; } = 30.0;
        public double XMax { get; set; } = 90.0;
        public double YMin { get; set; } = 30.0;
        public double YMax { get; set; } = 60.0;
        public double ZMin { get; set; } = 0.0;
        public double ZMax { get; set; } = 100.0;

        // Linearisation coefficients - arrays for multiple components
        public double[] CoefficientsA1 { get; set; } = new double[13];
        public double[] CoefficientsB1 { get; set; } = new double[13];
        
        // For 2-variable systems
        public double[] CoefficientsAA1 { get; set; } = new double[13];
        public double[] CoefficientsAB1 { get; set; } = new double[13];
        public double[] CoefficientsBA1 { get; set; } = new double[13];
        public double[] CoefficientsB2 { get; set; } = new double[13];

        // For 3-variable systems
        public double[] CoefficientsAAA { get; set; } = new double[13];
        public double[] CoefficientsAAB { get; set; } = new double[13];
        public double[] CoefficientsBAA { get; set; } = new double[13];
        public double[] CoefficientsBAB { get; set; } = new double[13];
        public double[] CoefficientsABA { get; set; } = new double[13];
        public double[] CoefficientsABB { get; set; } = new double[13];
        public double[] CoefficientsB3 { get; set; } = new double[13];

        // Component-specific data
        public double[] PKa { get; set; } = new double[13];
        public double[] ComponentIntensities { get; set; } = new double[13];

        // File names for data loading
        public string FileName1 { get; set; } = "schmit.dat";
        public string FileName2 { get; set; } = "schmit3.dat";

        // Screen settings
        public bool ScreenOnZAxis { get; set; } = false;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class CalculationResults : INotifyPropertyChanged
    {
        private double _mobilePhaseVolume;
        private double _deadTime;
        private double _linearVelocity;
        private double _maxResolution;
        private double _minResolution;
        private double _currentResolution;

        public double MobilePhaseVolume
        {
            get => _mobilePhaseVolume;
            set
            {
                _mobilePhaseVolume = value;
                OnPropertyChanged();
            }
        }

        public double DeadTime
        {
            get => _deadTime;
            set
            {
                _deadTime = value;
                OnPropertyChanged();
            }
        }

        public double LinearVelocity
        {
            get => _linearVelocity;
            set
            {
                _linearVelocity = value;
                OnPropertyChanged();
            }
        }

        public double MaxResolution
        {
            get => _maxResolution;
            set
            {
                _maxResolution = value;
                OnPropertyChanged();
            }
        }

        public double MinResolution
        {
            get => _minResolution;
            set
            {
                _minResolution = value;
                OnPropertyChanged();
            }
        }

        public double CurrentResolution
        {
            get => _currentResolution;
            set
            {
                _currentResolution = value;
                OnPropertyChanged();
            }
        }

        // Arrays for component-specific results
        public double[] RetentionFactors { get; set; } = new double[13];
        public double[] RetentionTimes { get; set; } = new double[13];
        public double[] PeakWidths { get; set; } = new double[13];

        // Current position and values
        public double CurrentX { get; set; }
        public double CurrentY { get; set; }
        public double CurrentZ { get; set; }

        // Optimization results
        public double OptimalX { get; set; }
        public double OptimalY { get; set; }
        public double OptimalZ { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    // Enumeration for variable types
    public enum VariableType
    {
        Temperature = 1,
        pH = 2,
        GradientTime = 3,
        FlowRate = 4,
        IonicStrength = 5,
        GradientSlope = 6,
        TemperatureGradientSlope = 7,
        FlowRateGradientSlope = 8,
        PercentB = 9
    }

    // Class for storing experimental data points
    public class ExperimentalData
    {
        public double[] RetentionTimes { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        
        public ExperimentalData(int componentCount)
        {
            RetentionTimes = new double[componentCount];
        }
    }
}
