using BarRaider.SdTools.Backend;
using Cmpnnt.SdTools.Generators;

namespace SamplePlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            // Uncomment this line of code to allow for debugging
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }
            //PluginActionIdRegistry.GetPluginActionIds(); 
            SdWrapper.Run(args);
        }
    }
}
