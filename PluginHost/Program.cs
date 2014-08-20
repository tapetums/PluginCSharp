// Program.cs

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Plugin;

//---------------------------------------------------------------------------//

namespace PluginHost
{
    static class Program
    {
        private static char sep = Path.DirectorySeparatorChar;
        private static string IPluginTypeName = typeof(IPlugin).FullName;
        private static ICollection<IPlugin> plugins = new List<IPlugin>();
        public static Image bitmap = null;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            LoadAllPlugins();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void LoadAllPlugins()
        {
            var exe_path = Assembly.GetExecutingAssembly().Location;
            var dir_path = Path.GetDirectoryName(exe_path) + sep + "plugins";

            if ( ! Directory.Exists(dir_path) )
            {
                Directory.CreateDirectory(dir_path);
                if ( ! Directory.Exists(dir_path) )
                {
                    throw new ApplicationException("Not found:\n" + dir_path);
                }
            }

            SearchPlugins(dir_path);
        }

        private static void SearchPlugins(string dir_path, bool search_in_sub = true)
        {
            var dll_files = Directory.GetFiles(dir_path, "*.dll");
            foreach ( var dll_file in dll_files )
            {
                RegisterPlugin(dll_file);
            }

            if ( search_in_sub )
            {
                var sub_dir_paths = Directory.GetDirectories(dir_path);
                foreach ( var sub_dir_path in sub_dir_paths )
                {
                    SearchPlugins(sub_dir_path);
                }
            }
        }

        private static void RegisterPlugin(string dll_file)
        {
            try
            {
                var asm = Assembly.LoadFrom(dll_file);
                foreach ( var type in asm.GetTypes() )
                {
                    if ( ! type.IsClass || ! type.IsPublic || type.IsAbstract )
                    {
                        continue;
                    }
                    if ( null == type.GetInterface(IPluginTypeName) )
                    {
                        continue;
                    }

                    var plugin = Activator.CreateInstance(type) as IPlugin;
                    if ( false == plugin.Init(plugins) )
                    {
                        throw new ApplicationException("Failed to initialize plugin");
                    }

                    var infos = plugin.info;
                    foreach ( var info in infos )
                    {
                        if ( null == info || null == info.name || null == info.content )
                        {
                            continue;
                        }

                        var info_type = info.content.GetType();
                        if ( typeof(string) == info_type )
                        {
                            var name    = (string)info.name;
                            var content = (string)info.content;

                            var str = String.Format("{0}: {1}", name, content);
                            Console.WriteLine(str);
                        }
                        else if ( typeof(Bitmap) == info_type )
                        {
                            bitmap = info.content as Bitmap;
                            if ( null != bitmap )
                            {
                                var name    = (string)info.name;
                                var content = bitmap.ToString();

                                var str = String.Format("{0}: {1}", name, content);
                                Console.WriteLine(str);
                            }
                        }
                    }

                    plugins.Add(plugin);
                }
            }
            catch ( Exception e )
            {
                MessageBox.Show(e.Message, e.GetType().ToString());
            }
        }
    }
}

//---------------------------------------------------------------------------//

// Program.cs