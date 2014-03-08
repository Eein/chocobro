using System;
using System.Collections.Generic;
namespace Chocobro {

  public partial class Ability {

    public string name = "null";
    public string abilityType = "null";
    public int potency = 0;
    public int combopotency = 0;
    public int dotPotency = 0;
    public double recastTime = 0;
    public double animationDelay = 0;
    public int TPcost = 0;
    public double castTime = 0;
    public double duration = 0;
    public double nextCast = 0.0;
    public double buff = 0;
    public double debuff = 0;
    public double buffTime = 0;
    public double debuffTime = 0;
    public bool autoa = false;

    public bool pierce = false;
    public bool blunt = false;
    public bool slash = false;

    public int crits = 0;
    public int hits = 0;
    public int misses = 0;
    public int procs = 0;
    public int ticks = 0;
    public int tickcrits = 0;
    public double damage = 0;
    public double dotdamage = 0;

    public double dpet = 0.0;

    public int totalattacks = 0;
    public int totaldotticks = 0;
    //public bool dot = false; TODO: add this w/ smn.
    // Dots using dictionary lookup. easyyy.
    public Dictionary<String, Boolean> dotbuff = new Dictionary<String, Boolean>() {
        {"ragingstrikes",false},
        {"bloodforblood",false},
        {"straightshot",false},
        {"hawkseye",false},
        {"internalrelease",false},
        {"fightorflight",false},
        {"potion", false}
    };

    public double bonus = 0; // for abilitiy specific bonuses and potions
    public double percent = 0; // for things that increase by percentage.
    public void resetAbility() {
      this.crits = 0;
      this.hits = 0;
      this.misses = 0;
      this.procs = 0;
      this.ticks = 0;
      this.tickcrits = 0;
      this.nextCast = 0.0;
      this.damage = 0;
      this.dotdamage = 0;
      this.totalattacks = 0;
      this.totaldotticks = 0;
      this.dpet = 0;
    }
    //public double calculateCrit(Job _player) { return (0.0693 * _player.CRIT - 18.486); }



  }
}


