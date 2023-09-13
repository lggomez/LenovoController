using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using LenovoController.Features;

namespace LenovoController
{
    public partial class MainWindow : Window
    {
        private readonly RadioButton[] _batteryButtons;
        private readonly BatteryFeature _batteryFeature = new BatteryFeature();

        public MainWindow()
        {
            InitializeComponent();

            mainWindow.Title = "👀";
            _batteryButtons = new[] { radioConservation, radioNormalCharge, radioRapidCharge };
            Refresh();
        }

        private class FeatureCheck
        {
            private readonly Action _check;
            private readonly Action _disable;

            internal FeatureCheck(Action check, Action disable)
            {
                _check = check;
                _disable = disable;
            }

            internal void Check() => _check();
            internal void Disable() => _disable();
        }

        private void Refresh()
        {
            var features = new[]
            {
                new FeatureCheck(
                    () => _batteryButtons[(int) _batteryFeature.GetState()].IsChecked = true,
                    () => DisableControls(_batteryButtons))
            };

            foreach (var feature in features)
            {
                try
                {
                    feature.Check();
                }
                catch (Exception e)
                {
                    Trace.TraceInformation("Could not refresh feature: " + e);
                    feature.Disable();
                }
            }
        }

        private void DisableControls(Control[] buttons)
        {
            foreach (var btn in buttons)
                btn.IsEnabled = false;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void radioBattery_Checked(object sender, RoutedEventArgs e)
        {
            _batteryFeature.SetState((BatteryState)Array.IndexOf(_batteryButtons, sender));
        }
    }
}