using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.ApplicationSettings;
using Caliburn.Micro;
using FlashCards.Data;
using FlashCards.Extensions;
using FlashCards.Model;
using FlashCards.ViewModels;
using FlashCards.Views;
using Windows.UI;
using Windows.UI.Xaml.Media;

namespace FlashCards
{
    sealed partial class App
    {
        private WinRTContainer _container;

        public App()
        {
            InitializeComponent();

            var dbInit = new SqliteDbInitializer();
            dbInit.Initialize();
        }

        protected override void Configure()
        {
            base.Configure();

            LogManager.GetLog = x => new Logger();

            _container = new WinRTContainer();
            _container.RegisterWinRTServices();
            var settings = _container.RegisterSettingsService();

            settings.RegisterFlyoutCommand<UserSettingsViewModel>("Children",
                new Dictionary<string, object>
                {
                    {"ContentBackgroundBrush", new SolidColorBrush(Color.FromArgb(255, 50, 50, 50))}
                });

            _container.RegisterPerRequest(typeof(IUserRepository),"", typeof(UserRepository));
            _container.RegisterPerRequest(typeof(IDeckRepository),"", typeof(DeckRepository));
            _container.RegisterPerRequest(typeof(IAnswerRepository),"", typeof(AnswerRepository));

            _container.PerRequest<MainViewModel>()
                      .PerRequest<CardViewModel>()
                      .PerRequest<DeckViewModel>()
                      .PerRequest<NewDeckOptionsViewModel>()
                      .PerRequest<UserSettingsViewModel>()
                      .PerRequest<UserDetailsViewModel>()
                      .PerRequest<ItemViewModel>()
                      .PerRequest<GroupViewModel>();

 




            InitializeDatabase();

        }

        protected override object GetInstance(Type service, string key)
        {
            return _container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }


        private void InitializeDatabase()
        {
            using (var repo = new DeckRepository())
            {
                var defaultPresets = repo.All.Where(x => x.UserId == 0 && x.PresetOnly).ToList();

                // Addition 0-10
                if (!defaultPresets.Any(x => x.OperationsIncluded == "[A10]"))
                {
                    repo.InsertOrUpdate(new Deck
                    {
                        CardCount = 30,
                        DateStarted = DateTime.Now,
                        OperationsIncluded = "[A10]",
                        UserId = 0,
                        PresetOnly = true,
                        PresetName = "Addition 0-10"
                    });
                }


                // Subtraction 10-20
                if (!defaultPresets.Any(x => x.OperationsIncluded == "[S10]"))
                {
                    repo.InsertOrUpdate(new Deck
                    {
                        CardCount = 30,
                        DateStarted = DateTime.Now,
                        OperationsIncluded = "[S10]",
                        UserId = 0,
                        PresetOnly = true,
                        PresetName = "Subtraction 0-10"
                    });
                }

                // Multiplication
                if (!defaultPresets.Any(x => x.OperationsIncluded == "[M10]"))
                {
                    repo.InsertOrUpdate(new Deck
                    {
                        CardCount = 30,
                        DateStarted = DateTime.Now,
                        OperationsIncluded = "[M10]",
                        UserId = 0,
                        PresetOnly = true,
                        PresetName = "Multiplication 0-10"
                    });
                }

                // Division
                if (!defaultPresets.Any(x => x.OperationsIncluded == "[D10]"))
                {
                    repo.InsertOrUpdate(new Deck
                    {
                        CardCount = 30,
                        DateStarted = DateTime.Now,
                        OperationsIncluded = "[D10]",
                        UserId = 0,
                        PresetOnly = true,
                        PresetName = "Division 0-10"
                    });
                }
            }
        }


        protected override void PrepareViewFirst(Windows.UI.Xaml.Controls.Frame rootFrame)
        {
            _container.RegisterNavigationService(rootFrame);
        }

        protected override void OnLaunched(Windows.ApplicationModel.Activation.LaunchActivatedEventArgs args)
        {
            DisplayRootView<MainView>();
        }

    }


    public class Logger : ILog
    {
        public void Info(string format, params object[] args)
        {
            Debug.WriteLine("INFO: " + format, args);
        }

        public void Warn(string format, params object[] args)
        {
            Debug.WriteLine("WARNING: " + format, args);
        }

        public void Error(Exception exception)
        {
            Debug.WriteLine("ERROR: "+ exception.ToString());
        }
    }
}