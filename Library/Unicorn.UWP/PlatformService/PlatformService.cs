using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unicorn
{
    public static partial class PlatformService
    {
        static PlatformService()
        {
            ApplicationInformation = new ApplicationInformationService();
            Cryptography = new CryptographyService();
            Dispatcher = new DispatcherService();
            DeviceInformation = new DeviceInformationService();
            NetworkInformation = new NetowrkInformationService();
            File = new PlatformFile();
            Log = new NullLogService();
        }
    }
}
