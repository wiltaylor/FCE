﻿using System;

namespace FlexibleConfigEngine.Core.Helper
{
    public interface IConsole
    {
        void Information(Exception e, string message, params object[] props);
        void Information(string message, params object[] props);
        void Warning(Exception e, string message, params object[] props);
        void Warning(string message, params object[] props);
        void Error(Exception e, string message, params object[] props);
        void Error(string message, params object[] props);
        void Pause(string message = "");
        string ReadLine();
        string ReadPassword();
    }
}
