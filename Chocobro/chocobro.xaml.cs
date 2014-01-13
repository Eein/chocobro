// Chocobro FFXIV Simulator
// License: GPLv3
// Author: William Volin - (Eein Black)
// Copyright 2013-2014 Chocobro.com
// Dont fork! Just join the team :)
// Commit Policy: Dont ask. If it compiles and does what you want it to, commit it.
// -------------- Another developer will look into any resulting errors or reverse the commit.


using System;
using System.Windows;
using System.IO;

//-------------TODO-------------------
// 1. Make options for Logs: None - Show - Debug :: this only applies to first iteration ALWAYS.
// 2. Make streamwriter global for faster parsing.
//
//
//------------------------------------

namespace Chocobro {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>

  public partial class MainWindow : Window {
    Random randtick = new Random();
    
    //Global Definition
    public static double gcd = 2.5;
    public static double time = 0.00;
    public static double fightlength = 0.00;
    public static double nextability = 0.00;
    public static double nextinstant = 0.00;
    public static double nextauto = 0.00;
    public static int servertime = 0;
    public static int servertick = 0;
    public bool autoa = false;
    //Resources
    public static int TP = 1000;
    public static int MP = 1000;
    public static string logstring = "";
    //temp actionmade to move along sim
    public static bool actionmade = false;

    public void handler(ref Job p) {
      actionmade = false; //temp
      p.rotation();

    }
     static class Factory {
      	/// <summary>
      	/// Decides which class to instantiate.
      	/// </summary>
      	public static Job Get(string s)
      	{
      	    switch (s)
      	    {
      		case "Bard": return new Bard(); 
      		default:     return new Job(); 
      	    }
      	}
    }
    // Global Math
    public static int d100() {
      Random rand = new Random();
      return rand.Next(1, 101);
    }
    public static void tickevent() {
      //log(servertime.ToString("F2") + " - current server time");
      if (servertime == time) {
        if (servertick == 3) {
          //log("--- SERVER TICK - Next tick at " + (servertime + 3) + " st: " + servertick);
          //TP-MP
          
          servertick = 1;
        } else {
          servertick += 1;
        }
        //tick event

        servertime += 1;
      }
    }
    public static double nextTime(double instant, double ability, double st_t, double auto) {
      var value = 0.0;
      if (instant > time) {
        value = Math.Min(instant, Math.Min(ability, Math.Min(servertime, auto)));
        
      } else {
        value = Math.Min(ability, st_t);
      }

      //log("Next Action - " + value.ToString("F2") + " !! min value");
      return value;
      //add cast here later.
    }
    public static double floored(double number) {
      var value = Math.Floor(number * 100) / 100;
      return value;
    }

    // MAIN
    public MainWindow() {
      InitializeComponent();
      servertick = randtick.Next(1, 4);

      //Initialize default actions here. (autorun on load)
    }
    public static double calculateCrit(int crit) {
      var value = (0.0693 * crit) - 18.486;
      return value;
    }
    public void simulate() {
      var p = Factory.Get(job.Text);
      
      // p.name = "Job Not Defined"; // Debug text.

      // //Define Player Object
      // if (job.Text == "Bard") {
      //   p = new Job(b) { name = "Player(Bard)", STR = 161, DEX = 224, VIT = 202, INT = 151, MND = 141, PIE = 151 };
      // }

      //if (p.name == "Job Not Defined") { return; }

      debug(); //have option to disable TODO:
      while (time <= fightlength) {
        //MessageBox.Show(time.ToString() + "  " + fightlength.ToString());

        handler(ref p);

        tickevent();
        time = nextTime(nextinstant, nextability, servertime, nextauto);
      }

      //parse log into box
      writeLog();
      readLog();
      resetSim();
    }

    //Misc GUI elements
    private void applicationExit(object sender, RoutedEventArgs e) {
      Application.Current.Shutdown();
    }
    private void Button_Click(object sender, RoutedEventArgs e) {
      //Read Fight Length in as double.
      fightlength = Convert.ToInt16(fightLengthInput.Text);
      console.Document.Blocks.Clear(); // Clear Console before starting.
      clearLog();
      console.AppendText("" + Environment.NewLine); // This is required because who knows....
      simulate();

    }
    private void Window_Closed(object sender, EventArgs e) {
      Application.Current.Shutdown();
    } 

    // Logging
    public static void log(String s, bool newline = true) {
      logstring += s;
      if (newline) { logstring += "\n"; }
    }
    public static void writeLog() {
      StreamWriter sw = File.AppendText("output.txt");
      sw.WriteLine(logstring);
      sw.Close();
    }
    public static void clearLog() {
      StreamWriter sw = new StreamWriter("output.txt");
      sw.Write("");
      sw.Close();
    }
    public static void debug() {
      log("!! -- Tick Starting at: " + servertick);
    }
    public static void resetSim() {
      gcd = 2.5;
      time = 0.00;
      fightlength = 0.00;
      nextability = 0.00;
      nextinstant = 0.00;
      servertime = 0;
      servertick = 0;
      logstring = "";
    }
    public void readLog() {
      StreamReader sr = new StreamReader("output.txt"); //TODO allow user to rename this.
      var readContents = sr.ReadToEnd();
      console.AppendText(readContents);
      sr.Close();
    }

  }

}
