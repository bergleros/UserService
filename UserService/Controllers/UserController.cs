using Microsoft.AspNetCore.Mvc;
using UserService.Controllers.Models;
using UserService.Logic;
using UserService.Models;

namespace UserService.Controllers
{
    [ApiController]
    [Route("user.api/v1")] 
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserLogic _userLogic;
        public UserController(ILogger<UserController> logger, UserLogic userLogic)
        {
            _logger = logger;
            _userLogic = userLogic;
        }

        /// <summary>
        /// Get userid from a provided secret. If no user exists for the secret, it will be created and the new userid returned.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Userid for the provided secret</returns>
        [HttpGet]
        [Route("user")]
        public UserModel GetUserBySecret([FromQuery] UserRequestModel request)
        {
            // If the provided input is actually meant to be a secret that allows the user to get their information then obviously this solution is extremely vulnerable to dictionary attacks,
            // especially with the minimum length being only 3 characters. It must also be considered quite likely that two users will provide the same secret and subsequently get the same userid.
            // It is not clear from the description what the intention of the method is, and in a real-world project I would request clarification to better understand what the requirements are
            // in order to improve the implementation. Some ideas that come to mind include increasing the minimum length, have the user provide another piece of data such as username or a 3rd party auth token,
            // or generate a key for the user to sign subsequent requests.
            return new UserModel() { UserId = _userLogic.GetUserId(request.Secret), UserSecret = request.Secret };
        }
    }
}