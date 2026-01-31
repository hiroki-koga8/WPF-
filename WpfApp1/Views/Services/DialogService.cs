using WpfApp1.ViewModels;
using WpfApp1.Views;

public class DialogService : IDialogService
{
	public bool ShowEditTaskDialog(ViewModel.TaskItem task)
	{
		var window = new EditTaskWindow(task);
		return window.ShowDialog() == true;
	}
}
