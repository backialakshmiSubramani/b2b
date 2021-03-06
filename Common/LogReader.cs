﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogReader.cs" company="">
//   
// </copyright>
// <summary>
//   Defines the LogReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Modules.Channel.B2B.Common
{
    /// <summary>
    /// The log reader.
    /// </summary>
    public static class LogReader
    {
        /// <summary>
        /// Returns the end of a text reader.
        /// </summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="lineCount">he number of lines to return.</param>
        /// <returns>The last lineCount lines from the reader.</returns>
        public static string[] Tail(this TextReader reader, int lineCount)
        {
            var buffer = new List<string>(lineCount);
            string line;
            for (int i = 0; i < lineCount; i++)
            {
                line = reader.ReadLine();
                if (line == null)
                {
                    return buffer.ToArray();
                }

                buffer.Add(line);
            }

            int lastLine = lineCount - 1;           // The index of the last line read from the buffer.  Everything > this index was read earlier than everything <= this indes

            while (null != (line = reader.ReadLine()))
            {
                lastLine++;
                if (lastLine == lineCount)
                {
                    lastLine = 0;
                }

                buffer[lastLine] = line;
            }

            if (lastLine == lineCount - 1)
            {
                return buffer.ToArray();
            }

            var retVal = new string[lineCount];
            buffer.CopyTo(lastLine + 1, retVal, 0, lineCount - lastLine - 1);
            buffer.CopyTo(0, retVal, lineCount - lastLine - 1, lastLine + 1);
            return retVal;
        }

        /// <summary>
        /// Gets the most recent log file.
        /// </summary>
        /// <param name="logFileLocation">
        /// The log file location.
        /// </param>
        /// <returns>
        /// The <see cref="FileInfo"/>.
        /// </returns>
        public static FileInfo GetMostRecentFile(string logFileLocation)
        {
            var directoryInfo = new DirectoryInfo(logFileLocation);
            var fileInfo = (from f in directoryInfo.GetFiles()
                         orderby f.LastWriteTime descending
                         select f).First();

            return fileInfo;
        }
    }
}
