using System;
using System.IO;

namespace SetupPacker.Helper
{
    public static class Copier
    {
        public static void CopyStream(Stream from, Stream to, Action<long> progress)
        {
            int buffer_size = 10240;

            byte[] buffer = new byte[buffer_size];

            long totalRead = 0;

            while (totalRead < from.Length)
            {
                int read = from.Read(buffer, 0, buffer_size);

                to.Write(buffer, 0, read);

                totalRead += read;

                progress(totalRead);
            }
        }

    }
}