using System.Net;
using System.Windows;
using System.Windows.Controls;

using XArch.AppShell.Framework.Events;
using XArch.AppShell.Framework.UI;

using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace XArch.AppShell.Providers
{
    public class DockViewManager : IViewManager
    {
        private readonly DockingManager _dockManager;
        private readonly LayoutDocumentPane _documentPane;
        private readonly IEventManager _eventManager;
        private readonly Dictionary<DockSide, LayoutAnchorablePane> _toolPanes = new();

        public DockViewManager(IEventManager eventManager)
        {
            _eventManager = eventManager;
            _dockManager = new DockingManager();
            _documentPane = new LayoutDocumentPane();

            // Create dockable side panes with default sizes
            var leftToolPane = CreateToolPane(DockSide.Left, new GridLength(250));
            var rightToolPane = CreateToolPane(DockSide.Right, new GridLength(300));
            var bottomToolPane = CreateToolPane(DockSide.Bottom, new GridLength(180));

            _dockManager.Layout = new LayoutRoot
            {
                RootPanel = new LayoutPanel
                {
                    Orientation = Orientation.Vertical,
                    Children =
                {
                    new LayoutPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Children =
                        {
                            leftToolPane,
                            new LayoutDocumentPaneGroup
                            {
                                Children = { _documentPane }
                            },
                            rightToolPane
                        }
                    },
                    bottomToolPane
                }
                }
            };
        }

        private LayoutAnchorablePaneGroup CreateToolPane(DockSide side, GridLength size)
        {
            var pane = new LayoutAnchorablePane();
            _toolPanes[side] = pane;

            var group = new LayoutAnchorablePaneGroup
            {
                Orientation = (side == DockSide.Bottom) ? Orientation.Horizontal : Orientation.Vertical
            };

            if (side == DockSide.Bottom)
                group.DockHeight = size;
            else
                group.DockWidth = size;

            group.Children.Add(pane);
            return group;
        }

        public DockingManager DockingManager => _dockManager;

        public void OpenDocument(string contentId, string title, IFileEditorFactory editorFactory)
        {
            if (IsDocumentOpen(contentId))
            {
                FocusDocument(contentId);
                return;
            }

            var editor = editorFactory.Create(contentId);

            OpenDocument(contentId, title, editor);
        }

        public bool SaveActiveDocument()
        {
            if (_dockManager.ActiveContent is EditorControl editor && editor.IsDirty)
            {
                editor.SaveFile();                
                return true;
            }

            return false;
        }

        public void OpenDocument(string contentId, string title, EditorControl control)
        {
            if (_documentPane.Children.OfType<LayoutDocument>().Any(d => d.ContentId == contentId))
            {
                FocusDocument(contentId);
                return;
            }

            var doc = new LayoutDocument
            {
                ContentId = contentId,
                Title = title,
                Content = control,
                ToolTip = contentId
            };

            control.IsDirtyChanged += (s, e) => doc.Title = $"{title}{(control.IsDirty ? "*" : "")}";

            _documentPane.Children.Add(doc);
            _dockManager.ActiveContent = doc.Content;
        }

        public void RegisterTool(DockSide side, string contentId, string title, object content, bool canHide = true, bool hiddenByDefault = true)
        {
            var anchorable = new LayoutAnchorable
            {
                ContentId = contentId,
                Title = title,
                Content = content,
                CanClose = false,
                CanAutoHide = canHide,
                CanFloat = false,
                CanHide = false,
                CanDockAsTabbedDocument = false
            };

            _toolPanes[side].Children.Add(anchorable);

            // Open the tool window so that it can be focused
            if (anchorable.IsHidden)
            {
                anchorable.Show();
            }

            // Hide the tool window by default (un-pin)
            if (canHide && hiddenByDefault)
            {
                anchorable.ToggleAutoHide();
            }
        }

        public void FocusDocument(string contentId)
        {
            var doc = _documentPane.Children
                .OfType<LayoutDocument>()
                .FirstOrDefault(d => d.ContentId == contentId);

            if (doc != null)
                _dockManager.ActiveContent = doc.Content;
        }

        public bool IsDocumentOpen(string contentId)
        {
            return _documentPane.Children.OfType<LayoutDocument>().Any(d => d.ContentId == contentId);
        }

        public void FocusToolWindow(string contentId)
        {
            foreach (var pane in _toolPanes.Values)
            {
                var anchorable = pane.Children
                    .OfType<LayoutAnchorable>()
                    .FirstOrDefault(a => a.ContentId == contentId);

                if (anchorable != null)
                {
                    if (anchorable.IsHidden)
                        anchorable.Show();

                    anchorable.IsActive = true;
                    anchorable.IsSelected = true;
                    return;
                }
            }
        }

        internal void CloseDocument(string contentId, bool forceClose)
        {
            var doc = _documentPane.Children
                .OfType<LayoutDocument>()
                .FirstOrDefault(d => d.ContentId == contentId);

            if (doc != null)
            {
                if (forceClose)
                {
                    _documentPane.Children.Remove(doc);
                }
            }
        }
    }
}
