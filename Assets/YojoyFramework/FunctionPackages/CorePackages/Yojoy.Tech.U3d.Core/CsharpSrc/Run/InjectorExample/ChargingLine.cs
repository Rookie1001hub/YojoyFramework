#region Comment Head


#endregion

using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Core.Run
{
    public class ChargingLine
    {
        [Inject]
        private USBSocket usbSocket;

        public void Connect() => usbSocket.Connect();
    }
}
