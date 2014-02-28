﻿using System;
using System.Collections.Generic;
namespace Chocobro {

  // public partial class Job {
  //   static Job _player;
  //   public Job(job player){
  //     _player = player;
  //   }
  public partial class Job {

    public string name { get; set; }
    public string classname { get; set; }
    public int STR { get; set; }
    public int DEX { get; set; }
    public int VIT { get; set; }
    public int INT { get; set; }
    public int MND { get; set; }
    public int PIE { get; set; }
    public int WEP { get; set; }

    public string statweight { get; set; }
    public int StatGrwth { get; set; }
    public int delta { get; set; }
    public double AADMG = 0;
    public double AAPOT = 0;
    public double AADELAY = 0;
    //public double AADELAY = 0;
    public double CRIT = 341;
    public int DTR = 202;
    public int ACC = 341;
    public int SKS = 341;
    public int SPS = 341;
    public int AP  { get; set; } // Define after gear
    public int AMP  { get; set; } // Define after gear
    public double nextability = 0.00;
    public double nextinstant = 0.00;
    public double nextauto = 0.00;
    public bool actionmade = false;
    public int TP = 1000;
    public int MP = 1000; // TODO: formulate.
    public bool OOT = false;
    public bool OOM = false;
    public double gcd;
    private const double basegcd = 2.5;
    public int totaldamage = 0;
    public int numberofcrits = 0;
    public int numberofattacks = 0;
    public int numberofhits = 0;
    public int numberofticks = 0;
    public int numberofmisses = 0;
    public int ticknumber = 0;
    public double averagedps = 0;
    //stat weights
    public List<double> DPSarray = new List<double>();
    public string statforweights;
    public double simulationtime = 0;

    //ability reporting list
    public List<Ability> areport = new List<Ability>();

    public virtual void getStats(MainWindow cs) {
      cs.Dispatcher.Invoke((Action)(() => {
        STR = Convert.ToInt32(cs.STR.Text);
        DEX = Convert.ToInt32(cs.DEX.Text);
        VIT = Convert.ToInt32(cs.VIT.Text);
        INT = Convert.ToInt32(cs.INT.Text);
        MND = Convert.ToInt32(cs.MND.Text);
        PIE = Convert.ToInt32(cs.PIE.Text);

        WEP = Convert.ToInt32(cs.WEP.Text);
        AADMG = Convert.ToDouble(cs.AADMG.Text);
        AADELAY = Convert.ToDouble(cs.DELAY.Text);

        DTR = Convert.ToInt32(cs.DTR.Text);
        CRIT = Convert.ToInt32(cs.CRIT.Text);
        SKS = Convert.ToInt32(cs.SKSPD.Text);
        SPS = Convert.ToInt32(cs.SPSPD.Text);
        ACC = Convert.ToInt32(cs.ACC.Text);
        AAPOT = AADMG / System.Convert.ToDouble(WEP);


        statweight = Convert.ToString(cs.statweights.Text);
        delta = Convert.ToInt32(cs.Delta.Text);
        StatGrwth = Convert.ToInt32(cs.StatGrwth.Text);

        //Define AA
        totaldamage = 0;
        numberofcrits = 0;
        numberofattacks = 0;
        numberofhits = 0;
        numberofticks = 0;
        numberofmisses = 0;
        nextability = 0.00;
        nextinstant = 0.00;
        nextauto = 0.00;
        TP = 1000;
        MP = 1000;
        actionmade = false;
        OOT = false;
        OOM = false;


      }));

    }
    public virtual void impact(ref Ability ability) {
      // 
    }
    public virtual void execute(ref Ability ability) {


      if (ability.abilityType == "AUTOA" && MainWindow.time >= ability.nextCast) {
        MainWindow.time = MainWindow.floored(MainWindow.time);
        MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name);
        ability.nextCast = MainWindow.floored((MainWindow.time + ability.recastTime));
        nextauto = MainWindow.floored((MainWindow.time + ability.recastTime));
        impact(ref ability);
      }

      if (ability.abilityType == "Weaponskill" && !OOT) {

        //If time >= next cast time and time >= nextability)
        if (TP - ability.TPcost < 0) { //attempted to not allow TP to be less than 0, needs to be remade
          MainWindow.log("Was unable to execute " + ability.name + ". Not enough TP. Current TP is " + TP + "TP.");
          //nextability = MainWindow.time;
          //force nextability to next server tick
          //if invigorate is used and OOM then it resets the time to now.
          nextability = MainWindow.servertime + (3 - MainWindow.servertick);
          OOT = true;
        } else {

          if (MainWindow.time >= ability.nextCast && MainWindow.time >= nextability && actionmade == false) {
            //Get game time (remove decimal error)
            MainWindow.time = MainWindow.floored(MainWindow.time);
            MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name + ". Cost is " + ability.TPcost + "TP. TP is " + TP + " => " + (TP - ability.TPcost) + ".");
            // remove TP
            TP -= ability.TPcost;
            //if doesnt miss, then impact

            //set nextCast.
            ability.nextCast = MainWindow.floored((MainWindow.time + calculateGCD()));


            //set nextability
            nextability = MainWindow.floored((MainWindow.time + calculateGCD()));
            nextinstant = MainWindow.floored((MainWindow.time + ability.animationDelay));

            //time = nextTime(nextinstant, nextability);
            actionmade = true;

            //var critroll = d100.Next(1, 101);
            // var critbonus = calculateCrit();
            impact(ref ability);
          }
        }
      }
      if (ability.abilityType == "Instant" || ability.abilityType == "Cooldown") {
        //If time >= next cast time and time >= nextability)
        if (MainWindow.time >= ability.nextCast && MainWindow.time >= nextinstant && nextability > MainWindow.time + ability.animationDelay) { //&& nextability > MainWindow.time + ability.animationDelay is what i added
          //Get game time (remove decimal error)
          MainWindow.time = MainWindow.floored(MainWindow.time);
          MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name);
          //if doesnt miss, then impact

          //set nextCast.
          ability.nextCast = MainWindow.floored((MainWindow.time + ability.recastTime));

          //set nextability
          if (MainWindow.time + ability.animationDelay > nextability) {
            nextability = MainWindow.floored((MainWindow.time + ability.animationDelay));
          }

          nextinstant = MainWindow.floored((MainWindow.time + ability.animationDelay));

          impact(ref ability);
        }
      }


    }
    public virtual void decrement(ref Ability ability) {

      if (MainWindow.time == MainWindow.servertime && ability.buff > 0) {
        ability.buff -= 1.0;
        if (ability.buff <= 0.0) {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " has fallen off.");
        }
      }

    }


    public double calculateGCD() {
      var skillcalc = basegcd - (Math.Round(((SKS - 341) * 0.00095308) * 100) / 100);

      return skillcalc;
    }
    public void calculateSGCD(double castspeed) {
      var skillcalc = castspeed - (Math.Round(((SPS - 341) * 0.00095308) * 100) / 100);
    }
    public double calculateACC() {
      var acccalc = 84.0 + ((ACC - 341) * 0.1363636363);
      return acccalc;
    }
    public void addTP(int amount) {
      TP += amount;
    }
    public virtual void report() {

    }
    public virtual void rotation() { }
    public virtual void regen() {
      if (MainWindow.time == MainWindow.servertime && MainWindow.servertick == 3) {
        //TODO: SOME LOGIC HERE IS WRONG.  TP does NOT regen on the server dot tick (tested myself)
        //TP regen
        if (TP < 1000) {
          TP += 60;
          OOT = false;
          if (TP > 1000) { TP = 1000; }
          MainWindow.log(MainWindow.time.ToString("F2") + " - TP Regen Tick - Restored 60 TP. TP is now: " + TP);
        }
        //MP regen (add this eventually. Check old sim for reference)
      }
    }

    //Pots

    public class XPotionDexterity : Ability {
      public XPotionDexterity() {
        //HQ
        name = "HQ - X-Potion of Dexterity";
        recastTime = 300;
        animationDelay = 0.3;
        abilityType = "Cooldown";
        buffTime = 15;
        bonus = 67;
        percent = 16;
      }
    }

    //function to convert percentages to multipliers (ie. 16% to 1.16)
    public double percentToMulti(double percent) {
      var value = percent / 100;
      value += 1;
      return value;
    }
    public double percentageOfStat(double percent, double stat) {
      var value = percent / 100;
      value = value * stat;
      return value;
    }
    public virtual void resetAbilities() {
      foreach (Ability ability in areport) {
        ability.resetAbility();
      }
    }


  }
}
