#region Comment Head



#endregion

using Yojoy.Tech.Common.Core.Run;

namespace Yojoy.Tech.U3d.Core.Run
{
    public class Phone
    {
        [Inject]
        private ChargingLine chargingLine;

        public void Charging() => chargingLine.Connect();
    }
}
