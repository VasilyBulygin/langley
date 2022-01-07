using ReactiveUI;

namespace langley.gui.ViewModels
{
    public class BaseViewModel : ReactiveObject
    {

    }

    public class BaseViewModel<T> : BaseViewModel
    {
        protected BaseViewModel(T model)
        {
            Model = model;
        }

        public T Model { get; protected set; }
    }
}
