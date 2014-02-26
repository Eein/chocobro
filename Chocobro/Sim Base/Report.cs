using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Chocobro {
  class Report {

    Utility util = new Utility();

    public List<double> timeline = new List<double>();
    string report = "";
    string dpetcolor = "006699";
    double fightlength = MainWindow.fightlength;
    int countdpet = 0;
    public void parse(Job j) {
      var dps = j.totaldamage / MainWindow.fightlength;

      report += "<!doctype html><html class='no-js' lang='en'><head><title>Chocobro Report</title><style>";
      //style
      report += "meta.foundation-version {font-family:'/5.1.0/';}meta.foundation-mq-small {font-family:'/only screen and (max-width: 40em)/';width:0em;}meta.foundation-mq-medium {font-family:'/only screen and (min-width:40.063em)/';width:40.063em;}meta.foundation-mq-large {font-family:'/only screen and (min-width:64.063em)/';width:64.063em;}meta.foundation-mq-xlarge {font-family:'/only screen and (min-width:90.063em)/';width:90.063em;}meta.foundation-mq-xxlarge {font-family:'/only screen and (min-width:120.063em)/';width:120.063em;}meta.foundation-data-attribute-namespace {font-family:false;}html ,body {height:100%;}* ,*:before ,*:after {-moz-box-sizing:border-box;-webkit-box-sizing:border-box;box-sizing:border-box;}html ,body {font-size:100%;}body {background:#ffffff;color:#222222;padding:0;margin:0;font-family:'Helvetica Neue','Helvetica',Helvetica,Arial,sans-serif;font-weight:normal;font-style:normal;line-height:1;position:relative;cursor:default;}a:hover {cursor:pointer;}img ,object ,embed {max-width:100%;height:auto;}object ,embed {height:100%;}img {-ms-interpolation-mode:bicubic;}#map_canvas img ,#map_canvas embed ,#map_canvas object ,.map_canvas img ,.map_canvas embed ,.map_canvas object {max-width:none !important;}.left {float:left !important;}.right {float:right !important;}.clearfix {*zoom:1;}.clearfix:before ,.clearfix:after {content:' ';display:table;}.clearfix:after {clear:both;}.hide {display:none;}.antialiased {-webkit-font-smoothing:antialiased;-moz-osx-font-smoothing:grayscale;}img {display:inline-block;vertical-align:middle;}textarea {height:auto;min-height:50px;}select {width:100%;}body {width:100%;margin-left:auto;margin-right:auto;margin-top:0;margin-bottom:0;max-width:62.5rem;*zoom:1;background-color:#333333;color:#ffffff;font-size:14px;}body:before ,body:after {content:' ';display:table;}body:after {clear:both;}a {text-decoration:none;}a:visited ,a:link ,a:active {color:#006699;}a:hover {color:#ffffff;}.tile ,.info ,.stats ,.weights ,.dpet ,.dpstimeline ,.damagesources ,.blank ,.abilities ,.buffs ,.debuffs {background-color:#222222;padding:10px;min-height:360px;border:2px solid #000000;}.wrapper {background-color:#333333;width:100%;margin-left:auto;margin-right:auto;margin-top:0;margin-bottom:0;max-width:62.5rem;*zoom:1;border-left:2px solid #000000;border-right:2px solid #000000;height:auto;}.wrapper:before ,.wrapper:after {content:' ';display:table;}.wrapper:after {clear:both;}.header ,.footer {width:100%;margin-left:auto;margin-right:auto;margin-top:0;margin-bottom:0;max-width:62.5rem;*zoom:1;background-color:#000000;font-size:18px;padding:10px;}.header:before ,.header:after {content:' ';display:table;}.header:after {clear:both;}.info {padding-left:0.9375rem;padding-right:0.9375rem;width:45%;float:left;}.stats {padding-left:0.9375rem;padding-right:0.9375rem;width:45%;float:left;}.weights {padding-left:0.9375rem;padding-right:0.9375rem;width:100%;float:left;min-height:300px;}.weights table {width:100%;background-color:#000000;}.weights table td {padding:5px;}.weights table thead {background-color:#000000;}.weights table tbody tr:nth-child(even) {background:#444444;}.weights table tbody tr:nth-child(odd) {background:#111111;}.dpet {padding-left:0.9375rem;padding-right:0.9375rem;width:45%;float:left;}.dpstimeline {padding-left:0.9375rem;padding-right:0.9375rem;width:45%;float:left;}.damagesources {padding-left:0.9375rem;padding-right:0.9375rem;width:45%;float:left;}.blank {padding-left:0.9375rem;padding-right:0.9375rem;width:45%;float:left;}.abilities {padding-left:0.9375rem;padding-right:0.9375rem;width:100%;float:left;min-height:300px;}.abilities table {background-color:#000000;width:100%;}.abilities table td {padding:5px;}.abilities table thead {background-color:#000000;}.abilities table tbody tr:nth-child(even) {background:#444444;}.abilities table tbody tr:nth-child(odd) {background:#111111;}.buffs {padding-left:0.9375rem;padding-right:0.9375rem;width:45%;float:left;}.debuffs {padding-left:0.9375rem;padding-right:0.9375rem;width:45%;float:left;}.footer {font-size:12px;}.footer .social {float:right;}";
      report += "</style></head><body><div class='header'>chocobro.</div><div class='wrapper'><div class='info'><h3>Sim info</h3><div class='job'>";
      //stats
      report += "Job:" + j.name + "</div><div class='class'>Class:N/A</div><div class='race'>Race:N/A</div><div class='iterations'>Iterations:" + MainWindow.iterations + "</div><div class='fightlength'>Fightlength (s):" + MainWindow.fightlength + "</div></div><div class='stats'><h3>Character stats</h3>";
      report += "<div class'STR'>STR:" + j.STR + "</div><div class'DEX'>DEX:" + j.DEX + "</div><div class'VIT'>VIT:" + j.VIT + "</div><div class'INT'>INT:" + j.INT + "</div><div class'MND'>MND:" + j.MND + "</div><div class'PIE'>PIE:" + j.PIE + "</div><div class'ACC'>ACC:" + j.ACC + "</div><div class'CRIT'>CRIT:" + j.CRIT + "</div><div class'DET'>DET:" + j.DTR + "</div><div class'AP'>AP:" + j.AP + "</div><div class'AMP'>AMP:" + j.AMP + "</div><div class'WEP'>WEP:" + j.WEP + "</div><div class'AADMG'>AADMG:" + j.AADMG + "</div><div class'AADELAY'>AADELAY:" + j.AADELAY + "</div></div></div>";
      //weights
      if (j.statforweights != "None") {
        report += "<div class='wrapper'><div class='weights'><h3>Weights</h3><table><thead><tr><td>STAT</td><td>-10</td><td>-9</td><td>-8</td><td>-7</td><td>-6</td><td>-5</td><td>-4</td><td>-3</td><td>-2</td><td>-1</td><td>0</td><td>1</td><td>2</td><td>3</td><td>4</td><td>5</td><td>6</td><td>7</td><td>8</td><td>9</td><td>10</td></tr></thead><tbody><tr><td>";
        report += j.statforweights + "</td>";
        foreach (int stat in j.DPSarray) {
          report += "<td>" + stat + "</td>";

        }
        report += "</tr></tbody></table></div></div>";
      }
      countdpet = 0;
      foreach (Ability ability in j.areport) {
        if (ability.damage > 0) { //checks to make sure abilities do damage....
          countdpet += 1;
        }
      }
      report += " <div class='wrapper'><div class='dpet'><h3>DPET</h3><div><img border='0' width='500' height='" + ((countdpet + 1) * 30) + "' src=\"http://chart.googleapis.com/chart?cht=bhg&chf=bg,s,00000000&chtt=Damage Per Execute Time (DPET)&chts=FFFFFF,18&chs=500x" + ((countdpet + 1) * 30) + "&chd=t:";
      var maxdpet = 0.0; //for calculating chart scope, then calculate dpet
      foreach (Ability ability in j.areport) {
        if (ability.damage > 0) { //checks to make sure abilities do damage....

          ability.dpet = 0;
          //check if the ability has a dot component
          if (ability.dotPotency > 0) {
            ability.dpet = ((ability.damage + ability.dotdamage) / ((double)ability.hits + (double)ability.crits));
          } else {
            ability.dpet = ((ability.damage) / ((double)ability.hits + (double)ability.crits));
          }

          if (ability.dpet > maxdpet) { maxdpet = ability.dpet; }
        }
      }
      //sort then...
      j.areport.Sort((x, y) => y.dpet.CompareTo(x.dpet));

      // add to dpet.

      foreach (Ability ability in j.areport) {
        if (ability.damage > 0) { //checks to make sure abilities do damage....
          report += Math.Round(ability.dpet) + "|";
        }
      }
      // eating the last pipe off the end of the string ..
      report = report.Substring(0, report.Length - 1);

      report += "&chds=0," + (int)(maxdpet * 3) + "&chco=FFFFFF&chm=t++";

      for (var x = 0; x < countdpet; ++x) {
        var abilityname = j.areport[x].name.Replace(" ", "_");
        if (j.areport[x].dpet != 0) { //checks to make sure abilities do damage.... TODO: goign to have to change this 'last' logic in case a non-dps ability is added.
          //WHEN YOU GET BACK: Dpet value isn't being assigned... figure this out.
          report += Math.Round(j.areport[x].dpet) + "++" + abilityname + "," + dpetcolor + "," + x + ",0,15";
        }
        if (x < countdpet - 1) { report += "|t++"; }  // add pipe to all but last... 


      }
      report += "\" /></div></div><div class='dpstimeline'><h3>Timeline</h3>";
      report += "<img src='http://chart.apis.google.com/chart?cht=lc&chs=500x250&chco=006699&chxt=x,y";
      report += "&chxl=0:|0|fightlength=" + fightlength + "|1:|0|avg=" + Math.Round((j.totaldamage / fightlength)) + "|max=" + (Math.Round(timeline.Max())) + "&chxp=1,1,51,100&chtt=Damage+Timeline&chts=FFFFFF,18&chf=bg,s,00000000&chd=";

      //loop through damage timeline.
      report += util.simpleEncode(timeline, (timeline.Max() + 10));
      //report = report.Substring(0, report.Length - 1);
      report += "' />";

      report += "</div></div><div class='wrapper'><div class='damagesources'><h3>Damage Sources</h3>";
      report += "<img src='http://chart.googleapis.com/chart?chxs=0,FFFFFF,11.5&&chco=006699&&chxt=x&&chs=500x225&chts=FFFFFF,18&chf=bg,s,00000000&cht=p&chd=t:";

      foreach (Ability ability in j.areport) {
        if (ability.damage > 0) { //checks to make sure abilities do damage....
          if (ability.dotPotency > 0) {
            report += dpspercent(ability.dotdamage, dps, fightlength) + ",";
          }
          report += dpspercent(ability.damage, dps, fightlength) + ",";
        }
      }
      // eating the last comma off the end of the string ..
      report = report.Substring(0, report.Length - 1);
      report += "&chl=";
      foreach (Ability ability in j.areport) {
        if (ability.damage > 0) { //checks to make sure abilities do damage....
          if (ability.dotPotency > 0) {
            report += ability.name + " Dot|";
          }
          report += ability.name + "|";
        }
      }
      // eating the last pipe off the end of the string ..
      report = report.Substring(0, report.Length - 1);

      report += "&chtt=Damage+Sources' width='450 height='225'/></div><div class='blank'><h3>MP/TP Regen (NA)</h3></div></div><div class='wrapper'>";
      report += "<div class='abilities'><h3>Ability Breakdown</h3>";
      report += "<table><thead><tr><td>Ability</td><td>DPS</td><td>DPS%</td><td>HIT</td><td>HIT%</td><td>CRIT</td><td>CRIT%</td><td>MISS</td><td>MISS%</td><td>UPTIME</td></tr></thead><tbody>";
      foreach (Ability ability in j.areport) {

        if (ability.dotPotency > 0) {
          report += "<tr><td>" + ability.name + " Dot</td><td>" + (Math.Floor((ability.dotdamage / fightlength) * 100) / 100) + "</td><td>" + (Math.Floor(dpspercent(ability.dotdamage, dps, fightlength) * 100) / 100) + "</td><td>" + ability.ticks + "</td><td>HIT%</td><td>" + ability.tickcrits + "</td><td>CRIT%</td><td>0</td><td>MISS%</td><td>UPTIME</td></tr>";
        }
        report += "<tr><td>" + ability.name + "</td><td>" + (Math.Floor((ability.damage / fightlength) * 100) / 100) + "</td><td>" + (Math.Floor(dpspercent(ability.damage, dps, fightlength) * 100) / 100) + "</td><td>" + ability.hits + "</td><td>HIT%</td><td>" + ability.crits + "</td><td>CRIT%</td><td>" + ability.misses + "</td><td>MISS%</td><td>UPTIME</td></tr>";

      }

      report += "</tbody></table>";
      report += "</div></div><div class='wrapper'><div class='buffs'><h3>Buffs</h3></div><div class='debuffs'><h3>Debuffs</h3></div></div><div class='footer'>Chocobro © 2013-2014. FINAL FANTASY XIV © 2010-2014 SQUARE ENIX CO., LTD. All Rights Reserved. <div class='social'><a href='http://github.com/eein/chocobro' target='_blank'>Github</a> - <a href='http://www.twitter.com/chocobrodotcom' target='_blank'>Twitter</a> - <a href='http://www.chocobro.com' target='_blank'>Homepage</a></div></div></body></html>";

      write();
    }

    public void write() {
      StreamWriter sw = new StreamWriter("report.html");
      sw.Write(report);
      sw.Close();
    }
    public double dpspercent(double abilitydamage, double dps, double fightlength) {
      double value = (((abilitydamage / fightlength) / dps) * 100);
      return value;
    }
  }
}
