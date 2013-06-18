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

using System.Collections.Generic;

namespace SoftwareForge.Common.Models
{
    public abstract class CompositeItem
    {
        protected string Path;
        protected List<CompositeItem> SubItems = new List<CompositeItem>();

        // Constructor
        protected CompositeItem(string path)
        {
            Path = path;
        }

       

        public abstract void Add(CompositeItem c);

        public abstract void Remove(CompositeItem c);

        public string GetPath()
        {
            return Path;
        }

        public string GetName()
        {
            return Path.Remove(0, Path.LastIndexOf('/') + 1);
        }

        public List<CompositeItem> GetSubItems()
        {
            return SubItems;
        }
    }
}
