using MAUISql.Models;
using MauScan.Data;

namespace MauScan
{
    public partial class ListQRCodes : ContentPage
    {
        private Database database;

        public ListQRCodes()
        {
            InitializeComponent();

            database = new Database(Path.Combine(FileSystem.AppDataDirectory, "mauscan.db3"));
            database.Init();

            LoadQRCodeList();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadQRCodeList();
        }

        private void DeleteQRCode_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            QRCode selectedQRCode = (QRCode)btn.CommandParameter;

            if (selectedQRCode != null)
            {
                database.deleteQRCode(selectedQRCode);
                LoadQRCodeList();
            }
        }

        private void LoadQRCodeList()
        {
            List<QRCode> qrCodes = database.GetQRCode();

            qrCodeListView.ItemsSource = qrCodes;
        }
        private async void OpenExtractedURL(string url)
        {
	        if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri) &&
	            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
	        {
		        await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
	        }
	        else
	        {
		        DisplayAlert("Invalid URL", "The extracted URL is not valid.", "OK");
	        }
        }
        private void VisitWebPage_Clicked(object sender, EventArgs e)
        {
	        var webPageUrl = (string)((Button)sender).CommandParameter;

	        if (!string.IsNullOrEmpty(webPageUrl))
	        {
		        // Otevřít webovou stránku ve výchozím prohlížeči
		        OpenExtractedURL(webPageUrl);
	        }
        }

        private void CopyBarcode_Clicked(object sender, EventArgs e)
        {
	        var copyString = (string)((Button)sender).CommandParameter;
	        Clipboard.SetTextAsync(copyString);
        }
	}
}
