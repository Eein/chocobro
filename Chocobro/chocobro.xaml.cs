using System;
using System.Collections.Generic;
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
using System.IO;
 

namespace Chocobro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window {
       
        //Global Definition
        static double time = 0.00;
        static double fightlength = 0.00;

        public void scheduler() {
            log("schedule");

        }
        public void handler(ref Job p) {
            log("handle:");
            p.rotation();
        }
        //append line

                
        // MAIN
        public MainWindow(){
            InitializeComponent();
            SortedDictionary<double, string> queue = new SortedDictionary<double, string>(); //Initialize Queue 
            //Initialize default actions here. (autorun on load)
            fightlength = 10.0;

        }
        

        public void simulate() {
            clearLog();
            Job p = new Job();
            p.name = "Job Not Defined"; // Debug text.
            
            //Define Player Object
            createJobObject(ref p);

            time = 0;
            if (p.name == "Job Not Defined") { return; }
            while (time <= fightlength) {
                handler(ref p);
                scheduler();
                time += 1;
            }

            //parse log into box
            readLog();
        }

        public void createJobObject(ref Job p) {
            //Dynamic Player Object Creation based on dropdown...
            if (job.Text == "Bard") {
                p = new Bard() { name = "Player(Bard)", STR = 161, DEX = 224, VIT = 202, INT = 151, MND = 141, PIE = 151};
                p.name = "Player(Bard)";
            }
            
        }


        //Misc GUI elements
        private void applicationExit(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            //Read Fight Length in as double.
            fightlength = Convert.ToInt16(fightLengthInput.Text);

            console.Document.Blocks.Clear(); // Clear Console before starting.
            console.AppendText("" + Environment.NewLine); // This is required because who knows....
            simulate();

        }
        
        // Global accessors
        public static double time_t() { return time; }
        public static double fightlength_t() { return time; }


        // Logging
        public static void log(String s) {
            StreamWriter sw = File.AppendText("output.txt");
            sw.WriteLine(s);
            sw.Close();
        }
        public static void clearLog() {
            StreamWriter sw = new StreamWriter("output.txt");
                sw.Write("");
                sw.Close();
        }
        public void readLog() {
            StreamReader sr = new StreamReader("output.txt"); //TODO allow user to rename this.
            var readContents = sr.ReadToEnd();
            console.AppendText(readContents);
            sr.Close();
        }
 
    }

}
