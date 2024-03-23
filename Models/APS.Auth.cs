using System;
using System.Threading.Tasks;
using Autodesk.Authentication;
using Autodesk.Authentication.Model;

public partial class APS
{
    public string GetAuthorizationURL()
    {
        AuthenticationClient authenticationClient = new AuthenticationClient(_SDKManager);
        ResponseType responseType = ResponseType.Code;
        return authenticationClient.Authorize(_clientId, responseType, _callbackUri, TokenScopes);

    }

    public async Task<Tokens> GenerateTokens(string code)
    {
        AuthenticationClient authenticationClient = new AuthenticationClient(_SDKManager);
        dynamic Token = await authenticationClient.GetThreeLeggedTokenAsync(_clientId, _clientSecret, code, _callbackUri);
        
        return new Tokens
        {
            AccessToken = Token.AccessToken,
            RefreshToken = Token.RefreshToken,
            ExpiresAt = DateTime.Now.ToUniversalTime().AddSeconds(Token.ExpiresIn)
        };
    }

    public async Task<Tokens> RefreshTokens(Tokens tokens)
    {
        AuthenticationClient authenticationClient = new AuthenticationClient(_SDKManager);
        dynamic Token = await authenticationClient.GetRefreshTokenAsync(_clientId, _clientSecret, tokens.RefreshToken, TokenScopes);
        return new Tokens
        {
            AccessToken = Token.AccessToken,
            RefreshToken = Token.RefreshToken,
            ExpiresAt = DateTime.Now.ToUniversalTime().AddSeconds(Token.ExpiresIn)
        };
    }

    public async Task<dynamic> GetUserProfile(Tokens tokens)
    {
        AuthenticationClient authenticationClient = new AuthenticationClient(_SDKManager);
        dynamic profile = await authenticationClient.GetUserInfoAsync(tokens.AccessToken);
        return profile;
    }
}
