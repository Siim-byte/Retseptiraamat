using Microsoft.Extensions.DependencyInjection;

namespace Retseptiraamat
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new Pealeht();
        }

    }
}