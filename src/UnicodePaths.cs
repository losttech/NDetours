namespace NDetours;

using System.Runtime.InteropServices;
using System.Text;

class UnicodePaths {
    public static string GetShortPath(string fullPath) {
        var shortPath = new StringBuilder(8);
        while (true) {
            int result = GetShortPathName(fullPath, shortPath, shortPath.Capacity);
            if (result == 0)
                throw new System.ComponentModel.Win32Exception();
            if (result < shortPath.Capacity)
                return shortPath.ToString();
            shortPath.Capacity *= 2;
        }
    }

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    static extern int GetShortPathName(
        [MarshalAs(UnmanagedType.LPTStr)]
        string path,
        [MarshalAs(UnmanagedType.LPTStr)]
        StringBuilder shortPath,
        int shortPathLength
   );
}
