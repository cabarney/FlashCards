using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using FlashCards.Data;
using FlashCards.Extensions;
using FlashCards.Model;
using FlashCards.Results;

namespace FlashCards.ViewModels
{
    public class UserSettingsViewModel : NavigationViewModelBase
    {
        private readonly INavigationService _navigation;
        private readonly IEventAggregator _events;

        public UserSettingsViewModel(INavigationService navigation, IEventAggregator events) : base(navigation)
        {
            _navigation = navigation;
            _events = events;

            using(var repo = IoC.Get<IUserRepository>())
            {
                ExistingUsers = new BindableCollection<User>(repo.All.ToList().OrderBy(x=>x.Name));
            }
        }

        public IObservableCollection<User> ExistingUsers { get; set; }
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                if (Equals(value, _selectedUser)) return;
                _selectedUser = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(() => CanDeleteUser);
            }
        }

        private string _newUserName;
        private User _selectedUser;
        private bool _errorShown;
        private string _errorMessage;

        public string NewUserName
        {
            get { return _newUserName; }
            set
            {
                if (value == _newUserName) return;
                _newUserName = value;
                NotifyOfPropertyChange();
                //NotifyOfPropertyChange(()=>CanAddUser);
            }
        }

        public void AddUser()
        {
            if(string.IsNullOrWhiteSpace(NewUserName))
            {
                ErrorMessage = "Child's name is required";
                ErrorShown = true;
                return;
            }

            if(ExistingUsers.Select(x=>x.Name.ToLower()).Contains(NewUserName.ToLower()))
            {
                ErrorMessage = "This child already exists!";
                ErrorShown = true;
                return;
            }

            var user = new User {Name = NewUserName};
            using(var repo = IoC.Get<IUserRepository>())
            {
                repo.InsertOrUpdate(user);
            }
            using(var repo = IoC.Get<IDeckRepository>())
            {
                var defaults = repo.All.Where(x => x.PresetOnly && x.UserId == 0).ToList();
                foreach(var preset in defaults)
                {
                    preset.UserId = user.Id;
                    preset.DateStarted = DateTime.Now;
                    preset.Id = 0;
                    repo.InsertOrUpdate(preset);
                }
            }
            NewUserName = "";
            ExistingUsers.Add(user);

            ErrorMessage = "";
            ErrorShown = false;

            _events.Publish(new AddUserMessage(user.Id));
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                if (value == _errorMessage) return;
                _errorMessage = value;
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public bool ErrorShown
        {
            get { return _errorShown; }
            set
            {
                if (value.Equals(_errorShown)) return;
                _errorShown = value;
                NotifyOfPropertyChange(() => ErrorShown);
            }
        }

        public void DeleteUser()
        {
            if (SelectedUser == null)
                return;

            var userId = SelectedUser.Id;
            using (var repo = IoC.Get<IUserRepository>())
            {
                repo.Delete(SelectedUser);
                ExistingUsers.Remove(SelectedUser);
            }
            using (var repo = IoC.Get<IDeckRepository>())
            {
                var decks = repo.All.Where(x => x.UserId == userId).ToList();
                foreach (var d in decks)
                {
                    repo.Delete(d);
                }
            }
            using(var repo = IoC.Get<IAnswerRepository>())
            {
                var answers = repo.All.Where(x => x.UserId == userId).ToList();
                foreach (var answer in answers)
                {
                    repo.Delete(answer);
                }
            }
            SelectedUser = null;
            _events.Publish(new RemoveUserMessage(userId));
        }

        public bool CanDeleteUser
        {
            get { return SelectedUser != null; }
        }


    }
}
