using System;
using System.Windows;

namespace Chocobro {

  public class WhiteMage : Job {
    // Proc Booleans - Set all proc booleans false initially.
    bool freecureproc = false;
    bool overcureproc = false;
    int medicaiitargets = 0;
    int targets = 0;
    bool MPfix = true;

    public WhiteMage() {
      name = "White Mage";
      classname = "Conjurer";

      shroudofsaints = new ShroudOfSaints();
      cureii = new CureII();
      cure = new Cure();
      whmregen = new WHMRegen();
      feylight = new FeyLight();
      feyglow = new FeyGlow();
      regen = new Regen();
      medica = new Medica();
      medicaii = new MedicaII();
      cureiii = new CureIII();


    }

    public override void getStats(MainWindow cs) {
      base.getStats(cs);
      MPfix = true;
    }

    public override void rotation() {
      if (MPfix) { MPMax = (3523 + 8 * (PIE - 244)); MP = MPMax; MPfix = false; }
      int cureiivar = (int)MainWindow.d100(1, 100);
      int medicavar = (int)MainWindow.d100(1, 100);
      if (regen.nextCast <= MainWindow.time) {
        execute(ref regen);
      }

      if (MainWindow.time == cure.endcast && cure.casting) { cure.casting = false; impact(ref cure); }
      if (MainWindow.time == cureii.endcast && cureii.casting) { cureii.casting = false; impact(ref cureii); }
      if (MainWindow.time == medicaii.endcast && medicaii.casting) { medicaii.casting = false; impact(ref medicaii); }
      if (MainWindow.time == cureiii.endcast && cureiii.casting) { cureiii.casting = false; impact(ref cureiii); }
      if (MainWindow.time == medica.endcast && medica.casting) { medica.casting = false; impact(ref medica); }

      if (MP < MPMax - (665 + MPMax * 0.02)) { execute(ref shroudofsaints); }
      if (whmregen.hot <= 0) { execute(ref whmregen); }
      if (medicaii.hot <= 0) { execute(ref medicaii); }
      if (overcureproc) { execute(ref cureiii); }
      if (freecureproc) { execute(ref cureii); }
      if (cureiivar <= 10) { execute(ref cureii); }
      if (medicavar <= 10) { execute(ref medica); }
      execute(ref cure);

      if (MainWindow.selenebuff == true) {
        if (feyglow.buff <= 0 && MainWindow.time >= feylight.nextCast) { execute(ref feylight); }
        if (feylight.buff <= 0 && MainWindow.time >= feyglow.nextCast) { execute(ref feyglow); }
      }

      //server actionable - ticks/decrements then server tick action
      //if tick is 3 
      tick(ref whmregen);
      tick(ref medicaii);

      //decrement buffs
      decrement(ref feylight);
      decrement(ref feyglow);
      decrement(ref shroudofsaints);
    }

    public override void execute(ref Ability ability) {

      if (ability.name == "Regen" && shroudofsaints.buff > 0) {
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + "MP is " + MP + ".");
        MP += 212;
        if (MP > MPMax) { MP = MPMax; }
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + "Shroud of Saints refreshes 212 MP. MP is now " + MP + ".");
      }

      base.execute(ref ability);
    }

    public override void impact(ref Ability ability) {



      if (ability.abilityType == "Cooldown") {
        ability.hits += 1;
      }

      if (ability.abilityType == "HealSpell" || ability.abilityType == "Weaponskill") {

        ability.hits += 1;

        if (ability.name == "Cure") {
          var freecureroll = MainWindow.d100(1, 100);
          if (freecureroll <= 15) {
            freecureproc = true; MainWindow.log(MainWindow.time.ToString("F2") + " - Freecure proc!");
          }

        }

        if (ability.name == "Cure II") {
          var overcureroll = MainWindow.d100(1, 100);
          if (overcureroll <= 15) { overcureproc = true; MainWindow.log(MainWindow.time.ToString("F2") + " - Overcure proc!"); }
        }




        int mpbefore = MP;
        int mpcost = 0;
        mpcost = ability.MPcost;
        if (freecureproc && ability.name == "Cure II") { mpcost = 0; freecureproc = false; }
        if (overcureproc && ability.name == "Cure III") { mpcost /= 2; overcureproc = false; }
        mpused += mpcost;
        MP -= mpcost;
        targets = 1;
        if (ability.name == "Cure III") { targets = (int)MainWindow.d100(5, 8); }
        if (ability.name == "Medica") { targets = (int)MainWindow.d100(6, 8); }
        if (ability.name == "Medica II") { targets = (int)MainWindow.d100(7, 8); medicaiitargets = targets; }
        if (ability.name != "Cure III" && ability.name != "Medica" && ability.name != "Medica II") {
          double thisdamage = damage(ref ability, ability.potency);
          totaldamage += (int)Math.Round(thisdamage);
          ability.damage += (int)Math.Round(thisdamage);
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Heals " + thisdamage + " HP. Next ability at: " + nextability + ". MP is now " + MP + ".");
        } else {
          for (var x = 1; x <= targets; ++x) {
            double thisdamage = damage(ref ability, ability.potency);
            totaldamage += (int)Math.Round(thisdamage);
            ability.damage += (int)Math.Round(thisdamage);
            MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Heals " + thisdamage + " HP to target " + x + ". Next ability at: " + nextability + ". MP is now " + MP + ".");
          }
        }
      }

      if (ability.hotTime > 0) {
        ability.hot = ability.hotTime;

        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " buff has been applied.  Time Left: " + ability.hot + ". Next ability at: " + nextability);
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
      if (MainWindow.time == MainWindow.servertime && ability.hot > 0) {
        ability.hot -= 1.0;
        if (ability.hot <= 0.0) {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " has fallen off.");
          //clear buffs from object.

          ability.dotbuff["potion"] = false;
          ability.dotbuff["song"] = false;
        }
      }
      if ((MainWindow.servertick == 3 && MainWindow.time == MainWindow.servertime) && (ability.hot > 0 && ability.hotPotency > 0)) {


        if (ability.name == "Medica II") {
          for (var x = 1; x <= medicaiitargets; ++x) {
            numberofticks += 1;
            ability.ticks += 1;
            var tickheal = damage(ref ability, ability.hotPotency, true);
            ability.hotheals += tickheal;
            totaldamage += tickheal;
            MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " is ticking now for " + tickheal + " HP on target " + x + ". - Time Left: " + ability.hot);
          }
        } else {
          numberofticks += 1;
          ability.ticks += 1;
          var tickheal = damage(ref ability, ability.hotPotency, true);
          ability.hotheals += tickheal;
          totaldamage += tickheal;
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " is ticking now for " + tickheal + " HP - Time Left: " + ability.hot);
        }
      }
    }

    public int damage(ref Ability ability, int pot, bool dot = false) {
      double damageformula = 0.0;
      double tempmnd = MND;


      if (ability.abilityType == "Weaponskill" || ability.abilityType == "Instant" || ability.abilityType == "Spell" || ability.abilityType == "HealSpell") {
        damageformula = ((double)pot / 100) * ((MDMG * 0.353554382 + tempmnd * 0.176111019 + DTR * 0.00911378 + MDMG * tempmnd * 0.00739962 + MDMG * (DTR - 202) * 0.002393685) / 1.5);

      }

      //crit
      double critroll = MainWindow.d100(1, 1000001) / 10000; //critroll was only rolling an interger between 1-101. Now has the same precision as critchance.
      double critchance = 0;
      critchance = 0.0697 * (double)CRIT - 18.437;

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
      damageformula = ((MainWindow.d100(-300, 300) / 10000) + 1) * (int)damageformula;
      return (int)damageformula;
    }

    // -------------------
    // Ability Definition
    // -------------------

    //resets


    public override void report() {
      base.report();
      // add abilities to list used for reporting. Each ability needs to be added ;(
      areport.Add(feylight);
      areport.Add(feyglow);
      areport.Add(whmregen);
      areport.Add(cure);
      areport.Add(cureii);
      areport.Add(shroudofsaints);
      areport.Add(regen);
      areport.Add(cureiii);
      areport.Add(medicaii);
      areport.Add(medica);
    }

    Ability medica;
    Ability medicaii;
    Ability cureiii;
    Ability shroudofsaints;
    Ability whmregen;
    Ability cureii;
    Ability cure;
    Ability feylight;
    Ability feyglow;
    Ability regen;



    // Set array of abilities for reportingz

    // Cure

    public class Cure : Ability {
      public Cure() {
        name = "Cure";
        abilityType = "HealSpell";
        castTime = 2.0;
        potency = 400;
        MPcost = 133;
      }
    }

    public class CureII : Ability {
      public CureII() {
        name = "Cure II";
        abilityType = "HealSpell";
        castTime = 2.0;
        potency = 650;
        MPcost = 266;
      }
    }

    public class CureIII : Ability {
      public CureIII() {
        name = "Cure III";
        abilityType = "HealSpell";
        castTime = 2.0;
        potency = 550;
        MPcost = 505;
      }
    }

    public class Medica : Ability {
      public Medica() {
        name = "Medica";
        abilityType = "HealSpell";
        castTime = 2.5;
        potency = 300;
        MPcost = 372;
      }
    }

    public class MedicaII : Ability {
      public MedicaII() {
        name = "Medica II";
        abilityType = "HealSpell";
        castTime = 3.0;
        potency = 200;
        MPcost = 452;
        hotPotency = 50;
        hotTime = 30;
      }
    }

    public class WHMRegen : Ability {
      public WHMRegen() {
        name = "Regen ";
        abilityType = "Weaponskill";
        potency = 0;
        MPcost = 186;
        hotPotency = 150;
        hotTime = 21;
        recastTime = 2.5;
      }
    }

    public class ShroudOfSaints : Ability {
      public ShroudOfSaints() {
        name = "Shroud of Saints";
        abilityType = "Cooldown";
        recastTime = 120;
        buffTime = 15;
        animationDelay = 0.0;
      }
    }
  }
}
