using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using MAUISql.Models;
using MauScan.Data;

namespace MauScan.ViewModels
{
	public class ListQRCodesViewModel : BindableObject
	{
		private readonly Database database;

		public ListQRCodesViewModel()
		{
			database = new Database(Path.Combine(FileSystem.AppDataDirectory, "mauscan.db3"));
			database.Init();

			LoadQRCodeList();
		}

		private ObservableCollection<QRCode> _qrCodes;
		public ObservableCollection<QRCode> QRCodes
		{
			get => _qrCodes;
			set
			{
				_qrCodes = value;
				OnPropertyChanged();
			}
		}

		private void LoadQRCodeList()
		{
			QRCodes = new ObservableCollection<QRCode>(database.GetQRCode());
		}

		public ICommand DeleteQRCodeCommand => new Command<QRCode>(DeleteQRCode);
		public ICommand VisitWebPageCommand => new Command<string>(OpenExtractedURL);
		public ICommand CopyBarcodeCommand => new Command<string>(CopyBarcode);

		private void DeleteQRCode(QRCode selectedQRCode)
		{
			if (selectedQRCode != null)
			{
				database.deleteQRCode(selectedQRCode);
				LoadQRCodeList();
			}
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
				await App.Current.MainPage.DisplayAlert("Invalid URL", "The extracted URL is not valid.", "OK");
			}
		}

		private void CopyBarcode(string copyString)
		{
			Clipboard.SetTextAsync(copyString);
		}

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}