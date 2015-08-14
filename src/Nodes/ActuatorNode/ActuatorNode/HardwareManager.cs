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
    class HardwareManager
    {
        private GTM.FEZCream _mainboard;
        private GTM.Button _button;
        private PGTM.RelayX1[] _relays = new PGTM.RelayX1[2];

        public async Task Initialize()
        {
            _mainboard = await GT.Module.CreateAsync<GTM.FEZCream>();
            _button = await GT.Module.CreateAsync<GTM.Button>(this._mainboard.GetProvidedSocket(3));
            _relays[0] = await GT.Module.CreateAsync<PGTM.RelayX1>(_mainboard.GetProvidedSocket(4));
            _relays[1] = await GT.Module.CreateAsync<PGTM.RelayX1>(_mainboard.GetProvidedSocket(8));
        }

        public void SetRelay(int relay, bool state)
        {
            if (relay < 0 || relay > 1)
                throw new ArgumentOutOfRangeException("relay");

            _relays[relay].Value = state;
        }

    }
}
