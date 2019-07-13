using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MMC = MementoConnection.MMConnection;

namespace tmpApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        

        private void button1_Click(object sender, EventArgs e)
        {
            int id = int.Parse(textBox1.Text);
            List<string> matches = MMC.GetItems<string>(
                "select MatchChar from info_room_type_matches where RTID=" + id);
            textBox2.Text = string.Join(",", matches.ToArray());
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            DataTable dt = MMC.Select("select * from info_room_types");
            if (dt.Rows.Count > 0)
            {
                var types = new List<string>();
                for(int i = 0; i < dt.Rows.Count; i++)
                {
                    var row = dt.Rows[i];
                    var items = new List<string>();
                    items.Add(row[2].ToString().Replace(" ",""));
                    items.Add(row[3].ToString().Trim());
                    items.Add(row[4].ToString().Trim());
                    items.Add(row[5].ToString().Trim());
                    var matches = MMC.GetItems<string>(
                        "select MatchChar from info_room_type_matches where RTID=" 
                        + row[1].ToString());
                    string line = string.Join(",", items.ToArray()) +"&"+
                        string.Join(",", matches.ToArray());
                    types.Add(line);
                }
                textBox2.Text = string.Join("|", types.ToArray());
            }
        }
    }
}
