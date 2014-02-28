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
// 
//
//
//------------------------------------

namespace Chocobro {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>

  public partial class MainWindow : Window {
    //Global Definition
    Random randtick = new Random();
    Stopwatch stopwatch = new Stopwatch();

    private static readonly Random rand = new Random();
    public static double time = 0.00;
    public static double fightlength = 0.00;
    public static int servertime = 0;
    public static int servertick = 0;
    public static int iterations = 0;
    public static bool logging = true;

    //Resources
    public static string logstring = "";

    //Global Stat - TODO: make this not global...
    public static double AADELAY;

    // Error List
    // 1 - Something impossible happened...
    //


    //Character Sheet
    public class Factory {
    // Decides which class to instantiate.
      public Job Get(string s) {
        switch (s) {
          case "Bard": return new Bard();
          case "Paladin": return new Paladin();
          //case "Warrior": return new Warrior();
          //case "Black Mage": return new Blackmage();
          //case "Summoner": return new Summoner();
          //case "Dragoon": return new Dragoon();
          //case "Monk": return new Monk();
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
      value = valuearray.Min();

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
      this.Dispatcher.Invoke((Action)(() => {
            jobtext = job.Text;
            statweighttext = statweights.Text;
            fightlengthtext = fightLengthInput.Text;

          }));
      stopwatch.Start();

      Factory fact = new Factory();
      Report r = new Report();

      var playerjob = fact.Get(jobtext);
      var p = playerjob;
      Trace.Assert(playerjob.name != null, "No job selected. ERROR 1"); //assert failure of factory set.
      p = playerjob;
      string swselected = Convert.ToString(statweighttext);
      p.statforweights = swselected;

      // TODO: steps and delta...
      int step = 5;
      int delta = 50;
      int newdelta = delta;
      // TODO: add negative and positive delta selection. Currently we're doing positive only.

      if (swselected == "None") { newdelta = 0; } else { newdelta *= 2; }

      //subtract an extra step because one gets added initially.


      List<double> DPSarray = new List<double>();
      List<double> WeightArray = new List<double>();
      for (int y = 0; y <= newdelta; y += step) {
        
        //progress bar incrementing here.. for stat weights
        if (swselected != "None") {
          this.Dispatcher.Invoke((Action)(() => {
            progressBar.Value = (int)((100) - ((newdelta) - (y)));
          }));
        }
        
        for (int x = 0; x < iterations; ++x) {
          
          servertick = randtick.Next(1, 4);
          //alt progress bar for iterations only
          if (swselected == "None") {
            this.Dispatcher.Invoke((Action)(() => {
              this.progressBar.Value = (int)(((double)x / (double)iterations) * 100) + 1;
            }));
          }

          p.getStats(this);

          if (swselected == "Weapon Damage") { p.WEP = p.WEP - (delta - y); }
          if (swselected == "Dexterity") { p.DEX = p.DEX - (delta - y); }
          if (swselected == "Accuracy") { p.ACC = p.ACC - (delta - y); }
          if (swselected == "Crit") { p.CRIT = p.CRIT - (delta - y); }
          if (swselected == "Determination") { p.DTR = p.DTR - (delta - y); }
          if (swselected == "Skill Speed") { p.SKS = p.SKS - (delta - y); }


          p.resetAbilities();
          resetSim();
          fightlength = (Convert.ToInt16(fightlengthtext)) + (d100(0, (int)Math.Floor(Convert.ToInt16(fightlengthtext) * 0.1)) - (int)Math.Floor(Convert.ToInt16(fightlengthtext) * 0.05));
         
          debug(); //have option to disable TODO:
          while (time <= fightlength) {
            handler(ref p);
            tickevent();
            time = nextTime(p.nextinstant, p.nextability, servertime, p.nextauto, p.OOT, p.OOM);
            //add timeline stuff
            if ((x == 0) && (y == 0 && time == servertime)) {
              r.timeline.Add(p.totaldamage / time);
            }

          }
          DPSarray.Add((p.totaldamage / fightlength));

          if (x == 0 && y == 0) {
            p.report(); // first iteration of abilities
            writeLog();
            readLog();
          } else {
            logging = false;
          }         
        } //end iteration set
      
        DPSarray.Sort();
        p.averagedps = DPSarray.Average();
        WeightArray.Add(Math.Round((DPSarray.Average() * 100)) / 100); //array of all DPSavg's from all sets of iterations
        DPSarray.Clear();
      } //end statweight set

      WeightArray.Sort();
      p.DPSarray = WeightArray;

      stopwatch.Stop();
      double simulationtime = (double)stopwatch.ElapsedMilliseconds;
      p.simulationtime = simulationtime / 1000;
      
      // Parse HTML log
      r.parse(p);
      
      stopwatch.Reset();
      
      this.Dispatcher.Invoke((Action)(() => {
        //refresh the html page
        this.htmlReport.Focus();

        this.browser.Navigate(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "report.html"));

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
      //reportConsole.Document.Blocks.Clear(); // Clear Report before starting.
      clearLog();
      //clearReport();
      logstring = "";
      //reportstring = "";
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
      //clearReport();
      console.Document.Blocks.Clear();
      //reportConsole.Document.Blocks.Clear();
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
    //public static void report(String s, bool newline = true) {
    //  reportstring += s;
    //  if (newline) { reportstring += "\n"; }
    //}
    //public static void writeReport() {
    //  StreamWriter sw = File.AppendText("report.txt");
    //  sw.WriteLine(reportstring);
    //  sw.Close();
    //}
    //public static void clearReport() {
    //  StreamWriter sw = new StreamWriter("report.txt");
    //  sw.Write("");
    //  sw.Close();
    //}
    public static void debug() {
      log("!! -- Tick Starting at: " + servertick);
    }
    public void resetSim() {
      time = 0.00;

      servertime = 0;
      servertick = randtick.Next(1, 4);
      logstring = "";
      //reportstring = "";

    }
    public void readLog() {
      this.Dispatcher.Invoke((Action)(() => {
      StreamReader sr = new StreamReader("output.txt"); //TODO allow user to rename this.
      var readContents = sr.ReadToEnd();
      console.AppendText(readContents);
      sr.Close();
    }));


    }
   // public void readReport() {
   //   this.Dispatcher.Invoke((Action)(() => {
   //   StreamReader sr = new StreamReader("report.txt"); //TODO allow user to rename this.
   //   var readContents = sr.ReadToEnd();
   //   //reportConsole.AppendText(readContents);
   //   sr.Close();
   // }));
   //
   // }

    private void console_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) {

    }



  }

}
