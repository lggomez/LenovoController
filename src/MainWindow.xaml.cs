﻿using LenovoController.Features;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace LenovoController
{
    public partial class MainWindow : Window
    {
        private readonly RadioButton[] _alwaysOnUsbButtons;
        private readonly AlwaysOnUsbFeature _alwaysOnUsbFeature = new();
        private readonly RadioButton[] _batteryButtons;
        private readonly BatteryFeature _batteryFeature = new();
        private readonly RadioButton[] _powerModeButtons;
        private readonly PowerModeFeature _powerModeFeature = new();
        private readonly FnLockFeature _fnLockFeature = new();
        private readonly OverDriveFeature _overDriveFeature = new();
        private readonly TouchpadLockFeature _touchpadLockFeature = new();

        public MainWindow()
        {
            InitializeComponent();

            mainWindow.Title += $" v{AssemblyName.GetAssemblyName(Assembly.GetExecutingAssembly().Location).Version}";
            _powerModeButtons = [radioQuiet, radioBalance, radioPerformance];
            _batteryButtons = [radioConservation, radioNormalCharge, radioRapidCharge];
            _alwaysOnUsbButtons = [radioAlwaysOnUsbOff, radioAlwaysOnUsbOnWhenSleeping, radioAlwaysOnUsbOnAlways];

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
                    () => DisableControls(_batteryButtons)),
                new FeatureCheck(
                    () => _batteryButtons[(int) _batteryFeature.GetState()].IsChecked = true,
                    () => DisableControls(_batteryButtons)),
                new FeatureCheck(
                    () => _alwaysOnUsbButtons[(int) _alwaysOnUsbFeature.GetState()].IsChecked = true,
                    () => DisableControls(_alwaysOnUsbButtons)),
                new FeatureCheck(
                    () => chkOverDrive.IsChecked = _overDriveFeature.GetState() == OverDriveState.On,
                    () => chkOverDrive.IsEnabled = false),
                new FeatureCheck(
                    () => chkTouchpadLock.IsChecked = _touchpadLockFeature.GetState() == TouchpadLockState.On,
                    () => chkTouchpadLock.IsEnabled = false),
                new FeatureCheck(
                    () => chkFnLock.IsChecked = _fnLockFeature.GetState() == FnLockState.On,
                    () => chkFnLock.IsEnabled = false)
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

        private void radioPowerMode_Checked(object sender, RoutedEventArgs e)
        {
            _powerModeFeature.SetState((PowerModeState)Array.IndexOf(_powerModeButtons, sender));
        }

        private void radioAlwaysOnUsb_Checked(object sender, RoutedEventArgs e)
        {
            _alwaysOnUsbFeature.SetState((AlwaysOnUsbState)Array.IndexOf(_alwaysOnUsbButtons, sender));
        }

        private void chkOverDrive_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkOverDrive.IsChecked.GetValueOrDefault(false)
                ? OverDriveState.On
                : OverDriveState.Off;
            _overDriveFeature.SetState(state);
        }

        private void chkTouchpadLock_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkTouchpadLock.IsChecked.GetValueOrDefault(false)
                ? TouchpadLockState.On
                : TouchpadLockState.Off;
            _touchpadLockFeature.SetState(state);
        }

        private void chkFnLock_Checked(object sender, RoutedEventArgs e)
        {
            var state = chkFnLock.IsChecked.GetValueOrDefault(false)
                ? FnLockState.On
                : FnLockState.Off;
            _fnLockFeature.SetState(state);
        }
    }
}