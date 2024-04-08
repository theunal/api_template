//using Core.Entities.Concrete;
//using Core.GeneralModels.Enums;
//using Core.GeneralModels.Models;
//using Core.Utilities.Encryption;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Hosting;
//using Microsoft.IdentityModel.Tokens;
//using Newtonsoft.Json;
//using System.Data;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;

//namespace Core.Utilities.JWT
//{
//    public class AccessToken
//    {
//        public string Token { get; set; } = null!;
//        public DateTime Expiration { get; set; }

//    }
//    public interface ITokenHelper
//    {
//        AccessToken CreateToken(User user);
//    }

//    public class TokenHelper : ITokenHelper
//    {
//        private DateTime _accessTokenExpiration;
//        private readonly IConfiguration conf;
//        private readonly IHostEnvironment hostEnvironment;
//        public TokenHelper(IConfiguration conf, IHostEnvironment hostEnvironment)
//        {
//            this.conf = conf;
//            this.hostEnvironment = hostEnvironment;
//        }

//        public AccessToken CreateToken(User user)
//        {
//            _accessTokenExpiration = hostEnvironment.IsDevelopment() ? DateTime.Now.AddDays(365) : DateTime.Now.AddMinutes(double.Parse(conf["Audience:TokenExpiration"]!));
//            var securityKey = SecurityKeyHelper.CreateSecurityKey(conf["Audience:Secret"]!);
//            var signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);

//            var jwt = CreateJwtSecurityToken(user, signingCredentials);
//            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
//            var token = jwtSecurityTokenHandler.WriteToken(jwt);

//            return new AccessToken
//            {
//                Token = token,
//                Expiration = _accessTokenExpiration
//            };
//        }

//        public JwtSecurityToken CreateJwtSecurityToken(User user, SigningCredentials signingCredentials) => new(
//                issuer: conf["Audience:Iss"],
//                audience: conf["Audience:Aud"],
//                expires: _accessTokenExpiration,
//                notBefore: DateTime.Now,
//                claims: SetClaims(user),
//                signingCredentials: signingCredentials);

//        private IEnumerable<Claim> SetClaims(User user)
//        {
//            var claims = new List<Claim>();
//            AddClaim(claims, "UsId", user.UserId.ToString());
//            AddClaim(claims, "BrId", user.BranchId.ToString());
//            AddClaim(claims, "ClId", user.ClientId.ToString() ?? "");
//            AddClaim(claims, "name", user.FirstName + " " + user.LastName);
//            AddClaim(claims, "email", user.Email);
//            AddClaim(claims, "phone", user.Phone);
//            AddClaim(claims, "title", user.JobTitle);

//            // olds
//            AddClaim(claims, "UsType", user.ClientId != null ? "client" : "branch");
//            AddClaim(claims, "UsLang", user.UserLang ?? "tr");
//            AddClaim(claims, "PPicture", user.ProfilePicture ?? "");
//            AddClaim(claims, "BranchIds", user.BranchIds != null ? JsonConvert.SerializeObject(user.BranchIds) : "[]");

//            AddRoles(claims, user.Claims);
//            AddClaim(claims, "NSessionHash", user.SessionHash!);

//            return claims;
//        }

//        private void AddClaim(List<Claim> claims, string key, string value)
//        {
//            claims.Add(new Claim(key, value));
//        }

//        private void AddRoles(List<Claim> claims, int[] roles)
//        {
//            claims.Add(new Claim(ClaimTypes.Role, System.Text.Json.JsonSerializer.Serialize(roles)));
//        }
//    }
//}