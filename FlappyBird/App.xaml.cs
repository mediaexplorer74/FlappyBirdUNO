using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]

namespace FlappyBird
{
	public partial class App : Application
	{
		public static float scale = 4.8f;
		public App()
		{
			InitializeComponent();

			MainPage = new MainPage();
		}
	}
}
