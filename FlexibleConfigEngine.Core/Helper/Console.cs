﻿using System;
using System.Collections.Generic;
using System.Text;
using Serilog;

namespace FlexibleConfigEngine.Core.Helper
{
    public class Console : IConsole
    {
        private readonly ILogger _log;

        public Console(ILogger log)
        {
            _log = log;
        }

        public void Information(Exception e, string message, params object[] props)
        {
            _log.ForContext("Console", true).Information(e, message, props);
        }

        public void Information(string message, params object[] props)
        {
            _log.ForContext("Console", true).Information(message, props);
        }

        public void Warning(Exception e, string message, params object[] props)
        {
            _log.ForContext("Console", true).Warning(e, message, props);
        }

        public void Warning(string message, params object[] props)
        {
            _log.ForContext("Console", true).Warning(message, props);
        }

        public void Error(Exception e, string message, params object[] props)
        {
            _log.ForContext("Console", true).Error(e, message, props);
        }

        public void Error(string message, params object[] props)
        {
            _log.ForContext("Console", true).Warning(message, props);
        }

        public void Pause(string message = "")
        {
            if (!string.IsNullOrEmpty(message))
                Information(message);

            System.Console.ReadKey();
        }

        public string ReadLine()
        {
            var input = System.Console.ReadLine();
            _log.Information("User entered: {input}", input);

            return input;
        }

        public string ReadPassword()
        {
            var sb = new StringBuilder();
            while (true)
            {
                var cki = System.Console.ReadKey(true);

                if (cki.Key == ConsoleKey.Enter)
                {
                    System.Console.WriteLine();
                    break;
                }

                if (cki.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length > 0)
                    {
                        var pos = System.Console.CursorLeft;
                        System.Console.SetCursorPosition(pos - 1, System.Console.CursorTop);
                        System.Console.Write(" ");
                        System.Console.SetCursorPosition(pos - 1, System.Console.CursorTop);
                        sb.Length--;
                    }
                    continue;
                }

                System.Console.Write("*");
                sb.Append(cki.KeyChar);
            }

            return sb.ToString();
        }
    }
}
