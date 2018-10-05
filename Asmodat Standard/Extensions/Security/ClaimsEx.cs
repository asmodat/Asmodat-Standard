using System;
using System.Security.Claims;
using System.Linq;
using AsmodatStandard.Extensions.Collections;
using System.Collections.Generic;

namespace AsmodatStandard.Extensions.Security
{
    public static class ClaimsEx
    {
        public static Claim GetClaim(this IEnumerable<Claim> claims, string type)
            => claims.Single(x => x.Type == type);

        public static Claim GetClaimOrDefault(this IEnumerable<Claim> claims, string type)
            => claims.SingleOrDefault(x => x.Type == type);
    }
}
