using System;

namespace XArch.AppShell.Framework
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class AtlasStudioPluginAttribute : Attribute
    {
        public string Name { get; }
        public string? Description { get; }
        public bool Enabled { get; }

        public AtlasStudioPluginAttribute(string name, string? description = null, bool enable = true)
        {
            Name = name;
            Description = description;
            Enabled = enable;
        }
    }

}
