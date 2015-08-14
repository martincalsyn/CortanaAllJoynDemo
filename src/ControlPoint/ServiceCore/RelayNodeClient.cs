using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;

using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;

using digital.pervasive.sample.relaynode;

namespace ServiceCore
{
    public class RelayNodeClient
    {
        private relaynodeConsumer _relayNodeConsumer;
        private relaynodeWatcher _relayNodeWatcher;
        private AllJoynBusAttachment _bus;
        private bool[] _relayState = new bool[2];

        public RelayNodeClient()
        {
            _relayNodeConsumer = null;
            _bus = new AllJoynBusAttachment();
            StartWatcher();
        }

        private void StartWatcher()
        {
            _relayNodeWatcher = new relaynodeWatcher(_bus);
            _relayNodeWatcher.Added += relayNodeWatcher_Added;
            _relayNodeWatcher.Start();
        }

        private async void relayNodeWatcher_Added(relaynodeWatcher sender, AllJoynServiceInfo args)
        {

            relaynodeJoinSessionResult joinResult = await relaynodeConsumer.JoinSessionAsync(args, sender);

            if (joinResult.Status == AllJoynStatus.Ok)
            {
                _relayNodeConsumer = joinResult.Consumer;
                _relayNodeConsumer.Signals.ButtonPressedReceived += Signals_ButtonPressedReceived;
                _relayNodeConsumer.Signals.RelayStateChangedReceived += Signals_RelayStateChangedReceived;
                RetrieveCurrentState();
            }
            else
            {
                Debug.WriteLine("Joining the session went wrong");
            }
        }

        private void Signals_RelayStateChangedReceived(relaynodeSignals sender, relaynodeRelayStateChangedReceivedEventArgs args)
        {
            if (args.RelayId >= 0 && args.RelayId <= 1)
                _relayState[args.RelayId] = args.State != 0;
        }

        private void Signals_ButtonPressedReceived(relaynodeSignals sender, relaynodeButtonPressedReceivedEventArgs args)
        {
            Debug.WriteLine("Button pressed");
        }

        public async void SetRelay(int relayId, bool state)
        {
            if (_relayNodeConsumer != null )
            {
                await _relayNodeConsumer.SetStateAsync(relayId, (byte)(state ? 1 : 0));
            }
        }

        public async void RetrieveCurrentState()
        {
            if (_relayNodeConsumer != null)
            {
                relaynodeGetStateResult result = await _relayNodeConsumer.GetStateAsync(0);
                _relayState[0] = result.State != 0;
                result = await _relayNodeConsumer.GetStateAsync(1);
                _relayState[1] = result.State != 0;
            }
        }
    }
}