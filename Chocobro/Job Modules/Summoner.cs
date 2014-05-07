using System;
using System.Windows;

namespace Chocobro {

  public class Summoner : Job {
    // Proc Booleans - Set all proc booleans false initially.

    public Summoner() {
      name = "Summoner";
      classname = "Summoner";
    }
    public override void getStats(MainWindow cs) {
      base.getStats(cs);
      // Define AP and MP conversion.
      AMP = INT;
    }

    public override void rotation() {
      var gcd = calculateGCD();
      execute(ref tpregen);
      //Regen Mana/TP
      //regen();

      //Abilities - execute(ref ability)
      if (MainWindow.time == windblade.endcast && windblade.casting) { impact(ref windblade); windblade.casting = false; }
      if (bioii.debuff > 0 && bio.debuff > 0 && miasma.debuff > 0) {
        execute(ref contagion);
      }
      execute(ref windblade);
      if (MainWindow.time == bioii.endcast && bioii.casting) { impact(ref bioii); bioii.casting = false; }
      if (MainWindow.time == miasma.endcast && miasma.casting) { impact(ref miasma); miasma.casting = false; }
      if (MainWindow.time == bio.endcast && bio.casting) { impact(ref bio); bio.casting = false; }
      if (MainWindow.time == shadowflare.endcast && shadowflare.casting) { impact(ref shadowflare); shadowflare.casting = false; }
      if (MainWindow.time == ruin.endcast && ruin.casting) { impact(ref ruin); ruin.casting = false; }

      if (bioii.debuff < gcd) { execute(ref bioii); }
      if (miasma.debuff < gcd) { execute(ref miasma); }
      if (bio.debuff < gcd) { execute(ref bio); }
      if (bioii.debuff > 0 && bio.debuff > 0 && miasma.debuff > 0) {
        execute(ref fester);
      }
      if (shadowflare.debuff < gcd) { execute(ref shadowflare); }


      execute(ref ruin);

      //Instants - execute(ref ability)
      execute(ref ragingstrikes);
      execute(ref xpotionintelligence);
      

      //Ticks - tick(ref DoTability)
      tick(ref bioii);
      tick(ref miasma);
      tick(ref bio);
      tick(ref shadowflare);
      //AutoAttacks (not for casters!) - execute(ref autoattack)

      //Decrement Buffs - decrement(ref buff)
      decrement(ref ragingstrikes);
      decrement(ref xpotionintelligence);

    }

    public override void execute(ref Ability ability) {
      base.execute(ref ability);
    }
    public override void impact(ref Ability ability) {

      //var critchance = calculateCrit(_player);
      //set potency for now, but change to damage later.
      var accroll = (MainWindow.d100(1, 10001)) / 100;

      if (ability.abilityType == "Cooldown" || ability.abilityType == "PETCOOLDOWN") {
        ability.hits += 1;
        if (ability.name == "Contagion") { bio.debuff += 10; bioii.debuff += 10; miasma.debuff += 10; MainWindow.log(MainWindow.time.ToString("F2") + " - Contagion extended dots."); }
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
          if (ability.potency > 0) { MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + thisdamage + " Damage. Next ability at: " + nextability); }
        } else {
          numberofmisses += 1;
          ability.misses += 1;
          MainWindow.log("!!MISS!! - " + MainWindow.time.ToString("F2") + " - " + ability.name + " missed! Next ability at: " + ability.nextCast);
          // Does heavyshot buff get eaten by a miss?
        }
      }

      if (ability.abilityType == "Spell" || ability.abilityType == "PETSPELL") {
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
          if (ability.potency > 0) { MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + thisdamage + " Damage. Next ability at: " + nextability); }
        } else {
          numberofmisses += 1;
          ability.misses += 1;
          MainWindow.log("!!MISS!! - " + MainWindow.time.ToString("F2") + " - " + ability.name + " missed! Next ability at: " + ability.nextCast);
          // Does heavyshot buff get eaten by a miss?
        }
      }

      // If ability has debuff, create its timer.
      if (ability.debuffTime > 0 && accroll < calculateACC()) {
        if (ability.debuff > 0) {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + "  DOT clipped.");
          //reset all buffs if clipping
          ability.dotbuff["ragingstrikes"] = false;
          ability.dotbuff["potion"] = false;
        }
        //If dot exists and ability doesn't miss, enable its time.

        ability.debuff = ability.debuffTime;

        if (ragingstrikes.buff > 0) { ability.dotbuff["ragingstrikes"] = true; }
        if (xpotionintelligence.buff > 0) { ability.dotbuff["potion"] = true; }



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
      double tempint = INT;
      //potion check
      if (xpotionintelligence.buff > 0 || (dot == true && ability.dotbuff["potion"] == true)) {
        //check for max dex increase from pot - NEEDS to be refactored...

        if (percentageOfStat(xpotionintelligence.percent, tempint) > xpotionintelligence.bonus) {
          //MainWindow.log("yolo: " + percentageOfStat(xpotiondexterity.percent, tempdex) + " tempdex " + tempdex);
          tempint += xpotionintelligence.bonus;
          //MainWindow.log("capBonus Dex from potion: " + xpotiondexterity.bonus + " percent of stat: " + percentageOfStat(xpotiondexterity.percent, tempdex));
        } else {
          tempint += percentageOfStat(xpotionintelligence.percent, tempint);
          //MainWindow.log("smBonus Dex from potion: " + percentageOfStat(xpotiondexterity.percent, tempdex));
        }
      }
      //end potion check
      if (ability.abilityType == "Weaponskill" || ability.abilityType == "Instant" || ability.abilityType == "Spell" || ability.abilityType == "PETSPELL") {
        damageformula = ((double)pot / 100) * (0.005126317 * WEP * tempint + 0.000128872 * WEP * DTR + 0.049531324 * WEP + 0.087226457 * tempint + 0.050720984 * DTR);

      }
      if (ability.abilityType == "AUTOA") {
        damageformula = (AAPOT) * (0.408 * WEP + 0.103262731 * tempint + 0.003029823 * WEP * tempint + 0.003543121 * WEP * (DTR - 202));
      }

      //crit
      double critroll = MainWindow.d100(1, 1000001) / 10000; //critroll was only rolling an interger between 1-101. Now has the same precision as critchance.
      double critchance = 0;
      critchance = 0.0697 * (double)CRIT - 18.437; //Heavyshot interaction
      //MainWindow.log("CRIT CHANCE IS:" + critchance + " ROLL IS: " + critroll);
      if (dot) {
        if (ability.dotbuff["ragingstrikes"]) { damageformula *= 1.20; }

      } else {
        if (ragingstrikes.buff > 0) { damageformula *= 1.20; }
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
      areport.Add(ruin);
      areport.Add(ruinii);
      areport.Add(bio);
      areport.Add(bioii);
      areport.Add(miasma);
      areport.Add(miasmaii);
      areport.Add(shadowflare);
      areport.Add(fester);
      areport.Add(ragingstrikes);
      areport.Add(xpotionintelligence);
      areport.Add(windblade);
      areport.Add(contagion);
      if (MainWindow.selenebuff) {
        areport.Add(feylight);
        areport.Add(feyglow);
      }
    }
    Ability ruin = new Ruin();
    Ability ruinii = new RuinII();
    Ability bio = new Bio();
    Ability bioii = new BioII();
    Ability miasma = new Miasma();
    Ability miasmaii = new MiasmaII();
    Ability shadowflare = new ShadowFlare();
    Ability fester = new Fester();
    Ability ragingstrikes = new RagingStrikes();
    Ability xpotionintelligence = new XPotionIntelligence();
    Ability feylight = new FeyLight();
    Ability feyglow = new FeyGlow();
    Ability tpregen = new TPRegen();
    Ability windblade = new WindBlade();
    Ability contagion = new Contagion();



    // Set array of abilities for reportingz

    // -------------------
    // Ability Definition
    // -------------------

    public class WindBlade : Ability {
      public WindBlade() {
        name = "Wind Blade";
        abilityType = "PETSPELL";
        potency = 105;
        castTime = 2;
        recastTime = 3;
        animationDelay = 0.02;
      }
    }

    public class Contagion : Ability {
      public Contagion() {
        name = "Contagion";
        abilityType = "PETCOOLDOWN";
        recastTime = 60;
        animationDelay = 0.8;
      }
    }

    // Ruin ---------------------
    public class Ruin : Ability {
      public Ruin() {
        name = "Ruin";
        abilityType = "Spell";
        potency = 80;
        MPcost = 79;
        castTime = 2.5;
        recastTime = 2.5;
        animationDelay = 0.8;
      }
    }
    // End Ruin ---------------------

    // Bio ---------------------
    public class Bio : Ability {
      public Bio() {
        name = "Bio";
        abilityType = "Spell";
        potency = 0;
        dotPotency = 40;
        debuffTime = 18;
        MPcost = 106;
        castTime = 2.5;
        recastTime = 2.5;
        animationDelay = 0.8;
      }
    }
    // End Bio ---------------------

    // Miasma ---------------------
    public class Miasma : Ability {
      public Miasma() {
        name = "Miasma";
        abilityType = "Spell";
        potency = 20;
        dotPotency = 35;
        debuffTime = 24;
        MPcost = 133;
        castTime = 2.5;
        recastTime = 2.5;
        animationDelay = 0.8;
      }
    }
    // End Miasma ---------------------

    // Ruin II ---------------------
    public class RuinII : Ability {
      public RuinII() {
        name = "Ruin II";
        abilityType = "Spell";
        potency = 80;
        MPcost = 133;
        recastTime = 2.5;
        animationDelay = 0.8;
      }
    }
    // End Ruin II ---------------------

    // Bio II ---------------------
    public class BioII : Ability {
      public BioII() {
        name = "Bio II";
        abilityType = "Spell";
        potency = 0;
        dotPotency = 35;
        debuffTime = 30;
        MPcost = 186;
        castTime = 2.5;
        recastTime = 2.5;
        animationDelay = 0.8;
      }
    }
    // End Bio II ---------------------

    // Miasma II ---------------------
    public class MiasmaII : Ability {
      public MiasmaII() {
        name = "Miasma II";
        abilityType = "Spell";
        potency = 20;
        dotPotency = 10;
        debuffTime = 15;
        MPcost = 186;
        recastTime = 2.5;
        animationDelay = 0.8;
      }
    }
    // End Miasma II ---------------------

    // Shadow Flare ---------------------
    public class ShadowFlare : Ability {
      public ShadowFlare() {
        name = "Shadow Flare";
        abilityType = "Spell";
        potency = 0;
        dotPotency = 25;
        debuffTime = 30;
        MPcost = 212;
        castTime = 3.0;
        recastTime = 2.5;
        animationDelay = 0.8;
      }
    }
    // End Shadow Flare ---------------------

    // Fester ---------------------
    public class Fester : Ability {
      public Fester() {
        name = "Fester";
        abilityType = "Instant";
        potency = 300;
        recastTime = 10;
        animationDelay = 0.8;
      }
    }
    // End Fester ---------------------

    // Raging Strikes ---------------------
    public class RagingStrikes : Ability {
      public RagingStrikes() {
        name = "Raging Strikes";
        abilityType = "Cooldown";
        recastTime = 180;
        animationDelay = 0.8;
      }
    }
    // End Raging Strikes ---------------------

  }
}
