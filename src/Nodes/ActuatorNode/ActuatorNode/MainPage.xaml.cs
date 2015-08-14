using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using ajrelay = digital.pervasive.sample.relaynode;

namespace ActuatorNode
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HardwareManager _hw;
        private DispatcherTimer _timer;

        public MainPage()
        {
            this.InitializeComponent();

            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            await InitializeHardware();
        }

        private async Task InitializeHardware()
        {
            _hw = new HardwareManager();
            await _hw.Initialize();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(0.5);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private int _stateSequence = 0;
        private void _timer_Tick(object sender, object e)
        {
            _hw.SetRelay(0, (_stateSequence & 0x01) != 0);
            _hw.SetRelay(1, (_stateSequence & 0x02) != 0);
            if (++_stateSequence == 4)
                _stateSequence = 0;
        }

    }
}
