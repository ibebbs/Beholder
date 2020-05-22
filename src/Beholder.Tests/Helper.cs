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

        public static byte[] GetManifestResourceByteArray(string name)
        {
            var assemby = Assembly.GetExecutingAssembly();

            using (var source = GetManifestResourceStream(name))
            {
                using (var target = new MemoryStream())
                {
                    source.CopyTo(target);

                    return target.ToArray();
                }
            }
        }
    }
}
