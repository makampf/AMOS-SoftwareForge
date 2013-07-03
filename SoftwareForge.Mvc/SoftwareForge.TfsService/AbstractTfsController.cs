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
using Microsoft.TeamFoundation.Client;
using SoftwareForge.DbService;


namespace SoftwareForge.TfsService
{
    /// <summary>
    /// The TfsController. Has the logic to control and query the tfs.
    /// </summary>
    public abstract class AbstractTfsController
    {
        protected TfsConfigurationServer TfsConfigurationServer;
        protected readonly TfsDbController TfsDbController;


        /// <summary>
        /// Bool if the tfs has authenticated.
        /// </summary>
        public bool HasAuthenticated
        {
            get { return TfsConfigurationServer.HasAuthenticated; }
        }


        /// <summary>
        /// Constructor of the tfsController.
        /// </summary>
        /// <param name="tfsUri">The uri of the tfs</param>
        /// <param name="connectionString">The connection String to the mssql-server holding the ProjectCollections</param>
        protected AbstractTfsController(Uri tfsUri, String connectionString)
        {
            TfsConfigurationServer = new TfsConfigurationServer(tfsUri);
            TfsConfigurationServer.Authenticate();

            TfsDbController = new TfsDbController(connectionString);
        }



        /// <summary>
        /// Method to reconnect to the tfs, so the cache will be updated. Sometimes must be called multiple times.
        /// </summary>
        protected void ForceTfsCacheClean()
        {
            TfsConfigurationServer = new TfsConfigurationServer(TfsConfigurationServer.Uri);
            TfsConfigurationServer.Authenticate();
        }


    }

}