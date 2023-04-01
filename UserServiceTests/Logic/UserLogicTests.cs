using Microsoft.VisualStudio.TestTools.UnitTesting;
using UserService.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Models;

namespace UserService.Logic.Tests
{
    [TestClass()]
    public class UserLogicTests
    {
        UserLogic _logic;
        [TestInitialize]
        public void Init()
        {
            _logic = new UserLogic();
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetUserId_WhenSecretTooShort_ThrowsException()
        {   UserLogic logic = new UserLogic();
            _logic.GetUserId("a");
        }

        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void GetUserId_WhenSecretTooLong_ThrowsException()
        {
            _logic.GetUserId(Guid.NewGuid().ToString());
        }

        [TestMethod()]
        public void GetUserId_WhenSecretValid_ReturnsUserId()
        {
            int userid = _logic.GetUserId("abc");
            
            Assert.IsTrue(userid > 0);
        }

        [TestMethod]
        public void GetUserId_WhenSameSecret_ReturnsSameUserId()
        {
            int userId1 = _logic.GetUserId("abc");
            int userId2 = _logic.GetUserId("abc");

            Assert.AreEqual(userId1, userId2);
        }

        [TestMethod]
        public void GetUserId_WhenDifferentSecret_ReturnsDifferentUserId()
        {
            // Act
            int userId1 = _logic.GetUserId("abc");
            int userId2 = _logic.GetUserId("def");

            Assert.AreNotEqual(userId1, userId2);
        }

        [TestMethod]
        public void GetUserId_WhenConcurrentWithDifferentSecrets_GeneratesNewUserIds()
        {
            // Run 5 concurrent tasks to get userids for 5 different secrets 
            Task[] tasks = new Task[5];
            List<int> results = new();

            for (int i = 0; i < 5; i++)
            {
                string secret = $"abc{i}";
                tasks[i] = Task.Run(() => results.Add(_logic.GetUserId(secret)));
            }
            Task.WaitAll(tasks);

            // Create a hash set to get unique values from the result list and verify that we got 5 different userids
            HashSet<int> h = results.ToHashSet();
            Assert.AreEqual(5, h.Count);
        }

        [TestMethod]
        public void GetUsersTest_WhenNoUserAdded_ReturnsEmptyList()
        {
            IEnumerable<UserModel> users = _logic.GetUsers();
            
            Assert.AreEqual(0, users.Count());
        }

        [TestMethod]
        public void GetUsersTest_WhenUserAdded_ReturnsUserIdAndSecret()
        {
            int userId = _logic.GetUserId("abc");
            IEnumerable<UserModel> users = _logic.GetUsers();
            
            Assert.AreEqual(1, users.Count());
            Assert.AreEqual(userId, users.First().UserId);
            Assert.AreEqual("abc", users.First().UserSecret);
        }

    }
}