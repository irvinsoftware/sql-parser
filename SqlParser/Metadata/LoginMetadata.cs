using System;

namespace Irvin.SqlParser.Metadata
{
    public class LoginMetadata
    {
        public string Name { get; }
        public PrincipalKind Kind { get; set; }
        public string DefaultDatabaseName { get; }
        public string LanguageName { get; }
        public DateTime Created { get; }
        public DateTime LastUpdated { get; }
    }
}