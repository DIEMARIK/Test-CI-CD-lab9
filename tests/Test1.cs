using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
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
}
