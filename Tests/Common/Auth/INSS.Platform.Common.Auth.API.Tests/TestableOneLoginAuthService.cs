//using INSS.Platform.Common.Auth.API.Services;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace INSS.Platform.Common.Auth.API.Tests
//{
//    internal class TestableOneLoginAuthService : OneLoginAuthService
//    {
//        public string QueryJwtPrivateKey { get; set; }
//        public string StateJwtPrivateKey { get; set; }
//        public string StateJwtPublicKey { get; set; }
//        public System.Security.Claims.Claim[] MockClaims { get; set; }
//        public bool ThrowOnValidateToken { get; set; }

//        public TestableOneLoginAuthService(ILogger<OneLoginAuthService> logger, IConfiguration config, IHttpClientFactory factory)
//            : base(logger, config, factory) { }

//        protected override async Task<string> GetQueryJwtPrivateKeyAsync() => QueryJwtPrivateKey;
//        protected override async Task<string> GetStateJwtPrivateKeyAsync() => StateJwtPrivateKey;
//        protected override async Task<string> GetStateJwtPublicKeyAsync() => StateJwtPublicKey;

//        protected override IEnumerable<System.Security.Claims.Claim> GetClaimsFromJwt(string jwtToken)
//        {
//            return MockClaims ?? base.GetClaimsFromJwt(jwtToken);
//        }

//        // Simulate token validation
//        protected override void ValidateToken(string token, TokenValidationParameters parameters, out SecurityToken? validatedToken)
//        {
//            if (ThrowOnValidateToken)
//            {
//                throw new InvalidOperationException("Invalid token");
//            }

//            validatedToken = null;
//        }
//    }
//}
