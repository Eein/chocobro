using System;
using System.Windows;

namespace Chocobro {


  public class BlackMage : Job {
    // Proc Booleans - Set all proc booleans false initially.
     int AFstacks;
     int UIstacks;
     bool firestarter = false;
     bool thundercloud = false;
     string lastability = "";
     int opener = 0;
     int firestartercheck = 0;

    public BlackMage() {
      name = "Black Mage";
      classname = "Thaumaturge";
    }
    public override void getStats(MainWindow cs) {
      base.getStats(cs);
      // Define AP and MP conversion.
      AP = STR; //or STR
      AMP = INT;
      stance = Stance.BLMNone;
      opener = 0;
      firestarter = false;
      thundercloud = false;
      AFstacks = 0;
      UIstacks = 0;
      lastability = "";
      firestartercheck = 0;
      MPMax = 3629 + 8 * (PIE - 239);
      MP = MPMax;
    }

    public override void rotation() {
      execute(ref regen);
      if (MainWindow.selenebuff == true) {
        if (feyglow.buff <= 0 && MainWindow.time >= feylight.nextCast) { execute(ref feylight); }
        if (feylight.buff <= 0 && MainWindow.time >= feyglow.nextCast) { execute(ref feyglow); }
      }

      //Impact
      if (MainWindow.time == fireiii.endcast && fireiii.casting) { fireiii.casting = false; impact(ref fireiii); stance = Stance.AF3; AFstacks = 3; }
      if (MainWindow.time == fire.endcast && fire.casting) { fire.casting = false; impact(ref fire); }
      if (MainWindow.time == fireiiifsp.endcast && fireiiifsp.casting) { impact(ref fireiiifsp); fireiiifsp.casting = false; firestarter = false; }
      if (MainWindow.time == blizzardiii.endcast && blizzardiii.casting) { blizzardiii.casting = false; impact(ref blizzardiii);  UIstacks = 3; stance = Stance.UI3;}
      if (MainWindow.time == thunder.endcast && thunder.casting) { thunder.casting = false; impact(ref thunder); }
      if (MainWindow.time == thunderii.endcast && thunderii.casting) { thunderii.casting = false; impact(ref thunderii); }
      if (MainWindow.time == thunderiiitcp.endcast && thunderiiitcp.casting) {
        if (firestarter) { firestartercheck = 1; }
        thunderiiitcp.casting = false; thundercloud = false; thunder.debuff = 0; impact(ref thunderiiitcp); 
      }
      if (MainWindow.time == flare.endcast && flare.casting) { flare.casting = false; impact(ref flare); }
      if (MainWindow.time == blizzard.endcast && blizzard.casting) { blizzard.casting = false;  impact(ref blizzard); }

      //Abilities - execute(ref ability)
      if (convert.nextCast <= MainWindow.time && swiftcast.nextCast <= MainWindow.time && stance == Stance.AF3 && opener > 6) { opener = 2; }

      if (stance == Stance.AF3 && firestarter && MP >= 938) {
        if (firestartercheck == 1) { firestartercheck = 0; execute(ref fireiiifsp); }
        if (firestartercheck == 0) { firestartercheck = 1; execute(ref ragingstrikes); execute(ref xpotionintelligence); execute(ref fire); }
      }

      if (stance == Stance.UI3 && firestarter && MP >= MPMax * 0.9) { execute(ref transpose); }
      if (AFstacks == 1) { execute(ref fireiiifsp); }

      if (opener == 0) { execute(ref thunderii);  }

      if (opener == 1) { execute(ref fireiii);  }
      //execute(ref ragingstrikes);
            
      if (opener == 2) {
        
        if (stance == Stance.AF3 && MP >= (938) && lastability != "Fire") { execute(ref fire); }
        execute(ref xpotionintelligence);
        if (stance == Stance.AF3 && MP >= (938)) { execute(ref fire); }
        if (stance == Stance.AF3 && MP <= 938) { opener += 1; }
      }
      //execute(ref xpotionintelligence);
      if (opener == 3) {  execute(ref swiftcast); }
      if (opener == 4) {  execute(ref flare); }
      if (opener == 5) {  execute(ref convert); }
      if (opener == 6) { opener += 1; execute(ref fire); }

      if (stance == Stance.BLMNone) { execute(ref fireiii); }
      if (thundercloud && (lastability == "Fire" || lastability == "Blizzard III" && !firestarter)) { execute(ref thunderiiitcp); }
      if (stance == Stance.AF3 && firestarter && lastability != "Fire" && MP >= (638 + 79 + 212)) { execute(ref fire); }

      if (stance == Stance.AF3 && MP >= (638 + 79 + 212)) { execute(ref fire); }
      if (stance == Stance.AF3 && MP < (638 + 79 + 212) && opener > 6) {  execute(ref blizzardiii); }
      if (stance == Stance.UI3 && MP >= 212 && thunder.debuff <= calculateSGCD(2.5) && thunderiiitcp.debuff <= calculateSGCD(2.5) && thunderii.debuff <= calculateSGCD(2.5)) { execute(ref thunder); }
      if (stance == Stance.UI3 && (MainWindow.time  <= nextregentick - calculateSGCD(3)) || (firestarter && MainWindow.time <= nextregentick - calculateSGCD(1.25))) { execute(ref blizzard); }
      if ((stance == Stance.UI3 && MP >= (MPMax * 0.95) || (MP > MPMax * 0.5 && (MainWindow.time + calculateSGCD(1.75) + 0.01) > nextregentick)) && !firestarter) { execute(ref fireiii); }

      //Buffs/Cooldowns - execute(ref ability)

      //Instants - execute(ref ability)

      //Ticks - tick(ref DoTability)
      tick(ref thunder);
      tick(ref thunderii);
      tick(ref thunderiiitcp);
      //AutoAttacks (not for casters!) - execute(ref autoattack)

      //Decrement Buffs - decrement(ref buff)
      decrement(ref xpotionintelligence);
      decrement(ref ragingstrikes);
      decrement(ref feyglow);
      decrement(ref feylight);
    }

    public override void execute(ref Ability ability) {
      base.execute(ref ability);

    }
    public override void impact(ref Ability ability) {

      lastability = ability.name;
      ability.swings += 1;
      if (ability.name == "Swiftcast") {
        opener += 1;
        swift = true;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " used.");
        
      }
      if (ability.name == "Transpose" && stance == Stance.UI3) { UIstacks = 0;  AFstacks = 1; stance = Stance.AF1; }
      
      if (ability.name == "Convert") { opener += 1; MP += (int)(0.3 * MPMax); ; }
      if (opener <= 6 && ability.name == "Thunder II") { opener += 1; }
      if (opener <= 6 && ability.name == "Flare") { opener += 1; }
      if (opener <= 6 && ability.name == "Convert") { opener += 1; }
      if (opener <= 6 && ability.name == "Fire III") {opener += 1; }
      if (opener <= 6 && ability.name == "Fire" && MP >= 938 && MP <= 1576) {opener +=1;}
      
      //var critchance = calculateCrit(_player);
      //set potency for now, but change to damage later.
      var accroll = (MainWindow.d100(1, 10001)) / 100;

      if (ability.abilityType == "Cooldown") {
        
      }

      if (ability.abilityType == "Weaponskill" || (ability.abilityType == "Instant")) {
        
        if (accroll < calculateACC()) {
          
          
          double thisdamage = damage(ref ability, ability.potency);

          totaldamage += (int)thisdamage;
          ability.damage += thisdamage;
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + thisdamage + " Damage. Next ability at: " + nextability); 
        } else {
          ability.misses += 1;
          MainWindow.log("!!MISS!! - " + MainWindow.time.ToString("F2") + " - " + ability.name + " missed! Next ability at: " + ability.nextCast);
          // Does heavyshot buff get eaten by a miss?
        }
      }

      if (ability.abilityType == "Spell") {

        if (accroll < calculateACC()) {
          ability.hits += 1;
          
          double thisdamage = damage(ref ability, ability.potency);
          int mpbefore = MP;
          int mpcost = 0;
          mpcost = ability.MPcost;
          if (ability.name == "Thunder III (TCP)") { thunder.debuff = 0; thunderii.debuff = 0; thunderiiitcp.debuff = 0; }
          if (AFstacks == 3 && ability.aspect == "Fire") { mpcost = (ability.MPcost * 2); }
          if (AFstacks == 3 && ability.aspect == "Ice") { mpcost = (ability.MPcost / 4); }
          if (UIstacks == 3 && ability.aspect == "Fire") { mpcost = (ability.MPcost / 4); }
          if (ability.name == "Flare") { mpcost = MP; }
          mpused += mpcost;
          MP -= mpcost;
          totaldamage += (int)Math.Round(thisdamage);
          ability.damage += (int)Math.Round(thisdamage);
          if (ability.name == "Fire III") { AFstacks = 3; UIstacks = 0; stance = Stance.AF3; }
          if (ability.name == "Ice III") { UIstacks = 3; AFstacks = 0; stance = Stance.UI3; }
          if (ability.name == "Fire III (FSP)") { UIstacks = 0; stance = Stance.AF3; AFstacks = 3; firestarter = false; firestartercheck = 0; }
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Deals " + thisdamage + " Damage. Next ability at: " + nextability + ". MP is now " + MP + ". Times used: " + ability.swings + "Times hit: " + ability.hits  + " Total Damage: " + totaldamage + ". Opener: " + opener);

          if (ability.name == "Fire") {
            if (MainWindow.d100(1, 100) <= 40) {
              MainWindow.log("!!PROC!! - Firestarter!");
              ability.procs += 1;
              firestarter = true;
            }
          }


        } else {
          ability.misses += 1;
          MainWindow.log("!!MISS!! - " + MainWindow.time.ToString("F2") + " - " + ability.name + " missed! Next ability at: " + ability.nextCast);
          // Does heavyshot buff get eaten by a miss?
        }
      }

      if (ability.abilityType == "HealSpell") {

        
        double thisheal = damage(ref ability, ability.potency);
        int mpbefore = MP;
        mpused += ability.MPcost;
        totalhealed += (int)thisheal;
        ability.heals += thisheal;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " Heals " + thisheal + " HP. MP " + mpbefore + " => " + MP + ". Next ability at: " + nextability);
      }


      // If ability has debuff, create its timer.
      if (ability.debuffTime > 0 && accroll < calculateACC()) {
        if (ability.debuff > 0) {
          MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + "  DOT clipped.");
          //reset all buffs if clipping
          ability.dotbuff["raginstrikes"] = false;
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
          ability.dotbuff["raginstrikes"] = false;
          ability.dotbuff["potion"] = false;
        }
      }
      if ((MainWindow.servertick == 3 && MainWindow.time == MainWindow.servertime) && ability.debuff > 0) {
        var tickdmg = damage(ref ability, ability.dotPotency, true);
        ability.ticks += 1;
        ability.dotdamage += tickdmg;
        totaldamage += tickdmg;
        MainWindow.log(MainWindow.time.ToString("F2") + " - " + ability.name + " is ticking now for " + tickdmg + "  Damage - Time Left: " + ability.debuff);
        if (ability.name == "Thunder" || ability.name == "Thunder II" || ability.name == "Thunder III" || ability.name == "Thunder III (TCP)") {
          if (MainWindow.d100(1, 100) <= 5) {
            MainWindow.log("!!PROC!! - Thundercloud");
            ability.procs += 1;
            thundercloud = true;
          }
        }
        //MainWindow.log("---- " + ability.name + " - Dots - RS: " + ability.dotbuff["ragingstrikes"] + " BFB: " + ability.dotbuff["bloodforblood"] + " SS: " + ability.dotbuff["straightshot"] + " HE: " + ability.dotbuff["hawkseye"] + " IR: " + ability.dotbuff["internalrelease"] + " Potion: " + ability.dotbuff["potion"]);
      }
    }

    public int damage(ref Ability ability, int pot, bool dot = false) {
      double damageformula = 0.0;
      double tempint = INT;
      double multi = 1;
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
      if (AFstacks == 0 && UIstacks == 0 && (ability.aspect == "Fire" || ability.aspect == "Ice")) { multi = 1; }
      if (AFstacks == 3 && ability.aspect == "Fire") { multi = 1.8; }
      if (AFstacks == 1 && ability.aspect == "Fire") { multi = 1.4; }
      if (UIstacks == 3 && ability.aspect == "Fire") { multi = 0.7; }
      if (AFstacks == 3 && ability.aspect == "Ice") { multi = 0.7; }

      if (ability.abilityType == "Weaponskill" || ability.abilityType == "Instant" || ability.abilityType == "Spell" || ability.abilityType == "HealSpell") {
        damageformula = (multi * (double)pot / 100) * ((MDMG * 0.353554382 + tempint * 0.176111019 + DTR * 0.00911378 + MDMG * tempint * 0.00739962 + MDMG * (DTR - 202) * 0.002393685) / 1.5);

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
      //damageformula = ((MainWindow.d100(-500, 500) / 10000) + 1) * (int)damageformula;
      return (int)damageformula;
    }

    // -------------------
    // Ability Definition
    // -------------------

    //resets


    public override void report() {
      base.report();
      // add abilities to list used for reporting. Each ability needs to be added ;(
      areport.Add(ragingstrikes);
      areport.Add(flare);
      areport.Add(swiftcast);
      areport.Add(convert);
      areport.Add(regen);
      areport.Add(fireiii);
      areport.Add(fireiiifsp);
      areport.Add(fire);
      areport.Add(blizzardiii);
      areport.Add(thunder);
      areport.Add(thunderii);
      areport.Add(blizzard);
      areport.Add(thunderiiitcp);
      areport.Add(transpose);

      if (MainWindow.selenebuff) {
        areport.Add(feylight);
        areport.Add(feyglow);
      }
    }

    Ability ragingstrikes = new RagingStrikes();
    Ability xpotionintelligence = new XPotionIntelligence();
    Ability blizzard = new Blizzard();
    Ability fireiii = new FireIII();
    Ability fireiiifsp = new FireIIIfsp();
    Ability fire = new Fire();
    Ability blizzardiii = new BlizzardIII();
    Ability thunder = new Thunder();
    Ability thunderii = new ThunderII();
    Ability thunderiiitcp = new ThunderIIItcp();
    Ability flare = new Flare();
    Ability swiftcast = new Swiftcast();
    Ability convert = new Convert();
    Ability feylight = new FeyLight();
    Ability feyglow = new FeyGlow();
    Ability regen = new Regen();
    Ability transpose = new Transpose();



    // Set array of abilities for reportingz

    // -------------------
    // Ability Definition
    // -------------------

    // Weaponskill 1  ---------------------
    public class FireIII : Ability {
      public FireIII() {
        name = "Fire III";
        abilityType = "Spell";
        castTime = 3.50;
        potency = 240;
        MPcost = 532;
        aspect = "Fire";
        recastTime = 2.5;
      }
    }

    public class Fire : Ability {
      public Fire() {
        name = "Fire";
        abilityType = "Spell";
        castTime = 2.5;
        potency = 170;
        MPcost = 319;
        aspect = "Fire";
        recastTime = 2.5;
      }
    }

    public class Blizzard : Ability {
      public Blizzard() {
        name = "Blizzard";
        abilityType = "Spell";
        castTime = 2.5;
        potency = 170;
        MPcost = 106;
        aspect = "Ice";
        recastTime = 2.5;
      }
    }

    public class FireIIIfsp : Ability {
      public FireIIIfsp() {
        name = "Fire III (FSP)";
        abilityType = "Spell";
        castTime = 0.02;
        potency = 240;
        MPcost = 0;
        aspect = "Fire";
        recastTime = 2.5;
      }
    }

    public class BlizzardIII : Ability {
      public BlizzardIII() {
        name = "Blizzard III";
        abilityType = "Spell";
        castTime = 3.50;
        potency = 240;
        MPcost = 79;
        aspect = "Ice";
        recastTime = 2.5;
      }
    }

    public class Thunder : Ability {
      public Thunder() {
        name = "Thunder";
        abilityType = "Spell";
        castTime = 2.50;
        potency = 30;
        dotPotency = 35;
        debuffTime = 18;
        MPcost = 212;
      }
    }

    public class ThunderII : Ability {
      public ThunderII() {
        name = "Thunder II";
        abilityType = "Spell";
        castTime = 3.00;
        potency = 50;
        dotPotency = 35;
        debuffTime = 21;
        MPcost = 319;
      }
    }

    public class ThunderIII : Ability {
      public ThunderIII() {
        name = "Thunder III";
        abilityType = "Spell";
        castTime = 3.50;
        potency = 60;
        dotPotency = 35;
        debuffTime = 24;
        MPcost = 425;
      }
    }

    public class ThunderIIItcp : Ability {
      public ThunderIIItcp() {
        name = "Thunder III (TCP)";
        abilityType = "Spell";
        castTime = 0.02;
        potency = 340;
        dotPotency = 35;
        debuffTime = 24;
        MPcost = 0;
      }
    }

    public class Flare : Ability {
      public Flare() {
        name = "Flare";
        abilityType = "Spell";
        castTime = 4.00;
        potency = 260;
        MPcost = 0;
        aspect = "Fire";
        recastTime = 2.5;
      }
    }

    public class Swiftcast : Ability {
      public Swiftcast() {
        name = "Swiftcast";
        abilityType = "Cooldown";
        recastTime = 60;
        animationDelay = 0.8;
      }
    }

    public class Convert : Ability {
      public Convert() {
        name = "Convert";
        abilityType = "Cooldown";
        recastTime = 180;
        animationDelay = 0.8;
      }
    }

    public class RagingStrikes : Ability {
      public RagingStrikes() {
        name = "Raging Strikes";
        abilityType = "Cooldown";
        recastTime = 180;
        animationDelay = 0.08;
        buff = 20;
      }
    }

    public class Transpose : Ability {
      public Transpose() {
        name = "Transpose";
        abilityType = "Cooldown";
        recastTime = 12;
        animationDelay = 0.1;
      }
    }
  }
}
