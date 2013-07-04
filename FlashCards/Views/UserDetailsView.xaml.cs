using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace FlashCards.Views
{
    public partial class UserDetailsView : Page
    {
        public UserDetailsView()
        {
            InitializeComponent();
        }

        private void itemListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (itemGridView.SelectedItem != null)
                MainAppBar.IsOpen = true;
            else
                MainAppBar.IsOpen = false;
        }
    }
}
