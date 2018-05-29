﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScoreTracking
{
    public class PartidaPaladins : Partida
    {
        public PartidaPaladins(Vencedor vencedor, Champion_Paladins champion, Champion_Paladins[] aliados, Champion_Paladins[] inimigos, int pontosAliado, int pontosInimigo,string mapa, DateTime dateTime) 
            : base(vencedor, dateTime)
        {
            this.SeuHeroi = champion;
            this.pontosAliado = pontosAliado;
            this.pontosInimigo = pontosInimigo;
            this.Time_Aliado = aliados;
            this.Time_Inimigo = inimigos;
            this.Mapa = mapa;
        }

        [JsonProperty]
        public string Mapa { get; private set; }        
        [JsonProperty]
        public int pontosAliado { get; private set; }
        [JsonProperty]
        public int pontosInimigo { get; private set; }
        [JsonProperty]
        public Champion_Paladins[] Time_Aliado { get; private set; }
        [JsonProperty]
        public Champion_Paladins[] Time_Inimigo { get; private set; }

        public void Altera(Vencedor vencedor, Champion_Paladins champion, Champion_Paladins[] aliados, Champion_Paladins[] inimigos, int pontosAliado, int pontosInimigo, string mapa)
        {
            this.Ganhador = vencedor;
            this.SeuHeroi = champion;
            this.Time_Aliado = aliados;
            this.Time_Inimigo = inimigos;
            this.pontosAliado = pontosAliado;
            this.pontosInimigo = pontosInimigo;
            this.Mapa = mapa;
        }
    }
}
