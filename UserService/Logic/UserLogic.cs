using UserService.Models;

namespace UserService.Logic
{
    /// <summary>
    /// This class manages access to the user data
    /// </summary>
    public class UserLogic
    {
        // The data is stored here in memory of the class instance. Of course as soon as there is more than one node running the application then this becomes a problem,
        // each node will only have its own data and know nothing about what userids the other nodes have returned.
        //
        // In a production environment, one solution could be to route requests to nodes based on the secret (e.g. calculate a numeric value from the secret and select
        // the node based on the modulo of the number of nodes). Another solution could be a shared data storage such as a database or a Redis cache.
        private readonly Dictionary<string, int> _users = new();
        private int _lastUserId = 0;
        private readonly object _addLock = new object();

        /// <summary>
        /// Get the userid for a given secret. If the secret is not found then a new userid will be created.
        /// </summary>
        /// <param name="secret"></param>
        /// <returns>The userid for the provided secret</returns>
        public int GetUserId(string secret)
        {
            if (secret.Length < 3 || secret.Length > 32)
            {
                // This shouldn't happen since the API controller restricts the input. Adding a check here and raising an exception
                // in case it gets bypassed, e.g. if someone adds another API endpoint and forgets to restrict the length. 
                // It will be annoying though to have to change this in many places if we were to change the length restrictions,
                // so it might be a good idea to define this as a shared constant.
                throw new ArgumentException($"Secret must be between 3 and 32 characters, is {secret.Length}");
            }

            // According to the description we expect a high number of requests to get user by secret. Depending on how high the number is and
            // how often we are getting existing users, we might want to cache the results to avoid entering this locked section for every request.

            lock (_addLock)
            {
                if (_users.ContainsKey(secret))
                {
                    return _users[secret];
                }
                else
                {
                    int userId = GenerateNextUserId();
                    _users.Add(secret, userId);
                    return userId;
                }
            }
        }

        /// <summary>
        /// Get a user by id, this method is inefficient and should not be used where a high number of requests are expected
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UserModel GetUserById(int id)
        {
            foreach (KeyValuePair<string, int> user in _users)
            {
                if (user.Value == id)
                {
                    return new UserModel() { UserId = user.Value, UserSecret = user.Key };
                }
            }
            return null;
        }

        /// <summary>
        /// Get a list of all existing users
        /// </summary>
        /// <returns>A list of all existing users</returns>
        public IEnumerable<UserModel> GetUsers() 
        {
            // I could have opted to store the UserModel for each user in the Hashtable and then this could simply be `return _users.Values.ToList()`
            // but because we expect a low number of these requests I decided in favor of keeping the data in memory small. 
            // The list could also be generated with a one line linq expression, but this is more readable in my opinion.
            List<UserModel> users = new ();
            foreach (KeyValuePair<string, int> user in _users)
            {
                users.Add(new UserModel { UserId = user.Value, UserSecret = user.Key });
            }
            return users;
        }

        /// <summary>
        /// Generates a new userid
        /// </summary>
        /// <returns>The generated userid</returns>
        private int GenerateNextUserId()
        {
            // The Interlocked.Increment operation is an atomic version of the ++ operator
            return Interlocked.Increment(ref _lastUserId);
        }
    }
}
