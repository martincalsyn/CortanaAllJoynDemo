using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Devices.AllJoyn;

using digital.pervasive.sample.relaynode;

namespace ActuatorNode
{
    class relaynodeService : IrelaynodeService
    {
        private readonly HardwareManager _hm;
        private relaynodeSignals _relayNodeSignals;

        public relaynodeService(HardwareManager hm, relaynodeSignals signals)
        {
            _hm = hm;
            _hm.ButtonPressed += _hm_ButtonPressed;
            _relayNodeSignals = signals;
        }

        private void _hm_ButtonPressed(object sender, EventArgs args)
        {
            _relayNodeSignals.ButtonPressed();
        }

        public IAsyncOperation<relaynodeSetStateResult> SetStateAsync(AllJoynMessageInfo info, int interfaceMemberRelayId, byte interfaceMemberState)
        {
            Task<relaynodeSetStateResult> task = new Task<relaynodeSetStateResult>(() =>
            {
                _hm.SetRelay(interfaceMemberRelayId, interfaceMemberState != 0);
                _relayNodeSignals.RelayStateChanged(interfaceMemberRelayId, interfaceMemberState);
                return relaynodeSetStateResult.CreateSuccessResult();
            });

            task.Start();
            return task.AsAsyncOperation();
        }

        public IAsyncOperation<relaynodeGetStateResult> GetStateAsync(AllJoynMessageInfo info, int interfaceMemberRelayId)
        {
            Task<relaynodeGetStateResult> task = new Task<relaynodeGetStateResult>(() =>
            {
                return relaynodeGetStateResult.CreateSuccessResult((byte)(_hm.GetRelayState(interfaceMemberRelayId ) ? 1 : 0));
            });

            task.Start();
            return task.AsAsyncOperation();
        }
    }
}
