
namespace Chocobro {
  public class Bard : Job {


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

      miserysend.execute();

      bloodletter.execute();

      //server actionable - ticks/decrements then server tick action
      //if tick is 3
      windbite.tick();
      venomousbite.tick();

      //decrement buffs
      straightshot.decrement();

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
        abilityCost = 60;
        animationDelay = 0.3;
        abilityType = "Weaponskill";
        castTime = 0.0;
        duration = 0.0;
      }
      public override void impact() {
        //add heavier shot buff activation here
        base.impact();
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
        abilityCost = 80;
        animationDelay = 0.3;
        abilityType = "Weaponskill";
        castTime = 0.0;
        duration = 0.0;

      }
      public override void impact() {
        //Start ticking for 18s
        this.debuff = 18;
        base.impact();
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
        abilityCost = 80; 
        animationDelay = 0.3; 
        abilityType = "Weaponskill"; 
        castTime = 0.0; 
        

      }
      public override void impact() {
        //Start ticking for 18s
        this.debuff = 18;
        base.impact();
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
        abilityCost = 70;
        animationDelay = 0.3; 
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
    Ability bloodletter = new Bloodletter();
    public class Bloodletter : Ability {
      public Bloodletter() {
        name = "Bloodletter";
        potency = 150;
        dotPotency = 0;
        recastTime = 15;
        abilityCost = 0;
        animationDelay = 0.5;
        abilityType = "Instant";
        castTime = 0.0;
      }
      public override void impact() {
        base.impact();
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
        abilityCost = 0;
        animationDelay = 0.3;
        abilityType = "Instant";
        castTime = 0.0;
      }
      public override void impact() {
        if (time >= (fightlength * 0.20)) {
          base.impact();
        }
      }
    }

  }
}
