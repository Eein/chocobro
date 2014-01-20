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
    public double AADMG = 0;
    public double AAPOT = 0;
    public double AADELAY = 0;
    //public double AADELAY = 0;
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
    public bool OOT = false;
    public bool OOM = false;
    public double gcd;
    private const double basegcd = 2.5;
    public int totaldamage = 0;
    public int numberofcrits = 0;
    public int numberofhits = 0;
    public int numberofmisses = 0;

    public void getStats(MainWindow cs) {
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

      //Define AA
      

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

    public virtual void rotation() { }
    public virtual void regen() {
      if (MainWindow.time == MainWindow.servertime && MainWindow.servertick == 3) {

        //TODO: SOME LOGIC HERE IS WRONG.  TP does NOT regen on the server dot tick (tested myself)
        //TP regen
        if (TP < 1000) {
          TP += 60;
          OOT = false;
          if (TP > 1000) { TP = 1000; }
          MainWindow.log(MainWindow.time.ToString("F2") + " - TP Regen Tick - Restored 60 TP. Current TP: " + TP);
        }
        //MP regen (add this eventually. Check old sim for reference)
      }
    }

  }
}
