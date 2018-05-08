
namespace prime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Title = "prime [alpha " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version + "] - Supranational asset manager."; 
        }
    }
}
