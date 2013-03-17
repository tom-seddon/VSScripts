// Guids.cs
// MUST match guids.h
using System;

namespace Company.VSScripts
{
    static class GuidList
    {
        public const string guidVSScriptsPkgString = "174b2dd0-5bde-4d1b-9e44-027f43e07c79";
        public const string guidVSScriptsCmdSetString = "0ff35915-3812-4541-892b-a3ef0e316ddd";

        public static readonly Guid guidVSScriptsCmdSet = new Guid(guidVSScriptsCmdSetString);
    };
}