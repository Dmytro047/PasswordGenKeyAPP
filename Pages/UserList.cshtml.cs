using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PasswordGenKeyAPP.Models;

namespace PasswordGenKeyAPP.Pages
{
    public class UserListModel : PageModel
    {
        [BindProperty]
        public static List<UserEntity> UserList { get; private set; } = new List<UserEntity>();

        private readonly DBContext _dbContext;

        public UserListModel(DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void OnGet()
        {
            UserList = _dbContext.Users.ToList();
        }
    }
}
