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

namespace SoftwareForge.Common.Models
{
    public class WorkItem
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public string Iteration { get; set; }
        public string Area { get; set; }

        //Zugewiesen an
        public string AssignedTo { get; set; }

        //Aufwand
        public string StoryPoints { get; set; }

        //Schweregrad
        public string Severity { get; set; }
    }
}