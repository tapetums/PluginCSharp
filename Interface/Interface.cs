// Interface.cs

using System;
using System.Collections.Generic;

//---------------------------------------------------------------------------//

namespace Plugin
{
    public interface IData
    {
        string name    { get; }
        uint   flag    { get; set; }
        object content { get; set; }
    }

    public abstract class Data<T> : IData
    {
        protected string m_name;
        protected T      m_content;

        public Data(string data_name, T content)
        {
            m_name    = data_name;
            m_content = content;
        }

        public string name { get { return m_name; } }
        public uint   flag { get { return 0; } set { } }
        public object content
        {
            get
            {
                return m_content;
            }
            set
            {
                if ( typeof(T) != value.GetType() )
                {
                    throw new System.InvalidCastException();
                }
                else
                {
                    m_content = (T)value;
                }
            }
        }
    }

    public class PluginEventArgs : EventArgs
    {
        private string m_msg;
        private IData  m_data;

        public PluginEventArgs(string msg, IData data)
        {
            m_msg  = msg;
            m_data = data;
        }

        public string msg  { get { return m_msg; } }
        public IData  data { get { return m_data; } }
    }

    public delegate void PluginEventHandler(object sender, PluginEventArgs e);

    public interface IPlugin
    {
        ICollection<IData> info { get; }
        ICollection<IData> data { get; }

        bool Init(ICollection<IPlugin> collection);
        void Attach(string msg, PluginEventHandler handler);
        void Detach(string msg, PluginEventHandler handler);
        void Notify(object sender, PluginEventArgs e);
        ICollection<PluginEventHandler> Handler(string msg);
    }
}

//---------------------------------------------------------------------------//

// Interface.cs