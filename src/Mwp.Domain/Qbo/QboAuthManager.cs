using System;
using System.Threading.Tasks;
using Mwp.Qbo.Repositories;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;

namespace Mwp.Qbo
{
    public class QboAuthManager : DomainService, IQboAuthManager
    {
        private readonly IQboOAuth _qboOAuth;
        private readonly IQboTokenRepository _qboTokenRepository;
        private readonly IRepository<QboTenant> _qboTenantRepository;

        public QboAuthManager(IQboOAuth qboOAuth,
            IQboTokenRepository qboTokenRepository,
            IRepository<QboTenant> qboTenantRepository)
        {
            _qboOAuth = qboOAuth;
            _qboTokenRepository = qboTokenRepository;
            _qboTenantRepository = qboTenantRepository;
        }

        public async Task<QboToken> GetAccessToken(string code, string state, string realmId)
        {
            var (qboToken, qboTenant) = await _qboOAuth.GetAccessTokenAsync(code, state, realmId);
            using (CurrentTenant.Change(qboToken.MwpTenantId))
            {
                await AddQboTenant(qboTenant);
                await AddQboToken(qboToken);
            }

            return qboToken;
        }

        public async Task<string> GetAuthoriseUrl(Guid mwpUserId, Guid mwpTenantId)
        {
            return await Task.FromResult(_qboOAuth.GetAuthoriseUrl(mwpUserId, mwpTenantId));
        }

        public async Task<QboToken> GetCurrentToken(Guid? mwpUserId, string qboTenantId)
        {
            return await _qboTokenRepository.GetCurrentToken(mwpUserId, qboTenantId);
        }

        public async Task<QboToken> RefreshAccessToken(Guid mwpUserId, string qboTenantId)
        {
            var currentToken = await GetCurrentToken(mwpUserId, qboTenantId);

            if (currentToken.IsRefreshTokenExpired())
            {
                throw new Exception("Refresh access token is expired. You will need to sign in again.");
            }

            var newToken = await _qboOAuth.RefreshAccessToken(currentToken);
            await AddQboToken(newToken);
            return newToken;
        }

        public async Task RemoveConnection(Guid mwpUserId, string qboTenantId)
        {
            var currentToken = await GetCurrentToken(mwpUserId, qboTenantId);
            await _qboOAuth.RemoveConnection(currentToken);
            currentToken.IsRevoked = true;
            await _qboTokenRepository.UpdateAsync(currentToken);
        }

        #region private methods

        [UnitOfWork]
        private async Task AddQboTenant(QboTenant qboTenant)
        {
            var existingQboTenant = await _qboTenantRepository.FindAsync(e => e.QboTenantId == qboTenant.QboTenantId);
            if (existingQboTenant == null)
            {
                await _qboTenantRepository.InsertAsync(qboTenant, true);
                return;
            }

            existingQboTenant.Name = qboTenant.Name;
            await _qboTenantRepository.UpdateAsync(existingQboTenant, true);
        }

        [UnitOfWork]
        private async Task AddQboToken(QboToken qboToken)
        {
            var currentToken = await GetCurrentToken(qboToken.MwpUserId, qboToken.QboTenantId);
            if (currentToken != null)
            {
                currentToken.IsRefreshed = true;
                await _qboTokenRepository.UpdateAsync(currentToken);
            }

            await _qboTokenRepository.InsertAsync(qboToken);
        }

        #endregion private methods
    }
}