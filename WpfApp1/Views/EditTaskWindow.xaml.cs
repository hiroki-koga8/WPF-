using System.Windows;
using WpfApp1.ViewModels;

namespace WpfApp1.Views
{
	public partial class EditTaskWindow : Window
	{
		public List<string> StatusOptions { get; } = new() { "未対応", "対応中", "完了" };

		public EditTaskWindow(ViewModel.TaskItem taskItem)
		{
			InitializeComponent();
			DataContext = taskItem;
		}

		private void Ok_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = true;
			Close();
		}

		private void Cancel_Click(object sender, RoutedEventArgs e)
		{
			DialogResult = false;
			Close();
		}
	}
}
