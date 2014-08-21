// TestPlugin.cs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;

//---------------------------------------------------------------------------//

namespace Plugin
{
    using MessageMap = MultiMap<string, PluginEventHandler>;

    public class StringData : Data<string>
    {
        public StringData(string name, string content) : base(name, content) { }
    }

    public class BitmapData : Data<Bitmap>
    {
        public BitmapData(string name, Bitmap content) : base(name, content) { }
    }

    public class Plugin : IPlugin
    {
        private ICollection<IData>   m_info = null;
        private ICollection<IData>   m_data = null;
        private ICollection<IPlugin> m_collection = null;

        private MessageMap m_msg_map = new MessageMap();
        private ReaderWriterLock m_rwl = new ReaderWriterLock();

        public ICollection<IData> info { get { return m_info; } }
        public ICollection<IData> data { get { return m_data; } }

        public bool Init(ICollection<IPlugin> collection)
        {
            m_collection = collection;

            m_info = new List<IData>
            {
                new StringData("name", "skelton"),
                new StringData("description", "skelton plugin"),
                new StringData("copyright", "Copyright (C) 2014 tapetums"),
            };

            var asm = System.Reflection.Assembly.GetExecutingAssembly();
            var names = asm.GetManifestResourceNames();
            foreach ( var name in names )
            {
                if ( name.Contains(".ico") )
                {
                    var stream = asm.GetManifestResourceStream(name);
                    var bmp_data = new BitmapData("icon", new Bitmap(stream));
                    m_info.Add(bmp_data);
                }
            }

            return true;
        }

        public async void Attach(string msg, PluginEventHandler handler)
        {
            Console.WriteLine("Attach(\"" + msg + "\") begin");
            if ( null == handler )
            {
                Console.WriteLine("Attach(): handler is null");
                Console.WriteLine("Attach(\"" + msg + "\") end");
                return;
            }

            await Task.Run(() =>
            {
                using ( new WriteLock(m_rwl) )
                {
                    if ( m_msg_map.Contains(msg, handler) )
                    {
                        Console.WriteLine("Attach(\"" + msg + "\") is already attached with " + handler.ToString());
                    }
                    else
                    {
                        m_msg_map.Add(msg, handler);
                        Console.WriteLine("Attach(\"" + msg + "\") attached");
                    }
                }
            });
            Console.WriteLine("Attach(\"" + msg + "\") end");
        }

        public async void Detach(string msg, PluginEventHandler handler)
        {
            Console.WriteLine("Detach(\"" + msg + "\") begin");
            if ( null == handler )
            {
                Console.WriteLine("Detach(): handler is null");
                Console.WriteLine("Detach(\"" + msg + "\") end");
                return;
            }

            await Task.Run(() =>
            {
                using ( new WriteLock(m_rwl) )
                {
                    var ret = m_msg_map.Remove(msg, handler);
                    if ( ret )
                    {
                        Console.WriteLine("Detach(\"" + msg + "\") detached");
                    }
                    else
                    {
                        Console.WriteLine("Detach(\"" + msg + "\") is not attached with " + handler.ToString());
                    }
                }
            });
            Console.WriteLine("Detach(\"" + msg + "\") end");
        }

        public void Notify(object sender, PluginEventArgs e)
        {
            var cnt_handled = 0;

            Console.WriteLine("Notify(\"" + e.msg + "\") begin");
            using ( new ReadLock(m_rwl) )
            {
                var handlers = m_msg_map[e.msg];
                if ( null == handlers )
                {
                    Console.WriteLine("Notify(\"" + e.msg + "\") is not attached");
                    Console.WriteLine("Notify(\"" + e.msg + "\") end");
                    return;
                }

                foreach ( var handler in handlers )
                {
                    Task.Factory.StartNew(() =>
                    {
                        handler(sender, e);
                    });
                    Console.WriteLine("Notify(\"" + e.msg + "\") notified");

                    ++cnt_handled;
                }
            }
            if ( cnt_handled < 1 )
            {
                Console.WriteLine("Notify(\"" + e.msg + "\") was sent to no handler");
            }

            Console.WriteLine("Notify(\"" + e.msg + "\") end");
        }

        public ICollection<PluginEventHandler> Handler(string msg)
        {
            return m_msg_map[msg];
        }
    }
}

//---------------------------------------------------------------------------//

// TestPlugin.cs