namespace НарlochromisBurtoni.Security.Cryptography.BCrypt
{
    /// <summary>
    ///     Result codes from BCrypt APIs
    /// </summary>
    internal enum ErrorCode
    {
        Success = 0x00000000,                                       // STATUS_SUCCESS
        AuthenticationTagMismatch = unchecked((int)0xC000A002),     // STATUS_AUTH_TAG_MISMATCH
        BufferToSmall = unchecked((int)0xC0000023),                 // STATUS_BUFFER_TOO_SMALL
    }
}
