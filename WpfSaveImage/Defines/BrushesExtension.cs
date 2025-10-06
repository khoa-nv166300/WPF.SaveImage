using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace WpfSaveImage
{
    public static class BrushExtension
    {
        public static Brush FromHex(string hex)
        {
            return (Brush)(new BrushConverter()).ConvertFrom(hex);
        }
    }

    public struct HexStatusColor
    {
        public const string Pending = "#95d8dad9";

        public const string Error = "#98ff002a";

        public const string Complete = "#65fe08";

        //

        public const string Accepted = "#5080e2";

        public const string Denied = "#b72228";

        public const string Disabled = "#808080";

    }
}
