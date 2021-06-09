using System.Collections.Generic;
using System.Linq;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AppUser>> GetUsers()
        {
            return _context.AppUsers.ToList();
        }

        // api/users/1
        [HttpGet("{id}")]
        public ActionResult<AppUser> GetUsers(int id)
        {
            return _context.AppUsers.Find(id);
        }
    }
}