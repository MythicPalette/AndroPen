using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroPen.Data;

/// <summary>
/// This is a class designed to contain all remote points along with a sender ID.
/// </summary>
internal class RemoteEvent
{
    internal List<RemotePointerInfo> Touches { get; set; } = [];
    internal RemotePointerInfo? Pen { get; set; }
    internal int Sender {  get; set; }
}
