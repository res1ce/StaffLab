using sotrudniki.Model;
using sotrudniki.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace sotrudniki.View
{
    /// <summary>
    /// Логика взаимодействия для WindowRole.xaml
    /// </summary>
    public partial class WindowRole : Window
    {
        RoleViewModel RVM;
        public WindowRole()
        {
            InitializeComponent();
            RVM = new RoleViewModel();
            DataContext = RVM;
        }

        private void RoleListView_Select(object sender, SelectionChangedEventArgs e)
        {
            if (LvRole.SelectedItem != null)
            {
                RVM.SelectedRole = (Role)LvRole.SelectedItem;
            }
        }
    }
}
