namespace XArch.AppShell.Framework
{
    public sealed class Void
    {
        private Void() { }

        public static Void Instance { get; } = new Void();
    }
}
