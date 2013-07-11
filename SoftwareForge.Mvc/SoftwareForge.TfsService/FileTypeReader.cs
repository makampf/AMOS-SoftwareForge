/*
 * Copyright (c) 2013 by Denis Bach, Marvin Kampf, Konstantin Tsysin, Taner Tunc, Florian Wittmann
 *
 * This file is part of the Software Forge Overlay rating application.
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU Affero General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU Affero General Public License for more details.
 *
 * You should have received a copy of the GNU Affero General Public
 * License along with this program. If not, see
 * <http://www.gnu.org/licenses/>.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace SoftwareForge.TfsService
{
    public class FileTypeReader
    {
        /// <summary>
        /// Checks if a file is a binary with a blacklist and a whitelist
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns></returns>
        public bool CheckifFileisBinary(string path)
        {
            List<string> whitelist = new List<string>
                {
                    ".txt",
                    ".cs",
                    ".html",
                    ".c",
                    ".cshtml",
                    ".php",
                    ".js",
                    ".xml",
                    ".csproj",
                    ".htm",
                    ".rtf",
                    ".xaml",
                    ".css"
                };
            List<string> blacklist = new List<string> { ".pdf", ".exe", ".zip", ".doc", ".docx", ".dot", ".png" };
            string extension = Path.GetExtension(path);
            if (whitelist.Contains(extension))
            {
                return true;
            }
            if (blacklist.Contains(extension))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads a file line by line 
        /// </summary>
        /// <param name="path">file path</param>
        /// <param name="content">file content</param>
        /// <returns>a list of strings which each includes a line</returns>
        public String[] GetFilesFromPath(string path, String content)
        {
            if (CheckifFileisBinary(path))
            {
                if (content.Contains("\0\0"))
                    throw new Exception(path + " seems to be a binary file. Found 2 consecutive \0\0");

                return content.Split('\n');
            }
            throw new Exception(path + " seems to be a binary file. Found this file extension on the blacklist");
        }
    }
}
