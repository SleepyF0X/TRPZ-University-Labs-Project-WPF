using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRPZLabRab.Messages
{
    public sealed class RoutingMessage
    {
        public RoutingMessage(string viewModelName, Guid? itemId = null)
        {
            ViewModelName = viewModelName;
            ItemId = itemId;
        }

        public string ViewModelName { get; }
        public Guid? ItemId { get; }
    }
}
