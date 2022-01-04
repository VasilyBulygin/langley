using ReactiveUI;

namespace langley.gui.ViewModels
{
    public class BaseViewModel : ReactiveObject
    {

    }

    public class BaseViewModel<T> : BaseViewModel
    {
        protected T Model;

        protected BaseViewModel(T model)
        {
            Model = model;
        }
    }
}
