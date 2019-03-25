using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BoxyBot
{
    public partial class AllMonstersForm : Form
    {
        public List<string> selectedMonsters;

        public AllMonstersForm(List<string> Monsters)
        {
            InitializeComponent();
            Monsters.Distinct();
            Monsters.Sort();
            foreach (var monster in Monsters)
            {
                this.monstersListBox.Items.Add(monster);
            }
            this.selectedMonsters = new List<string>();
            Console.Write("Monsters loaded...");
        }

        private void MonstersListBox_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            if (this.selectedMonsters.Contains(this.monstersListBox.Items[e.Index].ToString()) && e.NewValue == CheckState.Unchecked)
            {
                    this.selectedMonsters.RemoveAll(npc => npc == this.monstersListBox.Items[e.Index].ToString());
            }
            if (!this.selectedMonsters.Contains(this.monstersListBox.Items[e.Index].ToString()) && e.NewValue == CheckState.Checked)
            {
                    this.selectedMonsters.Add(this.monstersListBox.Items[e.Index].ToString());
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
