using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DownUnder.UI.Widgets.Interfaces
{
    public interface IAcceptsDrops
    {
        bool AcceptsDrops { get; }
        List<Type> AcceptedDropTypes { get; }
        bool IsDropAcceptable(object drop);
        void HandleDrop(object drop);
    }
}
