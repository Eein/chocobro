
namespace Chocobro {

  public class Bard : Job {

    //-----------------------
    // Make sure Flaming arrow isn't effected by Blood for Blood....
    public Bard() {
    
      //Temporary Initiation of stats. Need to rip these from the Sim GUI.
      WEP = 41;
      //NEED AADAM
      //NEED AAPOT.
      STR = 161;
      DEX = 224;
      VIT = 202;
      INT = 151;
      MND = 141;
      PIE = 151;
    }
    public override void rotation() {
      var gcd = calculateGCD();

      if (heavyshot.buff > 0) {
        execute(ref straightshot);
        heavyshot.buff = 0;
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

      if (MainWindow.TP < 540) {
        execute(ref invigorate);
      }

      execute(ref ragingstrikes);
      execute(ref hawkseye);
      execute(ref bloodforblood);
      execute(ref internalrelease);
      execute(ref barrage);
      execute(ref miserysend);
      execute(ref bloodletter);
      execute(ref flamingarrow);
      execute(ref repellingshot);
      execute(ref bluntarrow);

      //server actionable - ticks/decrements then server tick action
      //if tick is 3
      tick(ref windbite);
      tick(ref venomousbite);
      tick(ref flamingarrow);
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
      if (ability.abilityType == "Weaponskill") {

        //If time >= next cast time and time >= nextability)
        if (MainWindow.time >= ability.nextCast && MainWindow.time >= MainWindow.nextability && MainWindow.actionmade == false) {
          MainWindow.time = MainWindow.floored(MainWindow.time);
          string executestring = MainWindow.time.ToString("F2") + " - Executing " + ability.name;
          MainWindow.log(executestring);
          // remove TP
          MainWindow.TP -= ability.TPcost;
          MainWindow.log("Cost is " + ability.TPcost + "TP. Current TP: " + MainWindow.TP); //test for tp
          //if doesnt miss, then impact

          //set nextCast.
          ability.nextCast = MainWindow.floored((MainWindow.time + ability.recastTime));


          //set nextability
          MainWindow.nextability = MainWindow.floored((MainWindow.time + MainWindow.gcd));
          MainWindow.nextinstant = MainWindow.floored((MainWindow.time + ability.animationDelay));

          //time = nextTime(nextinstant, nextability);
          MainWindow.actionmade = true;

          //var critroll = d100.Next(1, 101);
          // var critbonus = calculateCrit();
          impact(ref ability);
        }
      }
      if (ability.abilityType == "Instant" || ability.abilityType == "Cooldown") {
        //If time >= next cast time and time >= nextability)
        if (MainWindow.time >= ability.nextCast && MainWindow.time >= MainWindow.nextinstant) {
          MainWindow.time = MainWindow.floored(MainWindow.time);
          string executestring = MainWindow.time.ToString("F2") + " - Executing " + ability.name;
          MainWindow.log(executestring);
          //if doesnt miss, then impact

          //set nextCast.
          ability.nextCast = MainWindow.floored((MainWindow.time + ability.recastTime));


          //set nextability
          if (MainWindow.time + ability.animationDelay > MainWindow.nextability) {
            MainWindow.nextability = MainWindow.floored((MainWindow.time + ability.animationDelay));
          }

          MainWindow.nextinstant = MainWindow.floored((MainWindow.time + ability.animationDelay));

          impact(ref ability);
        }
      }


    }
    public virtual void impact(ref Ability ability) {
      //var critchance = calculateCrit(_player);
      //if (bard.straightshot.buff > 0) {  critchance += 10; }
      //set potency for now, but change to damage later.


      if (ability.name == "Invigorate") {
        MainWindow.TP += 400;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " used. 400 TP Restored. TP: " + MainWindow.TP);
      }
      if (ability.name == "Heavyshot") {
        int minirand = MainWindow.d100();
        if (20 >= minirand) {
          ability.buff = 10;
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " has procced.  Time Left: " + ability.buff + " - Rolled a " + minirand);
        }
      }
      if (ability.abilityType == "Cooldown") {
       
      } else {
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + damage(ref ability) + " Potency Damage. Next ability at: " + MainWindow.nextability);
      }

      // If ability has debuff, create its timer.
      if (ability.debuffTime > 0) {

        //If dot exists, enable its time.
        ability.debuff = ability.debuffTime;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " DoT has been applied.  Time Left: " + ability.debuff);
      }
      if (ability.buffTime > 0) {
        ability.buff = ability.buffTime;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " buff has been activated.  Time Left: " + ability.buff + ". Next ability at: " + MainWindow.nextability);
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
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " is ticking now for " + ability.dotPotency + " Potency Damage - Time Left: " + ability.debuff);
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

    public int damage(ref Ability ability) {
      var damageformula = 0.0;
      var tempdex = (double)DEX;
      if (hawkseye.buff > 0){ tempdex *= 1.15; }
      if (ability.autoa == false){
        damageformula = (ability.potency) * 0.01037485 * WEP + 0.080343406 * tempdex + 0.026212395 * WEP + 0.003889894 * WEP * tempdex + 0.000800141 * WEP * DTR;

      }else{
        //Autoattack damage.
      }
      if (ragingstrikes.buff > 0 && ability.name != "Flaming Arrow") { damageformula *= 1.20; }
      if (bloodforblood.buff > 0) { damageformula *= 1.10; }
      if (ragingstrikes.buff > 0) { damageformula *= 1.20; }
      if (ragingstrikes.buff > 0) { damageformula *= 1.20; }
      if (ragingstrikes.buff > 0) { damageformula *= 1.20; }
      if (ragingstrikes.buff > 0) { damageformula *= 1.20; }
      MainWindow.log("!! DEALING " + damageformula + " DAMAGE!");
      damageformula = (int)damageformula;
      MainWindow.log("!! TRUNCATED " + damageformula + " DAMAGE!");
      return (int)damageformula;
        
        
      // int formulaToInt = (int) damageformula;
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
  }
}
