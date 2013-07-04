using System;
using Caliburn.Micro;

namespace FlashCards.ViewModels
{
    public class ItemViewModel : PropertyChangedBase
    {
        private string _title;
        private string _timeLimit;
        private string _image;
        private string _itemType;
        private int _id;
        private DateTime _lastUsed;
        private int _timesTaken;
        private int _averageScore;
        private int _last10Score;
        private int _highScore;
        private bool _hasBeenUsed;

        public string Title
        {
            get { return _title; }
            set
            {
                if (value == _title) return;
                _title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public string TimeLimit
        {
            get { return _timeLimit; }
            set
            {
                if (value == _timeLimit) return;
                _timeLimit = value;
                NotifyOfPropertyChange(() => TimeLimit);
            }
        }

        public string Image
        {
            get { return _image; }
            set
            {
                if (value == _image) return;
                _image = value;
                NotifyOfPropertyChange(() => Image);
            }
        }

        public string ItemType
        {
            get { return _itemType; }
            set
            {
                if (value == _itemType) return;
                _itemType = value;
                NotifyOfPropertyChange(() => ItemType);
            }
        }

        public int Id
        {
            get { return _id; }
            set
            {
                if (value == _id) return;
                _id = value;
                NotifyOfPropertyChange(() => Id);
            }
        }

        public DateTime LastUsed
        {
            get { return _lastUsed; }
            set
            {
                if (value.Equals(_lastUsed)) return;
                _lastUsed = value;
                NotifyOfPropertyChange(() => LastUsed);
            }
        }

        public int TimesTaken
        {
            get { return _timesTaken; }
            set
            {
                if (value == _timesTaken) return;
                _timesTaken = value;
                NotifyOfPropertyChange(() => TimesTaken);
            }
        }

        public int AverageScore
        {
            get { return _averageScore; }
            set
            {
                if (value == _averageScore) return;
                _averageScore = value;
                NotifyOfPropertyChange(() => AverageScore);
            }
        }

        public int Last10Score
        {
            get { return _last10Score; }
            set
            {
                if (value == _last10Score) return;
                _last10Score = value;
                NotifyOfPropertyChange(() => Last10Score);
            }
        }

        public int HighScore
        {
            get { return _highScore; }
            set
            {
                if (value == _highScore) return;
                _highScore = value;
                NotifyOfPropertyChange(() => HighScore);
            }
        }

        public bool HasBeenUsed
        {
            get { return _hasBeenUsed; }
            set
            {
                if (value.Equals(_hasBeenUsed)) return;
                _hasBeenUsed = value;
                NotifyOfPropertyChange(() => HasBeenUsed);
            }
        }

        public string ShortLastUsedDate { get { return LastUsed.ToString("MM/dd/yyyy"); } }

        public bool IncludesAddition { get; set; }
        public bool IncludesSubtraction { get; set; }
        public bool IncludesMultiplication { get; set; }
        public bool IncludesDivision { get; set; }
    }
}