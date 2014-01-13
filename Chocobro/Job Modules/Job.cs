using System;
namespace Chocobro {

  // public partial class Job {
  //   static Job _player;
  //   public Job(job player){
  //     _player = player;
  //   }
  public partial class Job {
    public string name { get; set; }
    public int STR { get; set; }
    public int DEX { get; set; }
    public int VIT { get; set; }
    public int INT { get; set; }
    public int MND { get; set; }
    public int PIE { get; set; }
    public int WEP { get; set; }
    public double AADMG = 0.0;
    public double AAPOT = 0;
    public double CRIT = 341;
    public int DTR = 202;
    public int ACC = 341;
    public int SKS = 341;
    public int SPS = 341;
    public int AP = 0; // Define after gear
    public int AMP = 0; // Define after gear
    public double nextability = 0.00;
    public double nextinstant = 0.00;
    public double nextauto = 0.00;
    public bool actionmade = false;
    public int TP = 1000;
    public int MP = 1000; // TODO: formulate.
    public double gcd = 2.5;
    public int totaldamage = 0;


    public double calculateGCD() {
      var skillcalc = gcd - (Math.Round(((SKS - 341) * 0.00095308) * 100) / 100);
      return skillcalc;
    }
    public void calculateSGCD(double castspeed) {
      var skillcalc = castspeed - (Math.Round(((SPS - 341) * 0.00095308) * 100) / 100);
    }

    public void addTP(int amount) {
      TP += amount;
    }

    public virtual void rotation() { }
    public virtual void regen() {
      if (MainWindow.time == MainWindow.servertime && MainWindow.servertick == 3) {
        //TP regen
        if (TP != 1000) {
          TP += 60;
          if (TP > 1000) { TP = 1000; }
          MainWindow.log("TP Regen Tick - Restored 60 TP. Current TP: " + TP);
        }
        //MP regen (add this eventually. Check old sim for reference)
      }
    }

  }
}
