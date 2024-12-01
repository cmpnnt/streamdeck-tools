using BarRaider.SdTools.Backend;
using BarRaider.SdTools.Wrappers;
using Cmpnnt.SdTools.Generators;

namespace SamplePlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            // Uncomment this line of code to allow for debugging
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }
            PluginActionId[] pluginActionIds = PluginActionIdRegistry.AutoLoadPluginActions(); 
            SdWrapper.Run(args, pluginActionIds);
        }
    }
}
