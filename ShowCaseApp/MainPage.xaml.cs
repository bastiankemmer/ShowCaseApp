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


// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=391641 dokumentiert.

namespace ShowCaseApp
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet werden kann oder auf die innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private string appName;
        private StorageFolder localFolder;
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            HardwareButtons.BackPressed += this.MainPage_BackPressed;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            appName = "myhtml.zip";
            localFolder = KnownFolders.MusicLibrary;
        }

        private void MainPage_BackPressed(object sender, BackPressedEventArgs e)
        {
            App.Current.Exit();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            HardwareButtons.BackPressed -= this.MainPage_BackPressed;
        }

        private async void UnpackZipFiles()
        {
            var myZip = await localFolder.CreateFileAsync(appName, CreationCollisionOption.OpenIfExists);
            using (var zipToOpen = await myZip.OpenStreamForReadAsync())
            {
                using (var zipMemoryStream = new MemoryStream((int)zipToOpen.Length))
                {
                    await zipToOpen.CopyToAsync(zipMemoryStream);

                    using (var archive = new ZipArchive(zipMemoryStream, ZipArchiveMode.Read))
                    {
                        var folder = await localFolder.CreateFolderAsync(myZip.DisplayName + "unzip", CreationCollisionOption.ReplaceExisting);
                        var unzipFolder = folder;

                        foreach (var entry in archive.Entries)
                        {
                            if (entry.Name != "")
                            {
                                using (var fileData = entry.Open())
                                {
                                    var outputFile = await unzipFolder.CreateFileAsync(entry.Name, CreationCollisionOption.ReplaceExisting);
                                    using (var outputFileStream = await outputFile.OpenStreamForWriteAsync())
                                    {
                                        await fileData.CopyToAsync(outputFileStream);
                                        await outputFileStream.FlushAsync();
                                    }
                                }
                            }
                        }
                        
                    }
                }
            }
        }

        private void GoTo_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(WebView));
        }
        private void Unpacker_Click(object sender, RoutedEventArgs e)
        {
            UnpackZipFiles();
        }
    }
}
