
namespace Chocobro {
  public class Blackmage : Job {


    public override void rotation() {
      var gcd = calculateGCD();
      regen();
    }

    // -------------------
    // Ability Definition
    // -------------------

    // Fire  ---------------------
    Ability fire = new Fire();
    public class Fire : Ability {
      public Fire() {
        name = "Fire";
        potency = 150;
        dotPotency = 0;
        recastTime = 2.5;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Weaponskill"; //Change to Spell
        castTime = 2.5;
        duration = 0.0;
        //manaCost = 100
      }
      public override void impact() { // Needs Firestarter procs
        //Add in Astral Fire stacks
        base.impact();
      }
    }
    // End Fire  ---------------------

    // Fire III ---------------------
    Ability fireiii = new FireIII();
    public class FireIII : Ability {
      public FireIII() {
        name = "Fire III";
        potency = 220;
        dotPotency = 0;
        recastTime = 2.5;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Weaponskill"; //change to Spell
        castTime = 3.5;
        duration = 0.0;
        //manaCost = 100;
      }
      public override void impact() {
        //Add in Astral Fire stacks
        base.impact();
      }
    }
    // End Fire III ---------------------

    // Blizzard ------------------------
    Ability blizzard = new Blizzard();
    public class Blizzard : Ability {
      public Blizzard() {
        name = "Blizzard";
        potency = 150;
        dotPotency = 0;
        recastTime = 2.5;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Weaponskill"; //change to Spell
        castTime = 3.5;
        duration = 0.0;
        //manaCost = 100;
      }
      public override void impact() {
        //Add in Umbral Ice stacks
        base.impact();
      }
    }
    // End Blizzard ---------------------

    // Blizzard III ---------------------
    Ability blizzardiii = new BlizzardIII();
    public class BlizzardIII : Ability {
      public BlizzardIII() {
        name = "Blizzard III";
        potency = 220;
        dotPotency = 0;
        recastTime = 2.5;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Weaponskill"; //change to Spell
        castTime = 3.5;
        duration = 0.0;
        //manaCost = 100;
      }
      public override void impact() {
        //Add in Umbral Ice stacks
        base.impact();
      }
    }
    // End Blizzard III ---------------------

    // Thunder ---------------------
    Ability thunder = new Thunder();
    public class Thunder : Ability {
      public Thunder() {
        name = "Thunder";
        potency = 30;
        dotPotency = 35;
        recastTime = 2.5;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Weaponskill"; //change to Spell
        castTime = 2.5;
        duration = 0.0;
        //manaCost = 100;
      }
      public override void impact() { //Needs Thundercloud procs
        this.debuff = 18;
        base.impact();
        log(time.ToString("F2") + " - " + name + " DoT has been applied.  Time Left: " + debuff);
      }
    }
    //  End Thunder ---------------------

    // Thunder II ---------------------
    Ability thunderii = new ThunderII();
    public class ThunderII : Ability {
      public ThunderII() {
        name = "Thunder II";
        potency = 50;
        dotPotency = 35;
        recastTime = 2.5;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Weaponskill"; //change to Spell
        castTime = 3.0;
        duration = 0.0;
        //manaCost = 100;
      }
      public override void impact() { //Needs Thundercloud procs
        this.debuff = 21;
        base.impact();
        log(time.ToString("F2") + " - " + name + " DoT has been applied.  Time Left: " + debuff);
      }
    }
    // End Thunder II ---------------------

    // Thunder III ---------------------
    Ability thunderiii = new ThunderIII();
    public class ThunderIII : Ability {
      public ThunderIII() {
        name = "Thunder III";
        potency = 60;
        dotPotency = 35;
        recastTime = 2.5;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Weaponskill"; //change to Spell
        castTime = 3.5;
        duration = 0.0;
        //manaCost = 100;
      }
      public override void impact() { //Needs Thundercloud procs
        this.debuff = 24;
        base.impact();
        log(time.ToString("F2") + " - " + name + " DoT has been applied.  Time Left: " + debuff);
      }
    }
    // End Thunder II ---------------------

    // Raging Strikes
    Ability ragingstrikes = new Ragingstrikes();
    public class Ragingstrikes : Ability {
      public Ragingstrikes() {
        name = "Raging Strikes";
        recastTime = 180;
        animationDelay = 0.9;
        abilityType = "Cooldown";
      }
      public override void impact() {
        this.buff = 20;
        log(time.ToString("F2") + " - " + name + " Buff has been applied. Time Left: " + buff);
      }
    }
    // End Raging Strikes

    // Swiftcast
    Ability swiftcast = new Swiftcast();
    public class Swiftcast : Ability {
      public Swiftcast() {
        name = "Swiftcast";
        recastTime = 60;
        animationDelay = 0.9;
        abilityType = "Cooldown";
      }
      public override void execute() {
        //Swiftcast interaction
        base.execute();
      }
    }
    // End Swiftcast

    // Transpose
    Ability transpose = new Transpose();
    public class Transpose : Ability {
      public Transpose() {
        name = "Tranpose";
        recastTime = 12;
        animationDelay = 0.9;
        abilityType = "Cooldown";
      }
      public override void execute() {
        //Transpose interaction
        base.execute();
      }
    }
    // End Swiftcast

    // Convert
    Ability convert = new Convert();
    public class Convert : Ability {
      public Convert() {
        name = "Convert";
        recastTime = 180;
        animationDelay = 0.9;
        abilityType = "Cooldown";
      }
      public override void execute() {
        //Convert interaction
        base.execute();
      }
    }
    // End Convert

    // Flare ---------------------
    Ability flare = new Flare();
    public class Flare : Ability {
      public Flare() {
        name = "Flare";
        potency = 260;
        dotPotency = 0;
        recastTime = 2.5;
        TPcost = 0;
        animationDelay = 0.8;
        abilityType = "Weaponskill"; //change to Spell
        castTime = 4.0;
        duration = 0.0;
        //manaCost = 100;
      }
      public override void impact() { 
        //Astral Fire interaction
        base.impact();
      }
    }
    // End Flare ---------------------

  }
}
