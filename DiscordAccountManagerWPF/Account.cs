// Account.cs
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DiscordAccountManagerWPF
{
    public class Account : INotifyPropertyChanged
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}