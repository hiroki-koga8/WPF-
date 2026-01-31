using System.Windows;

public class MessageService : IMessageService
{
	public void ShowInfo(string message, string title = "情報")
		=> MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);

	public void ShowError(string message, string title = "エラー")
		=> MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);

	public bool ShowConfirm(string message, string title = "確認")
		=> MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question)
		   == MessageBoxResult.Yes;
}
