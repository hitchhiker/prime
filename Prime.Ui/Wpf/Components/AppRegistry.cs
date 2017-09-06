using GalaSoft.MvvmLight.Messaging;
using StructureMap;

namespace Prime.Ui.Wpf
{
    public class AppRegistry : Registry
    {
        public AppRegistry()
        {
            Model();

            // Messaging
            For<IMessenger>().Use(PrimeWpf.I.Messenger);

            // Model
        }

        public void Model()
        {
            /*
            // Results
            For<IResultConverter>().Singleton().Use<ResultConverter>();
            For<IResultSerializer>().Singleton().Use<ResultSerializer>();
            For<IResultMutator>().Singleton().Use<BenchmarkResultMutator>(); // Implement pipeline pattern if more mutators will exist in the future.
            For<IStatisticsFormatter>().Use<StatisticsFormatter>();
            */
            // Sessions
            //For<ISessionService>().Singleton().Use<SessionService>();

            /*
            // Api
            For<IApiClient>().Singleton().Use<ApiClient>();
            */

            // ViewModel
            For<ILayoutManager>().Use<LayoutManager>();
            Scan(scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.WithDefaultConventions();
                //scanner.AddAllTypesOf<INewSessionViewModel>();
            });
        }
    }
}