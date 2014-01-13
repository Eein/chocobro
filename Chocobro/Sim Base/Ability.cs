using System;

namespace Chocobro {

  public partial class Ability : MainWindow {

    public string name;
    public string abilityType;
    public int potency;
    public int dotPotency;
    public double recastTime;
    public double animationDelay;
    public int TPcost;
    public double castTime;
    public double duration;
    public double nextCast = 0.0;
    public double buff;
    public double debuff;
    public double buffTime;
    public double debuffTime;
    public bool autoa = false;
    // COPY THIS WHEREVER BUFFED IS USED 
    // 0 - Raging Strikes 
    // 1 - Blood for Blood 
    // 2 - Straighter Shot  
    // 3 - Hawk's Eye  
    // 4 - Internal Release  // 5 -  //
    int[] buffed = new int[6] { 0, 0, 0, 0, 0, 0 };

    public double bonus = 0; // for abilitiy specific bonuses.x


    //public double calculateCrit(Job _player) { return (0.0693 * _player.CRIT - 18.486); }



  }
}


