using SQLisHard.Core;
using SQLisHard.Core.Data;
using SQLisHard.Core.Interfaces;
using SQLisHard.Core.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace SQLisHard.Core
{
	public class CoreMembership
	{
		private IUserStore _userStore;
		private ISessionStore _sessionStore;

		public CoreMembership(string connectionString) : this(
			new UserStore(connectionString),
			new SessionStore(connectionString)) { }

		public CoreMembership(IUserStore userStore, ISessionStore sessionStore)
		{
			_userStore = userStore;
			_sessionStore = sessionStore;
		}
		
		public UserPrincipal CreateGuest()
		{
			var coreUser = _userStore.GetNewGuestUser();
			var principal = new UserPrincipal(new GuestUser(coreUser));
			return principal;
		}

		public UserPrincipal GetUser(UserId id)
		{
			// TODO add logic to decide between creating a guest and regular user
			var coreUser = _userStore.GetUser(id);
			return new UserPrincipal(new GuestUser(coreUser));
		}

		public void CaptureSession(UserId userId, string userAgent, string hostAddress)
		{
			_sessionStore.CaptureSession(new Session(userId, userAgent, hostAddress));
}
	}

	public class UserPrincipal : IPrincipal
	{
		private GuestUser _user;

		public UserPrincipal(GuestUser user)
		{
			_user = user;
		}

		public IIdentity Identity { get { return _user; } }

		public IUserIdentity UserIdentity { get { return _user; } }

		public bool IsInRole(string role)
		{
			return false;
		}
	}

	public class GuestUser : IIdentity, IUserIdentity
	{
		public GuestUser(IUserIdentity user)
		{
			Id = user.Id;
			Name = "Guest " + user.Id.ToString();
		}

		public UserId Id { get; protected set; }

		public string AuthenticationType { get { return "forms"; } }

		public bool IsAuthenticated { get { return true; } }

		public string Name { get; protected set; }
	}
}