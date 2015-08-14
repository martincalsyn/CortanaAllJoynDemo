using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GT = GHIElectronics.UAP.Gadgeteer;
using GTM = GHIElectronics.UAP.Gadgeteer.Modules;
using PGTM = PervasiveDigital.UAP.Gadgeteer.Modules;

namespace ActuatorNode
{
    internal delegate void ButtonPressedHandler(object sender, EventArgs args);

    class HardwareManager
    {
        private GTM.FEZCream _mainboard;
        private GTM.Button _button;
        private PGTM.RelayX1[] _relays = new PGTM.RelayX1[2];

        public event ButtonPressedHandler ButtonPressed;

        public async Task Initialize()
        {
            _mainboard = await GT.Module.CreateAsync<GTM.FEZCream>();
            _button = await GT.Module.CreateAsync<GTM.Button>(this._mainboard.GetProvidedSocket(3));
            _button.Released += _button_Released;
            _relays[0] = await GT.Module.CreateAsync<PGTM.RelayX1>(_mainboard.GetProvidedSocket(4));
            _relays[1] = await GT.Module.CreateAsync<PGTM.RelayX1>(_mainboard.GetProvidedSocket(8));
            SetButtonLed(false);
        }

        private void _button_Released(GTM.Button sender, object args)
        {
            if (this.ButtonPressed != null)
                this.ButtonPressed(this, new EventArgs());
        }

        public void SetButtonLed(bool state)
        {
            _button.SetLed(state);
        }

        public void SetRelay(int relay, bool state)
        {
            if (relay < 0 || relay > 1)
                throw new ArgumentOutOfRangeException("relay");

            _relays[relay].Value = state;
        }

        public bool GetRelayState(int relay)
        {
            if (relay < 0 || relay > 1)
                throw new ArgumentOutOfRangeException("relay");

            return _relays[relay].Value;
        }
    }
}
