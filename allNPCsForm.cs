using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BoxyBot
{
    public partial class AllNPCsForm : Form
    {
        public List<string> selectedNPCs;  

        public AllNPCsForm(List<string> NPCs)
        {
            InitializeComponent();
            NPCs.Distinct();
            NPCs.Sort();
            foreach (var npc in NPCs)
            {
                this.npcsListBox.Items.Add(npc);
            }
            this.selectedNPCs = new List<string>();
            Console.Write("NPCs loaded.");
        }

        private void NpcsListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (this.selectedNPCs.Contains(this.npcsListBox.Items[e.Index].ToString()) && e.NewValue == CheckState.Unchecked)
            {
                    this.selectedNPCs.RemoveAll(npc => npc == this.npcsListBox.Items[e.Index].ToString());
            }
            if (!this.selectedNPCs.Contains(this.npcsListBox.Items[e.Index].ToString()) && e.NewValue == CheckState.Checked)
            {
                    this.selectedNPCs.Add(this.npcsListBox.Items[e.Index].ToString());
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
