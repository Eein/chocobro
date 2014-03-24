using System;
using System.Windows;

namespace Chocobro {
  public enum Form { OpoOpo = 1, Couerl = 2, Raptor = 3, MNKNone = 4 };

  public class Monk : Job {
    // Proc Booleans - Set all proc booleans false initially.
    public static int glstacks = 0;
    public static bool perfectbalancebuff = false;
    Form form;
    public Monk() {
      name = "Monk";
      classname = "Pugilist";
    }
    public override void getStats(MainWindow cs) {
      base.getStats(cs);
      // Define AP and MP conversion.
      AP = STR; //or STR
      AMP = INT;
      form = Form.MNKNone;
    }

    public override void rotation() {
      if (glstacks > 3) { glstacks = 3; }
      var gcd = calculateGCD();
      autoattack.recastTime = AADELAY * (1 - glstacks * 0.05);

      //Regen Mana/TP
      regen();
      execute(feylight);
      if (perfectbalance.buff > 0) { perfectbalancebuff = true; } else { perfectbalancebuff = false; }

      //Abilities - execute(ability)
      if (perfectbalancebuff == true) {
        if (glstacks < 1) { execute(snappunch); }
        if (glstacks < 2) { execute(demolish); }
        if (glstacks < 3) { execute(snappunch); }
        if (glstacks == 3 && dragonkick.debuff < gcd) { execute(dragonkick); }
        if (glstacks == 3 && twinsnakes.buff < gcd) { execute(twinsnakes); }
        if (glstacks == 3 && touchofdeath.debuff < gcd) { execute(touchofdeath); }
        if (glstacks == 3) { execute(bootshine); }
      }

      if (touchofdeath.debuff < gcd) { execute(touchofdeath); }
      if (fracture.debuff < gcd) { execute(fracture); }

      if (form == Form.Couerl) { if (demolish.debuff < gcd) { execute(demolish); } else { execute(snappunch); } }

      if (form == Form.OpoOpo) { if (dragonkick.debuff < gcd) { execute(dragonkick); } else { execute(bootshine); } }

      if (form == Form.Raptor) { if (twinsnakes.buff < gcd) { execute(twinsnakes); } else { execute(truestrike); } }

      //Buffs/Cooldowns - execute(ability)
      if (TP <= 540) { execute(invigorate); }
      execute(perfectbalance);
      execute(xpotionstrength);
      execute(internalrelease);
      execute(bloodforblood);

      //Instants - execute(ability)
      if (MainWindow.fightlength * 0.8 >= MainWindow.time) { execute(mercystroke); }
      execute(howlingfist);
      execute(steelpeak);

      //Ticks - tick(DoTability)
      tick(touchofdeath);
      tick(demolish);
      tick(dragonkick);
      tick(fracture);
      //AutoAttacks (not for casters!) - execute(autoattack)
      execute(autoattack);
      //Decrement Buffs - decrement(buff)
      decrement(feylight);
      decrement(perfectbalance);
      decrement(xpotionstrength);
      decrement(internalrelease);
      decrement(bloodforblood);
      decrement(twinsnakes);
    }

    public override void execute(Ability ability) {
      base.execute(ability);
    }
    public override void impact(Ability ability) {

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
          //Forms
          if (ability.name == "Bootshine" || ability.name == "Dragon Kick") { form = Form.Raptor; }
          if (ability.name == "True Strike" || ability.name == "Twin Snakes") { form = Form.Couerl; }
          if (ability.name == "Snap Punch" || ability.name == "Demolish") { form = Form.OpoOpo; }

          if (ability.name == "Snap Punch" || ability.name == "Demolish") { glstacks += 1; }

          double thisdamage = damage(ability, ability.potency);

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
          var thisdamage = damage(ability, ability.potency);
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

    public virtual void tick(Ability ability) {
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
        var tickdmg = damage(ability, ability.dotPotency, true);
        ability.dotdamage += tickdmg;
        totaldamage += tickdmg;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " is ticking now for " + tickdmg + "  Damage - Time Left: " + ability.debuff);
        //MainWindow.log("---- " + ability.name + " - Dots - RS: " + ability.dotbuff["ragingstrikes"] + " BFB: " + ability.dotbuff["bloodforblood"] + " SS: " + ability.dotbuff["straightshot"] + " HE: " + ability.dotbuff["hawkseye"] + " IR: " + ability.dotbuff["internalrelease"] + " Potion: " + ability.dotbuff["potion"]);
      }
    }

    public int damage(Ability ability, int pot, bool dot = false) {
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
      if (MainWindow.selenebuff) {
        areport.Add(feylight);
        areport.Add(feyglow);
      }
    }

    Ability bootshine = new Bootshine();
    Ability truestrike = new TrueStrike();
    Ability snappunch = new SnapPunch();
    Ability touchofdeath = new TouchOfDeath();
    Ability twinsnakes = new TwinSnakes();
    Ability demolish = new Demolish();
    Ability dragonkick = new DragonKick();
    Ability internalrelease = new InternalRelease();
    Ability perfectbalance = new PerfectBalance();
    Ability fracture = new Fracture();
    Ability invigorate = new Invigorate();
    Ability mercystroke = new MercyStroke();
    Ability steelpeak = new SteelPeak();
    Ability howlingfist = new HowlingFist();
    Ability bloodforblood = new BloodForBlood();
    Ability autoattack = new Autoattack();
    Ability xpotionstrength = new XPotionStrength();
    Ability feylight = new FeyLight();
    Ability feyglow = new FeyGlow();



    // Set array of abilities for reportingz

    // -------------------
    // Ability Definition
    // -------------------

    //Bootshine
    public class  Bootshine: Ability {
      public Bootshine() {
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
      public TrueStrike() {
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
      public SnapPunch() {
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
      public TouchOfDeath() {
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
      public TwinSnakes() {
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
      public Demolish() {
        name = "Demolish";
        potency = 70;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
        dotPotency = 40;
        debuffTime = 18;
      }
    }
    //End Demolish

    //Dragon Kick
    public class DragonKick : Ability {
      public DragonKick() {
        name = "Dragon Kick";
        potency = 150;
        debuffTime = 15;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
      }
    }
    //End Dragon Kick

    //Internal Release
    public class InternalRelease : Ability {
      public InternalRelease() {
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
      public PerfectBalance() {
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
      public Fracture() {
        name = "Fracture";
        potency = 100;
        debuffTime = 18;
        dotPotency = 20;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
      }
    }
    //End Fracture

    //Invigorate
    public class Invigorate : Ability {
      public Invigorate() {
        name = "Invigorate";
        animationDelay = 0.6;
        recastTime = 120;
        abilityType = "Cooldown";
      }
    }
    //End Invigorate

    //Mercy Stroke
    public class MercyStroke : Ability {
      public MercyStroke() {
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
      public SteelPeak() {
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
      public HowlingFist() {
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
      public BloodForBlood() {
        name = "Fracture";
        recastTime = 80;
        buffTime = 20;
        animationDelay = 0.6;
        abilityType = "Cooldown";
      }
    }
    //End Blood for Blood

    //Auto Attack
    public class Autoattack : Ability {
      public Autoattack() {
        name = "Auto Attack";
        recastTime = (MainWindow.AADELAY - (1 - 0.05 * glstacks));
        animationDelay = 0;
        abilityType = "AUTOA";
      }
    }
    //End Auto Attack

  }
}
