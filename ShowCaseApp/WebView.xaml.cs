using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using System.IO.Compression;
using Windows.Storage.Pickers.Provider;
using Windows.Storage.Provider;
using Windows.Storage.Search;
using Windows.Web;
using Windows.Phone.UI.Input;
using Windows.Storage.Streams;
using System.Threading.Tasks;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkID=390556 dokumentiert.

namespace ShowCaseApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class WebView : Page
    {
        public WebView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Wird aufgerufen, wenn diese Seite in einem Frame angezeigt werden soll.
        /// </summary>
        /// <param name="e">Ereignisdaten, die beschreiben, wie diese Seite erreicht wurde.
        /// Dieser Parameter wird normalerweise zum Konfigurieren der Seite verwendet.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            var MyUrl = MyWebViewControl.BuildLocalStreamUri("index", "myhtmlunzip/index.html");

            MyWebViewControl.NavigateToLocalStreamUri(MyUrl, new MyStreamUriResolver());

            HardwareButtons.BackPressed += this.HardwareButton_BackPressed;
        }

        private void HardwareButton_BackPressed(object sender, BackPressedEventArgs e)
        {
            if (this.Frame.CanGoBack)
            {
                e.Handled = true;
                this.Frame.GoBack();
            }
        }
    }
    public sealed class MyStreamUriResolver : IUriToStreamResolver
    {
        public IAsyncOperation<IInputStream> UriToStreamAsync(Uri uri)
        {
            lock (this)
            {
                if (uri == null)
                {
                    throw new Exception();
                }
                string path = uri.AbsolutePath;
                return GetContent(path).AsAsyncOperation();
            }
        }

        private async Task<IInputStream> GetContent(string path)
        {
            try
            {
                // Picking Only The Index.html 
                // Probably The Mistake Why It Doesn't Load JS and CSS Files
                var localFolder = KnownFolders.MusicLibrary;
                var folder = await localFolder.GetFolderAsync("myhtmlunzip");
                var file = await folder.GetFileAsync("index.html");


                // This Should Work But It Doesn't
                // Excepion: System.IO.FileNotFoundException: The system cannot find the file specified.
                //Uri localUri = new Uri("ms-appx://" + path);
                //StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(localUri);


                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read).AsTask().ConfigureAwait(false);

                if (true)
                {
                    Windows.Storage.Streams.DataReader reader = new DataReader(stream);
                    await reader.LoadAsync((uint)stream.Size);
                    var fileContent = reader.ReadString((uint)stream.Size);
                    stream.Seek(0);
                }
                return stream;

            }
            catch
            {
                throw new Exception("Path is invalid");
            }
        }
    }
}
