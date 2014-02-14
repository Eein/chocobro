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
using System.Diagnostics;
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
    Stopwatch stopwatch = new Stopwatch();
    private static readonly Random rand = new Random();
    //Global Definition

    public static double time = 0.00;
    public static double fightlength = 0.00;
    public static int servertime = 0;
    public static int servertick = 0;
    public static int iterations = 0;

    //Resources
    public static string logstring = "";
    public static string reportstring = "";
    public static double AADELAY;


    //Character Sheet



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
          case "Paladin": return new Paladin();
          //case "Black Mage": return new Blackmage();
          default: return new Job();
        }
      }
    }
    // Global Math

    public static double d100(int min, int max) {
      return (rand.Next(min, max));
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
    public static double nextTime(double instant, double ability, double st_t, double auto, bool OOT, bool OOM) {
      var value = 0.0;
      if (OOT) { //if out of TP
        if (instant > time) {
          value = Math.Min(instant, Math.Min(st_t, auto));

        } else {
          value = Math.Min(st_t, auto);
        }
      } else {
        if (instant > time) {
          value = Math.Min(instant, Math.Min(ability, Math.Min(st_t, auto)));

        } else {
          value = Math.Min(ability, Math.Min(st_t, auto));
        }
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
      
      stopwatch.Start();
      double[] DPSarray = new double[iterations];
      debug(); //have option to disable TODO:
      var playerjob = Factory.Get(job.Text);
      var p = playerjob;
      double thisdps = 0;
      for (int x = 0; x < iterations; ++x) {
        //var resultarray = [];
        //var parsedDPS;
        p = playerjob;
        //reset shit.
        p.getStats(this);
        p.resetAbilities();
        resetSim();
        thisdps = 0;

        while (!time.Equals(fightlength)) {
          handler(ref p);
          tickevent();
          time = nextTime(p.nextinstant, p.nextability, servertime, p.nextauto, p.OOT, p.OOM);
        }
        thisdps = p.totaldamage / fightlength;
        if (x == 0) {

          //read logstring into file
          writeLog();
          p.report();
          writeReport();
          readReport();
          clearReport();
          reportstring = "";
          //parse log into box
          readLog();
          //reset globals

        }

        //reset job object

        DPSarray[x] = thisdps;
        resetSim();
        fightlength = Convert.ToInt16(fightLengthInput.Text);
        //Somehow pass DPS back to array here
        //resultarray[x] = totalDPS;
        //totalDPS = 0;
      } //end for
      clearReport();
      double totaldps = 0;
      for (int index = 0; index < DPSarray.Length; index++) {
        totaldps += DPSarray[index];
        //reportstring += "eachDPS: " + DPSarray[index];
        //reportstring += Environment.NewLine;
      }
      double averageDPS = totaldps / iterations;
      double simulationtime = (double)stopwatch.ElapsedMilliseconds;
      stopwatch.Stop();
      reportstring += "AvgDPS: " + averageDPS + " iterations: " + DPSarray.Length;
      reportstring += Environment.NewLine;
      reportstring += "Total Simulation Time: " + ( simulationtime / 1000) + "s.";
      writeReport();
      readReport();
      stopwatch.Reset();
    }

    //Misc GUI elements
    private void applicationExit(object sender, RoutedEventArgs e) {
      Application.Current.Shutdown();
    }
    private void Button_Click(object sender, RoutedEventArgs e) {
      //TODO: disable button

      //Read Fight Length in as double.
      iterations = Convert.ToInt16(iterationsinput.Text);
      fightlength = Convert.ToInt16(fightLengthInput.Text);
      console.Document.Blocks.Clear(); // Clear Console before starting.
      reportConsole.Document.Blocks.Clear(); // Clear Report before starting.
      clearLog();
      clearReport();
      logstring = "";
      reportstring = "";
      console.AppendText("" + Environment.NewLine); // This is required because who knows....
      simulate();
      //console.AppendText("" + Environment.NewLine + DPSarray[0] + ", " + DPSarray[1] + ", " + DPSarray[2]);
    }
    private void Window_Closed(object sender, EventArgs e) {
      Application.Current.Shutdown();
    }

    private void Button_Clear(object sender, RoutedEventArgs e) {
      clearLog();
      clearReport();
      console.Document.Blocks.Clear();
      reportConsole.Document.Blocks.Clear();
      resetSim();
      console.AppendText("" + Environment.NewLine);
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
    public static void report(String s, bool newline = true) {
      reportstring += s;
      if (newline) { reportstring += "\n"; }
    }
    public static void writeReport() {
      StreamWriter sw = File.AppendText("report.txt");
      sw.WriteLine(reportstring);
      sw.Close();
    }
    public static void clearReport() {
      StreamWriter sw = new StreamWriter("report.txt");
      sw.Write("");
      sw.Close();
    }
    public static void debug() {
      log("!! -- Tick Starting at: " + servertick);
    }
    public void resetSim() {
      time = 0.00;
     
      servertime = 0;
      servertick = 0;
      logstring = "";
      reportstring = "";

    }
    public void readLog() {
      StreamReader sr = new StreamReader("output.txt"); //TODO allow user to rename this.
      var readContents = sr.ReadToEnd();
      console.AppendText(readContents);
      sr.Close();
    }
    public void readReport() {
      StreamReader sr = new StreamReader("report.txt"); //TODO allow user to rename this.
      var readContents = sr.ReadToEnd();
      reportConsole.AppendText(readContents);
      sr.Close();
    }

    private void console_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {

    }



  }

}
