// Chocobro FFXIV Simulator
// License: GPLv3
// Author: William Volin - (Eein Black), Jason Batson - (Phyre Xia)
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

  public class Simulator_Object_Struct {
    public Job p;
    public List<double> DPSarray = new List<double>();
    public double time_next_action = 0.0;
  }

  public partial class MainWindow : Window {
    //Global Definition
    Random randtick = new Random();
    Stopwatch stopwatch = new Stopwatch();

    public enum StatWeight { None, WeaponDamage, MagicDamage, Dexterity, Strength, Intelligence, Mind, Piety, Accuracy, Determination, Crit, SkillSpeed, SpellSpeed }
    private static readonly Random rand = new Random();
    public static double time = 0.00;
    public static double fightlength = 0.00;
    public static int servertime = 0;
    public static int servertick = 0;
    public static int iterations = 0;
    public static bool logging = false;
    public static bool disdebuff = false;
    public static bool selenebuff = false;
    public static string lagstring;
    public static int upperlag = 0;
    public static int lowerlag = 0;
    public static List<double> bucketlist = new List<double>(50);
    public static double dpsminlist = 0;
    public static double dpsmaxlist = 0;
    public static int iterationum = 0;
    public List<Simulator_Object_Struct> plist = new List<Simulator_Object_Struct>();

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
      public static Job Get(string s) {
        switch (s) {
          case "Bard": return new Bard();
          //case "Archer": return new Archer();
          //case "Template": return new Template();
          //case "Paladin": return new Paladin();
          //case "Warrior": return new Warrior();
          case "Black Mage": return new BlackMage();
          case "Summoner": return new Summoner();
          case "Dragoon": return new Dragoon();
          case "Monk": return new Monk();
          default: return new Job();
        }

      }
    }

    //this is important to engine...
    public void handler(ref Job p) {
      p.actionmade = false; //temp
      p.rotation();

    }

    // Global Math

    //random function
    public static double d100(int min, int max) {
      return (rand.Next(min, max));
    }
    public static void tickevent() {
      //log(servertime.ToString("F2") + " - current server time");
      if (servertime == time) {
        if (servertick >= 3) {
          //log("--- SERVER TICK - Next tick at " + (servertime + 3) + " st: " + servertick);          
          servertick = 1;
        } else {
          servertick += 1;
        }
        servertime += 1;
      }
    }

    //engine??
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

      if (lagstring == "None") { }
      if (lagstring == "Small (50-150ms)") { lowerlag = 50; upperlag = 150; }
      if (lagstring == "Med (125-225ms)") { lowerlag = 125; upperlag = 225; }

      logging = true;
      var jobtext = "";
      var statweighttext = "";
      var fightlengthtext = "";

      int num_jobs = 0;
      //bucket clear
      bucketlist.Clear();
      this.Dispatcher.Invoke((Action)(() => {
        jobtext = job.Text;
        statweighttext = statweights.Text;
        fightlengthtext = fightLengthInput.Text;
        num_jobs = Convert.ToInt32(PLAYERS.Text);

      }));
      stopwatch.Start();

      Report r = new Report();


      for (var x = 0; x < num_jobs; x++) {
        Simulator_Object_Struct player_object = new Simulator_Object_Struct();
        player_object.p = Factory.Get(jobtext);
        plist.Add(player_object); // complex objects are inherently implied references, so this is OK
      }

      //Trace.Assert(playerjob.name != null, "No job selected. ERROR 1"); //assert failure of factory set.
      //     p = playerjob;
      string swselected = Convert.ToString(statweighttext);
      //     p.statforweights = swselected;

      // TODO: steps and delta...
      int step = 5;
      int delta = 50;
      int newdelta = delta;

      if (num_jobs == 1)
        swselected = "None";

      // TODO: add negative and positive delta selection. Currently we're doing positive only.
      StatWeight swenum = StatWeight.None;
      if (swselected == "None") { swenum = StatWeight.None; newdelta = 0; } else {
        newdelta *= 2;
        if (swselected == "Weapon Damage") { swenum = StatWeight.WeaponDamage; } else if (swselected == "Magic Damage") { swenum = StatWeight.MagicDamage; } else if (swselected == "Dexterity") { swenum = StatWeight.Dexterity; } else if (swselected == "Strength") { swenum = StatWeight.Strength; } else if (swselected == "Mind") { swenum = StatWeight.Mind; } else if (swselected == "Piety") { swenum = StatWeight.Piety; } else if (swselected == "Intelligence") { swenum = StatWeight.Intelligence; } else if (swselected == "Accuracy") { swenum = StatWeight.Accuracy; } else if (swselected == "Crit") { swenum = StatWeight.Crit; } else if (swselected == "Determination") { swenum = StatWeight.Determination; } else if (swselected == "Skill Speed") { swenum = StatWeight.SkillSpeed; } else if (swselected == "Spell Speed") { swenum = StatWeight.SpellSpeed; }
      }
      //subtract an extra step because one gets added initially.
      //buckets for dpstimeline
      for (int x = 0; x < 50; x++) {
        bucketlist.Add(0);
      }

      List<double> DPSarray = new List<double>();
      List<double> WeightArray = new List<double>();
      List<double> WeightDiff = new List<double>();

      // clear DPSarray for each player
      for (int i = 0; i < plist.Count; i++) {
        plist[i].DPSarray = new List<double>();
      }

      for (int y = 0; y <= newdelta; y += step) {

        //progress bar incrementing here.. for stat weights
        if (swselected != "None") {
          this.Dispatcher.Invoke((Action)(() => {
            progressBar.Value = (int)((100) - ((newdelta) - (y)));
          }));
        }


        //start iterations (needs threading!!!)
        for (int x = 1; x <= iterations; ++x) {
          int ticknumber = 0;
          servertick = randtick.Next(1, 4);
          //alt progress bar for iterations only
          if (swselected == "None") {
            this.Dispatcher.Invoke((Action)(() => {
              this.progressBar.Value = (int)(((double)x / (double)iterations) * 100);
            }));
          }

          for (int i = 0; i < plist.Count; i++) {
            Job p = plist[i].p;
            p.getStats(this);
            if (i == 1) { p.SKS += 25; }
            p.resetAbilities();
          }
          if (num_jobs == 1) {
            Job p = plist[0].p;

            switch (swenum) {
              case StatWeight.WeaponDamage:
                p.WEP = p.WEP - (delta - y);
                break;
              case StatWeight.MagicDamage:
                p.MDMG = p.MDMG - (delta - y);
                break;
              case StatWeight.Dexterity:
                p.DEX = p.DEX - (delta - y);
                break;
              case StatWeight.Strength:
                p.STR = p.STR - (delta - y);
                break;
              case StatWeight.Intelligence:
                p.INT = p.INT - (delta - y);
                break;
              case StatWeight.Mind:
                p.MND = p.MND - (delta - y);
                break;
              case StatWeight.Piety:
                p.PIE = p.PIE - (delta - y);
                break;
              case StatWeight.Accuracy:
                p.ACC = p.ACC - (delta - y);
                break;
              case StatWeight.Determination:
                p.DTR = p.DTR - (delta - y);
                break;
              case StatWeight.Crit:
                p.CRIT = p.CRIT - (delta - y);
                break;
              case StatWeight.SkillSpeed:
                p.SKS = p.SKS - (delta - y);
                break;
              case StatWeight.SpellSpeed:
                p.SPS = p.SPS - (delta - y);
                break;
            }
          }

          resetSim();
          fightlength = (Convert.ToInt32(fightlengthtext)) + (d100(0, (int)Math.Floor(Convert.ToInt16(fightlengthtext) * 0.1)) - (int)Math.Floor(Convert.ToInt16(fightlengthtext) * 0.05));
          if ((x == 1) && (y == 0)) { r.dpstimeline.Select(i => 0); r.dpstimelinecount.Select(i => 0); }
          debug(); //have option to disable TODO:

          //actual simming
          while (time <= fightlength) {

            for (int i = 0; i < plist.Count; i++) {
              Job p = plist[i].p;
              if (plist[i].time_next_action >= time - 0.000001) {
                handler(ref p);
               }
            }

            tickevent();

            // get next time tick for all players
            int sum_partial_damage = 0;
            List<double> plist_times = new List<double>();
            for (int i = 0; i < plist.Count; i++) {
              Job p = plist[i].p;
              double this_time = nextTime(p.nextinstant, p.nextability, servertime, p.nextauto, p.OOT, p.OOM);
              plist[i].time_next_action = this_time;
              plist_times.Add(this_time);
              sum_partial_damage += p.totaldamage;
            }
            time = plist_times.Min();

            //add timeline stuff

            int ending = ticknumber + 1;
            if ((y == 0 && time == servertime)) {
              if (r.dpstimeline.Count < ending) { r.dpstimeline.Add(0); }
              if (r.dpstimelinecount.Count < ending) { r.dpstimelinecount.Add(0); }
              if (r.tptimeline.Count < ending) { r.tptimeline.Add(0); }
              if (r.tptimelinecount.Count < ending) { r.tptimelinecount.Add(0); }
              if (r.mptimeline.Count < ending) { r.mptimeline.Add(0); }
              if (r.mptimelinecount.Count < ending) { r.mptimelinecount.Add(0); }
              r.dpstimeline[ticknumber] += sum_partial_damage / time;
              r.dpstimelinecount[ticknumber] += 1;

              // print TP / MP for each player
              for (int i = 0; i < plist.Count; i++) {
                Job p = plist[i].p;
                r.tptimeline[ticknumber] += p.TP;
                r.tptimelinecount[ticknumber] += 1;
                r.mptimeline[ticknumber] += p.MP;
                r.mptimelinecount[ticknumber] += 1;
              }
              ticknumber += 1;
            }
          }

          // get final sum of damage
          int sum_total_damage = 0;
          for (int i = 0; i < plist.Count; i++) {
            Job p = plist[i].p;
            sum_total_damage += p.totaldamage;
            DPSarray.Add((p.totaldamage / fightlength)); // per player
          }
          DPSarray.Add((sum_total_damage / fightlength)); // sum for all players


          //////////////////////////////////////////////////////////////////////////

          //first iteration reporting
          if (x == 1 && y == 0) {
            for (int i = 0; i < num_jobs; i++) {
              Job p = plist[i].p;
              p.report(); // first iteration of abilities
              writeLog();
              readLog();
            }
          } else {
            logging = false;
          }
        } //end iteration set

        /*
                // average per player DPS?
                for (int i = 0; i < plist.Count; i++) {
                  var ref_plist_elem = plist[i];
                  ref_plist_elem.DPSarray.Sort();
                  ref_plist_elem.p.averagedps = DPSarray.Average();
                  var l_dpsmin = ref_plist_elem.DPSarray.Min();
                  var l_dpsmax = ref_plist_elem.DPSarray.Max();
                  dpsminlist = l_dpsmin;
                  dpsmaxlist = l_dpsmax;
                  var l_dpsdiff = l_dpsmax - l_dpsmin;
                  var l_numbuckets = 50;
                  var l_dpsdiv = l_dpsdiff / l_numbuckets;
                }*/

        DPSarray.Sort();
        //        p.averagedps = DPSarray.Average(); (TODO: ?)
        var dpsmin = DPSarray.Min();
        var dpsmax = DPSarray.Max();
        dpsminlist = dpsmin;
        dpsmaxlist = dpsmax;
        var dpsdiff = dpsmax - dpsmin;
        var numbuckets = 50;
        var dpsdiv = dpsdiff / numbuckets;

        foreach (var it in DPSarray) {
          for (var x = 0; x <= numbuckets; ++x) {
            if (it >= dpsmin + (dpsdiv * x) && it <= dpsmin + (dpsdiv * (x + 1))) {
              bucketlist[x] += 1;
              break;
            }
          }
        }

        // calculate last difference of array

        var beforelast = 0.0;

        if (swselected != "None") {
          Job p = plist[0].p; // if swselected != None, there's only one player...
          if (p.passoverweight > 0.0) {
            //probably a better way to do this, but it stores the first attempt, then adds it as the subtractive weight
            beforelast = p.passoverweight;
            WeightDiff.Add((double)(beforelast - p.averagedps));
            p.passoverweight = 0.0;
          } else {

            if (WeightDiff.Count > 0) {
              beforelast = WeightArray.Last();
              WeightDiff.Add((double)(beforelast - p.averagedps));
            } else {
              p.passoverweight = p.averagedps;
            }
          }
        }
        // for avg difference of array used for calculating weight. 
        // avgoflist / steps for weight (non normalized) - no full calcs yet, but this is the method.

        WeightArray.Add(Math.Round((plist[0].p.averagedps * 100)) / 100); //array of all DPSavg's from all sets of iterations

        DPSarray.Clear();

        // clear player DPS arrays
        for (int i = 0; i < plist.Count; i++) {
          plist[i].DPSarray.Clear();
        }

      } //end statweight set

      WeightArray.Sort();

      if (num_jobs == 1) { //??
        Job p = plist[0].p;
        p.DPSarray = WeightArray;
      }
      //print stat weight if calculating weights.
      if (swselected != "None" && num_jobs == 1) {
        plist[0].p.weight = ((WeightDiff.Average() * -1) / step);
      }
      stopwatch.Stop();
      double simulationtime = (double)stopwatch.ElapsedMilliseconds;
      plist[0].p.simulationtime = simulationtime / 1000;

      for (var x = 0; x < r.dpstimeline.Count; x++) {
        r.dpstimeline[x] = r.dpstimeline[x] / r.dpstimelinecount[x];
        r.tptimeline[x] = r.tptimeline[x] / r.tptimelinecount[x];
        r.mptimeline[x] = r.mptimeline[x] / r.mptimelinecount[x];
      }
      // Parse HTML log
      r.parse(plist[0].p);

      stopwatch.Reset();

      this.Dispatcher.Invoke((Action)(() => {

        //refresh the html page
        this.htmlReport.Focus();
        this.browser.Navigate(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "report.html"));

        this.WEP.IsEnabled = true;
        this.MDMG.IsEnabled = true;
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
        this.disembowelbox.IsEnabled = true;
        this.selenebox.IsEnabled = true;
        this.job.IsEnabled = true;
        this.simulateButton.IsEnabled = true;
        this.statweights.IsEnabled = true;
        this.inputlag.IsEnabled = true;
        this.progressBar.Value = 100;
        //this.StatGrwth.IsEnabled = true; // TODO: RENAME THESE RIGHT
        //this.Delta.IsEnabled = true;
      }));

    }

    //Misc GUI elements
    private void applicationExit(object sender, RoutedEventArgs e) {
      Application.Current.Shutdown();
    }
    private void Button_Click(object sender, RoutedEventArgs e) {
      //TODO: disable button
      if (disembowelbox.IsChecked.HasValue && disembowelbox.IsChecked.Value) {
        disdebuff = true;
      } else { disdebuff = false; }

      if (selenebox.IsChecked.HasValue && selenebox.IsChecked.Value) {
        selenebuff = true;
      } else { selenebuff = false; }

      WEP.IsEnabled = false;
      MDMG.IsEnabled = false;
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
      disembowelbox.IsEnabled = false;
      selenebox.IsEnabled = false;
      job.IsEnabled = false;
      simulateButton.IsEnabled = false;
      statweights.IsEnabled = false;
      StatGrwth.IsEnabled = false; // TODO: RENAME THIS RIGHT
      Delta.IsEnabled = false;
      inputlag.IsEnabled = false;



      this.Dispatcher.Invoke((Action)(() => {
        this.progressBar.Value = 0;
        Thread simming = new Thread(simulate);
        //Read Fight Length in as double.
        lagstring = Convert.ToString(inputlag.Text);
        iterations = Convert.ToInt32(iterationsinput.Text);
        fightlength = Convert.ToInt32(fightLengthInput.Text);
        console.Document.Blocks.Clear(); // Clear Console before starting.
        clearLog();
        logstring = "";
        console.AppendText("" + Environment.NewLine); // This is required because who knows....
        simming.Start();

      }));

    }
    private void Window_Closed(object sender, EventArgs e) {
      Application.Current.Shutdown();
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
    public static void debug() {
      log("!! -- Tick Starting at: " + servertick);
    }
    public void resetSim() {
      time = 0.00;

      servertime = 0;
      servertick = randtick.Next(1, 4);
      logstring = "";
    }
    public void readLog() {
      this.Dispatcher.Invoke((Action)(() => {
        StreamReader sr = new StreamReader("output.txt"); //TODO allow user to rename this. (use a FileDlg
        var readContents = sr.ReadToEnd();
        console.AppendText(readContents);
        sr.Close();
      }));

    }
  }
}
