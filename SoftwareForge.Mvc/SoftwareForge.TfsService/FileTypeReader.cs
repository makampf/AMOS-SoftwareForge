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
            List<string> blacklist = new List<string> { ".pdf", ".exe", ".zip", ".doc", ".docx", ".dot" };
            string extension = Path.GetExtension(path);
            if (whitelist.Contains(extension))
            {
                return true;
            }
            if (blacklist.Contains(extension))
            {
                return false;
            }
            //TODO check if binary file wihtout blacklist
            return false;
        }

        /// <summary>
        /// Reads a file line by line 
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>a list of strings</returns>
        public List<string> GetFilesFromPath(string path)
        {
            if (File.Exists(path))
            {

                if (CheckifFileisBinary(path))
                {
                    FileStream open = new FileStream(path, FileMode.Open, FileAccess.Read);
                    StreamReader reader = new StreamReader(open);
                    List<string> list = new List<string>();
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        list.Add(line);
                    }
                    open.Close();
                    reader.Close();
                    return list;

                }
                throw new Exception(path + " seems to be a binary file");
            }
            throw new Exception(path + " seems to be not existing");
        }
        
    }
}
