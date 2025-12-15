using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace myNameIsSkylerWhiteYo
{
    public partial class AdminPanel : Form
    {
        string connString = ConfigurationManager
                            .ConnectionStrings["MyDB"]
                            .ConnectionString;

        public AdminPanel()
        {
            InitializeComponent();
            System.Windows.Forms.ToolTip tt = new System.Windows.Forms.ToolTip();
            tt.SetToolTip(button6, "Çıkış");

            SehirleriYukle();
        }

        private void label1_Click(object sender, EventArgs e)
        {
            string kalkisIsim = cmbKalkis.Text;
            string varisIsim = cmbVaris.Text;
            string saat = maskedTextBox1.Text;

            UcusEkle(kalkisIsim, varisIsim, dateTimePicker1.Value, saat);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string kalkisIsim = cmbKalkis.Text;
            string varisIsim = cmbVaris.Text;
            string saat = maskedTextBox1.Text;

            UcusBul(kalkisIsim, varisIsim, dateTimePicker1.Value, saat);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UcuslariListele();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            UcusDuzenle();
        }
        private void label4_Click(object sender, EventArgs e)
        {
            string kalkisIsim = cmbKalkis.Text;
            string varisIsim = cmbVaris.Text;
            string saat = maskedTextBox1.Text;

            UcusSil(kalkisIsim, varisIsim, dateTimePicker1.Value, saat);
        }

        private void SehirleriYukle()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string query = "SELECT SehirID, Sehir FROM Sehirler";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    MessageBox.Show($"Toplam şehir sayısı: {dt.Rows.Count}");

                    // Kalkış için yeni BindingSource
                    cmbKalkis.DataSource = new BindingSource(dt, null);
                    cmbKalkis.DisplayMember = "Sehir";
                    cmbKalkis.ValueMember = "SehirID";

                    // Varış için ayrı BindingSource
                    cmbVaris.DataSource = new BindingSource(dt, null);
                    cmbVaris.DisplayMember = "Sehir";
                    cmbVaris.ValueMember = "SehirID";
                }
            }
        }


        private void UcusEkle(string kalkisNoktasi, string varisNoktasi, DateTime ucusTarih, string ucusSaat)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                string query = "INSERT INTO UcusBilgi (KalkisNoktasi, VarisNoktasi, Tarih, Saat) " +
                               "VALUES (@KalkisNoktasi, @VarisNoktasi, @Tarih, @Saat)";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@KalkisNoktasi", kalkisNoktasi);
                    cmd.Parameters.AddWithValue("@VarisNoktasi", varisNoktasi);
                    cmd.Parameters.AddWithValue("@Tarih", ucusTarih.ToString("dd-MM-yyyy"));
                    cmd.Parameters.AddWithValue("@Saat", ucusSaat);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Uçuş başarıyla eklenmiştir!");
                    }
                    else
                    {
                        MessageBox.Show("Uçuş eklenirken bir sorun oluştu.");
                    }
                }
            }
        }

        private void UcusBul(string kalkisNoktasi, string varisNoktasi, DateTime ucusTarih, string ucusSaat)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                    conn.Open();

                    string query = @"SELECT 
                            FROM UcusBilgi
                            WHERE KalkisNoktasi = @KalkisNoktasi
                            AND VarisNoktasi = @VarisNoktasi
                            AND date(Tarih) = date(@Tarih)
                            ORDER BY Saat";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@KalkisNoktasi", kalkisNoktasi);
                        cmd.Parameters.AddWithValue("@VarisNoktasi", varisNoktasi);
                        cmd.Parameters.AddWithValue("@Tarih", ucusTarih.ToString("dd-MM-yyyy"));
                        cmd.Parameters.AddWithValue("@Saat", ucusSaat);

                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Uçuş başarıyla eklenmiştir!");
                        }
                        else
                        {
                            MessageBox.Show("Uçuş eklenirken bir sorun oluştu.");
                        }
                    }
                }
            }
        private void UcuslariListele()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string query = "SELECT * FROM YolcuBilgi";
                using (SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
        }

        private void UcusDuzenle()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                string query = @"UPDATE YolcuBilgi
                             SET Isim=@Isim, 
                             Soyisim=@Soyisim, 
                             KalkisNoktasi=@KalkisNoktasi, 
                             VarisNoktasi=@VarisNoktasi, 
                             Tarih=@Tarih, 
                             Saat=@Saat
                             WHERE ID=@ID";

                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Isim", textBox5.Text);
                    cmd.Parameters.AddWithValue("@Soyisim", textBox6.Text);
                    cmd.Parameters.AddWithValue("@KalkisNoktasi", comboBox1.Text);
                    cmd.Parameters.AddWithValue("@VarisNoktasi", comboBox2.Text);
                    cmd.Parameters.AddWithValue("@Tarih", dateTimePicker1.Value.Date);
                    cmd.Parameters.AddWithValue("@Saat", maskedTextBox1.Text); // veya DateTime

                    cmd.Parameters.AddWithValue("@ID", textBox2.Text);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        private void UcusSil(string kalkisNoktasi, string varisNoktasi, DateTime ucusTarih, string ucusSaat)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string query = "INSERT INTO UcusBilgi (KalkisNoktasi, VarisNoktasi, Tarih, Saat) " +
                               "VALUES (@KalkisNoktasi, @VarisNoktasi, @Tarih, @Saat)";
                SQLiteCommand cmd = new SQLiteCommand(query, conn);

                cmd.Parameters.AddWithValue("@KalkisNoktasi", kalkisNoktasi);
                cmd.Parameters.AddWithValue("@VarisNoktasi", varisNoktasi);
                cmd.Parameters.AddWithValue("@Tarih", ucusTarih.ToString("dd-MM-yyyy"));
                cmd.Parameters.AddWithValue("@Saat", ucusSaat);

                cmd.ExecuteNonQuery();
            }
        }

        private void label3_Click_1(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label4_Click_1(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
