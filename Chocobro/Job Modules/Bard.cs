using System;
using System.Windows;
namespace Chocobro {

  public class Bard : Job {
 
    //-----------------------
    public Bard() {


     
    }
  
    public override void rotation() {
      var gcd = calculateGCD();
      autoattack.recastTime = AADELAY;
      if (heavyshot.buff > 0) {
        if (straightshot.buff < gcd) {
          execute(ref straightshot);
          heavyshot.buff = 0;
        }
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

      if (TP < 540) {
        execute(ref invigorate);
      }

      execute(ref ragingstrikes); //"smart" use of buffs, needs more attention
      if (ragingstrikes.buff > 0) {
        execute(ref hawkseye);
        execute(ref internalrelease);
        execute(ref bloodforblood);
        execute(ref barrage);
      }
      if (ragingstrikes.nextCast >= MainWindow.time + 57 && ragingstrikes.nextCast <= 63) { //better use of internal release
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


      regen();
    }

    public void execute(ref Ability ability) {


      if (ability.abilityType == "AUTOA" && MainWindow.time >= ability.nextCast) {
        MainWindow.time = MainWindow.floored(MainWindow.time);
        MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name);
        ability.nextCast = MainWindow.floored((MainWindow.time + ability.recastTime));
        nextauto = MainWindow.floored((MainWindow.time + ability.recastTime));
        impact(ref ability);
        if (barrage.buff > 0) {  //barrage interactions
          impact(ref ability);
          impact(ref ability);
        }

      }

      if (ability.abilityType == "Weaponskill") {

        //If time >= next cast time and time >= nextability)
        if (TP - ability.TPcost < 0) { //attempted to not allow TP to be less than 0, needs to be remade
          MainWindow.log("Was unable to execute " + ability.name + ". Not enough TP. Current TP is " + TP + "TP.");
          nextability = MainWindow.time;
          OOT = true;
        } else {
          if (MainWindow.time >= ability.nextCast && MainWindow.time >= nextability && actionmade == false) {
            //Get game time (remove decimal error)
            MainWindow.time = MainWindow.floored(MainWindow.time);
            MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name + ". Cost is " + ability.TPcost + "TP. Current TP is " + TP + "TP.");
            // remove TP
            TP -= ability.TPcost;
            //if doesnt miss, then impact

            //set nextCast.
            ability.nextCast = MainWindow.floored((MainWindow.time + calculateGCD()));


            //set nextability
            nextability = MainWindow.floored((MainWindow.time + calculateGCD()));
            nextinstant = MainWindow.floored((MainWindow.time + ability.animationDelay));

            //time = nextTime(nextinstant, nextability);
            actionmade = true;

            //var critroll = d100.Next(1, 101);
            // var critbonus = calculateCrit();
            impact(ref ability);
          }
        }
      }
      if (ability.abilityType == "Instant" || ability.abilityType == "Cooldown") {
        //If time >= next cast time and time >= nextability)
        if (MainWindow.time >= ability.nextCast && MainWindow.time >= nextinstant && nextability > MainWindow.time + ability.animationDelay) { //&& nextability > MainWindow.time + ability.animationDelay is what i added
          //Get game time (remove decimal error)
          MainWindow.time = MainWindow.floored(MainWindow.time);
          MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name);
          //if doesnt miss, then impact

          //set nextCast.
          ability.nextCast = MainWindow.floored((MainWindow.time + ability.recastTime));


          //set nextability
          if (MainWindow.time + ability.animationDelay > nextability) {
            nextability = MainWindow.floored((MainWindow.time + ability.animationDelay));
          }

          nextinstant = MainWindow.floored((MainWindow.time + ability.animationDelay));

          impact(ref ability);
        }
      }




    }
    public virtual void impact(ref Ability ability) {

      //var critchance = calculateCrit(_player);
      //if (bard.straightshot.buff > 0) {  critchance += 10; }
      //set potency for now, but change to damage later.
      var accroll = (MainWindow.d100(1, 10001)) / 100;


      if (ability.name == "Invigorate") {
        TP += 400;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " used. 400 TP Restored. TP: " + TP);
      }
      if (ability.name == "Heavyshot") {
        double buffroll = MainWindow.d100(1, 101);
        if (20 >= buffroll) {
          ability.buff = 10;
          MainWindow.log("!!PROC!! - Heavier Shot. Time Left: " + ability.buff);
        }
      }
      if (ability.abilityType == "Cooldown") {

      }
      if (ability.abilityType == "Weaponskill" || (ability.abilityType == "Instant" && ability.potency > 0)) {
        if (accroll < calculateACC()) {
          numberofhits += 1;
          totaldamage += damage(ref ability, ability.potency);
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + damage(ref ability, ability.potency) + " Damage. Next ability at: " + nextability);
        } else {
          numberofmisses += 1;
          MainWindow.log("!!MISS!! - " + MainWindow.time.ToString("F2") + " - " + ability.name + " missed! Next ability at: " + ability.nextCast); 
        }
      }
      if (ability.abilityType == "AUTOA") {
        if (accroll < calculateACC()) {
          numberofhits += 1;
          totaldamage += damage(ref ability, ability.potency);
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + damage(ref ability, ability.potency) + " Damage. Next AA at: " + ability.nextCast);
        } else {
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
        }
        //If dot exists and ability doesn't miss, enable its time.
      
        ability.debuff = ability.debuffTime;
     
        if (ragingstrikes.buff > 0) { ability.dotbuff["ragingstrikes"] = true; }
        if (bloodforblood.buff > 0) { ability.dotbuff["bloodforblood"] = true; }
        if (straightshot.buff > 0) { ability.dotbuff["straightshot"] = true; }
        if (hawkseye.buff > 0) { ability.dotbuff["hawkseye"] = true; }
        if (internalrelease.buff > 0) { ability.dotbuff["internalrelease"] = true; }


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
        }
      }
      if ((MainWindow.servertick == 3 && MainWindow.time == MainWindow.servertime) && ability.debuff > 0) {
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " is ticking now for " + damage(ref ability, ability.dotPotency, true) + "  Damage - Time Left: " + ability.debuff);
        //MainWindow.log("---- " + ability.name + " - Dots - RS: " + ability.dotbuff["ragingstrikes"] + " BFB: " + ability.dotbuff["bloodforblood"] + " SS: " + ability.dotbuff["straightshot"] + " HE: " + ability.dotbuff["hawkseye"] + " IR: " + ability.dotbuff["internalrelease"]);
      }
    }

    public virtual void decrement(ref Ability ability) {

      if (MainWindow.time == MainWindow.servertime && ability.buff > 0) {
        ability.buff -= 1.0;
        if (ability.buff <= 0.0) {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " has fallen off.");
        }
      }
    }

    public int damage(ref Ability ability, int pot, bool dot = false) {
      double damageformula = 0.0;
      double tempdex = DEX;
      if (hawkseye.buff > 0) { tempdex *= 1.15; }
      if (ability.abilityType == "Weaponskill" || ability.abilityType == "Instant") {
        damageformula = ((double)pot / 100) * (0.005126317 * WEP * tempdex + 0.000128872 * WEP * DTR + 0.049531324 * WEP + 0.087226457 * tempdex + 0.050720984 * DTR);

      }
      if (ability.abilityType == "AUTOA") {
        damageformula = (AAPOT) * (0.408 * WEP + 0.103262731 * tempdex + 0.003029823 * WEP * tempdex + 0.003543121 * WEP * (DTR - 202));
      }

      //crit
      double critroll = MainWindow.d100(1, 1000001) / 10000; //critroll was only rolling an interger between 1-101. Now has the same precision as critchance.
      var critchance = 0.0697 * (double)CRIT - 18.437;
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
          //Bloodletter procs        
          if (bloodletter.nextCast > MainWindow.time && ((ability.name == "Windbite" && windbite.debuff > 0) || (ability.name == "Venomous Bite" && venomousbite.debuff > 0))) {
            var dotRoll = MainWindow.d100(1, 101);
            if (dotRoll >= 50) {
              bloodletter.nextCast = MainWindow.time;
              MainWindow.log("!!PROC!! - Bloodletter reset!");
            }
          }
        }
      } else {
        if (dot) {
          numberofhits += 1;
        }
      }

      // added variance to damage.
      damageformula = ((MainWindow.d100(-500, 500) / 10000) + 1) * (int)damageformula;
      return (int)damageformula;
    }



    // -------------------
    // Ability Definition
    // -------------------

    // Heavy Shot ---------------------

    Ability heavyshot = new Heavyshot();

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
    Ability windbite = new Windbite();
    public class Windbite : Ability {
      public Windbite() {
        name = "Windbite";
        potency = 60;
        dotPotency = 45;

        TPcost = 80;
        animationDelay = 1.3;
        abilityType = "Weaponskill";
        castTime = 0.0;
        duration = 0.0;
        debuffTime = 18;
      }
    }
    // End Windbite --------------------------

    // Venomous Bite -------------------------
    Ability venomousbite = new Venomousbite();
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
    Ability straightshot = new Straightshot();
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
    Ability bloodletter = new Bloodletter();
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
    Ability miserysend = new Miserysend();
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
    Ability bluntarrow = new Bluntarrow();
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
    Ability repellingshot = new Repellingshot();
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
    Ability flamingarrow = new Flamingarrow();
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
    Ability internalrelease = new Internalrelease();
    public class Internalrelease : Ability {
      public Internalrelease() {
        name = "Internal Release";
        recastTime = 60;
        animationDelay = 0.75;
        abilityType = "Cooldown";
        buffTime = 15;
      }

    }
    // End Internal Release

    // Blood for Blood
    Ability bloodforblood = new Bloodforblood();
    public class Bloodforblood : Ability {
      public Bloodforblood() {
        name = "Blood for Blood";
        recastTime = 80;
        animationDelay = 0.9;
        abilityType = "Cooldown";
        buffTime = 20;
      }

    }
    // End Blood for Blood

    // Raging Strikes
    Ability ragingstrikes = new Ragingstrikes();
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
    Ability hawkseye = new Hawkseye();
    public class Hawkseye : Ability {
      public Hawkseye() {
        name = "Hawks Eye";
        recastTime = 90;
        animationDelay = 0.7;
        abilityType = "Cooldown";
        buffTime = 20;
      }

    }
    // End Hawks Eye

    // Barrage
    Ability barrage = new Barrage();
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
    Ability invigorate = new Invigorate();
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
    Ability autoattack = new Autoattack();
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
