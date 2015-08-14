using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;

using digital.pervasive.sample.relaynode;

namespace ActuatorNode
{
    class RelayServer
    {
        private readonly HardwareManager _hm;
        private relaynodeProducer _relayNodeProducer;
        private AllJoynBusAttachment _bus;

        public RelayServer(HardwareManager hm)
        {
            _hm = hm;
            _relayNodeProducer = null;
            _bus = new AllJoynBusAttachment();
            StartService();
        }

        private void StartService()
        {
            _relayNodeProducer = new relaynodeProducer(_bus);

            // Create a secure service to handle the async interactions
            _relayNodeProducer.Service = new relaynodeService(_hm, _relayNodeProducer.Signals);

            // Create interface as defined in the introspect xml, create AllJoyn bus object and announce the about interface.
            _relayNodeProducer.Start();
        }
    }
}
