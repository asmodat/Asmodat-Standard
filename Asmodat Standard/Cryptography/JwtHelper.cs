using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using AsmodatStandard.Cryptography;
using AsmodatStandard.Extensions;
using AsmodatStandard.Extensions.Collections;
using AsmodatStandard.Extensions.IO;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;

namespace AsmodatStandard.Cryptography
{
    public static class JwtHelper
    {
        public static string GenerateTokenPEMRSA(
            string issuer,
            string audience,
            IEnumerable<Claim> claims,
            string privatePemKey,
            DateTime? expires,
            DateTime? issuedAt = null,
            DateTime? notBefore = null,
            string algorithm = SecurityAlgorithms.RsaSha256Signature)
        {
            if (!algorithm.Contains("rsa-sha", CompareOptions.IgnoreCase))
                throw new ArgumentException($"Algorithm must an RSA-SHAXXX, but was: {algorithm}");

            if (claims.IsNullOrEmpty())
                throw new ArgumentException("At least one claim must be specified such for example: User Name.");

            if (issuedAt != null && !issuedAt.Value.IsUTC())
                throw new ArgumentException($"Issuance Date Time must have an UTC format.");

            if (expires != null && !expires.Value.IsUTC())
                throw new ArgumentException($"Expiration Date Time must have an UTC format.");

            var issuedAtDateTime = issuedAt ?? DateTime.UtcNow;
            var rsaSK = RSA.PemToRsaSecurityKey(privatePemKey);
            var descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                claims.ToArray()),
                SigningCredentials = new SigningCredentials(rsaSK, algorithm),
                IssuedAt = issuedAtDateTime,
                NotBefore = notBefore ?? issuedAtDateTime,
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
    }
}
