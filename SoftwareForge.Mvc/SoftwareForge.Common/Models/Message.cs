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

using System.ComponentModel.DataAnnotations;

namespace SoftwareForge.Common.Models
{
    public class Message
    {
        /// <summary>
        /// The message id
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The text message
        /// </summary>
        [MaxLength(100)]
        [DataType(DataType.MultilineText)]
        public string Title { get; set; }

        /// <summary>
        /// The text message
        /// </summary>
        [MaxLength(4000)]
        [DataType(DataType.MultilineText)]
        public string Text { get; set; }

        /// <summary>
        /// The user id, the message is sent from
        /// </summary>
        public int FromUserId { get; set; }
       

        /// <summary>
        /// The user id, the message is sent to
        /// </summary>
        public int ToUserId { get; set; }
        
    }
}
