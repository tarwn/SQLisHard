using Moq;
using NUnit.Framework;
using SQLisHard.Core.Data;
using SQLisHard.Core.Models;

namespace SQLisHard.Core.Tests
{
	[TestFixture]
    public class CoreMembershipTests
    {

		[Test]
		public void CreateGuest_NoArgs_ReturnsGuestUserWithDatabaseId()
		{
			int dbId = 1234;
			var userStore = new Mock<IUserStore>();
			userStore.Setup(us => us.GetNewGuestUser())
					 .Returns(new User() { Id = dbId });
			var sessionStore = new Mock<ISessionStore>();
			var membership = new CoreMembership(userStore.Object, sessionStore.Object);

			var newGuest = membership.CreateGuest();

			Assert.That(newGuest.UserIdentity.Id.Value, Is.EqualTo(dbId));
		}

		[Test]
		public void GetUser_ValidId_ReturnsUserPrincipalWithDatabaseId()
		{
			var expectedId = new UserId(1234);
			var userStore = new Mock<IUserStore>();
			userStore.Setup(us => us.GetUser(expectedId))
					 .Returns(new User() { Id = expectedId.Value });
			var sessionStore = new Mock<ISessionStore>();
			var membership = new CoreMembership(userStore.Object, sessionStore.Object);

			var newGuest = membership.GetUser(expectedId);

			Assert.That(newGuest.UserIdentity.Id, Is.EqualTo(expectedId));
		}

		[Test]
		public void CaptureSession_ValidInputs_StoresSessionToDatabase()
		{
			var userStore = new Mock<IUserStore>();
			var sessionStore = new Mock<ISessionStore>();
			var membership = new CoreMembership(userStore.Object, sessionStore.Object);

			membership.CaptureSession(new UserId(123), "abc 123", "123.456.789.000");

			sessionStore.Verify(ss => ss.CaptureSession(It.Is<Session>(s => s.UserId == 123 && s.UserAgent == "abc 123" && s.RemoteAddress == "123.456.789.000")));
		}
    }
}
