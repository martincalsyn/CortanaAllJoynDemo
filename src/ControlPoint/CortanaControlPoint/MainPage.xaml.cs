using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using ServiceCore;

namespace CortanaControlPoint
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private RelayNodeClient _relayClient;
        private DispatcherTimer _timer;

        public MainPage()
        {
            this.InitializeComponent();
            _relayClient = new RelayNodeClient();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(3);
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private int _stateSequence = 0;
        private void _timer_Tick(object sender, object e)
        {
            _relayClient.SetRelay(0, (_stateSequence & 0x01) != 0);
            _relayClient.SetRelay(1, (_stateSequence & 0x02) != 0);
            if (++_stateSequence == 4)
                _stateSequence = 0;
        }

    }
}
