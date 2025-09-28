using Microsoft.EntityFrameworkCore;
using Reactive.Bindings;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;

namespace WpfApp1.ViewModels;

public class ViewModel
{
	public ReactiveCollection<TaskItem> Tasks { get; } = new();
	public ReactiveProperty<TaskItem> SelectedTask { get; } = new();

	public ReactiveProperty<string> Filter { get; } = new("すべて");

	public ReactiveCommand LoadCommand { get; }
	public ReactiveCommand AddTaskCommand { get; }
	public ReactiveCommand DeleteTaskCommand { get; }
	public ReactiveCommand EditTaskCommand { get; }


	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ViewModel()
	{
		Tasks = new ReactiveCollection<TaskItem>();
		LoadCommand = new ReactiveCommand();
		LoadCommand.Subscribe(async () => await LoadTasksFromDbAsync());
		AddTaskCommand = new ReactiveCommand();
		AddTaskCommand.Subscribe(OnAddAsync);
		DeleteTaskCommand = SelectedTask.Select(task => task != null).ToReactiveCommand();
		DeleteTaskCommand.Subscribe(OnDeleteAsync);
		EditTaskCommand = SelectedTask.Select(task => task != null).ToReactiveCommand();
		EditTaskCommand.Subscribe(OnEditAsync);

		_ = LoadTasksFromDbAsync(); // 非同期だが待たない
	}

	/// <summary>
	/// DBからタスク一覧のデータを読み込む
	/// </summary>
	/// <returns></returns>
	public async Task LoadTasksFromDbAsync()
	{
		try
		{
			//MessageBox.Show("DB読み込み開始");

			using var db = new TaskDbContext();
			var entities = await db.Tasks.ToListAsync();

			//MessageBox.Show($"DBから {entities.Count} 件取得");

			Tasks.Clear();
			foreach (var entity in entities)
			{
				Tasks.Add(new TaskItem
				{
					TaskName = { Value = entity.TaskName ?? string.Empty },
					StartDate = { Value = entity.StartDate },
					EndDate = { Value = entity.EndDate },
					Description = { Value = entity.Description ?? string.Empty },
					Status = { Value = entity.Status ?? "未対応" },
					PlannedHours = { Value = entity.PlannedHours ?? 0 },
					ActualHours = { Value = entity.ActualHours ?? 0 },
					Remarks = { Value = entity.Remarks ?? string.Empty }
				});
			}

			//MessageBox.Show("読み込み完了");
		}
		catch (Exception ex)
		{
			MessageBox.Show($"読み込み失敗：{ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	/// <summary>
	/// 新規追加の処理
	/// </summary>
	private async void OnAddAsync()
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

		var editWindow = new Views.EditTaskWindow(newTask);
		var result = editWindow.ShowDialog();

		if (result == true)
		{

			using var db = new TaskDbContext();

			var entity = new TaskEntity
			{
				TaskName = newTask.TaskName.Value,
				StartDate = newTask.StartDate.Value,
				EndDate = newTask.EndDate.Value,
				Description = newTask.Description.Value,
				Status = newTask.Status.Value,
				PlannedHours = newTask.PlannedHours.Value,
				ActualHours = newTask.ActualHours.Value,
				Remarks = newTask.Remarks.Value
			};

			db.Tasks.Add(entity);
			await db.SaveChangesAsync();

			Tasks.Add(newTask);
		}
	}

	/// <summary>
	/// 削除時の処理
	/// </summary>
	/// <param name="_"></param>
	private async void OnDeleteAsync()
	{
		var task = SelectedTask.Value;
		if (task == null)
		{
			return;
		}
		using var db = new TaskDbContext();

		// TaskEntity を ID で検索（ここでは TaskName 等で一意に特定できるよう仮定）
		var entity = await db.Tasks
			.FirstOrDefaultAsync(t => t.TaskName == task.TaskName.Value
								   && t.StartDate == task.StartDate.Value);

		if (entity != null)
		{
			db.Tasks.Remove(entity);
			await db.SaveChangesAsync();
		}

		Tasks.Remove(task);
	}

	/// <summary>
	/// 編集時の処理
	/// </summary>
	private async void OnEditAsync()
	{
		if (SelectedTask.Value == null)
			return;

		// 編集用にウィンドウを開く
		var editWindow = new Views.EditTaskWindow(SelectedTask.Value);
		var result = editWindow.ShowDialog();

		if (result == true)
		{
			var task = SelectedTask.Value;

			using var db = new TaskDbContext();

			// DB のレコードを取得
			var entity = await db.Tasks
				.FirstOrDefaultAsync(t => t.TaskName == task.TaskName.Value
									   && t.StartDate == task.StartDate.Value);

			if (entity != null)
			{
				entity.TaskName = task.TaskName.Value;
				entity.StartDate = task.StartDate.Value;
				entity.EndDate = task.EndDate.Value;
				entity.Description = task.Description.Value;
				entity.Status = task.Status.Value;
				entity.PlannedHours = task.PlannedHours.Value;
				entity.ActualHours = task.ActualHours.Value;
				entity.Remarks = task.Remarks.Value;

				await db.SaveChangesAsync();
			}
		}
	}


	/// <summary>
	/// タスク一覧の行を保持するクラス
	/// </summary>
	public class TaskItem
	{
		public int Id { get; set; }
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