using System.ComponentModel;
using MauScan.Data;
using System.Runtime.CompilerServices;
using MAUISql.Models;
using Camera.MAUI;

namespace MauScan
{
	public partial class GenerateQRCode : ContentPage, INotifyPropertyChanged
	{
		private Database database;

		public GenerateQRCode()
		{
			InitializeComponent();

			database = new Database(Path.Combine(FileSystem.AppDataDirectory, "mauscan.db3"));
			database.Init();

			this.BindingContext = this;
		}


		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		private int _barcodeWidth = 400;
		public int BarcodeWidth
		{
			get { return _barcodeWidth; }
			set
			{
				if (_barcodeWidth != value)
				{
					_barcodeWidth = value;
					OnPropertyChanged();
				}
			}
		}

		private int _barcodeHeight = 400;
		public int BarcodeHeight
		{
			get { return _barcodeHeight; }
			set
			{
				if (_barcodeHeight != value)
				{
					_barcodeHeight = value;
					OnPropertyChanged();
				}
			}
		}

		private int _barcodeMargin = 5;
		public int BarcodeMargin
		{
			get { return _barcodeMargin; }
			set
			{
				if (_barcodeMargin != value)
				{
					_barcodeMargin = value;
					OnPropertyChanged();
				}
			}
		}

		private ColorRGB _barcodeBackgroundColor = new ColorRGB { Red = 1, Green = 1, Blue = 1 }; // Default to White
		public ColorRGB BarcodeBackgroundColor
		{
			get { return _barcodeBackgroundColor; }
			set
			{
				if (_barcodeBackgroundColor != value)
				{
					_barcodeBackgroundColor = value;
					OnPropertyChanged();
				}
			}
		}

		private ColorRGB _barcodeForegroundColor = new ColorRGB { Red = 0, Green = 0, Blue = 0 }; // Default to Black
		public ColorRGB BarcodeForegroundColor
		{
			get { return _barcodeForegroundColor; }
			set
			{
				if (_barcodeForegroundColor != value)
				{
					_barcodeForegroundColor = value;
					OnPropertyChanged();
				}
			}
		}

		private void GenerateQRCode_Clicked(object sender, EventArgs e)
		{
			string userInput = textInput.Text;

			if (!string.IsNullOrEmpty(userInput))
			{
				barcodeImage.Barcode = userInput;

				GenerateQRCodeSvg(userInput, BarcodeWidth, BarcodeHeight, "GeneratedQRCode.svg", BarcodeForegroundColor, BarcodeBackgroundColor);
			}
		}

		private void GenerateQRCodeSvg(string inputText, int width, int height, string fileName, ColorRGB foregroundColor, ColorRGB backgroundColor)
		{
			var barcodeWriter = new ZXing.BarcodeWriterSvg
			{
				Format = ZXing.BarcodeFormat.QR_CODE,
				Options = new ZXing.Common.EncodingOptions
				{
					Width = width,
					Height = height,
					Margin = BarcodeMargin
				},
				Renderer = new ZXing.Rendering.SvgRenderer
				{
					Foreground = ColorRGBToZXingColor(foregroundColor),
					Background = ColorRGBToZXingColor(backgroundColor)
				}
			};

			var qrImage = barcodeWriter.Write(inputText);

			QRCode newQRCode = new QRCode
			{
				Text = inputText,
				TimeScan = DateTime.Now,
			};
			database.addQRCode(newQRCode);

			SaveSvgToFile(qrImage.Content, fileName);
		}

		private void SaveSvgToFile(string svgContent, string fileName)
		{
			string filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
			File.WriteAllText(filePath, svgContent);
			DisplayAlert("QR Code Saved", $"QR code saved to {filePath}", "OK");
		}
		private void ColorSlider_ValueChanged(object sender, ValueChangedEventArgs e)
		{
			UpdateColorPreview();
		}


		public static ZXing.Rendering.SvgRenderer.Color ColorRGBToZXingColor(ColorRGB colorRGB)
		{
			return new ZXing.Rendering.SvgRenderer.Color(255, (byte)(colorRGB.Red * 255), (byte)(colorRGB.Green * 255), (byte)(colorRGB.Blue * 255));
		}
		private void UpdateColorPreview()
		{
			double backgroundRed = BackgroundRedSlider.Value;
			double backgroundGreen = BackgroundGreenSlider.Value;
			double backgroundBlue = BackgroundBlueSlider.Value;

			double foregroundRed = ForegroundRedSlider.Value;
			double foregroundGreen = ForegroundGreenSlider.Value;
			double foregroundBlue = ForegroundBlueSlider.Value;

			BarcodeBackgroundColor = new ColorRGB { Red = backgroundRed, Green = backgroundGreen, Blue = backgroundBlue };
			BarcodeForegroundColor = new ColorRGB { Red = foregroundRed, Green = foregroundGreen, Blue = foregroundBlue };
		}
	}


}