using System;
using System.Runtime.InteropServices;

namespace Tel.Egram.Services.Utils.Platforms
{
    public class Platform : IPlatform
    {
        public static Platform GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return new WindowsPlatform();

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                return new MacosPlatform();

            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
                ? new LinuxPlatform()
                : throw new NotSupportedException("OS is not supported");
        }

        public virtual int PixelDensity => 1;
    }
}