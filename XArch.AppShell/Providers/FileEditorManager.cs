using System.IO;

using XArch.AppShell.Framework;
using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.UI;

namespace XArch.AppShell.Providers
{
    public class FileEditorManager : ProviderManagerBase<IFileEditorFactory>, IFileEditorManager
    {
        private readonly DockViewManager _viewManager;
        private readonly IEventManager _eventManager;

        public FileEditorManager(DockViewManager viewManager, IEventManager eventManager)
        {
            this._viewManager = viewManager;
            this._eventManager = eventManager;

            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            _eventManager.Subscribe("project.file.open", @event =>
            {
                var filePath = @event.Payload?.ToString();

                if (filePath == null || !File.Exists(filePath))
                {
                    @event.Cancel();
                    return;
                }

                if (_viewManager.IsDocumentOpen(filePath))
                {
                    _viewManager.FocusDocument(filePath);
                    return;
                }

                var factory = GetFactory(filePath);

                if (factory == null)
                {
                    @event.Cancel();
                    return;
                }

                _viewManager.OpenDocument(filePath, Path.GetFileName(filePath), factory);
            });

            _eventManager.Subscribe("project.file.deleted", @event =>
            {
                var filePath = @event.Payload?.ToString();

                if (_viewManager.IsDocumentOpen(filePath))
                {
                    _viewManager.CloseDocument(filePath, true);
                    return;
                }
            });

            _eventManager.Subscribe("project.file.save_active", @event =>
            {
                if (!_viewManager.SaveActiveDocument())
                {
                    @event.Cancel(); 
                    return;
                }              
            });
        }

        private IFileEditorFactory? GetFactory(string filePath)
        {
            var ext = Path.GetExtension(filePath);
            return _providers.FirstOrDefault(f => f.CanOpen(ext));
        }

        public EditorControl? CreateEditor(string filePath)
        {
            var factory = GetFactory(filePath);
            return factory?.Create(filePath);
        }
    }
}
