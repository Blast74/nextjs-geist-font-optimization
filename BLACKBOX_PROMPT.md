# Optimized Prompt for BlackBox App Builder

## Task: Create a C# WPF Chromatography Analysis Application

Create a modern C# WPF graphical application that reproduces the functionality of a QuickBasic chromatography/gradient analysis program. The application should perform complex mathematical calculations for chromatographic separations and provide interactive visualization capabilities.

## Application Requirements

### Core Functionality
- **Chromatography Parameter Management**: Handle column dimensions, flow rates, gradient parameters, and experimental variables
- **Mathematical Calculations**: Implement retention factor calculations, linearization algorithms, and resolution optimization
- **Interactive Visualization**: Real-time graphical display with user-controlled navigation and zooming
- **Multi-dimensional Analysis**: Support 1D, 2D, and 3D variable optimization (temperature, pH, gradient time, etc.)
- **Resolution Mapping**: Generate color-coded resolution maps for optimization visualization

### Technical Architecture

#### 1. MainWindow.xaml
```xml
<Window x:Class="ChromatographyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="Chromatography Analysis Application" Height="600" Width="800">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="250"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>
        
        <!-- Left Panel: Parameter Inputs -->
        <StackPanel Grid.Column="0" Margin="10" Background="#F5F5F5">
            <TextBlock Text="Column Parameters" FontWeight="Bold" Margin="0,0,0,10"/>
            
            <TextBlock Text="Column Length (cm):"/>
            <TextBox x:Name="ColumnLengthTextBox" Text="25" Margin="0,0,0,5"/>
            
            <TextBlock Text="Column Diameter (cm):"/>
            <TextBox x:Name="ColumnDiameterTextBox" Text="0.46" Margin="0,0,0,5"/>
            
            <TextBlock Text="Flow Rate (ml/min):"/>
            <TextBox x:Name="FlowRateTextBox" Text="1.0" Margin="0,0,0,5"/>
            
            <TextBlock Text="Particle Diameter (μm):"/>
            <TextBox x:Name="ParticleDiameterTextBox" Text="5" Margin="0,0,0,5"/>
            
            <TextBlock Text="Plate Number:"/>
            <TextBox x:Name="PlateNumberTextBox" Text="32000" Margin="0,0,0,10"/>
            
            <TextBlock Text="Variable Configuration" FontWeight="Bold" Margin="0,10,0,10"/>
            
            <TextBlock Text="Number of Variables:"/>
            <ComboBox x:Name="VariableCountComboBox" SelectedIndex="2" Margin="0,0,0,5">
                <ComboBoxItem Content="1"/>
                <ComboBoxItem Content="2"/>
                <ComboBoxItem Content="3"/>
            </ComboBox>
            
            <Button Content="Calculate Parameters" Click="CalculateButton_Click" Margin="0,10,0,5"/>
            <Button Content="Scan Resolution" Click="ScanButton_Click" Margin="0,0,0,5"/>
            <Button Content="Reset View" Click="ResetButton_Click" Margin="0,0,0,5"/>
            <Button Content="Generate Spectrum" Click="SpectrumButton_Click" Margin="0,0,0,10"/>
            
            <TextBlock Text="Results" FontWeight="Bold" Margin="0,10,0,5"/>
            <TextBlock x:Name="ResultsTextBlock" TextWrapping="Wrap" FontSize="10"/>
        </StackPanel>
        
        <!-- Right Panel: Visualization Canvas -->
        <Canvas x:Name="DrawingCanvas" Grid.Column="1" Background="White" 
                MouseMove="Canvas_MouseMove" MouseLeftButtonDown="Canvas_MouseDown"
                KeyDown="Canvas_KeyDown" Focusable="True"/>
    </Grid>
</Window>
```

#### 2. MainWindow.xaml.cs - Event Handling and UI Logic
```csharp
using System;
using System.Windows;
using System.Windows.Input;

namespace ChromatographyApp
{
    public partial class MainWindow : Window
    {
        private DataModel _dataModel;
        private MeasurementService _measurementService;
        private GraphicsRenderer _graphicsRenderer;
        private bool _isSpectrumMode = false;
        private Point _currentMousePosition;

        public MainWindow()
        {
            InitializeComponent();
            InitializeApplication();
        }

        private void InitializeApplication()
        {
            _dataModel = new DataModel();
            _measurementService = new MeasurementService(_dataModel);
            _graphicsRenderer = new GraphicsRenderer(DrawingCanvas, _dataModel);
            
            // Set initial focus for keyboard events
            DrawingCanvas.Focus();
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateParametersFromUI();
                var results = _measurementService.ComputeParameters();
                _dataModel.Results = results;
                UpdateResultsDisplay();
                RedrawCanvas();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Calculation Error: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ScanButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateParametersFromUI();
                _measurementService.PerformResolutionScan();
                _graphicsRenderer.DrawResolutionMap();
                _isSpectrumMode = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Scan Error: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SpectrumButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UpdateParametersFromUI();
                _measurementService.GenerateSpectrum(_currentMousePosition.X, _currentMousePosition.Y);
                _graphicsRenderer.DrawSpectrum();
                _isSpectrumMode = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Spectrum Error: {ex.Message}", "Error", 
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            _currentMousePosition = e.GetPosition(DrawingCanvas);
            _graphicsRenderer.UpdateCrosshairs(_currentMousePosition.X, _currentMousePosition.Y);
            UpdateStatusDisplay();
        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            HandleKeyboardNavigation(e.Key);
        }

        private void HandleKeyboardNavigation(Key key)
        {
            // Implement keyboard controls similar to original QuickBasic
            switch (key)
            {
                case Key.Left:
                case Key.Right:
                case Key.Up:
                case Key.Down:
                    // Move crosshairs
                    break;
                case Key.C:
                    // Generate spectrum
                    SpectrumButton_Click(null, null);
                    break;
                case Key.R:
                    // Refresh resolution
                    break;
                case Key.Z:
                    // Zoom in
                    break;
                case Key.D:
                    // Zoom out
                    break;
                case Key.B:
                    // Reset bounds
                    ResetButton_Click(null, null);
                    break;
            }
        }

        private void UpdateParametersFromUI()
        {
            _dataModel.Parameters.ColumnLength = double.Parse(ColumnLengthTextBox.Text);
            _dataModel.Parameters.ColumnDiameter = double.Parse(ColumnDiameterTextBox.Text);
            _dataModel.Parameters.FlowRate = double.Parse(FlowRateTextBox.Text);
            _dataModel.Parameters.ParticleDiameter = double.Parse(ParticleDiameterTextBox.Text);
            _dataModel.Parameters.PlateNumber = int.Parse(PlateNumberTextBox.Text);
            _dataModel.Parameters.NumberOfVariables = VariableCountComboBox.SelectedIndex + 1;
        }

        private void UpdateResultsDisplay()
        {
            var results = _dataModel.Results;
            ResultsTextBlock.Text = $"Mobile Phase Volume: {results.MobilePhaseVolume:F2} ml\n" +
                                   $"Dead Time: {results.DeadTime:F2} min\n" +
                                   $"Linear Velocity: {results.LinearVelocity:F2} cm/min\n" +
                                   $"Current Resolution: {results.CurrentResolution:F3}";
        }

        private void RedrawCanvas()
        {
            _graphicsRenderer.ClearCanvas();
            _graphicsRenderer.DrawMeasurementFrame();
            _graphicsRenderer.DrawScale();
        }
    }
}
```

#### 3. Core Service Classes

**MeasurementService.cs** - Implements all chromatography calculations:
- Parameter validation and mobile phase volume calculations
- Multi-dimensional linearization algorithms (1D, 2D, 3D)
- Retention factor computations with variable transformations
- Resolution optimization and critical pair identification
- Gaussian peak generation for spectrum simulation

**DataModel.cs** - Data structures with property change notifications:
- Parameters class with validation for all input values
- CalculationResults class for computed outputs
- Support for up to 13 components with individual properties
- Variable type definitions (Temperature, pH, Gradient Time, etc.)

**GraphicsRenderer.cs** - Canvas drawing operations:
- Coordinate mapping between data space and canvas pixels
- Resolution map visualization with color coding
- Spectrum curve rendering with peak markers
- Interactive crosshairs and target markers
- Measurement frame drawing with proper scaling

**Utility.cs** - Helper methods:
- Coordinate scaling and inverse scaling functions
- Gaussian peak generation algorithms
- QuickSort implementation for data sorting
- Bilinear interpolation for smooth data mapping
- Input validation and error handling utilities

### Key Features to Implement

1. **Interactive Navigation**
   - Mouse-controlled crosshairs with real-time coordinate display
   - Keyboard navigation (arrow keys, zoom controls)
   - Click-to-analyze functionality for spectrum generation

2. **Mathematical Accuracy**
   - Precise implementation of chromatographic equations
   - Multi-dimensional variable optimization
   - Resolution calculation using standard formulas
   - Peak width calculations based on plate theory

3. **Visual Excellence**
   - Clean, modern WPF interface with proper spacing
   - Color-coded resolution maps for easy interpretation
   - Smooth spectrum curves with peak identification
   - Responsive canvas that updates in real-time

4. **Error Handling**
   - Comprehensive input validation with user-friendly messages
   - Exception handling for mathematical edge cases
   - Graceful degradation for invalid parameter combinations

5. **Performance Optimization**
   - Efficient canvas rendering for large datasets
   - Optimized calculation algorithms for real-time updates
   - Memory management for continuous operation

### Expected User Experience

The application should provide a seamless experience where users can:
- Input chromatographic parameters through intuitive controls
- Visualize optimization landscapes through interactive maps
- Generate and analyze chromatographic spectra
- Navigate through parameter space using keyboard and mouse
- Receive immediate feedback on separation quality

### Technical Notes

- Use WPF Canvas for high-performance graphics rendering
- Implement proper MVVM patterns with INotifyPropertyChanged
- Follow SOLID principles for maintainable code architecture
- Include comprehensive error handling and input validation
- Ensure thread-safe operations for UI responsiveness

This application should faithfully reproduce the analytical capabilities of the original QuickBasic program while providing a modern, user-friendly interface suitable for professional chromatography work.
