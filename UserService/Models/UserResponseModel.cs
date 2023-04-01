namespace UserService.Models
{
    /// <summary>
    /// User information data model
    /// 
    /// This is quite trivial in this example and a model class is not really necessary, 
    /// but I would expect more data to belong here as requirements were clarified.
    /// </summary>
    public class UserModel
    {
        /// <summary>
        /// Unique userid
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Secret key from the user
        /// </summary>
        public string UserSecret { get; set; }
    }
}
