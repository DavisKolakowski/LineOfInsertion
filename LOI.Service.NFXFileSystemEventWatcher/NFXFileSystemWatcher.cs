
namespace LOI.Service.NFXFileSystemEventWatcher
{
    using System;
    using System.IO;

    public class NFXFileSystemWatcher : IDisposable
    {
        private readonly FileSystemWatcher _watcher;

        public NFXFileSystemWatcher(string path, string filter)
        {
            this._watcher = new FileSystemWatcher(path, filter)
            {
                NotifyFilter = NotifyFilters.Attributes
                              | NotifyFilters.CreationTime
                              | NotifyFilters.DirectoryName
                              | NotifyFilters.FileName
                              | NotifyFilters.LastWrite
                              | NotifyFilters.Security
                              | NotifyFilters.Size
            };

            this._watcher.Changed += OnChanged;
            this._watcher.Created += OnChanged;
            this._watcher.Deleted += OnChanged;
            this._watcher.Renamed += OnRenamed;
            this._watcher.Error += OnError;
        }

        public void Start()
        {
            this._watcher.EnableRaisingEvents = true;
        }

        public void Stop()
        {
            this._watcher.EnableRaisingEvents = false;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            Console.WriteLine($"File changed: {e.FullPath} | Change type: {e.ChangeType}");
        }

        private void OnRenamed(object sender, RenamedEventArgs e)
        {
            Console.WriteLine($"File renamed from {e.OldFullPath} to {e.FullPath}");
        }

        private void OnError(object sender, ErrorEventArgs e)
        {
            if (e.GetException() is InternalBufferOverflowException)
            {
                Console.WriteLine("Error: File system watcher internal buffer overflow at {0}", DateTime.Now);
            }
            else
            {
                   Console.WriteLine("Error: {0}", e.GetException().Message);
            }
        }

        public void Dispose()
        {
            this._watcher.Changed -= OnChanged;
            this._watcher.Created -= OnChanged;
            this._watcher.Deleted -= OnChanged;
            this._watcher.Renamed -= OnRenamed;
            this._watcher.Error -= OnError;
            this._watcher.Dispose();
        }
    }
}
