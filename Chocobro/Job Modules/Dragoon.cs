using System;
using System.Windows;

namespace Chocobro {
  public enum Combo { None = 0, ImpulseDrive = 1, Disembowel = 2, ChaosThrust = 3, VorpalThrust = 4, FullThrust = 5 };

  public class Dragoon : Job {
    // Proc Booleans - Set all proc booleans false initially.

    Combo combo;
    public Dragoon() {
      name = "Dragoon";
      classname = "Lancer";
      
    }
    public override void getStats(MainWindow cs) {
      base.getStats(cs);
      // Define AP and MP conversion.
      AP = STR; //or STR
      AMP = INT;
      combo = Combo.None;
    }

    public override void rotation() {
      var gcd = calculateGCD();
      autoattack.recastTime = AADELAY;

      //Regen Mana/TP
      regen();

      //Abilities - execute(ref ability)
      if (heavythrust.buff < gcd && combo == Combo.None) { execute(ref heavythrust); }

      if (disembowel.debuff < 2 * gcd && combo == Combo.None) { execute(ref impulsedrive); }
      if (combo == Combo.Disembowel) {  execute(ref disembowel); }
      if (combo == Combo.ChaosThrust) {  execute(ref chaosthrust); }

      if (phlebotomize.debuff < gcd && combo == Combo.None) {  execute(ref phlebotomize); }

      if (combo == Combo.None) { execute(ref truethrust); }
      if (combo == Combo.VorpalThrust) { execute(ref vorpalthrust); }
      if (combo == Combo.VorpalThrust) { execute(ref lifesurge); }
      if (combo == Combo.FullThrust) {  execute(ref fullthrust); }

      // NOTE: For combos do... if(combo == Combo.ChaosThrust) etc

      execute(ref autoattack);

      //Buffs/Cooldowns - execute(ref ability)
      execute(ref bloodforblood);
      if (TP <= 440) {
        execute(ref invigorate);
      }
      execute(ref xpotionstrength);
      //Instants - execute(ref ability)
      execute(ref dragonfiredive);
      execute(ref legsweep);
      execute(ref lifesurge);
      execute(ref jump);
      execute(ref spineshatterdive);

      //Ticks - tick(ref DoTability)
      tick(ref phlebotomize);
      tick(ref chaosthrust);
      tick(ref disembowel);
      //AutoAttacks (not for casters!) - execute(ref autoattack)
      execute(ref autoattack);

      //Decrement Buffs - decrement(ref buff)
      decrement(ref heavythrust);
      decrement(ref lifesurge);
      decrement(ref bloodforblood);
      decrement(ref powersurge);
    }

    public override void execute(ref Ability ability) {
      base.execute(ref ability);
    }
    public override void impact(ref Ability ability) {

      if (ability.name == "Invigorate") {
        TP += 500;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " used. 400 TP Restored. TP is " + (TP - 400) + " => " + TP);
        if (OOT) {
          OOT = false;
          nextability = MainWindow.time;
        }
      }
      //var critchance = calculateCrit(_player);
      //set potency for now, but change to damage later.
      var accroll = (MainWindow.d100(1, 10001)) / 100;

      if (ability.abilityType == "Cooldown") {
        ability.hits += 1;
      }

      if (ability.abilityType == "Weaponskill" || (ability.abilityType == "Instant")) {
        numberofattacks += 1;
        ability.attacks += 1;
        if (accroll < calculateACC()) {
          // change states for combos here...
          if (ability.name == "Heavy Thrust") { combo = Combo.None; }

          if (ability.name == "Impulse Drive") { combo = Combo.Disembowel; }
          if (ability.name == "Disembowel") { combo = Combo.ChaosThrust; }
          if (ability.name == "Chaos Thrust") { combo = Combo.None; }
          
          if (ability.name == "True Thrust") { combo = Combo.VorpalThrust; }
          if (ability.name == "Vorpal Thrust") { combo = Combo.FullThrust; }
          if (ability.name == "Full Thrust") { combo = Combo.None; }
          
          if (ability.name == "Phlebotomize") { combo = Combo.None; }

          //

          double thisdamage = damage(ref ability, ability.potency);

          if (disembowel.debuff > 0) {
            thisdamage = Math.Floor(thisdamage *= 1.12);
          }

          numberofhits += 1;
          ability.hits += 1;

          totaldamage += (int)thisdamage;
          ability.damage += thisdamage;
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + thisdamage + " Damage. Next ability at: " + nextability);
        } else {
          numberofmisses += 1;
          ability.misses += 1;
          MainWindow.log("!!MISS!! - " + MainWindow.time.ToString("F2") + " - " + ability.name + " missed! Next ability at: " + ability.nextCast);
          
        }
      }
      if (ability.abilityType == "AUTOA") {
        autoattack.hits += 1;
        numberofattacks += 1;
        if (accroll < calculateACC()) {
          var thisdamage = damage(ref ability, ability.potency);
          numberofhits += 1;
          totaldamage += thisdamage;
          autoattack.damage += thisdamage;
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + thisdamage + " Damage. Next AA at: " + ability.nextCast);
        } else {
          autoattack.misses += 1;
          numberofmisses += 1;
          MainWindow.log("!!MISS!! - " + MainWindow.time.ToString("F2") + " - " + ability.name + " missed! Next AA at: " + ability.nextCast);
        }
      }

      // If ability has debuff, create its timer.
      if (ability.debuffTime > 0 && accroll < calculateACC()) {
        if (ability.debuff > 0 && ability.dotPotency > 0) {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + "  DOT clipped.");
          //reset all buffs if clipping
          ability.dotbuff["heavythrust"] = false;
          ability.dotbuff["bloodforblood"] = false;
          ability.dotbuff["potion"] = false;
        }
        //If dot exists and ability doesn't miss, enable its time.

        ability.debuff = ability.debuffTime;
        if (heavythrust.buff > 0) { ability.dotbuff["heavythrust"] = true; }
        if (bloodforblood.buff > 0) { ability.dotbuff["bloodforblood"] = true; }
        if (xpotionstrength.buff > 0) { ability.dotbuff["potion"] = true; }


        if (ability.dotPotency > 0) { 
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " DoT has been applied.  Time Left: " + ability.debuff);
        } else {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Debuff has been applied.  Time Left: " + ability.debuff);
        }
      }
      if (ability.buffTime > 0 && accroll < calculateACC()) {
        ability.buff = ability.buffTime;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " buff has been activated.  Time Left: " + ability.buff + ". Next ability at: " + nextability);
      }
      //Add buffs to dots

    }

    //public virtual void expire() { } not really needed. Maybe handle expiration in ticks? hmmm.

    public virtual void tick(ref Ability ability) {
      //schedule tick
      if (MainWindow.time == MainWindow.servertime && ability.debuff > 0) {
        ability.debuff -= 1.0;
        if (ability.debuff <= 0.0) {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " has fallen off.");
          //clear buffs from object.
          ability.dotbuff["heavythrust"] = false;
          ability.dotbuff["bloodforblood"] = false;
          ability.dotbuff["potion"] = false;
        }
      }
      if ((MainWindow.servertick == 3 && MainWindow.time == MainWindow.servertime) && (ability.debuff > 0 && ability.dotPotency > 0)) {
        numberofticks += 1;
        ability.ticks += 1;
        var tickdmg = damage(ref ability, ability.dotPotency, true);
        ability.dotdamage += tickdmg;
        totaldamage += tickdmg;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " is ticking now for " + tickdmg + "  Damage - Time Left: " + ability.debuff);
        //MainWindow.log("---- " + ability.name + " - Dots - RS: " + ability.dotbuff["ragingstrikes"] + " BFB: " + ability.dotbuff["bloodforblood"] + " SS: " + ability.dotbuff["straightshot"] + " HE: " + ability.dotbuff["hawkseye"] + " IR: " + ability.dotbuff["internalrelease"] + " Potion: " + ability.dotbuff["potion"]);
      }
    }

    public int damage(ref Ability ability, int pot, bool dot = false) {
      double damageformula = 0.0;
      double tempstr = STR;
      //potion check
      if (xpotionstrength.buff > 0 || (dot == true && ability.dotbuff["potion"] == true)) {
        //check for max dex increase from pot - NEEDS to be refactored...

        if (percentageOfStat(xpotionstrength.percent, tempstr) > xpotionstrength.bonus) {
          //MainWindow.log("yolo: " + percentageOfStat(xpotiondexterity.percent, tempdex) + " tempdex " + tempdex);
          tempstr += xpotionstrength.bonus;
          //MainWindow.log("capBonus Dex from potion: " + xpotiondexterity.bonus + " percent of stat: " + percentageOfStat(xpotiondexterity.percent, tempdex));
        } else {
          tempstr += percentageOfStat(xpotionstrength.percent, tempstr);
          //MainWindow.log("smBonus Dex from potion: " + percentageOfStat(xpotiondexterity.percent, tempdex));
        }
      }
      //end potion check
      if (ability.abilityType == "Weaponskill" || ability.abilityType == "Instant") {
        damageformula = ((double)pot / 100) * (0.005126317 * WEP * tempstr + 0.000128872 * WEP * DTR + 0.049531324 * WEP + 0.087226457 * tempstr + 0.050720984 * DTR);

      }
      if (ability.abilityType == "AUTOA") {
        damageformula = (AAPOT) * (0.408 * WEP + 0.103262731 * tempstr + 0.003029823 * WEP * tempstr + 0.003543121 * WEP * (DTR - 202));
      }

      //crit
      double critroll = MainWindow.d100(1, 1000001) / 10000; //critroll was only rolling an interger between 1-101. Now has the same precision as critchance.
      double critchance = 0;
      critchance = 0.0697 * (double)CRIT - 18.437; //Heavyshot interaction
      //MainWindow.log("CRIT CHANCE IS:" + critchance + " ROLL IS: " + critroll);
      if (dot) {
        if (ability.dotbuff["heavythrust"]) { damageformula *= 1.15; }
        if (ability.dotbuff["bloodforblood"]) { damageformula *= 1.30; }

      } else {
        if (heavythrust.buff > 0) { damageformula *= 1.15; }
        if (bloodforblood.buff > 0) { damageformula *= 1.30; }
      }

      if (lifesurge.buff > 0) { critroll = 0; lifesurge.buff = 0; } //lifesurge
      if (powersurge.buff > 0 && ability.name == "Jump") { damageformula *= 1.5; powersurge.buff = 0; }
      if (critroll <= critchance) {
        numberofcrits += 1;

        MainWindow.log("!!CRIT!! - ", false);
        damageformula *= 1.5;
        if (dot) {
          ability.tickcrits += 1;
        } else {
          //normal attack crit
          ability.crits += 1;
        }
      }

      // added variance to damage.
      damageformula = ((MainWindow.d100(-500, 500) / 10000) + 1) * (int)damageformula;
      return (int)damageformula;
    }

    // -------------------
    // Ability Definition
    // -------------------

    //resets


    public override void report() {
      base.report();
      // add abilities to list used for reporting. Each ability needs to be added ;(
      areport.Add(autoattack);
      areport.Add(truethrust);
      areport.Add(vorpalthrust);
      areport.Add(impulsedrive);
      areport.Add(heavythrust);
      areport.Add(legsweep);
      areport.Add(lifesurge);
      areport.Add(jump);
      areport.Add(fullthrust);
      areport.Add(bloodforblood);
      areport.Add(phlebotomize);
      areport.Add(disembowel);
      areport.Add(spineshatterdive);
      areport.Add(powersurge);
      areport.Add(dragonfiredive);
      areport.Add(chaosthrust);
      areport.Add(invigorate);
      areport.Add(xpotionstrength);
      if (MainWindow.selenebuff) {
        areport.Add(feylight);
        areport.Add(feyglow);
      }
    }
    Ability autoattack = new AutoAttack();
    Ability truethrust = new TrueThrust();
    Ability vorpalthrust = new VorpalThrust();
    Ability impulsedrive = new ImpulseDrive();
    Ability heavythrust = new HeavyThrust();
    Ability legsweep = new LegSweep();
    Ability lifesurge = new LifeSurge();
    Ability jump = new Jump();
    Ability fullthrust = new FullThrust();
    Ability bloodforblood = new BloodForBlood();
    Ability phlebotomize = new Phlebotomize();
    Ability disembowel = new Disembowel();
    Ability spineshatterdive = new SpineshatterDive();
    Ability powersurge = new PowerSurge();
    Ability dragonfiredive = new DragonfireDive();
    Ability chaosthrust = new ChaosThrust();
    Ability invigorate = new Invigorate();
    Ability xpotionstrength = new XPotionStrength();
    Ability feylight = new FeyLight();
    Ability feyglow = new FeyGlow();



    // Set array of abilities for reportingz

    // -------------------
    // Ability Definition
    // -------------------

    // True Thrust  ---------------------
    public class TrueThrust : Ability {
      public TrueThrust() {
        name = "True Thrust";
        abilityType = "Weaponskill";
        potency = 150;
        TPcost = 70;
        animationDelay = 1.4;
      }
    }
    // End True Thrust ---------------------

    // Vorpal Thrust  ---------------------
    public class VorpalThrust  : Ability {
      public VorpalThrust() {
        name = "Vorpal Thrust";
        abilityType = "Weaponskill";
        potency = 200;
        TPcost = 60;
        animationDelay = 1.4;
      }
    }
    // End Vorpal Thrust ---------------------

    // Impulse Drive  ---------------------
    public class ImpulseDrive : Ability {
      public ImpulseDrive() {
        name = "Impulse Drive";
        abilityType = "Weaponskill";
        potency = 180;
        TPcost = 70;
        animationDelay = 1.4;
      }
    }
    // End Impulse Drive  ---------------------

    // Heavy Thrust  ---------------------
    public class HeavyThrust : Ability {
      public HeavyThrust() {
        name = "Heavy Thrust";
        abilityType = "Weaponskill";
        potency = 170;
        TPcost = 70;
        buffTime = 20;
        animationDelay = 1.4;
      }
    }
    // End Heavy Thrust  ---------------------

    // Leg Sweep ---------------------
    public class  LegSweep : Ability {
      public LegSweep() {
        name = "Leg Sweep";
        abilityType = "Instant";
        potency = 130;
        recastTime = 20;
        animationDelay = 0.8;
      }
    }
    // End Leg Sweep---------------------

    // Invigorate --------------------------
    public class Invigorate : Ability {
      public Invigorate() {
        name = "Invigorate";
        abilityType = "Cooldown";
        recastTime = 120;
        animationDelay = 0.8;
      }
    }
    // End Invigorate  ---------------------

    // Life Surge ---------------------------------
    public class LifeSurge : Ability {
      public LifeSurge() {
        name = "Life Surge";
        abilityType = "Cooldown";
        recastTime = 60;
        buffTime = 10;
        animationDelay = 0.8;
      }
    }
    // End Life Surge  ----------------------------

    // Jump ---------------------
    public class Jump : Ability {
      public Jump() {
        name = "Jump";
        abilityType = "Instant";
        potency = 200;
        recastTime = 40;
        animationDelay = 0.8;
      }
    }
    // End Jump---------------------

    // Full Thrust ---------------------------------
    public class FullThrust : Ability {
      public FullThrust() {
        name = "Full Thrust";
        abilityType = "Weaponskill";
        animationDelay = 1.4;
        TPcost = 60;
        potency = 330;
      }
    }
    // End Full Thrust  ----------------------------

    // Blood for Blood ---------------------------------
    public class BloodForBlood : Ability {
      public BloodForBlood() {
        name = "Blood for Blood";
        abilityType = "Cooldown";
        recastTime = 80;
        buffTime = 20;
        animationDelay = 0.8;
      }
    }
    // End Blood for Blood  ----------------------------

    // Phlebotomize
    public class Phlebotomize : Ability {
      public Phlebotomize() {
        name = "Phlebotomize";
        abilityType = "Weaponskill";
        potency = 170;
        TPcost = 90;
        debuffTime = 18;
        dotPotency = 25;
        animationDelay = 1.4;
      }
    }
    // End Phlebotomize

    // Disembowel
    public class Disembowel : Ability {
      public Disembowel() {
        name = "Disembowel";
        abilityType = "Weaponskill";
        potency = 220;
        debuffTime = 30;
        animationDelay = 1.4;
        TPcost = 60;
      }
    }
    // End Dismebowel

    // Spineshatter Dive ---------------------

    public class SpineshatterDive : Ability {
      public SpineshatterDive() {
        name = "Spineshatter Dive";
        abilityType = "Instant";
        potency = 170;
        recastTime = 90;
        animationDelay = 0.8;
      }
    }
    // End Spineshatter Dive ---------------------

    // Power Surge
    public class PowerSurge : Ability {
      public PowerSurge() {
        name = "Power Surge";
        abilityType = "Cooldown";
        recastTime = 60;
        buffTime = 10;
        animationDelay = 0.8;
      }
    }
    // End Power Surge  ----------------------------

    // Dragonfire Dive ---------------------

    public class DragonfireDive : Ability {
      public DragonfireDive() {
        name = "Dragonfire Dive";
        abilityType = "Instant";
        potency = 250;
        recastTime = 180;
        animationDelay = 0.8;
      }
    }
    // End Spineshatter Dive ---------------------

    // Chaos Thrust
    public class ChaosThrust : Ability {
      public ChaosThrust() {
        name = "Chaos Thrust";
        abilityType = "Weaponskill";
        potency = 100;
        combopotency = 200;
        dotPotency = 30;;
        debuffTime = 30;
        animationDelay = 1.4;
      }
    }
    // End Dismebowel
    // Auto Attack
    public class AutoAttack : Ability {
      public AutoAttack() {
        name = "Auto Attack";
        recastTime = MainWindow.AADELAY;
        animationDelay = 0;
        abilityType = "AUTOA";
      }
    }
    //End Auto Attack

  }
}
