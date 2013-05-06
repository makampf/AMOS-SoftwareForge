/*
 * Copyright (c) 2013 by Denis Bach, Konstantin Tsysin, Taner Tunc, Marvin Kampf, Florian Wittmann
 *
 * This file is part of the Software Forge Overlay application.
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
using System.Data.Entity;
using SoftwareForge.Common.Models;

namespace SoftwareForge.DbService
{
    /// <summary>
    /// The SoftwareForgeDbContext.
    /// </summary>
    class SoftwareForgeDbContext : DbContext
    {
        /// <summary>
        /// The DbSet for projects.
        /// </summary>
        public DbSet<Project> Projects { get; set; }

        /// <summary>
        /// The DbSet for users.
        /// </summary>
        public DbSet<User> Users { get; set; } 
    }
}
