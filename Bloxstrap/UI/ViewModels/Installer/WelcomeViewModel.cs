namespace Bloxstrap.UI.ViewModels.Installer
{
    public class WelcomeViewModel : NotifyPropertyChangedViewModel
    {
        // formatting is done here instead of in xaml, it's a bit easier
        public string MainText => String.Format(
            Strings.Installer_Welcome_MainText,
            "[github.com/nyxstrap/Nyxstrap](https://github.com/nyxstrap/Nyxstrap)"
        );

        public bool CanContinue { get; set; } = false;
    }
}
