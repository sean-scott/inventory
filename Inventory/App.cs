using System.Linq;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MvvmCross.Platform.IoC;
using Inventory.Interfaces;
using Inventory.ViewModels;

namespace Inventory
{
    public class App : MvxApplication
    {
        public override void Initialize()
        {
            RegisterDependencies();

            RegisterAppStart(new AppStart());
        }

        void RegisterDependencies()
        {
            //Mvx.RegisterSingleton(() => new GlobalBusyIndicatorState());
            //Mvx.RegisterType<IDatabaseManager, DatabaseManager>();
            //Mvx.RegisterSingleton<IPasswordStorage>(new PasswordStorage(Mvx.Resolve<IProtectedData>()));
            //Mvx.RegisterType(() => new Session());

            //Mvx.RegisterType<IBackupManager, BackupManager>();
            //Mvx.RegisterType<IAutobackupManager, AutoBackupManager>();
            
            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
            
            CreatableTypes()
                .EndingWith("DataAccess")
                .AsInterfaces()
                .RegisterAsDynamic();

            CreatableTypes()
                .EndingWith("Repository")
                .AsInterfaces()
                .RegisterAsLazySingleton();

            CreatableTypes()
                .EndingWith("Manager")
                .AsInterfaces()
                .RegisterAsDynamic();

            CreatableTypes()
                .EndingWith("ViewModel")
                .Where(x => !x.Name.StartsWith("DesignTime"))
                .AsInterfaces()
                .RegisterAsDynamic();

            CreatableTypes()
                .EndingWith("ViewModel")
                .AsTypes()
                .RegisterAsDynamic();
        }
    }
}