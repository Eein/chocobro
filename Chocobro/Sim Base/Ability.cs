using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chocobro
{   
   
    public partial class Ability : MainWindow
    {

        public string name;
        public string abilityType;
        public int potency;
        public int dotPotency;
        public double recastTime;
        public double animationDelay;
        public double abilityCost;
        public double castTime;
        public double duration;
        public double nextCast = 0.0;
        public double buff;
        public double debuff;

        public double bonus = 0; // for abilitiy specific bonuses.x
        
        public virtual void execute() {
            if (abilityType == "Weaponskill") { 
            //If time >= next cast time and time >= nextability)
                if (time >= nextCast && time >= nextability) {
                    time = floored(time);
                    string executestring = time.ToString("F2") + " - Executing " + name;
                    log(executestring);
                    //if doesnt miss, then impact
                    impact();
                    //set nextCast.
                    nextCast = floored((time + recastTime));
                    
                    //set nextability
                    nextability = floored((time + gcd));
                    nextinstant = floored((time + animationDelay));
                    
                    time = nextTime(nextinstant, nextability);
                    actionmade = true;
                }
            } //else instant stuff
            
        }
        public virtual void impact() {
            //set potency for now, but change to damage later.
            log(time.ToString("F2") + " - " + name + " Deals " + potency + " Potency Damage.");
        }
        public virtual void expire() { }
        public virtual void tick() { }
        public virtual void decrement() { }
    }
}
