namespace Irvin.SqlParser.Metadata;

public class PermissionMetadata
{
    public AccessMode AccessMode { get; set; }
    public string ActionName { get; set; }
    public SubjectKind SubjectKind { get; set; }
    public int SubjectID { get; set; }
    public int SubSubjectID { get; set; }
    public int AccessorPrincipalID { get; set; }
}