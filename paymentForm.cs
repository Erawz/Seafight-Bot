using System.Windows.Forms;

namespace BoxyBot
{
    public partial class PaymentForm : Form
    {
        public PaymentForm()
        {
            this.InitializeComponent();
            this.Text += " " + Program.fingerprint;
            this.keyTextbox.Text = Program.fingerprint;
            this.paymentBrowser.Navigate("http://sinlyu.me/boxybot/buy.php?key=" + Program.fingerprint);
        }
    }
}
