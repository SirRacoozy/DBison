using DBison.Core.Entities.Enums;

namespace DBison.Core.Entities;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "DSN is a acronym therefore should be uppercase.")]
public record DSNEntry(string DSNPattern, string DatabaseName, string Username, string ServerName, bool TrustedConnection, eDSNArchitecture Architecture = eDSNArchitecture.x86x64);
