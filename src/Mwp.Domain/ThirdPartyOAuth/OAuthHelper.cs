using System;

namespace Mwp.ThirdPartyOAuth
{
    public static class OAuthHelper
    {
        public const string StateDelimiter = "_";

        public static string GenerateState(Guid? mwpUserId, Guid? mwpTenantId)
        {
            var state = $"{mwpUserId}";
            if (mwpTenantId != Guid.Empty)
            {
                state += $"{StateDelimiter}{mwpTenantId}";
            }

            return state;
        }

        public static Tuple<Guid, Guid> ExtractState(string state)
        {
            var splitState = state.Split(StateDelimiter);

            var mwpUserId = new Guid(splitState[0]);
            var mwpTenantId = Guid.Empty;
            if (splitState.Length == 2)
            {
                mwpTenantId = new Guid(splitState[1]);
            }

            return new Tuple<Guid, Guid>(mwpUserId, mwpTenantId);
        }
    }
}