using System;
using System.Threading.Tasks;

using GHIElectronics.UAP.Gadgeteer;
using GHIElectronics.UAP.Gadgeteer.SocketInterfaces;

namespace PervasiveDigital.UAP.Gadgeteer.Modules
{
    public class RelayX1 : Module
    {
        public override string Name => "Relay X1 2.0";
        public override string Manufacturer => "GHI Electronics, LLC";

        private DigitalIO _outputPin;

        protected async override Task Initialize(ISocket parentSocket)
        {
            _outputPin = await parentSocket.CreateDigitalIOAsync((SocketPinNumber)5, false);
        }

        public void TurnOn()
        {
            _outputPin.SetHigh();
        }

        public void TurnOff()
        {
            _outputPin.SetLow();
        }

        public void Set(bool state)
        {
            _outputPin.Value = state;
        }

        public bool Value
        {
            get { return _outputPin.Value; }
            set { _outputPin.Value = value; }
        }
    }
}