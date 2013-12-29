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

namespace Chocobro {
    public class Bard : Job {
        Ability heavyshot = new Heavyshot();
        
        public override void rotation() {
            
            heavyshot.execute();

        }
        

        // -------------------
        // Ability Definition
        // -------------------

        // Heavy Shot ---------------------
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
                //add heavier shot stuff here
                base.impact();
            }
        }

        // End Heavyshot ---------------------
        
        
        
        Ability windbite = new Ability() { name = "Windbite", potency = 60, dotPotency = 45, recastTime = 2.5, abilityCost = 80, animationDelay = 0.3, abilityType = "Weaponskill", castTime = 0.0, duration = 18 };
        Ability venomousbite = new Ability() { name = "Venomous Bite", potency = 100, dotPotency = 35, recastTime = 2.5, abilityCost = 80, animationDelay = 0.3, abilityType = "Weaponskill", castTime = 0.0, duration = 18 };
        
        
        
        //straightshot
        Ability straightshot = new Ability() { name = "Straight Shot", potency = 140, dotPotency = 0, recastTime = 2.5, abilityCost = 70, animationDelay = 0.3, abilityType = "Weaponskill", castTime = 0.3, duration = 20 };
        
        
    
    
    
    }
}
