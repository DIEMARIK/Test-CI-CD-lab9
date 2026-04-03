using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace lab9._1
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void Password_ShouldBeHashed()
        {
            var user = new User("test", "1234");

            Assert.AreNotEqual("1234", user.PasswordHash);
        }

        [TestMethod]
        public void ValidatePassword_ShouldReturnTrue_ForCorrectPassword()
        {
            var user = new User("test", "1234");

            Assert.IsTrue(user.ValidatePassword("1234"));
        }

        [TestMethod]
        public void ValidatePassword_ShouldReturnFalse_ForWrongPassword()
        {
            var user = new User("test", "1234");

            Assert.IsFalse(user.ValidatePassword("0000"));
        }

        [TestMethod]
        public void ChangeRole_ShouldUpdateRole()
        {
            var user = new User("test", "1234");

            user.ChangeRole(UserRole.Admin);

            Assert.AreEqual(UserRole.Admin, user.Role);
        }
    }
    [TestClass]
    public class AuthenticationServiceTests
    {
        [TestMethod]
        public void RegisterUser_ShouldCreateUser()
        {
            var service = new AuthenticationService();
            var user = service.RegisterUser("user1", "1234");

            Assert.IsNotNull(user);
            Assert.AreEqual("user1", user.Username);
        }

        [TestMethod]
        public void RegisterUser_ShouldThrowInvalidOperationException_IfUserExists()
        {
            var service = new AuthenticationService();
            service.RegisterUser("user1", "1234");

            try
            {
                service.RegisterUser("user1", "1234");
                Assert.Fail("Exception was not thrown");
            }
            catch (InvalidOperationException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Login_ShouldThrowUnauthorizedAccessException_ForWrongPassword()
        {
            var service = new AuthenticationService();
            service.RegisterUser("user1", "1234");

            try
            {
                service.Login("user1", "0000");
                Assert.Fail("Exception was not thrown");
            }
            catch (UnauthorizedAccessException)
            {
                Assert.IsTrue(true);
            }
        }

        [TestMethod]
        public void Login_ShouldThrowInvalidOperationException_IfUserNotFound()
        {
            var service = new AuthenticationService();

            try
            {
                service.Login("unknown", "1234");
                Assert.Fail("Exception was not thrown");
            }
            catch (InvalidOperationException)
            {
                Assert.IsTrue(true);
            }
        }
        [TestMethod]
        public void ChangeUserRole_ShouldChangeRole_WhenAdmin()
        {
            var service = new AuthenticationService();

            var admin = service.RegisterUser("admin", "1234");
            admin.ChangeRole(UserRole.Admin);

            var user = service.RegisterUser("user1", "1234");

            service.ChangeUserRole(admin, user, UserRole.Admin);

            Assert.AreEqual(UserRole.Admin, user.Role);
        }

        [TestMethod]
        public void ChangeUserRole_ShouldThrowUnauthorizedAccessException_WhenNotAdmin()
        {
            var service = new AuthenticationService();

            var user1 = service.RegisterUser("user1", "1234");
            var user2 = service.RegisterUser("user2", "1234");

            try
            {
                service.ChangeUserRole(user1, user2, UserRole.Admin);
                Assert.Fail("Exception was not thrown");
            }
            catch (UnauthorizedAccessException)
            {
                Assert.IsTrue(true);
            }
        }
    }
    [TestClass]
    public class IntegrationTests
    {
        private const string TestLogPath = "test_log.txt";

        [TestInitialize]
        public void Setup()
        {
            // Удаляем файл перед тестом, если существует
            if (File.Exists(TestLogPath))
                File.Delete(TestLogPath);
        }

        [TestMethod]
        public void BigBang_UpdateUser_ShouldUpdatePasswordAndRole_AndWriteLog()
        {
            var authService = new AuthenticationService();
            var logService = new FileLogService(TestLogPath);
            var updater = new UserUpdater(authService, logService);

            // создаём пользователя
            authService.RegisterUser("user1", "oldpass");

            // обновляем
            var updatedUser = updater.Update("user1", "newpass", UserRole.Admin);

            Assert.AreEqual(UserRole.Admin, updatedUser.Role);
            Assert.IsTrue(updatedUser.ValidatePassword("newpass"));
            Assert.IsFalse(updatedUser.ValidatePassword("oldpass"));

            Assert.IsTrue(File.Exists(TestLogPath));
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(TestLogPath))
                File.Delete(TestLogPath);
        }
    }
}
