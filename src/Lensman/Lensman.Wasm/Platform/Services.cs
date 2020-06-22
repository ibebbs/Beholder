using System.Net.Http;

namespace Lensman.Platform
{
    public partial class Services
    {
        partial void GetHttpMessageHandler(ref HttpMessageHandler handler)
        {
            handler = new Uno.UI.Wasm.WasmHttpHandler();
        }
    }
}
