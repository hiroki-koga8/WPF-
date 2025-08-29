using Reactive.Bindings;
using System.Reactive;
using System.Reactive.Linq;

namespace WpfApp1.ViewModels;

public class ViewModel
{
	public ReactiveCollection<TaskItem> Tasks { get; } = new();
	public ReactiveProperty<TaskItem> SelectedTask { get; } = new();

	public ReactiveProperty<string> Filter { get; } = new("すべて");

	public ReactiveCommand AddTaskCommand { get; }
	public ReactiveCommand DeleteTaskCommand { get; }
	public ReactiveCommand EditTaskCommand { get; }

	public ReactiveProperty<string> TaskName { get; set; } = new();
	public ReactiveProperty<DateTime> StartDate { get; set; } = new(DateTime.Now);
	public ReactiveProperty<DateTime> EndDate { get; set; } = new(DateTime.Now);
	public ReactiveProperty<string> Description { get; set; } = new();
	public ReactiveProperty<string> Status { get; set; } = new("未対応");
	public ReactiveProperty<double> PlannedHours { get; set; } = new(0);
	public ReactiveProperty<double> ActualHours { get; set; } = new(0);
	public ReadOnlyReactiveProperty<double> Gap { get; }

	public ReactiveProperty<string> Remarks { get; set; } = new();
	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ViewModel()
	{
		// タスクリスト初期化済みとして
		Tasks = new ReactiveCollection<TaskItem>();

		AddTaskCommand = new ReactiveCommand();
		AddTaskCommand.Subscribe(() =>
		{
			var newTask = new TaskItem
			{
				TaskName = { Value = "新規タスク" },
				StartDate = { Value = DateTime.Today },
				EndDate = { Value = DateTime.Today.AddDays(1) },
				Status = { Value = "未対応" },
				PlannedHours = { Value = 1.0 },
				ActualHours = { Value = 0.0 },
				Description = { Value = "説明を入力" },
				Remarks = { Value = "備考" }
			};
			Tasks.Add(newTask);
		});

		DeleteTaskCommand = SelectedTask.Select(task => task != null).ToReactiveCommand();
		DeleteTaskCommand.Subscribe(OnDelete);
		EditTaskCommand = SelectedTask.Select(task => task != null).ToReactiveCommand();
		EditTaskCommand.Subscribe(OnEdit);
	}

	/// <summary>
	/// 削除時の処理
	/// </summary>
	/// <param name="_"></param>
	private void OnDelete()
	{
		if (SelectedTask.Value != null)
			Tasks.Remove(SelectedTask.Value);
	}

	/// <summary>
	/// 編集時の処理
	/// </summary>
	private void OnEdit()
	{
		if (SelectedTask.Value == null)
			return;

		// 編集用にウィンドウを開く
		var editWindow = new Views.EditTaskWindow(SelectedTask.Value);
		var result = editWindow.ShowDialog();

		if (result == true)
		{
			// OKが押されたのでデータは既に更新されている（ReactivePropertyがバインド済みのため）
		}
	}


	/// <summary>
	/// タスク一覧の行を保持するクラス
	/// </summary>
	public class TaskItem
	{
		public ReactiveProperty<string> TaskName { get; set; } = new();
		public ReactiveProperty<DateTime> StartDate { get; set; } = new(DateTime.Now);
		public ReactiveProperty<DateTime> EndDate { get; set; } = new(DateTime.Now);
		public ReactiveProperty<string> Description { get; set; } = new();
		public ReactiveProperty<string> Status { get; set; } = new("未対応");
		public ReactiveProperty<double> PlannedHours { get; set; } = new(0);
		public ReactiveProperty<double> ActualHours { get; set; } = new(0);
		public ReadOnlyReactiveProperty<double> Gap { get; }

		public ReactiveProperty<string> Remarks { get; set; } = new();

		public TaskItem()
		{
			Gap = PlannedHours.CombineLatest(ActualHours, (plan, actual) => plan - actual).ToReadOnlyReactiveProperty();
		}
	}

}