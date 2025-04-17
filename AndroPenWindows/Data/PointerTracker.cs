using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AndroPen.Data;
internal class PointerTracker: List<RemotePointerInfo>
{
    internal void InsertOrUpdate(RemotePointerInfo pointer)
    {
        // Check if there is already a pointer with the id and add it to the list.
        RemotePointerInfo? test = this.FirstOrDefault(r => r.PointerId == pointer.PointerId );

        // If test is null, we can just add the pointer info to the list.
        if( test == null )
            Add( pointer );

        // Otherwise, update the pointer with the new info.
        else
            this[IndexOf( test )] = pointer;
    }

    internal void Remove(int pointerId)
    {
        RemotePointerInfo? test = this.FirstOrDefault(r => r.PointerId == pointerId );
        if ( test != null )
            _ = Remove( test );
    }

    internal RemotePointerInfo? Consume(int id = 0)
    {
        if( id < this.Count )
        {
            RemotePointerInfo rpi = this[id];
            RemoveAt( id );
            return rpi;
        }
        return null;
    }
}
