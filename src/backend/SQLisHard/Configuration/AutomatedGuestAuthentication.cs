

using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using SQLisHard.Core;

public class AutomatedGuestOptions : AuthenticationSchemeOptions
{
    public string SignInScheme;
}

public class AutomatedGuestHandler : AuthenticationHandler<AutomatedGuestOptions>
{
    private readonly IOptionsMonitor<AutomatedGuestOptions> _options;
    private readonly CoreMembership _membership;

    public AutomatedGuestHandler(
        IOptionsMonitor<AutomatedGuestOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        CoreMembership membership
        ) : base(options, logger, encoder)
    {
        _options = options;
        _membership = membership;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.HttpContext.User.Identity.IsAuthenticated)
        {
            return AuthenticateResult.NoResult();
        }

        // TODO: refactor CreateGuest to simply create + return user instead of principal/identity and to be async
        // TODO: add try/catch with Fail and capture exception to exception reporting for cleaner, non-leaky UX
        var user = _membership.CreateGuest();
        var claims = new[]{
                    new Claim("Id", user.UserIdentity.Id.ToString())
                };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        await Request.HttpContext.SignInAsync(_options.CurrentValue.SignInScheme, principal);
        return AuthenticateResult.Success(ticket);
    }
}