using System;
using System.Collections.Generic;
namespace Chocobro {

  public partial class Ability : MainWindow {

    public string name;
    public string abilityType;
    public int potency;
    public int combopotency;
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
    //public bool dot = false; TODO: add this w/ smn.
    // Dots using dictionary lookup. easyyy.
    public Dictionary<String, Boolean> dotbuff = new Dictionary<String, Boolean>() {
        {"ragingstrikes",false},
        {"bloodforblood",false},
        {"straightshot",false},
        {"hawkseye",false},
        {"internalrelease",false},
        {"fightorflight",false}
    };

    public double bonus = 0; // for abilitiy specific bonuses.x


    //public double calculateCrit(Job _player) { return (0.0693 * _player.CRIT - 18.486); }



  }
}


