using System;
using System.Windows;

namespace Chocobro {

  public class Bard : Job {
    // Proc Booleans - Set all proc booleans false initially.

    public Bard() {
      name = "Bard";
      classname = "Archer";
    }
    public override void getStats(MainWindow cs) {
      base.getStats(cs);
      // Define AP and MP conversion.
      AP = DEX;
      AMP = INT;
    }

    public override void rotation() {
      var gcd = calculateGCD();
      autoattack.recastTime = AADELAY;
      regen();


      if (MainWindow.flightbuff == true) {
        execute(ref feylight);
      }

      if (TP < 540) {
        execute(ref invigorate);
      }
      if (heavyshot.buff > 0 && straightshot.buff <= 4) {
        execute(ref straightshot);
      }

      if (straightshot.buff <= gcd) {
        execute(ref straightshot);
      }

      if (windbite.debuff <= gcd) {
        if (MainWindow.fightlength - MainWindow.time > 9) {
          execute(ref windbite);
        }
      }

      if (venomousbite.debuff <= gcd) {
        if (MainWindow.fightlength - MainWindow.time > 9) {
          execute(ref venomousbite);
        }
      }

      execute(ref heavyshot);

      execute(ref ragingstrikes); //"smart" use of buffs, needs more attention
      if (ragingstrikes.buff > 0) {
        execute(ref bloodforblood);
        execute(ref internalrelease);
        execute(ref hawkseye);
        execute(ref xpotiondexterity);
        execute(ref barrage);
      }

      if (ragingstrikes.nextCast >= MainWindow.time + 55 && ragingstrikes.nextCast <= MainWindow.time + 65) { //better use of internal release
        execute(ref internalrelease);
      }

      if (MainWindow.servertime >= 0.8 * MainWindow.fightlength) {
        execute(ref miserysend);
      }

      execute(ref bloodletter);
      execute(ref flamingarrow);
      execute(ref repellingshot);
      execute(ref bluntarrow);

      //server actionable - ticks/decrements then server tick action
      //if tick is 3 
      tick(ref windbite);
      tick(ref venomousbite);
      tick(ref flamingarrow);
      //auto 
      execute(ref autoattack);

      //decrement buffs
      decrement(ref straightshot);
      decrement(ref internalrelease);
      decrement(ref ragingstrikes);
      decrement(ref hawkseye);
      decrement(ref bloodforblood);
      decrement(ref barrage);
      decrement(ref heavyshot);
      decrement(ref xpotiondexterity);
      decrement(ref feylight);

    }

    public override void execute(ref Ability ability) {
      //barrage interactions
      if (ability.abilityType == "AUTOA" && MainWindow.time >= ability.nextCast) {
        if (barrage.buff > 0) {
          MainWindow.time = MainWindow.floored(MainWindow.time);
          MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name);
          impact(ref ability);
          MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name);
          impact(ref ability);
        }
      }
      base.execute(ref ability);
    }
    public override void impact(ref Ability ability) {

      //var critchance = calculateCrit(_player);
      //if (bard.straightshot.buff > 0) {  critchance += 10; }
      //set potency for now, but change to damage later.
      var accroll = (MainWindow.d100(1, 10001)) / 100;

      if (ability.name == "Invigorate") {
        TP += 400;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " used. 400 TP Restored. TP is " + (TP - 400) + " => " + TP);
        if (OOT) {
          OOT = false;
          nextability = MainWindow.time;
        }
      }

      if (ability.abilityType == "Cooldown") {
        ability.hits += 1;
      }

      if (ability.abilityType == "Weaponskill" || (ability.abilityType == "Instant")) {
        numberofattacks += 1;
        ability.attacks += 1;
        if (accroll < calculateACC() || hawkseye.buff > 0) {

          double thisdamage = damage(ref ability, ability.potency);

          if (MainWindow.disdebuff == true) {
            thisdamage = Math.Floor(thisdamage *= 1.12);
          }

          numberofhits += 1;
          ability.hits += 1;

          totaldamage += (int)thisdamage;
          ability.damage += thisdamage;
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + thisdamage + " Damage. Next ability at: " + nextability);
          if (ability.name == "Straight Shot") {
            heavyshot.buff = 0;
          }
          if (ability.name == "Heavy Shot") {
            double buffroll = MainWindow.d100(1, 101);
            if (20 >= buffroll) {
              heavyshot.buff = 10;
              heavyshot.procs += 1;
              MainWindow.log("!!PROC!! - Heavier Shot. Time Left: " + ability.buff);
            }
          }
        } else {
          numberofmisses += 1;
          ability.misses += 1;
          MainWindow.log("!!MISS!! - " + MainWindow.time.ToString("F2") + " - " + ability.name + " missed! Next ability at: " + ability.nextCast);
          // Does heavyshot buff get eaten by a miss?
          if (ability.name == "Straight Shot") {
            heavyshot.buff = 0;
          }
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
          ability.dotbuff["ragingstrikes"] = false;
          ability.dotbuff["bloodforblood"] = false;
          ability.dotbuff["straightshot"] = false;
          ability.dotbuff["hawkseye"] = false;
          ability.dotbuff["internalrelease"] = false;
          ability.dotbuff["potion"] = false;
        }
        //If dot exists and ability doesn't miss, enable its time.

        ability.debuff = ability.debuffTime;

        if (ragingstrikes.buff > 0) { ability.dotbuff["ragingstrikes"] = true; }
        if (bloodforblood.buff > 0) { ability.dotbuff["bloodforblood"] = true; }
        if (straightshot.buff > 0) { ability.dotbuff["straightshot"] = true; }
        if (hawkseye.buff > 0) { ability.dotbuff["hawkseye"] = true; }
        if (internalrelease.buff > 0) { ability.dotbuff["internalrelease"] = true; }
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
          ability.dotbuff["ragingstrikes"] = false;
          ability.dotbuff["bloodforblood"] = false;
          ability.dotbuff["straightshot"] = false;
          ability.dotbuff["hawkseye"] = false;
          ability.dotbuff["internalrelease"] = false;
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
      if (hawkseye.buff > 0 || ((dot) && ability.dotbuff["hawkseye"])) { tempdex *= 1.15; }
      if (ability.abilityType == "Weaponskill" || ability.abilityType == "Instant") {
        damageformula = ((double)pot / 100) * (0.005126317 * WEP * tempdex + 0.000128872 * WEP * DTR + 0.049531324 * WEP + 0.087226457 * tempdex + 0.050720984 * DTR);

      }
      if (ability.abilityType == "AUTOA") {
        damageformula = (AAPOT) * (0.408 * WEP + 0.103262731 * tempdex + 0.003029823 * WEP * tempdex + 0.003543121 * WEP * (DTR - 202));
      }

      //crit
      double critroll = MainWindow.d100(1, 1000001) / 10000; //critroll was only rolling an interger between 1-101. Now has the same precision as critchance.
      double critchance = 0;
      if (heavyshot.buff > 0 && ability.name == "Straight Shot") { critchance = 100; } else { critchance = 0.0697 * (double)CRIT - 18.437; } //Heavyshot interaction
      //MainWindow.log("CRIT CHANCE IS:" + critchance + " ROLL IS: " + critroll);
      if (dot) {
        if (ability.dotbuff["ragingstrikes"]) { damageformula *= 1.20; }
        if (ability.dotbuff["bloodforblood"]) { damageformula *= 1.10; }
        if (ability.dotbuff["straightshot"]) { critchance += 10; }
        if (ability.dotbuff["internalrelease"]) { critchance += 10; }

      } else {
        if (ragingstrikes.buff > 0 && ability.name != "Flaming Arrow") { damageformula *= 1.20; }
        if (bloodforblood.buff > 0 && ability.name != "Flaming Arrow") { damageformula *= 1.10; }
        if (straightshot.buff > 0) { critchance += 10; }
        if (internalrelease.buff > 0) { critchance += 10; }
      }

      if (critroll <= critchance) {
        numberofcrits += 1;

        MainWindow.log("!!CRIT!! - ", false);
        damageformula *= 1.5;
        if (dot) {
          ability.tickcrits += 1;
          //Bloodletter procs        
          if (bloodletter.nextCast > MainWindow.time && ((ability.name == "Windbite" && windbite.debuff > 0) || (ability.name == "Venomous Bite" && venomousbite.debuff > 0))) {
            var dotRoll = MainWindow.d100(1, 101);
            if (dotRoll >= 50) {
              bloodletter.nextCast = MainWindow.time;
              MainWindow.log("!!PROC!! - Bloodletter reset!");
              ability.procs += 1;
              bloodletter.procs += 1;
            }
          }
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
      areport.Add(heavyshot);
      areport.Add(venomousbite);
      areport.Add(windbite);
      areport.Add(straightshot);
      areport.Add(bloodletter);
      areport.Add(miserysend);
      areport.Add(bluntarrow);
      areport.Add(repellingshot);
      areport.Add(flamingarrow);
      areport.Add(internalrelease);
      areport.Add(bloodforblood);
      areport.Add(ragingstrikes);
      areport.Add(hawkseye);
      areport.Add(barrage);
      areport.Add(invigorate);
      areport.Add(autoattack);
      areport.Add(xpotiondexterity);
      if (MainWindow.flightbuff) {
        areport.Add(feylight);
      }
    }
    Ability heavyshot = new Heavyshot();
    Ability windbite = new Windbite();
    Ability venomousbite = new Venomousbite();
    Ability straightshot = new Straightshot();
    Ability bloodletter = new Bloodletter();
    Ability miserysend = new Miserysend();
    Ability bluntarrow = new Bluntarrow();
    Ability repellingshot = new Repellingshot();
    Ability flamingarrow = new Flamingarrow();
    Ability internalrelease = new Internalrelease();
    Ability bloodforblood = new Bloodforblood();
    Ability ragingstrikes = new Ragingstrikes();
    Ability hawkseye = new Hawkseye();
    Ability barrage = new Barrage();
    Ability invigorate = new Invigorate();
    Ability autoattack = new Autoattack();
    Ability xpotiondexterity = new XPotionDexterity();
    Ability feylight = new FeyLight();



    // Set array of abilities for reportingz

    // Heavy Shot ---------------------

    public class Heavyshot : Ability {
      public Heavyshot() {
        name = "Heavy Shot";
        potency = 150;
        dotPotency = 0;
        TPcost = 60;
        animationDelay = 0.8;
        abilityType = "Weaponskill";
        castTime = 0.0;
        duration = 0.0;
        buffTime = 10;
      }
    }
    // End Heavyshot ---------------------

    // Windbite --------------------------

    public class Windbite : Ability {
      public Windbite() {
        name = "Windbite";
        potency = 60;
        dotPotency = 45;
        TPcost = 80;
        animationDelay = 1.0;
        abilityType = "Weaponskill";
        castTime = 0.0;
        duration = 0.0;
        debuffTime = 18;
      }
    }
    // End Windbite --------------------------

    // Venomous Bite -------------------------

    public class Venomousbite : Ability {
      public Venomousbite() {
        name = "Venomous Bite";
        potency = 100;
        dotPotency = 35;
        TPcost = 80;
        animationDelay = 0.7;
        abilityType = "Weaponskill";
        castTime = 0.0;
        debuffTime = 18;
      }
    }
    // End Venomous Bite ----------------------

    // Straight Shot --------------------------

    public class Straightshot : Ability {
      public Straightshot() {
        name = "Straight Shot";
        potency = 140;
        dotPotency = 0;
        TPcost = 70;
        animationDelay = 0.8;
        abilityType = "Weaponskill";
        castTime = 0.0;
        buffTime = 20;
      }

    }
    // End Straight Shot --------------------------

    // Bloodletter --------------------------------

    public class Bloodletter : Ability {
      public Bloodletter() {
        name = "Bloodletter";
        potency = 150;
        dotPotency = 0;
        recastTime = 15;
        TPcost = 0;
        animationDelay = 1.25;
        abilityType = "Instant";
        castTime = 0.0;
      }

    }
    // End Bloodletter ---------------------------

    // Miserys End -------------------------------

    public class Miserysend : Ability {
      public Miserysend() {
        name = "Miserys End";
        potency = 190;
        dotPotency = 0;
        recastTime = 12;
        TPcost = 0;
        animationDelay = 1.1;
        abilityType = "Instant";
        castTime = 0.0;
      }

    }
    // End  Miserys End -------------------------------

    // Blunt Arrow ------------------------------------

    public class Bluntarrow : Ability {
      public Bluntarrow() {
        name = "Blunt Arrow";
        potency = 50;
        dotPotency = 0;
        recastTime = 30;
        TPcost = 0;
        animationDelay = 0.6;
        abilityType = "Instant";
        castTime = 0.0;
      }
    }
    // End Blunt Arrow -------------------------------

    // Repelling Shot ---------------------------------

    public class Repellingshot : Ability {
      public Repellingshot() {
        name = "Repelling Shot";
        potency = 80;
        dotPotency = 0;
        recastTime = 30;
        TPcost = 0;
        animationDelay = 1.2;
        abilityType = "Instant";
        castTime = 0.0;
      }
    }
    // End Repelling Shot ----------------------------

    //Flaming Arrow

    public class Flamingarrow : Ability {
      public Flamingarrow() {
        name = "Flaming Arrow";
        potency = 0;
        dotPotency = 35;
        recastTime = 60;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Instant";
        castTime = 0.0;
        debuffTime = 30;
      }

    }
    // End Flaming Arrow

    // Internal Release

    public class Internalrelease : Ability {
      public Internalrelease() {
        name = "Internal Release";
        recastTime = 60;
        animationDelay = 0.65;
        abilityType = "Cooldown";
        buffTime = 15;
      }

    }
    // End Internal Release

    // Blood for Blood

    public class Bloodforblood : Ability {
      public Bloodforblood() {
        name = "Blood for Blood";
        recastTime = 80;
        animationDelay = 0.75;
        abilityType = "Cooldown";
        buffTime = 20;
      }

    }
    // End Blood for Blood

    // Raging Strikes

    public class Ragingstrikes : Ability {
      public Ragingstrikes() {
        name = "Raging Strikes";
        recastTime = 120;
        animationDelay = 0.9;
        abilityType = "Cooldown";
        buffTime = 20;

      }

    }
    // End Raging Strikes

    // Hawks Eye

    public class Hawkseye : Ability {
      public Hawkseye() {
        name = "Hawks Eye";
        recastTime = 90;
        animationDelay = 0.65;
        abilityType = "Cooldown";
        buffTime = 20;
      }

    }
    // End Hawks Eye

    // Barrage

    public class Barrage : Ability {
      public Barrage() {
        name = "Barrage";
        recastTime = 90;
        animationDelay = 0.7;
        abilityType = "Cooldown";
        buffTime = 10;
      }

    }
    // End Barrage

    // Invigorate

    public class Invigorate : Ability {
      public Invigorate() {
        name = "Invigorate";
        recastTime = 120;
        animationDelay = 0.8;
        abilityType = "Cooldown";
      }

    }
    // End Invigorate
    // Auto Attack

    public class Autoattack : Ability {
      public Autoattack() {
        name = "Auto Attack";
        recastTime = MainWindow.AADELAY;
        animationDelay = 0;
        abilityType = "AUTOA";
      }
    }
    // End Auto Attack

  }
}
