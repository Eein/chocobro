
namespace Chocobro {

  public class Paladin : Job {
    public static double AADELAY = 2.16;
    public bool fastbladecombo = false;
    public bool savagebladecombo = false;

    //-----------------------

    public Paladin() {

      //Temporary Initiation of stats. Need to rip these from the Sim GUI in JOB.
      STR = 161;
      DEX = 224;
      VIT = 202;
      INT = 151;
      MND = 141;
      PIE = 151;

      //Phyre Xia
      WEP = 47;
      AADMG = 33.84;
      DEX = 491;
      STR = 450;
      DTR = 305;
      CRIT = 538;
      SKS = 432;
      ACC = 472;

      //--aapot
      AAPOT = 0.5 + (AADMG / System.Convert.ToDouble(WEP));

    }
    public override void rotation() {
      var gcd = calculateGCD();

      /*if (swordoath.buff <= 0) { //swordoath?
        execute(ref swordoath);
      }*/

      //Abilities - execute(ref ability)

      if (fracture.debuff <= gcd) {
        if (MainWindow.fightlength - MainWindow.time > 9) {
          execute(ref fracture);
        }
      }
      if (savagebladecombo == true) {
        execute(ref rageofhalone);
      }
      if (fastbladecombo == true) {
        execute(ref savageblade);
      }
      execute(ref fastblade);
      //Buffs/Cooldowns - execute(ref ability)
      execute(ref fightorflight);
      //Instants - execute(ref ability)
      execute(ref spiritswithin);
      if (MainWindow.fightlength * 0.80 < MainWindow.time) {
        execute(ref mercystroke);
      }
      //Ticks - tick(ref DoTability)
      tick(ref fracture);
      //AutoAttacks (not for casters!) - execute(ref autoattack)
      execute(ref autoattack);
      //Decrement Buffs - decrement(ref buff)
      decrement(ref swordoath);
      decrement(ref fightorflight);
      //Regen Mana/TP
      regen();
    }

    public void execute(ref Ability ability) {


      if (ability.abilityType == "AUTOA" && MainWindow.time >= ability.nextCast) {
        MainWindow.time = MainWindow.floored(MainWindow.time);
        MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name);
        ability.nextCast = MainWindow.floored((MainWindow.time + ability.recastTime));
        nextauto = MainWindow.floored((MainWindow.time + ability.recastTime));
        impact(ref ability);

      }

      if (ability.abilityType == "Weaponskill") {

        //If time >= next cast time and time >= nextability)
        if (TP - ability.TPcost < 0) { //attempted to not allow TP to be less than 0, needs to be remade
          MainWindow.log("Was unable to execute " + ability.name + ". Not enough TP. Current TP: " + TP);
          nextability = MainWindow.time;
          OOT = true;
        } else {
          if (MainWindow.time >= ability.nextCast && MainWindow.time >= nextability && actionmade == false) {
            //Get game time (remove decimal error)
            MainWindow.time = MainWindow.floored(MainWindow.time);
            MainWindow.log(MainWindow.time.ToString("F2") + " - Executing " + ability.name);
            // remove TP
            if (ability.TPcost > 0) {
              TP -= ability.TPcost;
              MainWindow.log("Cost is " + ability.TPcost + "TP. Current TP: " + TP); //test for tp
            }
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
      var accroll = (MainWindow.d100(1, 10001)) / 100;
      if (ability.name == "Fast Blade") {
        fastbladecombo = true;
        savagebladecombo = false;
      }
      if (ability.name == "Savage Blade") {
        if (fastbladecombo == true) {
          savageblade.potency = savageblade.combopotency;
        }
        savagebladecombo = true;
        fastbladecombo = false;
      }
      if (ability.name == "Rage of Halone") {
        if (savagebladecombo == true) {
          rageofhalone.potency = rageofhalone.combopotency;
        }
        savagebladecombo = false;
        fastbladecombo = false;
      }
      if (ability.abilityType == "Cooldown") {
      }

      if (ability.name != "Sword Oath" && (ability.abilityType == "Weaponskill" || (ability.abilityType == "Instant" && ability.potency > 0))) {
        if (accroll < calculateACC()) {
          numberofhits += 1;
          totaldamage += damage(ref ability, ability.potency);
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + damage(ref ability, ability.potency) + " Damage. Next ability at: " + nextability);
        } else {
          numberofmisses += 1;
          MainWindow.log("!!MISS!! - " + MainWindow.time.ToString("F2") + " - " + ability.name + " missed! Next ability at: " + ability.nextCast + " ACCROLL: " + accroll + " - ACC%: " + calculateACC());
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
      if (ability.debuffTime > 0) {
        if (ability.debuff > 0) {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + "  DOT clipped.");
          //reset all buffs if clipping
          ability.dotbuff["fightorflight"] = false;
        }
        //If dot exists, enable its time.
        ability.debuff = ability.debuffTime;
        if (fightorflight.buff > 0) { ability.dotbuff["fightorflight"] = true; }

        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " DoT has been applied.  Time Left: " + ability.debuff);
      }
      if (ability.buffTime > 0) {
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
        }
      }
      if ((MainWindow.servertick == 3 && MainWindow.time == MainWindow.servertime) && ability.debuff > 0) {
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " is ticking now for " + damage(ref ability, ability.dotPotency, true) + "  Damage - Time Left: " + ability.debuff);
        MainWindow.log("---- " + ability.name + " - Dots - " + "FoF: " + ability.dotbuff["fightorflight"]);
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
      double tempstr = STR;
      if (fightorflight.buff > 0) { damageformula *= 1.30; }
      if (ability.abilityType == "Weaponskill" || ability.abilityType == "Instant") {
        damageformula = ((double)pot / 100) * (0.005126317 * WEP * tempstr + 0.000128872 * WEP * DTR + 0.049531324 * WEP + 0.087226457 * tempstr + 0.050720984 * DTR);

      }
      if (ability.abilityType == "AUTOA") {
        damageformula = (AAPOT) * (0.408 * WEP + 0.103262731 * tempstr + 0.003029823 * WEP * tempstr + 0.003543121 * WEP * (DTR - 202));
      }

      //crit
      double critroll = MainWindow.d100(1, 1000001) / 10000; //critroll was only rolling an interger between 1-101. Now has the same precision as critchance.
      var critchance = 0.0697 * (double)CRIT - 18.437;
      //MainWindow.log("CRIT CHANCE IS:" + critchance + " ROLL IS: " + critroll);
      if (dot) {
        if (ability.dotbuff["fightorflight"]) { damageformula *= 1.30; }
      }

      if (critroll <= critchance) {
        numberofcrits += 1;
        MainWindow.log("!!CRIT!! - ", false);
        damageformula *= 1.5;
      }


      // added variance to damage.
      damageformula = ((MainWindow.d100(-500, 500) / 10000) + 1) * (int)damageformula;
      return (int)damageformula;
    }



    // -------------------
    // Ability Definition
    // -------------------

    // Fast Blade ---------------------

    Ability fastblade = new Fastblade();
    public class Fastblade : Ability {
      public Fastblade() {
        name = "Fast Blade";
        potency = 150;
        dotPotency = 0;

        TPcost = 70;
        animationDelay = 0.8;
        abilityType = "Weaponskill";
        castTime = 0.0;
        duration = 0.0;
        buffTime = 0.0;
      }
    }
    // End Fast Blade ---------------------

    // Savage Blade --------------------------
    Ability savageblade = new Savageblade();
    public class Savageblade : Ability {
      public Savageblade() {
        name = "Savage Blade";
        potency = 100;
        combopotency = 200;
        dotPotency = 0;

        TPcost = 60;
        animationDelay = 1.3;
        abilityType = "Weaponskill";
        castTime = 0.0;
        duration = 0.0;
        debuffTime = 0.0;
      }
    }
    // End Savage Blade --------------------------

    // Rage of Halone -------------------------
    Ability rageofhalone = new Rageofhalone();
    public class Rageofhalone : Ability {
      public Rageofhalone() {
        name = "Rage of Halone";
        potency = 100;
        combopotency = 260;
        dotPotency = 0;

        TPcost = 60;
        animationDelay = 1.75;
        abilityType = "Weaponskill";
        castTime = 0.0;
        debuffTime = 0.0;
      }
    }
    // End Rage of Halone ----------------------

    // Spirits Within --------------------------------
    Ability spiritswithin = new Spiritswithin();
    public class Spiritswithin : Ability {
      public Spiritswithin() {
        name = "Spirits Within";
        potency = 300;
        dotPotency = 0;
        recastTime = 30;
        TPcost = 0;
        animationDelay = 1.25;
        abilityType = "Instant";
        castTime = 0.0;
      }

    }
    // End Spirits Within ---------------------------

    // Mercy Stroke -------------------------------
    Ability mercystroke = new Mercystroke();
    public class Mercystroke : Ability {
      public Mercystroke() {
        name = "Mercy Stroke";
        potency = 200;
        dotPotency = 0;
        recastTime = 90;
        TPcost = 0;
        animationDelay = 1.1;
        abilityType = "Instant";
        castTime = 0.0;
      }

    }
    // Mercy Stroke -------------------------------

    // Fight or Flight ------------------------------------
    Ability fightorflight = new Fightorflight();
    public class Fightorflight : Ability {
      public Fightorflight() {
        name = "Fight or Flight";
        recastTime = 90;
        animationDelay = 0.6;
        abilityType = "Cooldown";
        buffTime = 30; //Can't get 30 seconds to work?
      }
    }
    // End Fight or Flight -------------------------------

    // Fracture ---------------------------------
    Ability fracture = new Fracture();
    public class Fracture : Ability {
      public Fracture() {
        name = "Fracture";
        potency = 100;
        dotPotency = 20;
        TPcost = 80;
        animationDelay = 1.2;
        abilityType = "Weaponskill";
        debuffTime = 18;
        castTime = 0.0;
      }
    }
    // End Fracture ----------------------------

    //Sword Oath
    Ability swordoath = new Swordoath();
    public class Swordoath : Ability {
      public Swordoath() {
        name = "Sword Oath";
        potency = 0;
        dotPotency = 0;
        recastTime = 60;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Weaponskill";
        castTime = 0.0;
        buffTime = MainWindow.fightlength;
      }
    }
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
  }
}
