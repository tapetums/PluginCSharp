// TestPlugin.cs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

//---------------------------------------------------------------------------//

using size_t = System.IntPtr;

//---------------------------------------------------------------------------//

namespace Plugin
{
    using MessageMap = MultiMap<string, IPlugin>;

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
        public abstract size_t size { get; }
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

    public class StringData : Data<string>
    {
        public StringData(string name, string content) : base(name, content) { }

        public override size_t size
        {
            get { return (size_t)m_content.Length; }
        }
    }

    public class BitmapData : Data<Bitmap>
    {
        public BitmapData(string name, Bitmap content) : base(name, content) { }

        public override size_t size
        {
            get { return (size_t)(m_content.Width * m_content.Height); }
        }
    }

    public class Plugin : IPlugin
    {
        private IData[] m_info =
        {
            new StringData("name", "skelton"),
            new StringData("description", "skelton plugin"),
            new StringData("copyright", "Copyright (C) 2014 tapetums"),
            new BitmapData("icon", new Bitmap(@"F:\Dropbox\GitHub\C#\Plugin\Plugin\takoyaki.png")),
        };
        private IData[] m_data = null;
        private ICollection<IPlugin> m_collection = null;
        private MessageMap m_msg_map = new MessageMap();
        private ReaderWriterLock m_rwl = new ReaderWriterLock();

        public IData[] info { get { return m_info; } }
        public IData[] data { get { return m_data; } }

        public bool Init(ICollection<IPlugin> collection)
        {
            m_collection = collection;
            return true;
        }

        public bool Attach(string msg, IPlugin plugin)
        {
            if ( plugin == null )
            {
                return false;
            }

            using ( new WriteLock(m_rwl) )
            {
                if ( m_msg_map.Contains(msg, plugin) )
                {
                    return false;
                }

                m_msg_map.Add(msg, plugin);
            }

            return true;
        }

        public bool Detach(string msg, IPlugin plugin)
        {
            if ( plugin == null )
            {
                return false;
            }

            using ( new WriteLock(m_rwl) )
            {
                return m_msg_map.Remove(msg, plugin, false);
            }
        }

        public void Notify(string msg, IData data)
        {
            using ( new ReadLock(m_rwl) )
            {
                foreach ( var list in m_msg_map[msg] )
                {
                    list.Notify(msg, data);
                }
            }
        }
    }
}

//---------------------------------------------------------------------------//

// TestPlugin.cs