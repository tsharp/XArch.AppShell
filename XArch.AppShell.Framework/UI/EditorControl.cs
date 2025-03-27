using System.Windows.Controls;

using XArch.AppShell.Framework.Events;

namespace XArch.AppShell.Framework.UI
{
    public class EditorControl : UserControl
    {
        protected readonly IEventManager _eventManager;

        public EditorControl(IEventManager eventManager, string filePath)
        {
            _eventManager = eventManager;
            this.FilePath = filePath;
        }

        public readonly string FilePath;
        private bool _isDirty = false;
        public event System.EventHandler? IsDirtyChanged;
        public void SaveFile()
        {
            _eventManager.Publish("project.file.saving", FilePath);
            Save();
            IsDirty = false;
            _eventManager.Publish("project.file.saved", FilePath);
        }

        protected virtual void Save() { }

        public virtual bool IsDirty
        {
            get => _isDirty;
            protected set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    IsDirtyChanged?.Invoke(this, System.EventArgs.Empty);
                }
            }
        }
    }
}
