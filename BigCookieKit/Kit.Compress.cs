using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Linq;
using System.Runtime.CompilerServices;

namespace BigCookieKit
{
    public static partial class Kit
    {
        /// <summary>
        /// 文件夹所有文件压缩到Zip
        /// </summary>
        /// <param name="ZipFilePath">Zip文件目标路径</param>
        /// <param name="DirPath">文件夹路径</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean DirToFormZipPacket(String zipFilePath, String dirPath)
        {
            FileInfo zipFile = new FileInfo(zipFilePath);
            if (!zipFile.Exists)
            {
                DirectoryInfo dir = new DirectoryInfo(dirPath);
                if (!dir.Exists)
                    return false;
                ZipFile.CreateFromDirectory(dir.FullName, zipFile.FullName, CompressionLevel.NoCompression, true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 文件压缩到Zip
        /// </summary>
        /// <param name="zipFilePath">Zip文件目标路径</param>
        /// <param name="filesPath">所有的文件路径</param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Boolean FileToFormZipPacket(String zipFilePath, params String[] filesPath)
        {
            FileInfo zipFile = new FileInfo(zipFilePath);
            if (zipFile.Exists)
            {
                using (var archive = ZipFile.Open(zipFile.FullName, ZipArchiveMode.Update))
                {
                    foreach (var item in filesPath)
                    {
                        FileInfo file = new FileInfo(item);
                        archive.CreateEntryFromFile(file.FullName, file.Name);
                    }
                }
                return true;
            }
            return false;
        }
    }
}
