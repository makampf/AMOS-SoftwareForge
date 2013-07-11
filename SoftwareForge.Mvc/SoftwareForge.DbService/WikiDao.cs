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
using System.Collections.Generic;
using System.Linq;
using SoftwareForge.Common.Models;

namespace SoftwareForge.DbService
{
    public class WikiDao
    {
        /// <summary>
        /// The DatabaseContext
        /// </summary>
        private static readonly SoftwareForgeDbContext SoftwareForgeDbContext = new SoftwareForgeDbContext();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="wiki"></param>
        /// <returns></returns>
        public static WikiModel AddEntry(WikiModel wiki)
        {
            WikiModel createdEntry = SoftwareForgeDbContext.Entries.Add(wiki);
            SoftwareForgeDbContext.SaveChanges();
            return createdEntry;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="wiki"></param>
        /// <returns></returns>
        public static WikiModel GetEntry(int id)
        {
            return SoftwareForgeDbContext.Entries.FirstOrDefault(e => e.Id == id);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectGuid"></param>
        /// <returns></returns>
        public static List<WikiModel> GetEntriesForProject(Guid projectGuid)
        {
            try
            {
                return SoftwareForgeDbContext.Entries.Where(e => e.ProjectGuid == projectGuid).ToList();
            }
            catch
            {
                return new List<WikiModel>();
            }
        }
    }
}
