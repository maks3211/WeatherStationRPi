using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaTest.Services.Interfaces
{
    public interface INetworkStatus
    {
        bool isConnected { get; }
        event EventHandler NetworkConnected;
        event EventHandler NetworkDisconnected;

    }
}
