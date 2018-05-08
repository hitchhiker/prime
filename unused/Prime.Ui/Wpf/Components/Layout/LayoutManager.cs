using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout.Serialization;

namespace Prime.Ui.Wpf
{
    public class LayoutManager : ILayoutManager
    {
        public void LoadLayout(DockingManager manager)
        {
            var serializer = new XmlLayoutSerializer(manager);
            var asm = Assembly.GetExecutingAssembly();

            using (var stream = asm.GetManifestResourceStream("Prime.Ui.Wpf.Resources.Layout.default.xml"))
                serializer.Deserialize(stream);

            manager.DocumentClosed += Manager_DocumentClosed;

            /*
            if (File.Exists(LayoutFileName))
                LoadLayout(manager, LayoutFileName);
            else
                ResetLayout(manager);*/
        }

        private void Manager_DocumentClosed(object sender, DocumentClosedEventArgs e)
        {
            if (!(e.Document?.Content is IDisposable doc))
                return;

            doc.Dispose();
        }

        private static void LoadLayout(DockingManager manager, string fileName)
        {
            var serializer = new XmlLayoutSerializer(manager);
            serializer.Deserialize(fileName);
        }

        public void ResetLayout(DockingManager manager)
        {
            //if (!File.Exists(DefaultLayoutFileName)) return;

            //LoadLayout(manager, DefaultLayoutFileName);
        }

        public void SaveLayout(DockingManager manager)
        {
            /*
            // Craete the folder if it does not exist yet
            if (!Directory.Exists(DataFolder)) Directory.CreateDirectory(DataFolder);
            if (File.Exists(LayoutFileName)) File.Delete(LayoutFileName);

            // Serialize the layout
            var serializer = new XmlLayoutSerializer(manager);
            serializer.Serialize(LayoutFileName);*/
        }

    }
}