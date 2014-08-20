// Interface.cs

using System;
using System.Collections.Generic;

//---------------------------------------------------------------------------//

using size_t = System.IntPtr;

//---------------------------------------------------------------------------//

namespace Plugin
{
    public interface IData
    {
        string name    { get; }
        uint   flag    { get; set; }
        size_t size    { get; }
        object content { get; set; }
    }

    public interface IPlugin
    {
        IData[] info { get; }
        IData[] data { get; }

        bool Init(ICollection<IPlugin> collection);
        bool Attach(string msg, IPlugin plugin);
        bool Detach(string msg, IPlugin plugin);
        void Notify(string msg, IData data);
    }
}

//---------------------------------------------------------------------------//

// Interface.cs