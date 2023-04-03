namespace MauiAppSignalR.Views;

public partial class HistoryPage : ContentPage
{
	public HistoryPage(HistoryViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}
