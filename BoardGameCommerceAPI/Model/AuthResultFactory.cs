public static class AuthResultFactory
{
    public static AuthResult Ok(string token)
        => new AuthResult { Success = true, Token = token };

    public static AuthResult Fail(string error)
        => new AuthResult { Success = false, Error = error };
}