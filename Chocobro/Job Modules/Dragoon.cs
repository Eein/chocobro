using System;
using System.Windows;

namespace Chocobro {

  public class Template : Job {
    // Proc Booleans - Set all proc booleans false initially.

    public Template() {
      name = "Template";
      classname = "Template";
    }
    public override void getStats(MainWindow cs) {
      base.getStats(cs);
      // Define AP and MP conversion.
      AP = DEX; //or STR
      AMP = INT;
    }

    public override void rotation() {
      var gcd = calculateGCD();
      autoattack.recastTime = AADELAY;

      //Regen Mana/TP
      regen();

      //Abilities - execute(ref ability)
      if (dot1.debuff <= 0) {
        execute(ref dot1);
      }
      execute(ref weaponskill1);

      //Buffs/Cooldowns - execute(ref ability)
      execute(ref cooldown1);
      execute(ref xpotiondexterity);

      //Instants - execute(ref ability)
      execute(ref instant1);

      //Ticks - tick(ref DoTability)
      tick(ref dot1);

      //AutoAttacks (not for casters!) - execute(ref autoattack)
      execute(ref autoattack);

      //Decrement Buffs - decrement(ref buff)
      decrement(ref cooldown1);
      decrement(ref xpotiondexterity);



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
          ability.dotbuff["cooldown1"] = false;
          ability.dotbuff["potion"] = false;
        }
        //If dot exists and ability doesn't miss, enable its time.

        ability.debuff = ability.debuffTime;

        if (cooldown1.buff > 0) { ability.dotbuff["cooldown1"] = true; }
        if (xpotiondexterity.buff > 0) { ability.dotbuff["potion"] = true; }



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
          ability.dotbuff["cooldown1"] = false;
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
      double tempdex = DEX;
      //potion check
      if (xpotiondexterity.buff > 0 || (dot == true && ability.dotbuff["potion"] == true)) {
        //check for max dex increase from pot - NEEDS to be refactored...

        if (percentageOfStat(xpotiondexterity.percent, tempdex) > xpotiondexterity.bonus) {
          //MainWindow.log("yolo: " + percentageOfStat(xpotiondexterity.percent, tempdex) + " tempdex " + tempdex);
          tempdex += xpotiondexterity.bonus;
          //MainWindow.log("capBonus Dex from potion: " + xpotiondexterity.bonus + " percent of stat: " + percentageOfStat(xpotiondexterity.percent, tempdex));
        } else {
          tempdex += percentageOfStat(xpotiondexterity.percent, tempdex);
          //MainWindow.log("smBonus Dex from potion: " + percentageOfStat(xpotiondexterity.percent, tempdex));
        }
      }
      //end potion check
      if (ability.abilityType == "Weaponskill" || ability.abilityType == "Instant") {
        damageformula = ((double)pot / 100) * (0.005126317 * WEP * tempdex + 0.000128872 * WEP * DTR + 0.049531324 * WEP + 0.087226457 * tempdex + 0.050720984 * DTR);

      }
      if (ability.abilityType == "AUTOA") {
        damageformula = (AAPOT) * (0.408 * WEP + 0.103262731 * tempdex + 0.003029823 * WEP * tempdex + 0.003543121 * WEP * (DTR - 202));
      }

      //crit
      double critroll = MainWindow.d100(1, 1000001) / 10000; //critroll was only rolling an interger between 1-101. Now has the same precision as critchance.
      double critchance = 0;
      critchance = 0.0697 * (double)CRIT - 18.437; //Heavyshot interaction
      //MainWindow.log("CRIT CHANCE IS:" + critchance + " ROLL IS: " + critroll);
      if (dot) {
        if (ability.dotbuff["cooldown1"]) { damageformula *= 1.20; }

      } else {
        if (cooldown1.buff > 0) { damageformula *= 1.20; }
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
      areport.Add(weaponskill1);
      areport.Add(dot1);
      areport.Add(cooldown1);
      areport.Add(instant1);
      areport.Add(autoattack);
      areport.Add(xpotiondexterity);
      if (MainWindow.selenebuff) {
        areport.Add(feylight);
        areport.Add(feyglow);
      }
    }
    Ability weaponskill1 = new Weaponskill1();
    Ability dot1 = new Dot1();
    Ability cooldown1 = new Cooldown1();
    Ability instant1 = new Instant1();
    Ability autoattack = new Autoattack();
    Ability xpotiondexterity = new XPotionDexterity();
    Ability feylight = new FeyLight();
    Ability feyglow = new FeyGlow();



    // Set array of abilities for reportingz

    // -------------------
    // Ability Definition
    // -------------------

    // Weaponskill 1  ---------------------

    public class Weaponskill1 : Ability {
      public Weaponskill1() {
        name = "Weaponskill 1";
        abilityType = "Weaponskill";
        potency = 150;
        TPcost = 70;
        animationDelay = 0.8;
      }
    }
    // End Ability 1 ---------------------

    // Dot 1 ------------------------------------
    public class Dot1 : Ability {
      public Dot1() {
        name = "Dot 1";
        abilityType = "Weaponskill";
        potency = 150;
        dotPotency = 50;
        debuffTime = 18;
        TPcost = 70;
        animationDelay = 0.8;
      }
    }
    // End Dot 1 -------------------------------

    // Cooldown 1 ---------------------------------
    public class Instant1 : Ability {
      public Instant1() {
        name = "Instant 1";
        abilityType = "Instant";
        potency = 150;
        recastTime = 15;
        animationDelay = 0.8;
      }
    }
    // End Cooldown 1 ----------------------------

    //Instant 1 --------------------------------
    public class Cooldown1 : Ability {
      public Cooldown1() {
        name = "Cooldown 1";
        abilityType = "Cooldown";
        buffTime = 20;
        recastTime = 60;
        animationDelay = 0.8;
      }
    }
    //End Instant 1 ------------------------------

    // Auto Attack
    public class Autoattack : Ability {
      public Autoattack() {
        name = "Auto Attack";
        recastTime = MainWindow.AADELAY;
        animationDelay = 0;
        abilityType = "AUTOA";
      }
    }
    //End Auto Attack

  }
}
