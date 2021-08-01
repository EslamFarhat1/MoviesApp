using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesApi.Data;
using MoviesApi.Dtos;
using MoviesApi.Entities;

namespace MoviesApi.Controllers
{

    public class AccountController : applicationBaseContoller
    {
        private DataContext _context { get; }
        private UserManager<ApplicationUser> _userManager { get; }
        public AccountController(DataContext context, UserManager<ApplicationUser> userManager)
        {
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
            ApplicationUser user=new ApplicationUser(){
                UserName=registerdto.userName,
                Email=registerdto.Email
            };
           var result =await _userManager.CreateAsync(user,registerdto.PasswordHash);
           if(result.Succeeded){
               return StatusCode(StatusCodes.Status201Created,user);
           }else{
           return BadRequest(result.Errors);
           }
            }
            return BadRequest("error .....");

        }

        private bool isValidEmail(string email)
        {
            Regex regex = new Regex(@"\w+@+\w+.com|\w+@+\w+.net");
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