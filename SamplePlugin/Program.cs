using System.Text.Json;
using System.Text.Json.Nodes;
using BarRaider.SdTools.Backend;
using BarRaider.SdTools.Tools;

namespace SamplePlugin
{
    class Program
    {
        static void Main(string[] args)
        {
            // Uncomment this line of code to allow for debugging
           //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }
           
            SdWrapper.Run(args);
        }
    }
}
