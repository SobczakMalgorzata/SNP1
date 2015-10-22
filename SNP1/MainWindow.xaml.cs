using Encog.App.Analyst;
using Encog.App.Analyst.CSV.Normalize;
using Encog.App.Analyst.Wizard;
using Encog.Engine.Network.Activation;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Util.CSV;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace SNP1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void chooseRegresion(object sender, RoutedEventArgs e)
        {
            menuItemClassiffication.IsEnabled = true;
            menuItemRegresion.IsEnabled = false;
        }

        private void chooseClassiffication(object sender, RoutedEventArgs e)
        {
            menuItemRegresion.IsEnabled = true;
            menuItemClassiffication.IsEnabled = false;
        }

        private void networkGenerateFromList(List<int> n, List<bool> b)
        {
            int l = 1;
            BasicNetwork network = new BasicNetwork();
            network.AddLayer(new BasicLayer(null, true, 2));
            foreach (int i in n)
            {
                network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, i));
            }
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 1));
            network.Structure.FinalizeStructure( );
            network.Reset();
        }

        private void networkGenerate(int input, int output)
        {
            if (equalNumberOfNeuronsInLayer.IsChecked == true)
            {
                int n, l;
                if (Int32.TryParse(numberOfNeurons.Text, out n) && Int32.TryParse(numberOfLayers.Text, out l))
                {
                    BasicNetwork network = new BasicNetwork();
                    network.AddLayer(new BasicLayer(null, true, input));
                    for (int i = 0; i < (l - 2); i++)
                    {
                        if (biasBool.IsChecked == true)
                            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, n));
                        else
                            network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, n + 1));
                    }
                    network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, output));
                    network.Structure.FinalizeStructure();
                    network.Reset();
                }
            }
            else { }
        }

        private void getLearningSet(object sender, RoutedEventArgs e)
        {
            string filename1, filename2;
            var analyst = new EncogAnalyst();

            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            
            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV Files |*.csv";
            
            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();
            
            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                filename1 = dlg.FileName;
                filename2 = System.IO.Path.ChangeExtension(filename1, "ega");
                dataNormalization(filename1, filename2);

                analyst.Load(new FileInfo("stats.ega"));

                int input = analyst.DetermineUniqueInputFieldCount();
                int output = analyst.DetermineUniqueOutputFieldCount();
                
                networkGenerate(input, output);

            }
        }
        private void dataNormalization(string f1, string f2)
        {
            var sourceFile = new FileInfo(f1);
            var targetFile = new FileInfo(f2);
            var analyst = new EncogAnalyst();
            var wizard = new AnalystWizard(analyst);
            wizard.Wizard(sourceFile, true, AnalystFileFormat.DecpntComma);
            var norm = new AnalystNormalizeCSV();
            norm.Analyze(sourceFile, true, CSVFormat.English, analyst);

            norm.ProduceOutputHeaders = true;

            norm.Normalize(targetFile);

            analyst.Save(new FileInfo("stats.ega"));
            analyst.Load(new FileInfo("stats.ega"));

        }
    }
}
