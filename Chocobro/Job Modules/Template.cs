using System;
namespace Chocobro {

  public class Template : Job {
    
    //-----------------------

    public Template() {
      
    }
   
    public override void rotation() {
      var gcd = calculateGCD();
      autoattack.recastTime = AADELAY;

      //Abilities - execute(ref ability)
      if (dot1.debuff <= 0) {
        execute(ref dot1);
      }
      execute(ref weaponskill1);

      //Buffs/Cooldowns - execute(ref ability)
      execute(ref cooldown1);

      //Instants - execute(ref ability)
      execute(ref instant1);

      //Ticks - tick(ref DoTability)
      tick(ref dot1);

      //AutoAttacks (not for casters!) - execute(ref autoattack)
      execute(ref autoattack);

      //Decrement Buffs - decrement(ref buff)
      decrement(ref cooldown1);

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
      var accroll = (MainWindow.d100(1, 10001)) / 100;

      //overrides for combos/special interactions

      //how to handle each type of skill on impact
      if (ability.abilityType == "Weaponskill" || (ability.abilityType == "Instant" && ability.potency > 0)) {
        numberofattacks += 1;
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
          numberofattacks += 1;
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
          ability.dotbuff["cooldown1"] = false;
        }
        //If dot exists, enable its time.
        ability.debuff = ability.debuffTime;
        if (cooldown1.buff > 0) { ability.dotbuff["cooldown1"] = true; }

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
          ability.dotbuff["cooldown1"] = false;
        }
      }
      if ((MainWindow.servertick == 3 && MainWindow.time == MainWindow.servertime) && ability.debuff > 0) {
        numberofticks += 1;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " is ticking now for " + damage(ref ability, ability.dotPotency, true) + "  Damage - Time Left: " + ability.debuff);
        //MainWindow.log("---- " + ability.name + " - Dots - " + "FoF: " + ability.dotbuff["fightorflight"]);
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
      if (cooldown1.buff > 0) { damageformula *= 1.30; }
      if (ability.abilityType == "Weaponskill" || ability.abilityType == "Instant") {
        damageformula = (((double)pot / 100) * (0.005126317 * WEP * tempstr + 0.000128872 * WEP * DTR + 0.049531324 * WEP + 0.087226457 * tempstr + 0.050720984 * DTR));

      }
      if (ability.abilityType == "AUTOA") {
        damageformula = ((AAPOT) * (0.408 * WEP + 0.103262731 * tempstr + 0.003029823 * WEP * tempstr + 0.003543121 * WEP * (DTR - 202)));
      }

      //crit
      double critroll = MainWindow.d100(1, 1000001) / 10000; //critroll was only rolling an interger between 1-101. Now has the same precision as critchance.
      var critchance = 0.0697 * (double)CRIT - 18.437;
      //MainWindow.log("CRIT CHANCE IS:" + critchance + " ROLL IS: " + critroll);
      if (dot) {
        if (ability.dotbuff["cooldown1"]) { damageformula *= 1.30; }
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

    // Weaponskill 1  ---------------------

    Ability weaponskill1 = new Weaponskill1();
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
    Ability dot1 = new Ability();
    public class Dot1 : Ability {
      public Dot1() {
        name = "Dot 1";
        abilityType = "Weaponskill";
        potency = 90;
        dotPotency = 25;
        TPcost = 60;
        animationDelay = 0.4;
        debuffTime = 30;
      }
    }
    // End Dot 1 -------------------------------

    // Cooldown 1 ---------------------------------
    Ability cooldown1 = new Cooldown1();
    public class Cooldown1 : Ability {
      public Cooldown1() {
        name = "Cooldown 1";
        abilityType = "Cooldown";
        animationDelay = 0.6;
        recastTime = 90;
        buffTime = 30;
      }
    }
    // End Cooldown 1 ----------------------------

    //Instant 1 --------------------------------
    Ability instant1 = new Instant1();
    public class Instant1 : Ability {
      public Instant1() {
        name = "Instant 1";
        abilityType = "Instant";
        potency = 60;
        dotPotency = 30;
        animationDelay = 0.8;
        recastTime = 15;
      }
    }
    //End Instant 1 ------------------------------

    // Auto Attack
    Ability autoattack = new Autoattack();
    public class Autoattack : Ability {
      public Autoattack() {
        name = "Auto Attack";
        abilityType = "AUTOA";        
        animationDelay = 0;
      }
    }
    //End Auto Attack

  }
}
