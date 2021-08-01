using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MoviesApi.Data;
using MoviesApi.Dtos;
using MoviesApi.Entities;
using MoviesApi.Services;

namespace MoviesApi.Controllers
{

    public class AccountController : applicationBaseContoller
    {
        private DataContext _context { get; }
        private UserManager<ApplicationUser> _userManager { get; }
        public IConfiguration Config { get; }
        public AccountController(DataContext context, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            this.Config = config;
            this._userManager = userManager;
            this._context = context;

        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto registerdto)
        {
            if (registerdto == null)
            {
                return BadRequest("not valid info");
            }
            if (ModelState.IsValid)
            {


                if (!isValidEmail(registerdto.Email.ToLower()))
                {
                    return BadRequest("Not Valid Email");
                }
                if (await checkEmail(registerdto.Email.ToLower()))
                {
                    return BadRequest("Email is Taken");
                }
                if (await checkUser(registerdto.userName.ToLower()))
                {
                    return BadRequest("userName is Taken");
                }
                ApplicationUser user = new ApplicationUser()
                {
                    UserName = registerdto.userName,
                    Email = registerdto.Email
                };
                var result = await _userManager.CreateAsync(user, registerdto.PasswordHash);

                if (result.Succeeded)
                {
                    //http://localhost:5000/Account/RegisterationConfirm?ID=5666&Token=5656
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var confirmationLink = Url.Action("RegistrationConfirm", "Account",
                    new { Id = user.Id, token = System.Web.HttpUtility.UrlEncode(token) }, Request.Scheme);
                    var plaintext = "please confirm your Registration";
                    var htmlContent = "<a href=\"" + confirmationLink + "\">Confirm Registration</a>";
                    var subject = "Registration Confirm";
                    var sendGrid = new SendGridApi(Config);
                    if (await sendGrid.SendMail(user.UserName, user.Email, plaintext, htmlContent, subject))
                        return Ok("Registration Complete");
                }
                else
                {
                    return BadRequest(result.Errors);
                }
            }
            return BadRequest("error .....");

        }
        [HttpGet]
        [Route("RegistrationConfirm")]
        public async Task<IActionResult> RegistrationConfirm(string Id, string token)
        {
            if (string.IsNullOrEmpty(Id) || string.IsNullOrEmpty(token))
                return NotFound();
            var user = await _userManager.FindByIdAsync(Id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.ConfirmEmailAsync(user, HttpUtility.UrlDecode(token));
            if (result.Succeeded)
            {
                return Ok("Email Address Confirm successfully");
            }
            return BadRequest(result.Errors);

        }

        private bool isValidEmail(string email)
        {
            Regex regex = new Regex(@"\w+@+\w+.com|\w+@+\w+.net|\w+@+\w+.ru");
            if (regex.IsMatch(email))
            {
                return true;
            }
            return false;
        }

        private async Task<bool> checkUser(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName.ToLower() == username);
        }

        private async Task<bool> checkEmail(string email)
        {
            return await _context.Users.AnyAsync(x => x.Email.ToLower() == email);
        }
    }
}