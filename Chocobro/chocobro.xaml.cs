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
      
      debug(); //have option to disable TODO:
      var playerjob = Factory.Get(job.Text);
      var p = playerjob;
      double thisdps = 0;
      double[] DPSavgarray = new double[21];
      var counter1 = 0;
      p = playerjob;
      string swselected = Convert.ToString(statweights.Text);
      var trigger1 = 50;
      if (swselected == "None") { trigger1 = -50; }
      for (int y = -50; y <= trigger1; y += 5) {

        double[] DPSarray = new double[iterations];

        for (int x = 0; x < iterations; ++x) {

          p.getStats(this);
          if (swselected == "Weapon Damage") { p.WEP += y; }
          if (swselected == "Dexterity") { p.DEX += y; }
          if (swselected == "Accuracy") { p.ACC += y; }
          if (swselected == "Crit") { p.CRIT += y; }
          if (swselected == "Determination") { p.CRIT += y; }
          if (swselected == "Skill Speed") { p.SKS += y; }
          p.resetAbilities();
          resetSim();
          fightlength = (Convert.ToInt16(fightLengthInput.Text)) + (d100(0, (int)Math.Floor(Convert.ToInt16(fightLengthInput.Text) * 0.1) ) - (int)Math.Floor(Convert.ToInt16(fightLengthInput.Text) * 0.05));
          thisdps = 0;

          while (!time.Equals(fightlength)) {
            handler(ref p);
            tickevent();
            time = nextTime(p.nextinstant, p.nextability, servertime, p.nextauto, p.OOT, p.OOM);
          }
          
          if ((x == 0) && (y == 0 || trigger1 == -50)) {

            writeLog();
            p.report();
            writeReport();
            readReport();
            clearReport();
            reportstring = "";
            readLog();
            
          }

          thisdps = p.totaldamage / fightlength;
          DPSarray[x] = thisdps;
          
        } //end iteration set
        
        double totaldps = 0;
        var slice = Math.Floor(DPSarray.Length * 0.05);
        Array.Sort(DPSarray);
        for (int index = (int)slice; index < DPSarray.Length - (int)slice; index++) {
          totaldps += DPSarray[index];
          //reportstring += (index - (int)slice + 1) + "/" + (DPSarray.Length - (2 * (int)slice)) + " eachDPS: " + DPSarray[index];
          //reportstring += Environment.NewLine;
        }

        double averageDPS = totaldps / (DPSarray.Length - (2 * (int)slice));
        DPSavgarray[counter1] = averageDPS; //array of all DPSavg's from all sets of iterations
        counter1 += 1;

      } //end statweight set

      reportstring += "AvgDPS" + " + StatWeights for \"" + swselected + "\"";
      reportstring += Environment.NewLine;
      for (int index = 0; index < counter1; index++) {  //prints Each DPSavg per interval
        reportstring += DPSavgarray[index];
        reportstring += Environment.NewLine;
      }
      
      double simulationtime = (double)stopwatch.ElapsedMilliseconds;
      stopwatch.Stop();
      //reportstring += "AvgDPS: " + averageDPS + " iterations: " + DPSarray.Length;
      //reportstring += Environment.NewLine;
      reportstring += Environment.NewLine;
      reportstring += "Total Simulation Time: " + ( simulationtime / 1000) + "s.";
      reportstring += Environment.NewLine;
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
