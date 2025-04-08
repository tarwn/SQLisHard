using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLisHard.Core.Data;
using SQLisHard.Core.Models;
using SQLisHard.Models;
using System.Security.Claims;

namespace SQLisHard.Controllers
{
    [Route("api/[controller]/[action]")]
    [Authorize]
	public class UserController : Controller
	{
        private readonly IHistoryStore _historyStore;
        private readonly IUserStore _userStore;

        public UserController(IHistoryStore historyStore, IUserStore userStore)
        {
            _historyStore = historyStore;
            _userStore = userStore;
        }

        [HttpGet]
        [UpperCaseJSONOutput]
		public IActionResult GetLoggedInUser()
		{
            var id = int.Parse(User.FindFirstValue("id")!);
			// TODO - make async
            var user = _userStore.GetUser(new UserId(id));
            // TODO - fix the mess around UserModel using identities/principals instead of one clear domain model to/from DB
            var completedExercises = _historyStore.GetCompletedExercises(new UserId(user.Id));
            var result = new LoggedInUser(){
                id = user.Id,
                Name = user.Name,
                CompletedExercises = completedExercises
            };

            return Ok(user);
		}

	}

    record LoggedInUser {
        required public int id;
        required public string Name;
        required public List<string> CompletedExercises;
    };
}