﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScoreTracking
{
    public partial class frm_Paladins : Form
    {        
        private List<Champion_Paladins> champions = new List<Champion_Paladins>();
        private List<string> mapas = new List<string>();
        private Control.ControlCollection form_Controls;
        private Champion_Paladins seu_heroi;
        private const string JSON_PATH = @".\partidas_paladins.json";
        Form formMenu;        

        public frm_Paladins(Form form)
        {
            InitializeComponent();
            PreencheCampos();
            formMenu = form;            
        }

        private void PreencheCampos()
        {
            if (!File.Exists(JSON_PATH))
            {
                FileStream file = File.Create(JSON_PATH);
                file.Close();
            }

            LeArquivo(Properties.Resources.champions_paladins);
            LeArquivo(Properties.Resources.maps_paladins);

            form_Controls = this.Controls;
            List<ComboBox> comboBoxes = form_Controls.OfType<ComboBox>().ToList().Where(x => !x.Name.Contains("mapa")).ToList();
            List<PictureBox> pictureBoxes = form_Controls.OfType<PictureBox>().ToList();

            int cont_ally = 0, cont_enemy = 0;

            for (int i = 0; i < comboBoxes.Count; i++)
            {
                Champion_Paladins[] championsTemp = new Champion_Paladins[champions.Count];
                champions.CopyTo(championsTemp);                
                comboBoxes[i].DataSource = championsTemp;                
                comboBoxes[i].DisplayMember = "Nome";

                PictureBox pictureBox = pictureBoxes.Where(x => x.Name.Contains(comboBoxes[i].Name.Substring(3))).ToArray()[0];
                Label label = form_Controls.OfType<Label>().ToList().Where(x => x.Name.Contains(comboBoxes[i].Name.Substring(3))).ToList()[0];

                if (comboBoxes[i].Name.Contains("ally"))
                {
                    comboBoxes[i].SelectedItem = champions[cont_ally];
                    pictureBox.Image = champions[cont_ally].GetImage();
                    label.Text = champions[cont_ally].Classe;
                    cont_ally++;
                }
                else
                {
                    comboBoxes[i].SelectedItem = champions[cont_enemy];
                    pictureBox.Image = champions[cont_enemy].GetImage();
                    label.Text = champions[cont_enemy].Classe;
                    cont_enemy++;
                }

                pictureBox.BringToFront();
                comboBoxes[i].SelectedIndexChanged += new EventHandler(AtualizaDados);                                
            }

            cb_mapa.DataSource = mapas.OrderBy(x => x).ToList();
            pb_mapa.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject((cb_mapa.SelectedItem as string).ToLower().Replace(" ", "_"));

            seu_heroi = (Champion_Paladins)comboBoxes.Where(x => x.Name.Contains("ally1")).ToList()[0].SelectedItem;            
        }

        private void SelecionaHeroi(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            Point position = pictureBox.Location;

            pictureBox.BringToFront();

            pn_selecao_heroi.Location = new Point(pn_selecao_heroi.Location.X, position.Y - 10);
            seu_heroi = (Champion_Paladins)form_Controls.OfType<ComboBox>().ToList().Where(x => x.Name.Contains(pictureBox.Name.Substring(3))).ToList()[0].SelectedItem;
        }

        private void AtualizaDados(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            Champion champion = (Champion_Paladins)comboBox.SelectedItem;

            PictureBox pictureBox = form_Controls.OfType<PictureBox>().ToList().Where(x => x.Name.Contains(comboBox.Name.Substring(3))).ToList()[0];
            Label label = form_Controls.OfType<Label>().ToList().Where(x => x.Name.Contains(comboBox.Name.Substring(3))).ToList()[0];

            label.Text = champion.Classe;

            pictureBox.Image = champion.GetImage();
        }

        private void LeArquivo(string file)
        {
            string[] infos;

            foreach(var line in file.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (line.Contains(':'))
                {
                    infos = line.Split(':');
                    champions.Add(new Champion_Paladins(infos[0], infos[1]));
                }
                else
                {
                    mapas.Add(line);
                }
            }            
        }

        private void bt_Voltar_Click(object sender, EventArgs e)
        {
            Hide();
            formMenu.Show();
        }

        private void frm_Paladins_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private List<PartidaPaladins> LeJSON()
        {
            JsonConverter[] converters = { new ChampionConverter() };

            List<PartidaPaladins> partidas = JsonConvert.DeserializeObject<List<PartidaPaladins>>(File.ReadAllText(JSON_PATH), new JsonSerializerSettings() { Converters = converters });

            if (partidas is null || partidas[0].Time_Aliado is null) partidas = new List<PartidaPaladins>();

            return partidas;
        }

        private void bt_Salvar_Click(object sender, EventArgs e)
        {
            List<ComboBox> comboBoxes_ally = form_Controls.OfType<ComboBox>().ToList().Where(x => x.Name.Contains("ally")).ToList();
            List<ComboBox> comboBoxes_enemy = form_Controls.OfType<ComboBox>().ToList().Where(x => x.Name.Contains("enemy")).ToList();

            List<Champion_Paladins> aliados = new List<Champion_Paladins>();
            List<Champion_Paladins> inimigos = new List<Champion_Paladins>();

            foreach(ComboBox comboBox in comboBoxes_ally)
            {
                aliados.Add((comboBox.SelectedItem as Champion_Paladins));
            }

            foreach (ComboBox comboBox in comboBoxes_ally)
            {
                inimigos.Add((comboBox.SelectedItem as Champion_Paladins));
            }

            int pont_aliado, pont_inimigo;

            if (string.IsNullOrEmpty(mtxb_ally_points.Text) || string.IsNullOrEmpty(mtxb_enemy_points.Text))
            {
                MessageBox.Show("Invira valores válidos para a pontuação", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            pont_aliado = int.Parse(mtxb_ally_points.Text);
            pont_inimigo = int.Parse(mtxb_enemy_points.Text);

            string mapa = cb_mapa.SelectedItem.ToString();

            Partida.Vencedor vencedor = (pont_aliado > pont_inimigo) ? Partida.Vencedor.Aliado : Partida.Vencedor.Inimigo;

            PartidaPaladins partida = new PartidaPaladins(vencedor, seu_heroi, aliados.ToArray(), inimigos.ToArray(), pont_aliado, pont_inimigo, mapa, DateTime.Now);

            List<PartidaPaladins> partidas = LeJSON();

            partidas.Add(partida);

            File.WriteAllText(JSON_PATH, JsonConvert.SerializeObject(partidas));

            Form alert = new frm_Notification("Salvo com sucesso");
            alert.ShowDialog();
        }

        private void estatísticaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form formEstatistica = new frm_Estatistica(this);
            formEstatistica.ShowDialog();
        }

        private void mtxb_ally_points_Click(object sender, EventArgs e)
        {
            mtxb_ally_points.SelectionStart = 0;
        }

        private void mtxb_enemy_points_Click(object sender, EventArgs e)
        {
            mtxb_ally_points.SelectionStart = 0;
        }

        private void cb_mapa_SelectedIndexChanged(object sender, EventArgs e)
        {            
            pb_mapa.Image = (Bitmap)Properties.Resources.ResourceManager.GetObject((cb_mapa.SelectedItem as string).ToLower().Replace(" ", "_"));
        }

        private void visualizarPartidaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form frmSelecao = new frmSelecao(PreenchePartida, this);
            frmSelecao.ShowDialog();
        }

        public void PreenchePartida(Partida partida)
        {

        }
    }
}
