using Microsoft.EntityFrameworkCore;
using Reactive.Bindings;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;

namespace WpfApp1.ViewModels;

public class ViewModel
{
	// 全件保持用
	public ReactiveCollection<TaskItem> AllTasks { get; } = new();

	// フィルター済み表示用
	public ReactiveCollection<TaskItem> FilteredTasks { get; } = new();
	public ReactiveProperty<TaskItem> SelectedTask { get; } = new();

	public ReactiveProperty<string> SelectedStatusFilter { get; } = new("すべて");
	public ReactiveProperty<DateTime?> FilterDate { get; } = new(DateTime.Today);

	public ReactiveCommand LoadCommand { get; }
	public ReactiveCommand AddTaskCommand { get; }
	public ReactiveCommand FilterCommand { get; } = new();
	public ReactiveCommand DeleteTaskCommand { get; }
	public ReactiveCommand EditTaskCommand { get; }
	public ReactiveCommand ExportToExcelCommand { get; }

	public ReadOnlyCollection<string> FilterOptions { get; } = new(new[] { "すべて", "未対応", "対応中", "完了" });

	/// <summary>
	/// コンストラクタ
	/// </summary>
	public ViewModel()
	{
		LoadCommand = new ReactiveCommand();
		LoadCommand.Subscribe(async () => await LoadTasksFromDbAsync());
		AddTaskCommand = new ReactiveCommand();
		AddTaskCommand.Subscribe(OnAddAsync);
		DeleteTaskCommand = SelectedTask.Select(task => task != null).ToReactiveCommand();
		DeleteTaskCommand.Subscribe(OnDeleteAsync);
		EditTaskCommand = SelectedTask.Select(task => task != null).ToReactiveCommand();
		EditTaskCommand.Subscribe(OnEditAsync);
		ExportToExcelCommand = new ReactiveCommand();
		ExportToExcelCommand.Subscribe(OnExportToExcel);

		FilterCommand.Subscribe(ExecuteFilter);

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
			using var db = new TaskDbContext();
			var entities = await db.Tasks.ToListAsync();

			AllTasks.Clear();
			foreach (var entity in entities)
			{
				var taskItem = new TaskItem
				{
					Id = entity.Id,
					TaskName = { Value = entity.TaskName ?? string.Empty },
					StartDate = { Value = entity.StartDate },
					EndDate = { Value = entity.EndDate },
					Description = { Value = entity.Description ?? string.Empty },
					Status = { Value = entity.Status ?? "未対応" },
					PlannedHours = { Value = entity.PlannedHours ?? 0 },
					ActualHours = { Value = entity.ActualHours ?? 0 },
					Remarks = { Value = entity.Remarks ?? string.Empty }
				};

				AllTasks.Add(taskItem);
			}

			ExecuteFilter();
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

			AllTasks.Add(newTask);
			ExecuteFilter();
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
			// ここは通らない想定
			return;
		}
		if (MessageBox.Show($"{task.TaskName.Value}を削除しますか？", "確認", MessageBoxButton.YesNo, MessageBoxImage.Information) != MessageBoxResult.Yes)
		{
			// Yes以外の時削除しない
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

		AllTasks.Remove(task);
		ExecuteFilter();
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
			var entity = await db.Tasks.FirstOrDefaultAsync(t => t.Id == task.Id);

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
				ExecuteFilter();
			}
		}
	}

	/// <summary>
	/// Excelファイル出力処理
	/// </summary>
	private void OnExportToExcel()
	{
		if (FilteredTasks.Count == 0)
		{
			MessageBox.Show("出力対象のデータが存在しません。", "情報", MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}

		try
		{
			var dialog = new Microsoft.Win32.SaveFileDialog
			{
				Filter = "Excel ファイル (*.xlsx)|*.xlsx",
				FileName = $"Tasks_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
			};

			if (dialog.ShowDialog() != true)
				return;

			using var workbook = new ClosedXML.Excel.XLWorkbook();
			var worksheet = workbook.Worksheets.Add("Tasks");

			// ヘッダー
			worksheet.Cell(1, 1).Value = "タスク名";
			worksheet.Cell(1, 2).Value = "開始日";
			worksheet.Cell(1, 3).Value = "終了日";
			worksheet.Cell(1, 4).Value = "状態";
			worksheet.Cell(1, 5).Value = "予定工数";
			worksheet.Cell(1, 6).Value = "実績工数";
			worksheet.Cell(1, 7).Value = "差分";
			worksheet.Cell(1, 8).Value = "説明";
			worksheet.Cell(1, 9).Value = "備考";

			// データ行
			int row = 2;
			foreach (var task in FilteredTasks)
			{
				worksheet.Cell(row, 1).Value = task.TaskName.Value;
				worksheet.Cell(row, 2).Value = task.StartDate.Value;
				worksheet.Cell(row, 3).Value = task.EndDate.Value;
				worksheet.Cell(row, 4).Value = task.Status.Value;
				worksheet.Cell(row, 5).Value = task.PlannedHours.Value;
				worksheet.Cell(row, 6).Value = task.ActualHours.Value;
				worksheet.Cell(row, 7).Value = task.Gap.Value;
				worksheet.Cell(row, 8).Value = task.Description.Value;
				worksheet.Cell(row, 9).Value = task.Remarks.Value;

				row++;
			}

			// 列幅を自動調整
			worksheet.Columns().AdjustToContents();

			workbook.SaveAs(dialog.FileName);

			MessageBox.Show("Excelファイルに出力しました。", "完了", MessageBoxButton.OK, MessageBoxImage.Information);
		}
		catch (Exception ex)
		{
			MessageBox.Show($"Excel出力中にエラーが発生しました：{ex.Message}", "エラー", MessageBoxButton.OK, MessageBoxImage.Error);
		}
	}

	/// <summary>
	/// 絞り込みを実行する
	/// </summary>
	private void ExecuteFilter()
	{
		if(FilterDate.Value is null)
		{
			MessageBox.Show("基準日を設定してください。", "エラー", MessageBoxButton.OK, MessageBoxImage.Information);
			return;
		}

		FilteredTasks.Clear();

		var filtered = AllTasks.Where(task =>
			(SelectedStatusFilter.Value == "すべて" || task.Status.Value == SelectedStatusFilter.Value) &&
			(task.StartDate.Value <= FilterDate.Value && FilterDate.Value <= task.EndDate.Value)
		);

		foreach (var task in filtered)
		{
			FilteredTasks.Add(task);
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