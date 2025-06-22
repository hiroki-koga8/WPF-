using Reactive.Bindings;
using System.Reactive;

namespace WpfApp1.ViewModels
{
    class ViewModel
    {
        /// <summary>
        /// テキストボックスに入力された値を格納する
        /// </summary>
        public ReactiveProperty<string> InputValue { get; init; } = new();
        /// <summary>
        /// 画面に表示する値を格納する
        /// </summary>
        public ReactiveProperty<string> OutputValue { get; init; } = new();
        /// <summary>
        /// 表示ボタンのコマンドを格納する
        /// </summary>
        public ReactiveCommand DisplayCommand { get; init; } = new();
		/// <summary>
		/// 削除ボタンのコマンドを格納する
		/// </summary>
		public ReactiveCommand DeleteCommand { get; init; } = new();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ViewModel()
        {
            InputValue = new ReactiveProperty<string>();
            DisplayCommand.Subscribe(Display);
			DeleteCommand.Subscribe(Delete);
		}

        /// <summary>
        /// 表示ボタンを押したときの挙動
        /// </summary>
        private void Display()
        {
            OutputValue.Value = InputValue.Value;
		}
		/// <summary>
		/// 削除ボタンを押したときの挙動
		/// </summary>
		private void Delete()
		{
			OutputValue.Value = string.Empty;
		}

	}
}
