using Windows;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.Collections.ObjectModel;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409


namespace WindsupRFID
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        // DateTime for deltaTime for price
        //List<SUPData> SUPCategory = new List<SUPData>();
        
        ObservableCollection<SUPData> SUPCategory = new ObservableCollection<SUPData>();
        ObservableCollection<SUP> LeftSUP = new ObservableCollection<SUP>();
        public MainPage()
        {
            this.InitializeComponent();
            
            SUPCategory.Add(new SUPData("PVC",1, 8, 10, 8));
            
            SUPCategory.Add(new SUPData("Gonflable",2, 8, 12, 8));

            LV_Category.ItemsSource = SUPCategory;
        }

        private void newTyp_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ValidNewType_Click(object sender, RoutedEventArgs e)
        {
            int nDemi;
            int nHour;
            int nSupp;
            if(Int32.TryParse(newDemi.Text, out nDemi)
              && Int32.TryParse(newHour.Text, out nHour)
              && Int32.TryParse(newSupp.Text, out nSupp))
            {
               SUPCategory.Add(new SUPData(newName.Text, SUPCategory.Count, nDemi, nHour, nSupp));

               flyNewType.Hide();
            }
            else
            {
                // TODO: Notify wrong value
            }
            
            



        }
    }
}
