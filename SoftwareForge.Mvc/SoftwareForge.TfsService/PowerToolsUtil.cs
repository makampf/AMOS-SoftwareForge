/*
 * Copyright (c) 2013 by Denis Bach, Konstantin Tsysin, Taner Tunc, Marvin Kampf, Florian Wittmann
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
using System.Diagnostics;

namespace SoftwareForge.TfsService
{
    class PowerToolsUtil
    {
        private static string GetPowerToolsInstallationPath()
        {
            string result = Environment.GetEnvironmentVariable("TFSPowerToolDir");
            if (String.IsNullOrEmpty(result))
                return null;

            if (!(result.EndsWith("\\")))
                result += "\\";

            return result;
        }

        public static void CreateTfsProject(String tfsUri ,String teamCollectionName, String projectName, String templateName)
        {
            String path = GetPowerToolsInstallationPath();
            if (path == null)
                throw  new Exception("Could not find TFS-Power-Tools");

            String command = @"""" + path + Properties.Settings.Default.TeamFoundationPowerToolExecutable + @"""";

            //sourcecontrol new = new source tree
            //sourcecontrol branch = branch from existing path
            String parameter = String.Format(@"createteamproject /collection:""{0}"" /teamproject:""{1}"" /processtemplate:""{2}"" /sourcecontrol:new /noportal", tfsUri + @"/" + teamCollectionName, projectName, templateName);
            

            Process p = Process.Start(command, parameter);
            if (p == null) 
                throw new Exception("Could not create process");

            p.WaitForExit();
        }
    }
}
