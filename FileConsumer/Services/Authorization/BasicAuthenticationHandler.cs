using FileConsumer.Services.Authorization;
using FileConsumer.Utilities;
using FileConsumer.Utilities.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace FileConsumer.Services.Validators;

public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly BasicAuthSettings _authSettings;
    private readonly IUserMessagesProvider _messagesProvider;

    [Obsolete]
    public BasicAuthenticationHandler(
        IUserMessagesProvider errorMessagesProvider,
        IOptions<BasicAuthSettings> authSettings,
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
        _authSettings = authSettings.Value;
        _messagesProvider = errorMessagesProvider;
    }


    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (Request.Path.StartsWithSegments("/swagger"))
            return AuthenticateResult.NoResult();


        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.Fail(_messagesProvider.GetString(MessageKey.MissigAuthorization));


        try
        {
            var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':', 2);
            var username = credentials[0];
            var password = credentials[1];

            if (username != _authSettings.Username || password != _authSettings.Password)
                return AuthenticateResult.Fail(_messagesProvider.GetString(MessageKey.InvalidCredentials));


            var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Name, username),
        };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            Logger.LogInformation(_messagesProvider.GetString(MessageKey.AuthenticationSuccessful));
            return AuthenticateResult.Success(ticket);
        }
        catch (Exception ex)
        {
            return AuthenticateResult.Fail(_messagesProvider.GetString(MessageKey.InvalidAuthorizationHeader));
        }
    }


}