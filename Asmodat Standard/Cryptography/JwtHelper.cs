using System;
using System.Collections.Generic;
using System.Linq;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.IO;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace AsmodatStandard.Cryptography
{
    public static class JwtHelper
    {
        public static string GenerateTokenPEMRS256(
            string issuer,
            string audience,
            IEnumerable<Claim> claims,
            string privatePemKey,
            DateTime expires,
            DateTime issuedAt,
            DateTime notBefore)
        {
            if (claims.IsNullOrEmpty())
                throw new ArgumentException("At least one claim must be specified such for example: User Name.");

            if (!issuedAt.IsUTC())
                throw new ArgumentException($"Issuance Date Time (issuedAt) must have an UTC format.");

            if (!expires.IsUTC())
                throw new ArgumentException($"Expiration Date Time (expires) must have an UTC format.");

            if (!notBefore.IsUTC())
                throw new ArgumentException($"Not Before Date Time (notBefore) must have an UTC format.");

            var rsaSK = RSA.PemToRsaSecurityKey(privatePemKey);
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                claims.ToArray()),
                SigningCredentials = new SigningCredentials(rsaSK, SecurityAlgorithms.RsaSha256Signature),
                IssuedAt = issuedAt,
                NotBefore = notBefore,
                Expires = expires,
                Audience = audience,
                Issuer = issuer,
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateJwtSecurityToken(descriptor);
            return handler.WriteToken(token);
        }

        /// <summary>
        /// Verifies Token signature unless other parameters are specified
        /// </summary>
        public static bool VerifyTokenPEMRSA(string token, string publiPemKey,
            IEnumerable<string> validIssuers = null,
            IEnumerable<string> validAudience = null,
            bool verifyIfNotExpired = false)
        {
            try
            {
                var rsaSK = RSA.PemToRsaSecurityKey(publiPemKey);
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = (JwtSecurityToken)handler.ReadToken(token);
                var tvp = new TokenValidationParameters()
                {
                    IssuerSigningKey = rsaSK,
                    RequireExpirationTime = verifyIfNotExpired,
                    ValidateLifetime = verifyIfNotExpired,
                    ValidateIssuer = !validIssuers.IsNullOrEmpty(),
                    ValidIssuers = validIssuers,
                    ValidateAudience = !validAudience.IsNullOrEmpty(),
                    ValidAudiences = validAudience,
                    RequireSignedTokens = true,
                    ValidateActor = false,
                    
                    ValidateIssuerSigningKey = true,
                    ValidateTokenReplay = false
                };

                var principal = handler.ValidateToken(token, tvp, out var securityToken);

                if(verifyIfNotExpired && securityToken.ValidTo < DateTime.UtcNow)
                        return false;
            }
            catch(Exception ex)
            {
                return false;
            }

            return true;
        }

        public static SecurityToken ToSecurityTokenPEMRSA(string token, string publiPemKey = null)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = (JwtSecurityToken)handler.ReadToken(token);
            var tvp = new TokenValidationParameters()
            {
                IssuerSigningKey = publiPemKey.IsNullOrEmpty() ? null : RSA.PemToRsaSecurityKey(publiPemKey),
                RequireExpirationTime = false,
                ValidateLifetime = false,
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireSignedTokens = !publiPemKey.IsNullOrEmpty(),
                ValidateActor = false,
                ValidateIssuerSigningKey = !publiPemKey.IsNullOrEmpty(),
                ValidateTokenReplay = false
            };

            var principal = handler.ValidateToken(token, tvp, out var securityToken);
            return securityToken;
        }

        public static ClaimsPrincipal ToClaimsPrincipalPEMRSA(string token, string publiPemKey = null)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = (JwtSecurityToken)handler.ReadToken(token);
            var tvp = new TokenValidationParameters()
            {
                IssuerSigningKey = publiPemKey.IsNullOrEmpty() ? null : RSA.PemToRsaSecurityKey(publiPemKey),
                RequireExpirationTime = false,
                ValidateLifetime = false,
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireSignedTokens = !publiPemKey.IsNullOrEmpty(),
                ValidateActor = false,
                ValidateIssuerSigningKey = !publiPemKey.IsNullOrEmpty(),
                ValidateTokenReplay = false
            };

            return handler.ValidateToken(token, tvp, out var securityToken);
        }
    }
}
