using Microsoft.AspNetCore.Mvc;
using UserService.Controllers.Models;
using UserService.Logic;
using UserService.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("admin.api/v1")]
    public class AdminController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserLogic _userLogic;
        public AdminController(ILogger<UserController> logger, UserLogic userLogic)
        {
            _logger = logger;
            _userLogic = userLogic;
        }

        /// <summary>
        /// Get all existing users. This should of course be protected by some authentication.
        /// </summary>
        /// <returns>All existing users</returns>
        [HttpGet]
        [Route("users")]
        public IEnumerable<UserModel> GetAllUsers()
        {
            return _userLogic.GetUsers();
        }

        /// <summary>
        /// This might also be useful, look up user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User, or HttpStatusCode.NotFound</returns>
        [HttpGet]
        [Route("user/{id}")]
        public ActionResult<UserModel> GetUserById(int id)
        {
            UserModel user = _userLogic.GetUserById(id);
            if (user == null)
            {
                return NotFound();
            }
            return user;
        }
    }
}
