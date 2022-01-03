using System;
using System.Collections.Generic;
using Irvin.Parser;

namespace Irvin.SqlFountain.Core
{
    public class NativeFunctions
    {
        public const string GetDate = "GETDATE";
        public const string ScopeIdentity = "SCOPE_IDENTITY";
        public const string IsNull = "ISNULL";
        public const string Cast = "CAST";
        public const string Sum = "Sum";

        private static List<string> FunctionNames
        {
            get
            {
                return new List<string>
                {
                    GetDate,
                    ScopeIdentity,
                    IsNull,
                    Cast,
                    Sum,
                };
            }
        }

        internal static bool IsNativeFunctionName(Token token, StringComparison compareOption = StringComparison.CurrentCultureIgnoreCase)
        {
            return FunctionNames.Exists(x => token.Content.Equals(x, compareOption));
        }
    }
}