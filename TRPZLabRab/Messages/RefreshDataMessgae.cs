using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRPZLabRab.Messages
{
    public sealed class RefreshDataMessge
    {
        public RefreshDataMessge(string viewModelName, dynamic item = null)
        {
            ViewModelName = viewModelName;
            Item = item;
        }

        public string ViewModelName { get; }
        public dynamic Item { get; }
    }
}
