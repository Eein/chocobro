
namespace Chocobro {
  public class Bard : Job {


    public override void rotation() {
      //actionable
      if (windbite.debuff <= MainWindow.gcd) {
        windbite.execute();
      }
      heavyshot.execute();
      //server actionable - ticks/decrements then server tick action
      //if tick is 3
      windbite.tick();
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



    //Ability windbite = new Ability() { name = "Windbite", potency = 60, dotPotency = 45, recastTime = 2.5, abilityCost = 80, animationDelay = 0.3, abilityType = "Weaponskill", castTime = 0.0, duration = 18 };
    Ability venomousbite = new Ability() { name = "Venomous Bite", potency = 100, dotPotency = 35, recastTime = 2.5, abilityCost = 80, animationDelay = 0.3, abilityType = "Weaponskill", castTime = 0.0, duration = 18 };

    //straightshot
    Ability straightshot = new Ability() { name = "Straight Shot", potency = 140, dotPotency = 0, recastTime = 2.5, abilityCost = 70, animationDelay = 0.3, abilityType = "Weaponskill", castTime = 0.3, duration = 20 };





  }
}
