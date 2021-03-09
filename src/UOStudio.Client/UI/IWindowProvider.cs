namespace UOStudio.Client.UI
{
    public interface IWindowProvider
    {
        void Draw();

        TWindow GetWindow<TWindow>()
            where TWindow : Window;

        void Load();
    }
}
