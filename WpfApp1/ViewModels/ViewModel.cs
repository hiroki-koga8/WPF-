using Reactive.Bindings;

namespace WpfApp1.ViewModels
{
    class ViewModel
    {
        public ReactiveProperty<string> InputValue { get; }
        public ReactiveProperty<string> OutputValue { get; }

        public ViewModel()
        {
            InputValue = new ReactiveProperty<string>();
            OutputValue = InputValue.ToReactiveProperty();
        }

    }
}
