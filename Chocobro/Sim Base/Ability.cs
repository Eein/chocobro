using System;

namespace Chocobro {

  public partial class Ability : MainWindow {
    
    public string name;
    public string abilityType;
    public int potency;
    public int dotPotency;
    public double recastTime;
    public double animationDelay;
    public int TPcost;
    public double castTime;
    public double duration;
    public double nextCast = 0.0;
    public double buff;
    public double debuff;
    // COPY THIS WHEREVER BUFFED IS USED 
    // 0 - Raging Strikes 
    // 1 - Blood for Blood 
    // 2 - Straighter Shot  
    // 3 - Hawk's Eye  
    // 4 - Internal Release  // 5 -  //
    int[] buffed = new int[6] { 0, 0, 0, 0, 0, 0 };

    public double bonus = 0; // for abilitiy specific bonuses.x

    static Job _player;
    public Ability(Job player) {
      _player = player;
    }

    public virtual void execute() {
      if (abilityType == "Weaponskill") {
        
        //If time >= next cast time and time >= nextability)
        if (time >= nextCast && time >= nextability && actionmade == false) {
          time = floored(time);
          string executestring = time.ToString("F2") + " - Executing " + name;
          log(executestring);
          // remove TP
          TP -= TPcost;
          log("Cost is " + TPcost + "TP. Current TP: " + TP); //test for tp
          //if doesnt miss, then impact

          //set nextCast.
          nextCast = floored((time + recastTime));


          //set nextability
          nextability = floored((time + gcd));
          nextinstant = floored((time + animationDelay));
          
          //time = nextTime(nextinstant, nextability);
          actionmade = true;

          //var critroll = d100.Next(1, 101);
         // var critbonus = calculateCrit();
          impact();
        }
      }
      if (abilityType == "Instant") {
        //If time >= next cast time and time >= nextability)
        if (time >= nextCast && time >= nextinstant) {
          time = floored(time);
          string executestring = time.ToString("F2") + " - Executing " + name;
          log(executestring);
          //if doesnt miss, then impact

          //set nextCast.
          nextCast = floored((time + recastTime));
          

          //set nextability
          if (time + animationDelay > nextability) {
            nextability = floored((time + animationDelay));
          }
          
          nextinstant = floored((time + animationDelay));

          impact();
        }
      }
      //else instant stuff
      if (abilityType == "Cooldown") {
        //If time >= next cast time and time >= nextability)
        if (time >= nextCast && time >= nextinstant) {
          time = floored(time);
          string executestring = time.ToString("F2") + " - Executing " + name;
          log(executestring);
          //if doesnt miss, then impact

          //set nextCast.
          nextCast = floored((time + recastTime));

          //set nextability
          if (time + animationDelay > nextability) {
            nextability = floored((time + animationDelay));
          }
          nextinstant = floored((time + animationDelay));
          impact();
        }
      }

    }
    public virtual void impact() {
      //set potency for now, but change to damage later.
      log(time.ToString("F2") + " - " + name + " Deals " + potency + " Potency Damage. Next ability at: " + nextability);
    }

    //public virtual void expire() { } not really needed. Maybe handle expiration in ticks? hmmm.

    public virtual void tick() {
      //schedule tick
      if (MainWindow.time == MainWindow.servertime && debuff != 0.0) {
        debuff -= 1.0;
        if (debuff <= 0.0) {
          log(time.ToString("F2") + " - " + name + " has fallen off.");
        }
      }
      if ((MainWindow.servertick == 3 && MainWindow.time == MainWindow.servertime) && debuff > 0) {
        log(time.ToString("F2") + " - " + name + " is ticking now for " + dotPotency + " Potency Damage - Time Left: " + debuff);
      }
    }

    public virtual void decrement() {

      if (MainWindow.time == MainWindow.servertime && buff != 0.0) {
        buff -= 1.0;
        if (buff <= 0.0) {
          log(time.ToString("F2") + " - " + name + " has fallen off.");
        }
      }


      
    }

  }
}


