using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TransportationApplication.SharedModels;
using TransportationApplication.UserService.DTOs;

namespace TransportationApplication.UserService.Services
{
    public class UserServices : IUserServices
    {
        //dependencies
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        //constructor
        public UserServices(UserManager<User> userManager, 
            SignInManager<User> signInManager,  
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }


        //LOGIN
        //method that will authenticate a user, accepts LoginViewModel object, returns a JWT token or null
        public async Task<string> LoginUserAsync(LoginViewModel loginViewModel)
        {
            var result = await _signInManager.PasswordSignInAsync(loginViewModel.Email, loginViewModel.Password,
                isPersistent: false, lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                return null;
            }

            //retrieve User object for JWT create
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);

            //call GenerateJwtTokenAsync to create the token with user-specific claims
            //return the generate token to allow the client to use it for auth requests
            return await GenerateJwtTokenAsync(user);
        }

        //JWT
        //initialize token handler and key, define claims, add role claims, configure token descriptor,
        //create and return the JWT
        public async Task<string> GenerateJwtTokenAsync(User user)
        {
            //tokenHandler - instance of JwtSecurityTokenHandler class
            //Handles the process of token creation and provides methods:
            //CreateToken and WriteToken
            var tokenHandler = new JwtSecurityTokenHandler();

            //key - retrieves the JWT key from the appsettings.json file, convert to byte arrary
            //Encoding.UTF8.GetBytes - converts the string to a byte array 
            //required to generate a SymmetricSecurityKey for token signing
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            //define user claims
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            //Add roles the user has
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            //configure the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"]
            };

            //generate and return the toke
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


}
