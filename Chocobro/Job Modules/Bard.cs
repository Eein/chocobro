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
        

        public override void rotation() {
            heavyshot.execute();
        }
        

        // -------------------
        // Ability Definition
        // -------------------

        // Heavy Shot
        Ability heavyshot = new Ability() { name = "Heavy Shot", potency = 150, dotPotency = 0, recastTime = 2.5, abilityCost = 60, animationDelay = 0.3, abilityType = "Weaponskill", castTime = 0.0, duration = 0.0 };
        Ability windbite = new Ability() { name = "Windbite", potency = 60, dotPotency = 45, recastTime = 2.5, abilityCost = 80, animationDelay = 0.3, abilityType = "Weaponskill", castTime = 0.0, duration = 18 };
        Ability venbite = new Ability() { name = "Venemous Bite", potency = 100, dotPotency = 35, recastTime = 2.5, abilityCost = 80, animationDelay = 0.3, abilityType = "Weaponskill", castTime = 0.0, duration = 18 };
        Ability straightshot = new Ability() { name = "Straight Shot", potency = 140, dotPotency = 0, recastTime = 2.5, abilityCost = 70, animationDelay = 0.3, abilityTyp = "Weaponskill", castTime = 0.3, duration = 20 };
    }
}
