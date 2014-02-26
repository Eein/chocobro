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
using System.Threading;
using System.Collections.Generic;
using System.Linq;
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
    public static bool logging = true;
    //Resources
    public static string logstring = "";
    public static string reportstring = "";
    public static double AADELAY;


    //Character Sheet
    public class Factory {
      /// <summary>
      /// Decides which class to instantiate.
      /// </summary>
      /// 
      public Job Get(string s) {

        switch (s) {
          case "Bard": return new Bard();
          case "Paladin": return new Paladin();
          //case "Black Mage": return new Blackmage();
          default: return new Job();
        }
      }
    }


    public void handler(ref Job p) {
      p.actionmade = false; //temp
      p.rotation();

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

      List<double> valuearray = new List<double>();
      if (instant > time) {
        valuearray.Add(instant);
      }
      if (ability > time) {
        valuearray.Add(ability);
      }
      if (st_t > time) {
        valuearray.Add(st_t);
      }
      if (auto > time) {
        valuearray.Add(auto);
      }
      //if (OOT) {
      //  //REFACTOR? More accuracy is probably better
      //  valuearray.Add(st_t + (3 - servertick));
      //}
      value = valuearray.Min();

      //    if (OOT) { //if out of TP
      //      if (instant > time) {
      //        value = Math.Min(instant, Math.Min(st_t, auto));
      //
      //      } else {
      //        value = Math.Min(st_t, auto);
      //      }
      //    } else {
      //      if (instant > time) {
      //        value = Math.Min(instant, Math.Min(ability, Math.Min(st_t, auto)));
      //
      //      } else {
      //        value = Math.Min(ability, Math.Min(st_t, auto));
      //      }
      //    }
      //    //log("Next Action - " + value.ToString("F2") + " !! min value");
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

      logging = true;
      var jobtext = "";
      var statweighttext = "";
      var fightlengthtext = "";
      Job.timelineDPS.Clear();
      Job.timelinetime.Clear();
      this.Dispatcher.Invoke((Action)(() => {
            jobtext = job.Text;
            statweighttext = statweights.Text;
            fightlengthtext = fightLengthInput.Text;

          }));




      stopwatch.Start();

      debug(); //have option to disable TODO:
      Factory fact = new Factory();
      Report r = new Report();

      var playerjob = fact.Get(jobtext);

      var p = playerjob;
      double thisdps = 0;
      double[] DPSavgarray = new double[21];
      var counter1 = 0;
      p = playerjob;
      string swselected = Convert.ToString(statweighttext);
      p.statforweights = swselected;
      var trigger1 = 50;
      if (swselected == "None") { trigger1 = -50; }
      for (int y = -50; y <= trigger1; y += 5) {
        //progress bar incrementing here.. for stat weights
        if (swselected != "None") {

          this.Dispatcher.Invoke((Action)(() => {
            progressBar.Value = (int)((100) - (trigger1 - (y)));
          }));


        }


        double[] DPSarray = new double[iterations];

        for (int x = 0; x < iterations; ++x) {

          //alt progress bar for iterations only
          if (swselected == "None") {

            this.Dispatcher.Invoke((Action)(() => {

              this.progressBar.Value = (int)(((double)x / (double)iterations) * 100) + 1;
              //MessageBox.Show(""+progressBar.Value + " - val: " + bacon+ " - x: " +x+ " - iterations: " +iterations);
            }));
          }

          p.getStats(this);
          if (swselected == "Weapon Damage") { p.WEP += y; }
          if (swselected == "Dexterity") { p.DEX += y; }
          if (swselected == "Accuracy") { p.ACC += y; }
          if (swselected == "Crit") { p.CRIT += y; }
          if (swselected == "Determination") { p.CRIT += y; }
          if (swselected == "Skill Speed") { p.SKS += y; }
          p.resetAbilities();
          resetSim();
          fightlength = (Convert.ToInt16(fightlengthtext)) + (d100(0, (int)Math.Floor(Convert.ToInt16(fightlengthtext) * 0.1)) - (int)Math.Floor(Convert.ToInt16(fightlengthtext) * 0.05));
          thisdps = 0;

          while (time <= fightlength) {
            handler(ref p);
            tickevent();
            time = nextTime(p.nextinstant, p.nextability, servertime, p.nextauto, p.OOT, p.OOM);
            //add timeline stuff
            if (((x == 0) && (y == 0 || trigger1 == -50)) && time == servertime) {
              r.timeline.Add(p.totaldamage / time);
            }

          }

          if ((x == 0) && (y == 0 || trigger1 == -50)) {

            writeLog();
            p.report();
            writeReport();
            readReport();
            clearReport();
            reportstring = "";
            readLog();

          } else {
            logging = false;
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
      //pass DPS array to Job for reporting.
      p.DPSarray = DPSavgarray;

      r.parse(p);
      double simulationtime = (double)stopwatch.ElapsedMilliseconds;
      stopwatch.Stop();
      //reportstring += "AvgDPS: " + averageDPS + " iterations: " + DPSarray.Length;
      //reportstring += Environment.NewLine;
      reportstring += Environment.NewLine;
      reportstring += "Total Simulation Time: " + (simulationtime / 1000) + "s.";
      reportstring += "\n\n";

      //for (int index = 0; index < Job.timelinetime.Count; index++) {
      //  reportstring += Job.timelinetime[index];
      //  reportstring += Environment.NewLine;
      //}
      //reportstring += "\n\n";
      //for (int index = 0; index < Job.timelineDPS.Count; index++) {
      //  reportstring += Job.timelineDPS[index];
      //  reportstring += Environment.NewLine;
      //}

      writeReport();
      readReport();
      // Add actual reporting here...

      // End HTML report

      stopwatch.Reset();
      this.Dispatcher.Invoke((Action)(() => {
        this.WEP.IsEnabled = true;
        this.AADMG.IsEnabled = true;
        this.DELAY.IsEnabled = true;
        this.STR.IsEnabled = true;
        this.DEX.IsEnabled = true;
        this.VIT.IsEnabled = true;
        this.INT.IsEnabled = true;
        this.MND.IsEnabled = true;
        this.PIE.IsEnabled = true;
        this.CRIT.IsEnabled = true;
        this.DTR.IsEnabled = true;
        this.ACC.IsEnabled = true;
        this.SKSPD.IsEnabled = true;
        this.SPSPD.IsEnabled = true;
        this.iterationsinput.IsEnabled = true;
        this.fightLengthInput.IsEnabled = true;
        this.job.IsEnabled = true;
        this.ClearLogs.IsEnabled = true;
        this.simulateButton.IsEnabled = true;
        this.statweights.IsEnabled = true;
        this.StatGrwth.IsEnabled = true;
        this.Delta.IsEnabled = true;
        this.ClearLogs.IsEnabled = true;
      }));

    }

    //Misc GUI elements
    private void applicationExit(object sender, RoutedEventArgs e) {
      Application.Current.Shutdown();
    }
    private void Button_Click(object sender, RoutedEventArgs e) {
      //TODO: disable button

      WEP.IsEnabled = false;
      AADMG.IsEnabled = false;
      DELAY.IsEnabled = false;
      STR.IsEnabled = false;
      DEX.IsEnabled = false;
      VIT.IsEnabled = false;
      INT.IsEnabled = false;
      MND.IsEnabled = false;
      PIE.IsEnabled = false;
      CRIT.IsEnabled = false;
      DTR.IsEnabled = false;
      ACC.IsEnabled = false;
      SKSPD.IsEnabled = false;
      SPSPD.IsEnabled = false;
      iterationsinput.IsEnabled = false;
      fightLengthInput.IsEnabled = false;
      job.IsEnabled = false;
      ClearLogs.IsEnabled = false;
      simulateButton.IsEnabled = false;
      statweights.IsEnabled = false;
      StatGrwth.IsEnabled = false;
      Delta.IsEnabled = false;
      ClearLogs.IsEnabled = false;


      this.Dispatcher.Invoke((Action)(() => {
      this.progressBar.Value = 0;
      Thread simming = new Thread(simulate);
      //Read Fight Length in as double.
      iterations = Convert.ToInt32(iterationsinput.Text);
      fightlength = Convert.ToInt32(fightLengthInput.Text);
      console.Document.Blocks.Clear(); // Clear Console before starting.
      reportConsole.Document.Blocks.Clear(); // Clear Report before starting.
      clearLog();
      clearReport();
      logstring = "";
      reportstring = "";
      console.AppendText("" + Environment.NewLine); // This is required because who knows....
      simming.Start();

    }));

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
      if (logging) {
        logstring += s;
        if (newline) { logstring += "\n"; }
      }

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
      this.Dispatcher.Invoke((Action)(() => {
      StreamReader sr = new StreamReader("output.txt"); //TODO allow user to rename this.
      var readContents = sr.ReadToEnd();
      console.AppendText(readContents);
      sr.Close();
    }));


    }
    public void readReport() {
      this.Dispatcher.Invoke((Action)(() => {
      StreamReader sr = new StreamReader("report.txt"); //TODO allow user to rename this.
      var readContents = sr.ReadToEnd();
      reportConsole.AppendText(readContents);
      sr.Close();
    }));

    }

    private void console_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {

    }



  }

}
