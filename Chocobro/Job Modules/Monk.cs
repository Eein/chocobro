﻿using System;
using System.Windows;

namespace Chocobro {
  public enum Form { OpoOpo = 1, Couerl = 2, Raptor = 3, MNKNone = 4 };

  public class Monk : Job {
    // Proc Booleans - Set all proc booleans false initially.
    public int glstacks;
    public bool perfectbalancebuff;
    Form form;
    public Monk() {
      name = "Monk";
      classname = "Pugilist";
      glstacks = 0;
      perfectbalancebuff = false;

      bootshine = new Bootshine(this);
      truestrike = new TrueStrike(this);
      snappunch = new SnapPunch(this);
      touchofdeath = new TouchOfDeath(this);
      twinsnakes = new TwinSnakes(this);
      demolish = new Demolish(this);
      dragonkick = new DragonKick(this);
      internalrelease = new InternalRelease(this);
      perfectbalance = new PerfectBalance(this);
      fracture = new Fracture(this);
      invigorate = new Invigorate(this);
      mercystroke = new MercyStroke(this);
      steelpeak = new SteelPeak(this);
      howlingfist = new HowlingFist(this);
      bloodforblood = new BloodForBlood(this);
      autoattack = new Autoattack(this);
      xpotionstrength = new XPotionStrength(this);

      feylight = new FeyLight();
      feyglow = new FeyGlow();
      tpregen = new Regen();
    }
    public override void getStats(MainWindow cs) {
      base.getStats(cs);
      // Define AP and MP conversion.
      AP = STR; //or STR
      AMP = INT;
      form = Form.MNKNone;
      MPMax = (1949 + 8 * (PIE - 168));
      MP = MPMax;
    }

    public override void rotation() {
      if (glstacks > 3) { glstacks = 3; }
      execute(ref tpregen);
      autoattack.recastTime = AADELAY;


      if (MainWindow.selenebuff == true) {
        if (fglow == false) { execute(ref feylight); }
        if (flight == false) { execute(ref feyglow); }
      }

      //Regen Mana/TP
      if (perfectbalance.buff > 0) { perfectbalancebuff = true; } else { perfectbalancebuff = false; }

      //Abilities - execute(ref ability)
      if (perfectbalancebuff == true) {
        if (glstacks < 3) { execute(ref snappunch); }
        if (glstacks < 2) { execute(ref demolish); }
        if (glstacks < 1) { execute(ref snappunch); }

        if (glstacks == 3 && dragonkick.debuff < calculateGCD()) { execute(ref dragonkick); }
        if (glstacks == 3 && twinsnakes.buff < calculateGCD()) { execute(ref twinsnakes); }
        if (glstacks == 3 && touchofdeath.debuff < calculateGCD()) { execute(ref touchofdeath); }
        if (glstacks == 3) { execute(ref bootshine); }
      }

      if (touchofdeath.debuff < calculateGCD()) { execute(ref touchofdeath); }
      if (fracture.debuff < calculateGCD()) { execute(ref fracture); }

      if (form == Form.Couerl) { if (demolish.debuff < calculateGCD()) { execute(ref demolish); } else { execute(ref snappunch); } }

      if (form == Form.OpoOpo) { if (dragonkick.debuff < calculateGCD()) { execute(ref dragonkick); } else { execute(ref bootshine); } }

      if (form == Form.Raptor) { if (twinsnakes.buff < calculateGCD()) { execute(ref twinsnakes); } else { execute(ref truestrike); } }

      //Buffs/Cooldowns - execute(ref ability)
      if (TP <= 540) { execute(ref invigorate); }
      execute(ref perfectbalance);
      execute(ref xpotionstrength);
      execute(ref internalrelease);
      execute(ref bloodforblood);

      //Instants - execute(ref ability)
      if (MainWindow.fightlength * 0.8 >= MainWindow.time) { execute(ref mercystroke); }
      execute(ref howlingfist);
      execute(ref steelpeak);

      //Ticks - tick(ref DoTability)
      tick(ref touchofdeath);
      tick(ref demolish);
      tick(ref dragonkick);
      tick(ref fracture);
      //AutoAttacks (not for casters!) - execute(ref autoattack)
      execute(ref autoattack);
      //Decrement Buffs - decrement(ref buff)
      decrement(ref feylight);
      decrement(ref perfectbalance);
      decrement(ref xpotionstrength);
      decrement(ref internalrelease);
      decrement(ref bloodforblood);
      decrement(ref twinsnakes);
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
        ability.swings += 1;
        if (accroll < calculateACC()) {
          //Forms
          if (ability.name == "Bootshine" || ability.name == "Dragon Kick") { form = Form.Raptor; }
          if (ability.name == "True Strike" || ability.name == "Twin Snakes") { form = Form.Couerl; }
          if (ability.name == "Snap Punch" || ability.name == "Demolish") { form = Form.OpoOpo; }
          if (ability.name == "Snap Punch" || ability.name == "Demolish") { glstacks += 1; if (glstacks > 3) { glstacks = 3; } }

          double thisdamage = damage(ref ability, ability.potency);

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
        numberofattacks += 1;
        ability.swings += 1;
        if (accroll < calculateACC()) {
          var thisdamage = damage(ref ability, ability.potency);
          numberofhits += 1;
          ability.hits += 1;
          totaldamage += thisdamage;
          ability.damage += thisdamage;
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
          ability.dotbuff["internalrelease"] = false;
          ability.dotbuff["bloodforblood"] = false;
          ability.dotbuff["potion"] = false;
        }
        //If dot exists and ability doesn't miss, enable its time.

        ability.debuff = ability.debuffTime;

        if (internalrelease.buff > 0) { ability.dotbuff["internalrelease"] = true; }
        if (bloodforblood.buff > 0) { ability.dotbuff["bloodforblood"] = true; }
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
          ability.dotbuff["internalrelease"] = false;
          ability.dotbuff["bloodforblood"] = false;
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
        if (dragonkick.debuff > 0) { damageformula *= 1.12; }
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
        if (ability.dotbuff["bloodforblood"]) { damageformula *= 1.20; }
        if (ability.dotbuff["internalrelease"]) { critchance += 10; }

      } else {
        if (bloodforblood.buff > 0) { damageformula *= 1.20; }
        if (internalrelease.buff > 0) { critchance += 30; }
      }

      if (critroll <= critchance || ability.name == "Bootshine") {
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

      //Fists of Fire
      damageformula *= 1.05;
      //Greased Lightning
      damageformula *= 1 + (0.09 * glstacks);
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
      areport.Add(bootshine);
      areport.Add(truestrike);
      areport.Add(snappunch);
      areport.Add(touchofdeath);
      areport.Add(twinsnakes);
      areport.Add(demolish);
      areport.Add(dragonkick);
      areport.Add(internalrelease);
      areport.Add(perfectbalance);
      areport.Add(fracture);
      areport.Add(invigorate);
      areport.Add(mercystroke);
      areport.Add(steelpeak);
      areport.Add(howlingfist);
      areport.Add(bloodforblood);
      areport.Add(autoattack);
      areport.Add(xpotionstrength);
      areport.Add(tpregen);
      if (MainWindow.selenebuff) {
        areport.Add(feylight);
        areport.Add(feyglow);
      }
    }

    Ability bootshine;
    Ability truestrike;
    Ability snappunch;
    Ability touchofdeath;
    Ability twinsnakes;
    Ability demolish;
    Ability dragonkick;
    Ability internalrelease;
    Ability perfectbalance;
    Ability fracture;
    Ability invigorate;
    Ability mercystroke;
    Ability steelpeak;
    Ability howlingfist;
    Ability bloodforblood;
    Ability autoattack;
    Ability xpotionstrength;
    Ability feylight;
    Ability feyglow;
    Ability tpregen;



    // Set array of abilities for reportingz

    // -------------------
    // Ability Definition
    // -------------------

    //Bootshine
    public class  Bootshine: Ability {
      public Bootshine(Monk parent) {
        name = "Bootshine";
        potency = 150;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
        TPcost = 60;
      }
    }
    //End Bootshine

    //True Strike
    public class TrueStrike : Ability {
      public TrueStrike(Monk parent) {
        name = "True Strike";
        potency = 190;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
        TPcost = 50;
      }
    }
    //End True Strike

    //Snap Punch
    public class SnapPunch : Ability {
      public SnapPunch(Monk parent) {
        name = "Snap Punch";
        potency = 180;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
        TPcost = 50;
      }
    }
    //End Snap Punch

    //Touch of Death
    public class TouchOfDeath : Ability {
      public TouchOfDeath(Monk parent) {
        name = "Touch of Death";
        potency = 20;
        dotPotency = 25;
        debuffTime = 30;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
        TPcost = 80;
      }
    }
    //End Touch of Death

    //Twin Snakes
    public class TwinSnakes : Ability {
      public TwinSnakes(Monk parent) {
        name = "Twin Snakes";
        potency = 140;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
        buffTime = 15;
        TPcost = 60;
      }
    }
    //End Twin Snakes

    //Demolish
    public class Demolish : Ability {
      public Demolish(Monk parent) {
        name = "Demolish";
        potency = 70;
        TPcost = 50;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
        dotPotency = 40;
        debuffTime = 18;
      }
    }
    //End Demolish

    //Dragon Kick
    public class DragonKick : Ability {
      public DragonKick(Monk parent) {
        name = "Dragon Kick";
        potency = 150;
        debuffTime = 15;
        TPcost = 60;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
      }
    }
    //End Dragon Kick

    //Internal Release
    public class InternalRelease : Ability {
      public InternalRelease(Monk parent) {
        name = "Internal Release";
        buffTime = 15;
        recastTime = 60;
        animationDelay = 0.6;
        abilityType = "Cooldown";
      }
    }
    //End Internal Release
 
    //Perfect Balance
    public class PerfectBalance : Ability {
      public PerfectBalance(Monk parent) {
        name = "Perfect Balance";
        animationDelay = 0.6;
        buffTime = 10;
        recastTime = 180;
        abilityType = "Cooldown";
      }
    }
    //End Perfect Balance

    //Fracture
    public class Fracture : Ability {
      public Fracture(Monk parent) {
        name = "Fracture";
        potency = 100;
        TPcost = 80;
        debuffTime = 18;
        dotPotency = 20;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
      }
    }
    //End Fracture

    //Invigorate
    public class Invigorate : Ability {
      public Invigorate(Monk parent) {
        name = "Invigorate";
        animationDelay = 0.6;
        recastTime = 120;
        abilityType = "Cooldown";
      }
    }
    //End Invigorate

    //Mercy Stroke
    public class MercyStroke : Ability {
      public MercyStroke(Monk parent) {
        name = "Mercy Stroke";
        potency = 200;
        recastTime = 90;
        animationDelay = 0.6;
        abilityType = "Instant";
      }
    }
    //End Mercy Stroke

    //Steel Peak
    public class SteelPeak : Ability {
      public SteelPeak(Monk parent) {
        name = "Steel Peek";
        potency = 150;
        recastTime = 40;
        animationDelay = 0.6;
        abilityType = "Instant";
      }
    }
    //End Steel Peek

    //Howling Fist
    public class HowlingFist : Ability {
      public HowlingFist(Monk parent) {
        name = "Howling Fist";
        potency = 170;
        recastTime = 60;
        animationDelay = 0.6;
        abilityType = "Instant";
      }
    }
    //End Howling Fist

    //Blood for Blood
    public class BloodForBlood : Ability {
      public BloodForBlood(Monk parent) {
        name = "Fracture";
        recastTime = 80;
        buffTime = 20;
        animationDelay = 0.6;
        abilityType = "Cooldown";
      }
    }
    //End Blood for Blood

    //Auto Attack
     class Autoattack : Ability {
       public Autoattack(Monk parent) {
        name = "Auto Attack";
        recastTime = (MainWindow.AADELAY - (1 - 0.05 * parent.glstacks));
        animationDelay = 0;
        abilityType = "AUTOA";
      }
    }
    //End Auto Attack

  }
}
