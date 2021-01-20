using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mwp.Xero.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Mwp.Xero
{
    public class XeroAuthManager : DomainService, IXeroAuthManager
    {
        protected IXeroTenantRepository XeroTenantRepository;
        protected IXeroTokenRepository XeroTokenRepository;
        protected IXeroConnectionRepository XeroConnectionsRepository;
        protected IXeroOAuth XeroOAuth;

        public XeroAuthManager(
            IXeroTenantRepository xeroTenantRepository,
            IXeroTokenRepository xeroTokenRepository,
            IXeroConnectionRepository xeroConnectionsRepository,
            IXeroOAuth xeroOAuth)
        {
            XeroTenantRepository = xeroTenantRepository;
            XeroTokenRepository = xeroTokenRepository;
            XeroConnectionsRepository = xeroConnectionsRepository;
            XeroOAuth = xeroOAuth;
        }

        public Task<string> GetAuthoriseUrl(Guid mwpUserId, Guid mwpTenantId)
        {
            return Task.FromResult(XeroOAuth.GetAuthoriseUrl(mwpUserId, mwpTenantId));
        }

        public async Task<XeroToken> GetAccessToken(string code, string state)
        {
            var xeroToken = await XeroOAuth.GetAccessTokenAsync(code, state);
            var xeroTenants = await XeroOAuth.GetConnectionsAsync(xeroToken);
            xeroTenants.ForEach(e =>
            {
                e.MwpTenantId = xeroToken.MwpTenantId;
                e.XeroConnection.MwpUserId = xeroToken.MwpUserId;
            });

            using (CurrentTenant.Change(xeroToken.MwpTenantId))
            {
                await AddXeroTenants(xeroTenants);
                await AddXeroConnections(xeroTenants.Select(e => e.XeroConnection));
                await AddXeroToken(xeroToken);
            }

            return xeroToken;
        }

        public async Task<XeroToken> GetCurrentXeroToken(Guid? mwpUserId, Guid xeroTenantId)
        {
            var connection = await XeroConnectionsRepository.GetAsync(mwpUserId.Value, xeroTenantId);
            if (connection == null)
            {
                return null;
            }

            return await XeroTokenRepository.GetCurrentToken(mwpUserId.Value, connection.AuthenticationEventId);
        }

        public async Task<XeroToken> RefreshAccessToken(Guid mwpUserId, Guid xeroTenantId)
        {
            var currentXeroToken = await GetCurrentXeroToken(mwpUserId, xeroTenantId);
            var refreshedXeroToken = await XeroOAuth.RefreshAccessToken(currentXeroToken);
            var newXeroToken = await UpdateRefreshedXeroToken(currentXeroToken, refreshedXeroToken);
            return newXeroToken;
        }

        public async Task RemoveConnection(Guid mwpUserId, Guid xeroTenantId)
        {
            var xeroToken = await GetCurrentXeroToken(mwpUserId, xeroTenantId);
            var xeroTenant = await GetXeroTenant(xeroTenantId, mwpUserId);
            await XeroOAuth.RemoveConnection(xeroToken, xeroTenant);
            await RemoveConnection(xeroTenant.XeroConnection);
        }

        #region private methods

        [UnitOfWork]
        private async Task<XeroToken> UpdateRefreshedXeroToken(XeroToken oldToken, XeroToken newToken)
        {
            oldToken.IsRefreshed = true;
            await XeroTokenRepository.UpdateAsync(oldToken);

            newToken.AuthenticationEventId = oldToken.AuthenticationEventId;
            newToken.MwpUserId = oldToken.MwpUserId;
            await XeroTokenRepository.InsertAsync(newToken);
            return newToken;
        }

        [UnitOfWork]
        private async Task RemoveConnection(XeroConnection xeroConnection)
        {
            xeroConnection.IsConnected = false;
            xeroConnection.IsDeleted = true;
            await XeroConnectionsRepository.UpdateAsync(xeroConnection);
        }

        private async Task<XeroTenant> GetXeroTenant(Guid xeroTenantId, Guid mwpUserId)
        {
            var xeroTenant = await XeroTenantRepository.GetAsync(e => e.XeroTenantId == xeroTenantId);
            xeroTenant.XeroConnection = await XeroConnectionsRepository.GetAsync(mwpUserId, xeroTenantId);

            return xeroTenant;
        }

        [UnitOfWork]
        private async Task AddXeroToken(XeroToken xeroToken)
        {
            var currentToken = await GetCurrentXeroToken(xeroToken.MwpUserId, xeroToken.AuthenticationEventId);
            if (currentToken != null)
            {
                currentToken.IsRefreshed = true;
                await XeroTokenRepository.UpdateAsync(currentToken);
            }

            await XeroTokenRepository.InsertAsync(xeroToken);
        }

        [UnitOfWork]
        private async Task AddXeroTenants(IEnumerable<XeroTenant> xeroTenants)
        {
            foreach (var xeroTenant in xeroTenants)
            {
                var existingXeroTenant = await XeroTenantRepository.FindAsync(e => e.XeroTenantId == xeroTenant.XeroTenantId);
                if (existingXeroTenant == null)
                {
                    await XeroTenantRepository.InsertAsync(xeroTenant, true);
                }
            }
        }

        [UnitOfWork]
        private async Task AddXeroConnections(IEnumerable<XeroConnection> xeroConnections)
        {
            foreach (var xeroConnection in xeroConnections)
            {
                var existingConnection = await XeroConnectionsRepository.GetAsync(
                    xeroConnection.XeroConnectionId,
                    xeroConnection.XeroTenantId,
                    xeroConnection.MwpUserId.Value,
                    xeroConnection.AuthenticationEventId);

                if (existingConnection != null)
                {
                    existingConnection.IsConnected = false;
                    await XeroConnectionsRepository.UpdateAsync(existingConnection);
                }

                await XeroConnectionsRepository.InsertAsync(xeroConnection);
            }
        }

        #endregion private methods
    }
}