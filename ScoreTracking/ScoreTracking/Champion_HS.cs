﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Resources;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreTracking
{
    public class Champion_HS : Champion
    {
        public Champion_HS(string nome, string classe) 
            : base(nome, classe)
        {
        }

        public override Bitmap GetImage()
        {
            ResourceManager rm = Properties.Resources.ResourceManager;
            return (rm.GetObject(this.Classe) as Bitmap);
        }
    }
}