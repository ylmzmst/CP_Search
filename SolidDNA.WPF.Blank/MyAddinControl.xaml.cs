using CADBooster.SolidDna;
using System.Collections.Generic;
using System.Windows.Controls;
using static CADBooster.SolidDna.SolidWorksEnvironment;

namespace SolidDNA.WPF.Blank
{
    /// <summary>
    /// Interaction logic for MyAddinControl.xaml
    /// </summary>
    public partial class MyAddinControl : UserControl
    {
        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public MyAddinControl()
        {
            InitializeComponent();
        }

        #endregion

        /// <summary>
        /// When the button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Get the number of selected objects
            var count = 0;
            Application.ActiveModel?.SelectedObjects(objects => count = objects?.Count ?? 0);

            // Let the user know
            Application.ShowMessageBox($"Looks like you have {count} objects selected");
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;
        }

        private void Application_ActiveModelInformationChanged(CADBooster.SolidDna.Model obj)
        {
            ThreadHelpers.RunOnUIThread(() => 
            {
                var swModel = Application.ActiveModel;
                if (swModel == null)
                {
                    nopropforconfig.Visibility = System.Windows.Visibility.Visible;
                    cpsearch.Visibility = System.Windows.Visibility.Hidden;
                    noprop_txt.Text = "There is no Document";
                }
                else if (swModel.IsDrawing == true)
                {
                    nopropforconfig.Visibility = System.Windows.Visibility.Visible;
                    cpsearch.Visibility = System.Windows.Visibility.Hidden;
                    noprop_txt.Text = "Drawing Documents has no Custom Properties for Configuration";
                }
                else
                {
                    nopropforconfig.Visibility = System.Windows.Visibility.Hidden;
                    cpsearch.Visibility = System.Windows.Visibility.Visible;
                }
            });

        }

        private void search_btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            searchCP(cpsearch_txt.Text);
        }

        private void cpsearch_txt_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            searchCP(cpsearch_txt.Text);
        }

        private void searchCP(string cpName)
        {
            var swModel = Application.ActiveModel;
            if (swModel != null||swModel.IsDrawing!=true)
            {
                List<cpsearchcolumns> cpsearchList = new List<cpsearchcolumns>();
                for (int i = 0; i < swModel.ConfigurationCount; i++)
                {
                    string configName = swModel.ConfigurationNames[i];
                    string propVal = swModel.GetCustomProperty(cpName, swModel.ConfigurationNames[i]);
                    string respropVal = swModel.GetCustomProperty(cpName, swModel.ConfigurationNames[i], true);
                    cpsearchList.Add(new cpsearchcolumns { ID = i+1, Name = swModel.ConfigurationNames[i], Value = propVal, ResValue = respropVal });
                }
                cpsearch_dg.ItemsSource = cpsearchList;
            }
        }
        public class cpsearchcolumns
        {
            public int ID { get; set; }
            public string Name { get; set; }
            public object Value { get; set; }
            public object ResValue { get; set; }
        }
    }
}
