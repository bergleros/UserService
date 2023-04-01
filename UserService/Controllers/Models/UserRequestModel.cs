using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace UserService.Controllers.Models
{
    public class UserRequestModel
    {
        /// <summary>
        /// The user secret
        /// It is quite an overkill to put this parameter in a request model when it really only needs to be a string.
        /// The nice thing about using the data annotations though is that it is a clear and simple way to define valid input and provide consistent error messages.
        /// </summary>
        [MinLength(3)]
        [MaxLength(32)]
        [JsonRequired]
        [JsonProperty(PropertyName = "secret")]
        public string Secret { get; set; }
    }
}
