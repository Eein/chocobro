﻿using System;
namespace Chocobro {

  public partial class Job {

    public string name { get; set; }
    public int STR { get; set; }
    public int DEX { get; set; }
    public int VIT { get; set; }
    public int INT { get; set; }
    public int MND { get; set; }
    public int PIE { get; set; }
    public int CRIT = 341;
    public int DTR = 202;
    public int ACC = 341;
    public int SKS = 341;
    public int SPS = 341;
    public int AP = 0; // Define after gear
    public int AMP = 0; // Define after gear
    

    public double calculateGCD() {
      var skillcalc = MainWindow.gcd - (Math.Round(((SKS - 341) * 0.00095308) * 100) / 100);
      return skillcalc;
    }
    public void calculateSGCD(double castspeed) {
      var skillcalc = castspeed - (Math.Round(((SPS - 341) * 0.00095308) * 100) / 100);
    }
    public int TP() {
      return MainWindow.TP;
    }
    public void addTP(int amount) {
      MainWindow.TP += amount;
    }

    public virtual void rotation() { }
    public virtual void regen() {
      if (MainWindow.time == MainWindow.servertime && MainWindow.servertick == 3) {
        //TP regen
        if (MainWindow.TP != 1000) {
          MainWindow.TP += 60;
          MainWindow.log("TP Regen Tick - Restored 60 TP. Current TP: " + MainWindow.TP);
        }
        //MP regen (add this eventually. Check old sim for reference)
      }
    }

  }
}
