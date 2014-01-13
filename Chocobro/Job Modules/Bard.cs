
namespace Chocobro {

  public class Bard : Job {
    public static double AADELAY = 3.28;
    //-----------------------
    
    public Bard() {

      //Temporary Initiation of stats. Need to rip these from the Sim GUI in JOB.
      WEP = 41;
      AADMG = 38.26;
      
      STR = 161;
      DEX = 224;
      VIT = 202;
      INT = 151;
      MND = 141;
      PIE = 151;
      //Phyre Xia
      DEX = 491;
      DTR = 305;
      CRIT = 538;
      
      //--aapot
      AAPOT = AADMG / System.Convert.ToDouble(WEP);

    }
    public override void rotation() {
      var gcd = calculateGCD();

      if (heavyshot.buff > 0) {
        if (straightshot.buff < 10) {
          execute(ref straightshot);
          heavyshot.buff = 0;
        }
      }
      if (straightshot.buff <= 0) {
        execute(ref straightshot);
      }

      if (windbite.debuff <= gcd) {
        execute(ref windbite);
      }

      if (venomousbite.debuff <= gcd) {
        execute(ref venomousbite);
      }

      execute(ref heavyshot);

      if (TP < 540) {
        execute(ref invigorate);
      }

      execute(ref ragingstrikes);
      if (ragingstrikes.buff > 0) {
        execute(ref hawkseye);
        execute(ref bloodforblood);
        execute(ref internalrelease);
        execute(ref barrage);
      }
      if (MainWindow.servertime > 0.8 * MainWindow.fightlength) {
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
        //Get game time (remove decimal error)
        MainWindow.time = MainWindow.floored(MainWindow.time);
        MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name);
        ability.nextCast = MainWindow.floored((MainWindow.time + ability.recastTime));
        nextauto = MainWindow.floored((MainWindow.time + ability.recastTime));
        impact(ref ability);
      }
      if (ability.abilityType == "Weaponskill") {

        //If time >= next cast time and time >= nextability)
        if (MainWindow.time >= ability.nextCast && MainWindow.time >= nextability && actionmade == false) {
          //Get game time (remove decimal error)
          MainWindow.time = MainWindow.floored(MainWindow.time);
          MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name);
          // remove TP
          TP -= ability.TPcost;
          MainWindow.log("Cost is " + ability.TPcost + "TP. Current TP: " + TP); //test for tp
          //if doesnt miss, then impact

          //set nextCast.
          ability.nextCast = MainWindow.floored((MainWindow.time + ability.recastTime));


          //set nextability
          nextability = MainWindow.floored((MainWindow.time + gcd));
          nextinstant = MainWindow.floored((MainWindow.time + ability.animationDelay));

          //time = nextTime(nextinstant, nextability);
          actionmade = true;

          //var critroll = d100.Next(1, 101);
          // var critbonus = calculateCrit();
          impact(ref ability);
        }
      }
      if (ability.abilityType == "Instant" || ability.abilityType == "Cooldown") {
        //If time >= next cast time and time >= nextability)
        if (MainWindow.time >= ability.nextCast && MainWindow.time >= nextinstant) {
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


      if (ability.name == "Invigorate") {
        TP += 400;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " used. 400 TP Restored. TP: " + TP);
      }
      if (ability.name == "Heavyshot") {
        int minirand = MainWindow.d100();
        if (20 >= minirand) {
          ability.buff = 10;
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " has procced.  Time Left: " + ability.buff + " - Rolled a " + minirand);
        }
      }
      if (ability.abilityType == "Cooldown") {

      }
      if (ability.abilityType == "Weaponskill") {
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + damage(ref ability, ability.potency) + " Damage. Next ability at: " + nextability);

      }
      if (ability.abilityType == "AUTOA") {

        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + damage(ref ability, ability.potency) + " Damage. Next AA at: " + ability.nextCast);

      }

      // If ability has debuff, create its timer.
      if (ability.debuffTime > 0) {

        //If dot exists, enable its time.
        ability.debuff = ability.debuffTime;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " DoT has been applied.  Time Left: " + ability.debuff);
      }
      if (ability.buffTime > 0) {
        ability.buff = ability.buffTime;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " buff has been activated.  Time Left: " + ability.buff + ". Next ability at: " + nextability);
      }


    }

    //public virtual void expire() { } not really needed. Maybe handle expiration in ticks? hmmm.

    public virtual void tick(ref Ability ability) {
      //schedule tick
      if (MainWindow.time == MainWindow.servertime && ability.debuff != 0.0) {
        ability.debuff -= 1.0;
        if (ability.debuff <= 0.0) {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " has fallen off.");
        }
      }
      if ((MainWindow.servertick == 3 && MainWindow.time == MainWindow.servertime) && ability.debuff > 0) {
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " is ticking now for " + damage(ref ability, ability.dotPotency) + "  Damage - Time Left: " + ability.debuff);
      }
    }

    public virtual void decrement(ref Ability ability) {

      if (MainWindow.time == MainWindow.servertime && ability.buff != 0.0) {
        ability.buff -= 1.0;
        if (ability.buff <= 0.0) {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " has fallen off.");
        }
      }
    }

    public int damage(ref Ability ability, int pot) {
      double damageformula = 0.0;
      double tempdex = DEX;
      //if (hawkseye.buff > 0) { tempdex *= 1.15; }
      if (ability.abilityType != "AUTOA") {
        damageformula = ((double)pot / 100) * (0.005126317 * WEP * tempdex + 0.000128872 * WEP * DTR + 0.049531324 * WEP + 0.087226457 * tempdex + 0.050720984 * DTR);
 
      } else {
        damageformula = (AAPOT) * (0.408 * WEP + 0.103262731 * tempdex + 0.003029823 * WEP * tempdex + 0.003543121 * WEP * (DTR - 202));
      }
      if (ragingstrikes.buff > 0 && ability.name != "Flaming Arrow") { damageformula *= 1.20; }
      if (bloodforblood.buff > 0) { damageformula *= 1.10; }


      //crit
      var critroll = MainWindow.d100();
      var critchance = 0.0697 * (double)CRIT - 18.437;
      //MainWindow.log("CRIT CHANCE IS:" + critchance + " ROLL IS: " + critroll);

      //Bloodletter procs
      if (ability.name == "Windbite" || ability.name == "Venomous Bite" && critroll <= critchance) {
        var dotRoll = MainWindow.d100();
        if (dotRoll >= 50) {
          if (bloodletter.nextCast > MainWindow.time) {
            bloodletter.nextCast = MainWindow.time;
           MainWindow.log("Bloodletter reset!!");
          }
        }
      }
      if (straightshot.buff > 0) { critchance *= 1.10; }
      if (internalrelease.buff > 0) { critchance *= 1.30; }

      if (critroll <= critchance) {
        MainWindow.log("!!CRIT!! - ", false);
        damageformula *= 1.5;
      }

      // add variance to damage.
      //damageformula = (int)damageformula;
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
        recastTime = 2.5;
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
        recastTime = 2.5;
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
        recastTime = 2.5;
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
        recastTime = 2.5;
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
        recastTime = AADELAY;
        animationDelay = 0;
        abilityType = "AUTOA";
      }

    }
    // End Invigorate
  }
}
