using WpfApp1.ViewModels;

public interface IDialogService
{
	bool ShowEditTaskDialog(ViewModel.TaskItem task);
}