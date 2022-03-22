using System;

namespace MccSoft.DbSyncApp.Http
{
    public class SignInException : Exception
    {
        public SignInException(string stringContent) : base(stringContent) { }
    }
}
