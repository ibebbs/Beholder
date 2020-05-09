using System.IO;
using System.Reflection;

namespace Beholder.Tests
{
    public static class Helper
    {
        public static Stream GetManifestResourceStream(string name)
        {
            var assemby = Assembly.GetExecutingAssembly();

            return assemby.GetManifestResourceStream(name);
        }
    }
}
