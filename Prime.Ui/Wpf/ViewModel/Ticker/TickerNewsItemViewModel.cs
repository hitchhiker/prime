namespace Prime.Ui.Wpf.ViewModel.Ticker
{
    public class TickerNewsItemViewModel : TickerItemBaseViewModel
    {
        public TickerNewsItemViewModel(string title, string newsText)
        {
            Title = title;
            NewsText = newsText;
        }

        public string Title { get; private set; }

        public string NewsText { get; private set; }
    }
}