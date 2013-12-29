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
using System.Collections;
 

namespace Chocobro
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window {
        
        //Global Definition
        public static double gcd = 2.5;
        public static double time = 0.00;
        public static double fightlength = 0.00;
        public static double nextability = 0.00;
        public static double nextinstant = 0.00;
        //temp actionmade to move along sim
        public static bool actionmade = false;

        public void handler(ref Job p) {

            actionmade = false; //temp
           
            p.rotation();

            if (actionmade == false) { time += 0.01; time = nextability; } else { }
        }

        // Global Math
        public static int d100() {
            Random rand = new Random();
            return rand.Next(1,101);
        }

        public static double nextTime(double instant, double ability) {
            return Math.Min(instant, ability);
            //add cast here later.
        }
        public static double floored(double number){
            var value = Math.Floor(number * 100) / 100;
            return value;
        }

                
        // MAIN
        public MainWindow(){
            InitializeComponent();
            //Initialize default actions here. (autorun on load)
        }
        

        public void simulate() {
            
            clearLog();
            Job p = new Job();
            p.name = "Job Not Defined"; // Debug text.
            
            //Define Player Object
            createJobObject(ref p);
 
            if (p.name == "Job Not Defined") { return; }
            
            while (time <= fightlength) {
                //MessageBox.Show(time.ToString() + "  " + fightlength.ToString());
                handler(ref p);

            }

            //parse log into box
            readLog();
            resetSim();
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
        private void Window_Closed(object sender, EventArgs e) {
            Application.Current.Shutdown();
        }
        


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
        public static void resetSim() {
        gcd = 2.5;
        time = 0.00;
        fightlength = 0.00;
        nextability = 0.00;
        nextinstant = 0.00;
        }
        public void readLog() {
            StreamReader sr = new StreamReader("output.txt"); //TODO allow user to rename this.
            var readContents = sr.ReadToEnd();
            console.AppendText(readContents);
            sr.Close();
        }

        
 
    }

}
