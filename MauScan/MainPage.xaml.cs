using GroupDocs.Parser;
using GroupDocs.Parser.Data;
using MAUISql.Models;
using MauScan.Data;
using Microsoft.Maui.Controls.PlatformConfiguration;

namespace MauScan
{
	public partial class MainPage : ContentPage
	{
		private Database database;
		private Camera.MAUI.ZXingHelper.BarcodeEventArgs lastDetectedBarcode;
		private readonly IFilePicker _filePicker;

		public MainPage(IFilePicker filePicker)
		{
			InitializeComponent();

			database = new Database(Path.Combine(FileSystem.AppDataDirectory, "mauscan.db3"));
			database.Init();

			_filePicker = filePicker;
		}

		protected override async void OnAppearing()
		{
			base.OnAppearing();

			try
			{
				bool cameraPermissionGranted = await CheckAndRequestCameraPermission();

				if (cameraPermissionGranted)
				{
					RefreshCamera();
				}
				else
				{
					DisplayAlert("QR Codes Extracted", $"Povolení na kameru není udělený", "OK");
				}

				RefreshCamera();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při zobrazení stránky: {ex.Message}");
			}
		}

		private async Task<bool> CheckAndRequestCameraPermission()
		{
			var status = await Permissions.CheckStatusAsync<Permissions.Camera>();

			if (status != PermissionStatus.Granted)
			{
				status = await Permissions.RequestAsync<Permissions.Camera>();
			}

			return status == PermissionStatus.Granted;
		}

		private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
		{
			try
			{
				lastDetectedBarcode = args;

				MainThread.BeginInvokeOnMainThread(() =>
				{
					barcodeResult.Text = args.Result[0].Text;
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při zpracování detekovaného čárového kódu: {ex.Message}");
			}
		}

		private void RefreshCamera()
		{
			try
			{
				MainThread.BeginInvokeOnMainThread(async () =>
				{
					await cameraView.StopCameraAsync();
					await cameraView.StartCameraAsync();
				});
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při obnovení kamery: {ex.Message}");
			}
		}

		private void cameraView_CamerasLoaded(object sender, EventArgs e)
		{
			try
			{
				if (cameraView.Cameras.Count > 0)
				{
					cameraView.Camera = cameraView.Cameras.First();
					RefreshCamera();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Chyba při načítání dostupných kamer: {ex.Message}");
			}
		}
		private void AddQRCode_Clicked(object sender, EventArgs e)
		{
			string barcodeText = lastDetectedBarcode?.Result?[0].Text;
			string barcodeFormat = lastDetectedBarcode?.Result?[0].BarcodeFormat.ToString();

			if (!string.IsNullOrEmpty(barcodeText) && !string.IsNullOrEmpty(barcodeFormat))
			{
				QRCode newQRCode = new QRCode
				{
					Text = barcodeText,
					TimeScan = DateTime.Now,
				};

				database.addQRCode(newQRCode);
				barcodeResult.Text = barcodeText;
			}

			else
			{
				DisplayAlert("Invalid Add QR Code", "You cannot add qr code because he it is null", "OK");
			}
		}

		private void OpenWebPage_Tapped(object sender, EventArgs e)
		{
			string lastExtractedURL = barcodeResult.Text;

			if (!string.IsNullOrEmpty(lastExtractedURL))
			{
				OpenExtractedURL(lastExtractedURL);
			}
			else
			{
				DisplayAlert("No URL", "No URL has been extracted yet.", "OK");
			}
		}


		private async void PickPhoto_Clicked(object sender, EventArgs e)
		{
			try
			{
				FileResult result = await FilePicker.PickAsync(new PickOptions
				{
					PickerTitle = "Pick a Photo",
					FileTypes = FilePickerFileType.Images,
				});

				if (result != null)
				{
					string filePath = result.FullPath;
					ExtractQRCodeFromFile(filePath);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Error picking a file: {ex.Message}");
			}
		}

		private void ExtractQRCodeFromFile(string fileName)
		{
			try
			{

				using (Parser parser = new Parser(fileName))
				{
					IEnumerable<PageBarcodeArea> barcodes = null;

						barcodes = parser.GetBarcodes();
					

					foreach (PageBarcodeArea barcode in barcodes)
					{
						QRCode newQRCode = new QRCode
						{
							Text = barcode.Value,
							TimeScan = DateTime.Now,
						};

						barcodeResult.Text = barcode.Value;
						database.addQRCode(newQRCode);
					}

					DisplayAlert("QR Codes Extracted", $"{barcodes.Count()} QR codes extracted from the image.", "OK");
				}
			}
			catch (Exception ex)
			{
				DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
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
				DisplayAlert("Invalid URL", "The extracted URL is not valid.", "OK");
			}
		}
		private void CopyBarcode_Clicked(object sender, EventArgs e)
		{
			Clipboard.SetTextAsync(barcodeResult.Text);
		}

	}
}