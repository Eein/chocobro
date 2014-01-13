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
    //Create a random tick to start on.
    Random randtick = new Random();
    private static readonly Random rand = new Random();
    //Global Definition

    public static double time = 0.00;
    public static double fightlength = 0.00;
    public static int servertime = 0;
    public static int servertick = 0;
    //Resources
    public static string logstring = "";

    public void handler(ref Job p) {
      p.actionmade = false; //temp
      p.rotation();

    }
    static class Factory {
      /// <summary>
      /// Decides which class to instantiate.
      /// </summary>
      public static Job Get(string s) {
        switch (s) {
          case "Bard": return new Bard();
          //case "Black Mage": return new Blackmage();
          default: return new Job();
        }
      }
    }
    // Global Math
    public static int d100() {
      return rand.Next(1, 101);
    }
    public static void tickevent() {
      //log(servertime.ToString("F2") + " - current server time");
      if (servertime == time) {
        if (servertick == 3) {
          //log("--- SERVER TICK - Next tick at " + (servertime + 3) + " st: " + servertick);          
          servertick = 1;
        } else {
          servertick += 1;
        }
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
      debug(); //have option to disable TODO:
      while (time <= fightlength) {
        handler(ref p);
        tickevent();
        time = nextTime(p.nextinstant, p.nextability, servertime, p.nextauto);
      }
      //read logstring into file
      writeLog();
      //parse log into box
      readLog();
      //reset globals
      resetSim();
    }

    //Misc GUI elements
    private void applicationExit(object sender, RoutedEventArgs e) {
      Application.Current.Shutdown();
    }
    private void Button_Click(object sender, RoutedEventArgs e) {
      //TODO: disable button

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
    public void resetSim() {
      time = 0.00;
      fightlength = 0.00;
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
