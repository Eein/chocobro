using System;
using System.Windows;

namespace Chocobro {

  public class Bard : Job {
    // Proc Booleans - Set all proc booleans false initially.
    public bool bloodletterreset = false;
    public int opener = 0;
    public enum Song { None = 0, ArmysPaeon = 1, MagesBallad = 2, FoesRequiem = 3 }
    public Song song;

    public Bard() {
      name = "Bard";
      classname = "Archer";

      heavyshot = new HeavyShot(this);
      windbite = new Windbite(this);
      venomousbite = new VenomousBite(this);
      straightshot = new StraightShot(this);
      bloodletter = new Bloodletter(this);
      miserysend = new MiserysEnd(this);
      bluntarrow = new BluntArrow(this);
      repellingshot = new RepellingShot(this);
      flamingarrow = new FlamingArrow(this);
      internalrelease = new InternalRelease(this);
      bloodforblood = new BloodForBlood(this);
      ragingstrikes = new RagingStrikes(this);
      hawkseye = new HawksEye(this);
      barrage = new Barrage(this);
      invigorate = new Invigorate(this);
      autoattack = new AutoAttack(this);
      armyspaeon = new ArmysPaeon(this);

      xpotiondexterity = new XPotionDexterity(this);
      feylight = new FeyLight();
      feyglow = new FeyGlow();
      regen = new Regen();

    }

    public override void getStats(MainWindow cs) {
      base.getStats(cs);
      // Define AP and MP conversion.
      AP = DEX;
      AMP = INT;
      tpused = 0;
      tpgained = 0;
      song = Song.None;
      MPMax = (1949 + 8 * (PIE - 168));
      MP = MPMax;
    }

    public override void rotation() {
      if (regen.nextCast <= MainWindow.time) { execute(ref regen); }
      if (MainWindow.time == armyspaeon.endcast && armyspaeon.casting) { armyspaeon.casting = false; impact(ref armyspaeon); }
      autoattack.recastTime = AADELAY;

      
      if (MainWindow.selenebuff == true) {
        if (feyglow.buff <= 0 && MainWindow.time >= feylight.nextCast) { execute(ref feylight); }
        if (feylight.buff <= 0 && MainWindow.time >= feyglow.nextCast) { execute(ref feyglow); }
      }

      if (TP <= 540) {
        execute(ref invigorate);
      }

      if (TP >= 540 && song == Song.ArmysPaeon && MP > 0.3 * MPMax && invigorate.nextCast - MainWindow.time < 15 ) { MainWindow.log(MainWindow.time.ToString("F2") + "Armys Paeon off."); song = Song.None; }
      if (ragingstrikes.buff > 0 && song == Song.ArmysPaeon) { MainWindow.log(MainWindow.time.ToString("F2") + "Armys Paeon off."); song = Song.None; }
      if (TP <= 160 && song == Song.None && MainWindow.time < MainWindow.fightlength * 0.9 && ragingstrikes.buff <= 0) { execute(ref armyspaeon); }

      if (heavyshot.buff > 0 && TP > 70) {
        execute(ref straightshot);
      }

      if (straightshot.buff <= 2 && TP > 70) {
        execute(ref straightshot);
      }



      if (windbite.debuff <= (nextregentick - (MainWindow.time + 0.01)) && TP > 80) {
        if (MainWindow.fightlength - MainWindow.time > 9) {
          execute(ref windbite);
        }
      }

      if (venomousbite.debuff <= (nextregentick - (MainWindow.time + 0.01)) && TP > 80) {
        if (MainWindow.fightlength - MainWindow.time > 9) {
          execute(ref venomousbite);
        }
      }
      
      execute(ref heavyshot); 

      if (ragingstrikes.nextCast <= MainWindow.time) { opener = 0; execute(ref ragingstrikes); } //"smart" use of buffs, needs more attention
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
      decrement(ref feyglow);

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

      if (ability.nextCast <= MainWindow.time) {
        if (ability.name == "Regen" && song == Song.ArmysPaeon) {
          if (MP >= 133) {
            tpgained += 30;
            MP -= 133;
            MainWindow.log(MainWindow.time.ToString("F2") + " - Extra TP. " + TP + " => " + (TP + 30) + " TP.  MP is now " + MP + ".");
            TP += 30;
          } else {
            tpgained += 30;
            MP = 0;
            MainWindow.log(MainWindow.time.ToString("F2") + " - Extra TP. " + TP + " => " + (TP + 30) + " TP.  MP is now " + 0 + ". Song is now off.");
            TP += 30;
            song = Song.None;
          }
        }
        base.execute(ref ability);
      }
    }
    public override void impact(ref Ability ability) {

      //var critchance = calculateCrit(_player);
      //if (bard.straightshot.buff > 0) {  critchance += 10; }
      //set potency for now, but change to damage later.
      var accroll = (MainWindow.d100(1, 10001)) / 100;

      if (ability.name == "Army's Paeon") { song = Song.ArmysPaeon; }
      if (ability.name == "Invigorate") {
        var tpbefore = TP;
        TP += 400;
        //tpgained += 400;
        if (TP > 1000) { TP = 1000; tpgained += 1000 - tpbefore; } else { tpgained += 400; }
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
        ability.swings += 1;
        if (accroll <= calculateACC() || hawkseye.buff > 0) {

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
        ability.swings += 1;
        numberofattacks += 1;
        if (accroll < calculateACC()) {
          double thisdamage = damage(ref ability, ability.potency);

          if (MainWindow.disdebuff == true) {
            thisdamage = Math.Floor(thisdamage *= 1.12);
          }
          numberofhits += 1;
          totaldamage += (int)thisdamage;
          autoattack.damage += thisdamage;
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + thisdamage + " Damage. Next AA at: " + ability.nextCast);
        } else {
          autoattack.misses += 1;
          numberofmisses += 1;
          MainWindow.log("!!MISS!! - " + MainWindow.time.ToString("F2") + " - " + ability.name + " missed! Next AA at: " + ability.nextCast);
        }
      }
      if (ability.abilityType == "Spell") {
        ability.hits += 1;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " now activated.");
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
          ability.dotbuff["song"] = false;  
        }
        //If dot exists and ability doesn't miss, enable its time.

        ability.debuff = ability.debuffTime;

        if (ragingstrikes.buff > 0) { ability.dotbuff["ragingstrikes"] = true; }
        if (bloodforblood.buff > 0) { ability.dotbuff["bloodforblood"] = true; }
        if (straightshot.buff > 0) { ability.dotbuff["straightshot"] = true; }
        if (hawkseye.buff > 0) { ability.dotbuff["hawkseye"] = true; }
        if (internalrelease.buff > 0) { ability.dotbuff["internalrelease"] = true; }
        if (xpotiondexterity.buff > 0) { ability.dotbuff["potion"] = true; }
        if (song == Song.ArmysPaeon) { ability.dotbuff["song"] = true; }



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
          ability.dotbuff["song"] = false;
        }
      }
      if ((MainWindow.servertick == 3 && MainWindow.time == MainWindow.servertime) && (ability.debuff > 0 && ability.dotPotency > 0)) {
        numberofticks += 1;
        ability.ticks += 1;
        var tickdmg = damage(ref ability, ability.dotPotency, true);
        ability.dotdamage += tickdmg;
        totaldamage += tickdmg;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " is ticking now for " + tickdmg + "  Damage - Time Left: " + ability.debuff);
        if ((ability.name == "Venomous Bite" || ability.name == "Windbite") && bloodletterreset) {
          bloodletter.nextCast = MainWindow.time;
          MainWindow.log("!!PROC!! - Bloodletter reset!");
          ability.procs += 1;
          bloodletter.procs += 1;
          bloodletterreset = false;
        }
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
        damageformula = ((double)pot / 100) * (WEP * .2714745 + tempdex * .1006032 + (DTR - 202) * .0241327 + WEP * tempdex * .0036167 + WEP * (DTR - 202) * .0010800 - 1);
        
      }
      if (ability.abilityType == "AUTOA") {
        damageformula = (AADELAY / 3.00) * (WEP * .2714745 + tempdex * .1006032 + (DTR - 202) * .0241327 + WEP * tempdex * .0036167 + WEP * (DTR - 202) * .0010800 - 1);
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
        if (ability.dotbuff["song"]) { damageformula *= 0.8; }

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
              bloodletterreset = true;
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
      areport.Add(armyspaeon);
      areport.Add(feylight);
      areport.Add(feyglow);
      areport.Add(regen);
      
    }
    Ability heavyshot;
    Ability windbite;
    Ability venomousbite;
    Ability straightshot;
    Ability bloodletter;
    Ability miserysend;
    Ability bluntarrow;
    Ability repellingshot;
    Ability flamingarrow;
    Ability internalrelease;
    Ability bloodforblood;
    Ability ragingstrikes;
    Ability hawkseye;
    Ability barrage;
    Ability invigorate;
    Ability autoattack;
    Ability xpotiondexterity;
    Ability feylight;
    Ability feyglow;
    Ability regen;
    Ability armyspaeon;


    // Set array of abilities for reportingz

    // Heavy Shot ---------------------

    public class HeavyShot : Ability {
      public HeavyShot(Bard parent) {
        name = "Heavy Shot";
        potency = 150;
        dotPotency = 0;
        TPcost = 60;
        animationDelay = 1.4;
        abilityType = "Weaponskill";
        castTime = 0.0;
        duration = 0.0;
        buffTime = 10;
      }
    }
    // End Heavyshot ---------------------

    // Windbite --------------------------

    public class Windbite : Ability {
      public Windbite(Bard parent) {
        name = "Windbite";
        potency = 60;
        dotPotency = 45;
        TPcost = 80;
        animationDelay = 1.4;
        abilityType = "Weaponskill";
        castTime = 0.0;
        duration = 0.0;
        debuffTime = 18;
      }
    }
    // End Windbite --------------------------

    // Venomous Bite -------------------------

    public class VenomousBite : Ability {
      public VenomousBite(Bard parent) {
        name = "Venomous Bite";
        potency = 100;
        dotPotency = 35;
        TPcost = 80;
        animationDelay = 1.4;
        abilityType = "Weaponskill";
        castTime = 0.0;
        debuffTime = 18;
      }
    }
    // End Venomous Bite ----------------------

    // Straight Shot --------------------------

    public class StraightShot : Ability {
      public StraightShot(Bard parent) {
        name = "Straight Shot";
        potency = 140;
        dotPotency = 0;
        TPcost = 70;
        animationDelay = 1.4;
        abilityType = "Weaponskill";
        castTime = 0.0;
        buffTime = 20;
      }

    }
    // End Straight Shot --------------------------

    // Bloodletter --------------------------------

    public class Bloodletter : Ability {
      public Bloodletter(Bard parent) {
        name = "Bloodletter";
        potency = 150;
        dotPotency = 0;
        recastTime = 15;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Instant";
        castTime = 0.0;
      }

    }
    // End Bloodletter ---------------------------

    // Miserys End -------------------------------

    public class MiserysEnd : Ability {
      public MiserysEnd(Bard parent) {
        name = "Miserys End";
        potency = 190;
        dotPotency = 0;
        recastTime = 12;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Instant";
        castTime = 0.0;
      }

    }
    // End  Miserys End -------------------------------

    // Blunt Arrow ------------------------------------

    public class BluntArrow : Ability {
      public BluntArrow(Bard parent) {
        name = "Blunt Arrow";
        potency = 50;
        dotPotency = 0;
        recastTime = 30;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Instant";
        castTime = 0.0;
      }
    }
    // End Blunt Arrow -------------------------------

    // Repelling Shot ---------------------------------

    public class RepellingShot : Ability {
      public RepellingShot(Bard parent) {
        name = "Repelling Shot";
        potency = 80;
        dotPotency = 0;
        recastTime = 30;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Instant";
        castTime = 0.0;
      }
    }
    // End Repelling Shot ----------------------------

    //Flaming Arrow

    public class FlamingArrow : Ability {
      public FlamingArrow(Bard parent) {
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

    public class InternalRelease : Ability {
      public InternalRelease(Bard parent) {
        name = "Internal Release";
        recastTime = 60;
        animationDelay = 0.8;
        abilityType = "Cooldown";
        buffTime = 15;
      }

    }
    // End Internal Release

    // Blood for Blood

    public class BloodForBlood : Ability {
      public BloodForBlood(Bard parent) {
        name = "Blood for Blood";
        recastTime = 80;
        animationDelay = 0.8;
        abilityType = "Cooldown";
        buffTime = 20;
      }

    }
    // End Blood for Blood

    // Raging Strikes

    public class RagingStrikes : Ability {
      public RagingStrikes(Bard parent) {
        name = "Raging Strikes";
        recastTime = 120;
        animationDelay = 0.8;
        abilityType = "Cooldown";
        buffTime = 20;

      }

    }
    // End Raging Strikes

    // Hawks Eye

    public class HawksEye : Ability {
      public HawksEye(Bard parent) {
        name = "Hawks Eye";
        recastTime = 90;
        animationDelay = 0.8;
        abilityType = "Cooldown";
        buffTime = 20;
      }

    }
    // End Hawks Eye

    // Barrage

    public class Barrage : Ability {
      public Barrage(Bard parent) {
        name = "Barrage";
        recastTime = 90;
        animationDelay = 0.8;
        abilityType = "Cooldown";
        buffTime = 10;
      }

    }
    // End Barrage

    // Invigorate

    public class Invigorate : Ability {
      public Invigorate(Bard parent) {
        name = "Invigorate";
        recastTime = 120;
        animationDelay = 0.8;
        abilityType = "Cooldown";
      }

    }
    // End Invigorate
    // Auto Attack

    public class AutoAttack : Ability {
      public AutoAttack(Bard parent) {
        name = "Auto Attack";
        recastTime = MainWindow.AADELAY;
        animationDelay = 0;
        abilityType = "AUTOA";
      }
    }
    // End Auto Attack

    public class ArmysPaeon : Ability {
      public ArmysPaeon(Bard parent) {
        name = "Army's Paeon";
        recastTime = 2.5;
        castTime = 3.0;
        abilityType = "Spell";
      }
    }
  }
}
