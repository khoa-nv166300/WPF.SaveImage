using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace WpfSaveImage
{
    public enum ESequence
    {
        Pending,
        Error,
        Complete,
        Accepted,
        Denied,
        Disabled,
    }
    public static class CDefines
    {
        public static event EventHandler<ESequence> SequenceChanged;
        public static void OnSequenceChanged(ESequence eSequence)
        {
            SequenceChanged?.Invoke(null, eSequence);
        }
    }
}
