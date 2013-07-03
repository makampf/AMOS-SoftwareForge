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
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;

namespace SoftwareForge.TfsService
{
    public class ImpersonatedTfsTeamProjectCollection : IDisposable
    {
        private readonly String _userToImpersonate;
        private readonly TfsConfigurationServer _tfsConfigurationServer;

        public ImpersonatedTfsTeamProjectCollection(TfsConfigurationServer tfsConfigurationServer, string userToImpersonate)
        {
            _tfsConfigurationServer = tfsConfigurationServer;
            _userToImpersonate = userToImpersonate;
        }

        public TfsTeamProjectCollection Impersonate(Guid teamCollectionGuid)
        {
            TfsTeamProjectCollection tfsTeamProjectCollection = _tfsConfigurationServer.GetTeamProjectCollection(teamCollectionGuid);

            IIdentityManagementService ims = tfsTeamProjectCollection.GetService<IIdentityManagementService>();

            // Read out the identity of the user we want to impersonate
            TeamFoundationIdentity identity = ims.ReadIdentity(IdentitySearchFactor.AccountName,
                _userToImpersonate,
                MembershipQuery.None,
                ReadIdentityOptions.None);

            var impersontedTfs = new TfsTeamProjectCollection(tfsTeamProjectCollection.Uri, identity.Descriptor);
            impersontedTfs.Authenticate();

            return impersontedTfs;

        }

        public void Dispose() { }
    }
}
