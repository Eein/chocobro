using System;
using System.Windows;

namespace Chocobro {
  public enum PLDCombo { PLDNone = 0, SavageBlade = 1, RageOfHalone = 2 };

  public class Paladin : Job {
    // Proc Booleans - Set all proc booleans false initially.
    PLDCombo combo;

    public Paladin() {
      name = "Paladin";
      classname = "Gladiator";
    }
    public override void getStats(MainWindow cs) {
      base.getStats(cs);
      // Define AP and MP conversion.
      AP = STR; //or STR
      AMP = INT;
      combo = PLDCombo.PLDNone;
    }

    public override void rotation() {
      var gcd = calculateGCD();
      autoattack.recastTime = AADELAY;
      execute(ref feylight);
      //Regen Mana/TP
      regen();

      //Abilities - execute(ref ability)
      if (fracture.debuff < gcd) { execute(ref fracture); }
      if (combo == PLDCombo.PLDNone) { execute(ref fastbalde); }
      if (combo == PLDCombo.SavageBlade) { execute(ref savageblade); }
      if (combo == PLDCombo.RageOfHalone) { execute(ref rageofhalone); }
      //Buffs/Cooldowns - execute(ref ability)
      execute(ref fightorflight);
      execute(ref xpotionstrength);

      //Instants - execute(ref ability)
      if (MainWindow.servertime >= 0.8 * MainWindow.fightlength) {
        execute(ref mercystroke);
      }
      execute(ref spiritswithin);
      execute(ref circleofscorn);

      //Ticks - tick(ref DoTability)
      tick(ref circleofscorn);
      tick(ref mercystroke);

      //AutoAttacks (not for casters!) - execute(ref autoattack)
      execute(ref autoattack);

      //Decrement Buffs - decrement(ref buff)
      decrement(ref fightorflight);
      decrement(ref xpotionstrength);



    }

    public override void execute(ref Ability ability) {
      base.execute(ref ability);
    }
    public override void impact(ref Ability ability) {

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

          if (ability.name == "Fast Blade") { combo = PLDCombo.SavageBlade; }
          if (ability.name == "Savage Blade") { combo = PLDCombo.RageOfHalone; }
          if (ability.name == "Rage of Halone") { combo = PLDCombo.PLDNone; }

          double thisdamage = damage(ref ability, ability.potency);

          if (MainWindow.disdebuff == true) {
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
          // Does heavyshot buff get eaten by a miss?
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
        if (ability.debuff > 0) {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + "  DOT clipped.");
          //reset all buffs if clipping
          ability.dotbuff["fightorflight"] = false;
          ability.dotbuff["potion"] = false;
        }
        //If dot exists and ability doesn't miss, enable its time.

        ability.debuff = ability.debuffTime;

        if (fightorflight.buff > 0) { ability.dotbuff["fightorflight"] = true; }
        if (xpotionstrength.buff > 0) { ability.dotbuff["potion"] = true; }



        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " DoT has been applied.  Time Left: " + ability.debuff);
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
          ability.dotbuff["fightorflight"] = false;
          ability.dotbuff["potion"] = false;
        }
      }
      if ((MainWindow.servertick == 3 && MainWindow.time == MainWindow.servertime) && ability.debuff > 0) {
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
        if (ability.dotbuff["fightorflight"]) { damageformula *= 1.30; }

      } else {
        if (fightorflight.buff > 0) { damageformula *= 1.30; }
      }

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
     areport.Add(fastbalde);
     areport.Add(savageblade);
     areport.Add(rageofhalone);
     areport.Add(circleofscorn);
     areport.Add(spiritswithin);
     areport.Add(mercystroke);
     areport.Add(fightorflight);
     areport.Add(fracture);
     areport.Add(xpotionstrength);
     areport.Add(autoattack);
      if (MainWindow.selenebuff) {
        areport.Add(feylight);
        areport.Add(feyglow);
      }
    }

    Ability fastbalde = new FastBlade();
    Ability savageblade = new SavageBlade();
    Ability rageofhalone = new RageOfHalone();
    Ability circleofscorn = new CircleOfScorn();
    Ability spiritswithin = new SpiritsWithin();
    Ability mercystroke = new MercyStroke();
    Ability fightorflight = new FightOrFlight();
    Ability fracture = new Fracture();
    Ability xpotionstrength = new XPotionStrength();
    Ability autoattack = new AutoAttack();
    Ability feylight = new FeyLight();
    Ability feyglow = new FeyGlow();



    // Set array of abilities for reportingz

    // -------------------
    // Ability Definition
    // -------------------

    // Fast Blade ---------------------
    public class FastBlade : Ability {
      public FastBlade() {
        name = "Fast Blade";
        potency = 150;
        TPcost = 70;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
      }
    }
    // End Fast Blade ---------------------

    // Savage Blade --------------------------
    public class SavageBlade : Ability {
      public SavageBlade() {
        name = "Savage Blade";
        potency = 200;
        TPcost = 60;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
      }
    }
    // End Savage Blade --------------------------

    // Rage of Halone -------------------------
    public class RageOfHalone : Ability {
      public RageOfHalone() {
        name = "Rage of Halone";
        potency = 260;
        TPcost = 60;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
      }
    }
    // End Rage of Halone ----------------------------

    // Circle of Scorn --------------------------------
    public class CircleOfScorn : Ability {
      public CircleOfScorn() {
        name = "Circle of Scorn";
        potency = 100;
        dotPotency = 30;
        abilityType = "Instant";
        recastTime = 25;
        debuffTime = 15;
        animationDelay = 0.8;
      }
    }
    // End Circle of Scorn --------------------------

    // Spirits Within --------------------------------
    public class SpiritsWithin : Ability {
      public SpiritsWithin() {
        name = "Spirits Within";
        potency = 300;
        recastTime = 30;
        animationDelay = 0.8;
        abilityType = "Instant";
      }
    }
    // End Spirits Within ---------------------------

    // Mercy Stroke -------------------------------
    public class MercyStroke : Ability {
      public MercyStroke() {
        name = "Mercy Stroke";
        potency = 200;
        recastTime = 90;
        animationDelay = 1.2;
        abilityType = "Instant";
      }
    }
    // Mercy Stroke -------------------------------

    // Fight or Flight ------------------------------------
    public class FightOrFlight : Ability {
      public FightOrFlight() {
        name = "Fight or Flight";
        recastTime = 90;
        animationDelay = 0.8;
        abilityType = "Cooldown";
        buffTime = 30;
      }
    }
    // End Fight or Flight -------------------------------

    // Fracture ---------------------------------
    public class Fracture : Ability {
      public Fracture() {
        name = "Fracture";
        potency = 100;
        dotPotency = 20;
        TPcost = 80;
        animationDelay = 0.8;
        abilityType = "Weaponskill";
        debuffTime = 18;
      }
    }
    // End Fracture ----------------------------

    // Auto Attack
    public class AutoAttack : Ability {
      public AutoAttack() {
        name = "Auto Attack";
        abilityType = "AUTOA";
      }
    }
    //End Auto Attack

  }
}

