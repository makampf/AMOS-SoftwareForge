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

using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SoftwareForge.TfsService.UnitTests
{
    [TestClass]
    public class FileTypeReaderUtc
    {
        [TestMethod]
        public void TestFile1()
        {
            FileTypeReader test = new FileTypeReader();
            string path = "C:\\Users\\Administrator\\Desktop\\AMOS-SoftwareForge\\SoftwareForge.Mvc\\SoftwareForge.TfsService.UnitTests\\Files\\test.html";
            //string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files\\test.txt");
            test.GetFilesFromPath(path);
            List<string> expectedFileContent = new List<string> { "test", "test123", "blatest" };
            List<string> actual = test.GetFilesFromPath(path);
            CollectionAssert.AreEqual(expectedFileContent, actual);

        }
    }
}
