
namespace Chocobro {
  public class Bard : Job {
    static Bard _player;
    public Bard(Bard player){
      _player = player;
    }
    
    //-----------------------


    public override void rotation() {
      var gcd = calculateGCD();
      

      if (straightshot.buff <= 0) {
        straightshot.execute();
      }

      if (windbite.debuff <= gcd) {
        windbite.execute();
      }

      if (venomousbite.debuff <= gcd) {
        venomousbite.execute();
      }

      heavyshot.execute();

      if (TP() < 540) {
        invigorate.execute();
        addTP(400);
      }

      ragingstrikes.execute();
      hawkseye.execute();
      bloodforblood.execute();
      internalrelease.execute();
      barrage.execute();

      miserysend.execute();
      bloodletter.execute();
      flamingarrow.execute();
      repellingshot.execute();
      bluntarrow.execute();

      //server actionable - ticks/decrements then server tick action
      //if tick is 3
      windbite.tick();
      venomousbite.tick();
      flamingarrow.tick();
      //decrement buffs
      straightshot.decrement();
      internalrelease.decrement();

      regen();
    }

    // -------------------
    // Ability Definition
    // -------------------

    // Heavy Shot ---------------------

    Ability heavyshot = new Heavyshot(_player);
   
    public class Heavyshot : Ability {
      public Heavyshot(Bard player) : base(player) {
        name = "Heavy Shot";
        potency = 150;
        dotPotency = 0;
        recastTime = 2.5;
        TPcost = 60;
        animationDelay = 0.8;
        abilityType = "Weaponskill";
        castTime = 0.0;
        duration = 0.0;
      }

      public override void impact() {
        //add heavier shot buff activation here
        log(calculateCrit(_player) + "% <--Crit Chance ");
        base.impact();  
      }
    }
    // End Heavyshot ---------------------

    // Windbite --------------------------
    Ability windbite = new Windbite(_player);
    public class Windbite : Ability {
      public Windbite(Job player) : base(player){
        name = "Windbite";
        potency = 60;
        dotPotency = 45;
        recastTime = 2.5;
        TPcost = 80;
        animationDelay = 1.3;
        abilityType = "Weaponskill";
        castTime = 0.0;
        duration = 0.0;
      }
      public override void impact() {
        //Start ticking for 18s
        this.debuff = 18;
        base.impact();
        log(time.ToString("F2") + " - " + name + " DoT has been applied.  Time Left: " + debuff);
      }
    }
    // End Windbite --------------------------

    // Venomous Bite -------------------------
    Ability venomousbite = new Venomousbite(_player);
    public class Venomousbite : Ability {
      public Venomousbite(Job player) : base(player) {
        name = "Venomous Bite";
        potency = 100;
        dotPotency = 35;
        recastTime = 2.5;
        TPcost = 80;
        animationDelay = 0.7;
        abilityType = "Weaponskill";
        castTime = 0.0;
      }
      public override void impact() {
        //Start ticking for 18s
        this.debuff = 18;
        base.impact();
        log(time.ToString("F2") + " - " + name + " DoT has been applied.  Time Left: " + debuff);
      }
    }
    // End Venomous Bite ----------------------

    // Straight Shot --------------------------
    Ability straightshot = new Straightshot(_player);
    public class Straightshot : Ability {
      public Straightshot(Job player) : base(player) {
        name = "Straight Shot";
        potency = 140;
        dotPotency = 0;
        recastTime = 2.5;
        TPcost = 70;
        animationDelay = 0.8;
        abilityType = "Weaponskill";
        castTime = 0.0;
      }
      public override void impact() {
        //Start ticking for 20s
        this.buff = 20;
        log(time.ToString("F2") + " - " + name + " has been applied.  Time Left: " + buff);
        base.impact();
      }
    }
    // End Straight Shot --------------------------

    // Bloodletter --------------------------------
    Ability bloodletter = new Bloodletter(_player);
    public class Bloodletter : Ability {
      public Bloodletter(Job player) : base(player) {
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
    Ability miserysend = new Miserysend(_player);
    public class Miserysend : Ability {
      public Miserysend(Job player) : base(player) {
        name = "Miserys End";
        potency = 190;
        dotPotency = 0;
        recastTime = 12;
        TPcost = 0;
        animationDelay = 1.1;
        abilityType = "Instant";
        castTime = 0.0;
      }
      public override void execute() {
        if (time >= (fightlength * 0.80)) {
          base.execute();
        }
      }
    }
    // End  Miserys End -------------------------------

    // Blunt Arrow ------------------------------------
    Ability bluntarrow = new Bluntarrow(_player);
    public class Bluntarrow : Ability {
      public Bluntarrow(Job player) : base(player) {
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
    Ability repellingshot = new Repellingshot(_player);
    public class Repellingshot : Ability {
      public Repellingshot(Job player) : base(player) {
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
    Ability flamingarrow = new Flamingarrow(_player);
    public class Flamingarrow : Ability {
      public Flamingarrow(Job player) : base(player) {
        name = "Flaming Arrow";
        potency = 0;
        dotPotency = 35;
        recastTime = 60;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Instant";
        castTime = 0.0;
      }
      public override void impact() {
        this.debuff = 30;
        base.impact();
        log(time.ToString("F2") + " - " + name + " DoT has been applied.  Time Left: " + debuff);
      }
    }
    // End Flaming Arrow

    // Internal Release
    Ability internalrelease = new Internalrelease(_player);
    public class Internalrelease : Ability {
      public Internalrelease(Job player) : base(player) {
        name = "Internal Release";
        recastTime = 60;
        animationDelay = 0.75;
        abilityType = "Cooldown";
      }
      public override void impact() {
        this.buff = 15;
        log(time.ToString("F2") + " - " + name + " Buff has been applied. Time Left: " + buff);
      }
    }
    // End Internal Release

    // Blood for Blood
       Ability bloodforblood = new Bloodforblood(_player);
    public class Bloodforblood : Ability {
      public Bloodforblood(Job player) : base(player) {
        name = "Blood for Blood";
        recastTime = 80;
        animationDelay = 0.9;
        abilityType = "Cooldown";
      }
      public override void impact() {
        this.buff = 20;
        log(time.ToString("F2") + " - " + name + " Buff has been applied. Time Left: " + buff);
      }
    }
    // End Blood for Blood

    // Raging Strikes
    Ability ragingstrikes = new Ragingstrikes(_player);
    public class Ragingstrikes : Ability {
      public Ragingstrikes(Job player) : base(player) {
        name = "Raging Strikes";
        recastTime = 120;
        animationDelay = 0.9;
        abilityType = "Cooldown";
      }
      public override void impact() {
        this.buff = 20;
        log(time.ToString("F2") + " - " + name + " Buff has been applied. Time Left: " + buff);
      }
    }
    // End Raging Strikes

    // Hawks Eye
    Ability hawkseye = new Hawkseye(_player);
    public class Hawkseye : Ability {
      public Hawkseye(Job player) : base(player) {
        name = "Hawks Eye";
        recastTime = 90;
        animationDelay = 0.7;
        abilityType = "Cooldown";
      }
      public override void impact() {
        this.buff = 20;
        log(time.ToString("F2") + " - " + name + " Buff has been applied. Time Left: " + buff);
      }
    }
    // End Hawks Eye

    // Barrage
    Ability barrage = new Barrage(_player);
    public class Barrage : Ability {
      public Barrage(Job player) : base(player) {
        name = "Barrage";
        recastTime = 90;
        animationDelay = 0.7;
        abilityType = "Cooldown";
      }
      public override void impact() {
        this.buff = 10;
        log(time.ToString("F2") + " - " + name + " Buff has been applied. Time Left: " + buff);
      }
    }
    // End Barrage

    // Invigorate
    Ability invigorate = new Invigorate(_player);
    public class Invigorate : Ability {
      public Invigorate(Job player) : base(player) {
        name = "Invigorate";
        recastTime = 120;
        animationDelay = 0.8;
        abilityType = "Cooldown";
      }
      public override void execute() {
        log(time.ToString("F2") + " - " + name + " has restored 400 TP");
      }
    }
    // End Invigorate
  }
}
