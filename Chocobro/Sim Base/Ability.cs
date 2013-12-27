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
        
        public string name { get; set; }
        public string abilityType { get; set; }
        public int potency { get; set; }
        public int dotPotency { get; set; }
        public double recastTime { get; set; }
        public double animationDelay { get; set; }
        public double abilityCost { get; set; }
        public double castTime { get; set; }
        public double duration { get; set; }
        public double nextCast = 0.0;

        double bonus = 0; // for abilitiy specific bonuses.x
        
        public virtual void execute() {
            
            string executestring = MainWindow.time_t().ToString("F2");
            executestring += " - Executing ";
            executestring += name;
            log(executestring);
            

        }
        public virtual void impact() { }
        public virtual void expire() { }
        public virtual void tick() { }
        public virtual void decrement() { }
    }
}
