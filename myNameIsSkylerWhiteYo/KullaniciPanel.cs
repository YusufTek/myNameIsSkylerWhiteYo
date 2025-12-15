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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace myNameIsSkylerWhiteYo
{
    public partial class KullaniciPanel : Form
    {
        string connString = ConfigurationManager
                            .ConnectionStrings["MyDB"]
                            .ConnectionString;
        public KullaniciPanel()
        {
            InitializeComponent();
            dataGridView1.Visible = false;

            SehirleriYukle();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string kalkis = cmbKalkis.Text;
            string varis = cmbVaris.Text;

            UcuslariListele(kalkis, varis, dateTimePicker1.Value);
            dataGridView1.Visible = true;
        }

        private void UcuslariListele(string kalkis, string varis, DateTime tarih)
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // Tarihi dd-MM-yyyy formatına çevir (database formatı)
                string tarihStr = tarih.ToString("dd-MM-yyyy");

                string query = @"SELECT KalkisNoktasi AS 'Nereden', 
                                   VarisNoktasi AS 'Nereye', 
                                   Tarih AS 'Tarih', 
                                   Saat AS 'Saat' 
                           FROM UcusBilgi 
                           WHERE KalkisNoktasi = @Kalkis 
                           AND VarisNoktasi = @Varis 
                           AND Tarih = @Tarih
                           ORDER BY Saat";

                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Kalkis", kalkis);
                cmd.Parameters.AddWithValue("@Varis", varis);
                cmd.Parameters.AddWithValue("@Tarih", tarihStr);

                SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                dataGridView1.DataSource = dt;

                // ucusSaat'e saatleri yükle
                SaatGoster();

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Bu kriterlere uygun uçuş bulunamadı!");
                }
            }
        }

        private void SaatGoster()
        {
            if (cmbKalkis.SelectedIndex == -1 || cmbVaris.SelectedIndex == -1)
                return;

            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // Tarihi dd-MM-yyyy formatına çevir
                string tarihStr = dateTimePicker1.Value.ToString("dd-MM-yyyy");

                string saatQuery = @"SELECT Saat
                             FROM UcusBilgi
                             WHERE KalkisNoktasi = @Kalkis
                             AND VarisNoktasi = @Varis
                             AND Tarih = @Tarih
                             ORDER BY Saat";

                using (SQLiteCommand cmd = new SQLiteCommand(saatQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Kalkis", cmbKalkis.Text);
                    cmd.Parameters.AddWithValue("@Varis", cmbVaris.Text);
                    cmd.Parameters.AddWithValue("@Tarih", tarihStr);

                    SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
                    DataTable dtSaat = new DataTable();
                    da.Fill(dtSaat);

                    ucusSaat.DataSource = dtSaat;
                    ucusSaat.DisplayMember = "Saat";
                }
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void SehirleriYukle()
        {
            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string query = "SELECT SehirID, Sehir FROM Sehirler";
                SQLiteDataAdapter da = new SQLiteDataAdapter(query, conn);
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

        private void yolcuKayit_Click(object sender, EventArgs e)
        {
            if (!BoslukKontrol())
            {
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                // Tarihi dd-MM-yyyy formatında kaydet
                string tarihStr = dateTimePicker1.Value.ToString("dd-MM-yyyy");

                string query = @"INSERT INTO YolcuBilgi (Isim, Soyisim, TC, Cinsiyet, Telefon, 
                                                 KalkisNoktasi, VarisNoktasi, Tarih, Saat) 
                        VALUES (@Isim, @Soyisim, @TC, @Cinsiyet, @Telefon, 
                                @Kalkis, @Varis, @Tarih, @Saat)";

                SQLiteCommand cmd = new SQLiteCommand(query, conn);
                cmd.Parameters.AddWithValue("@Isim", textBox1.Text);
                cmd.Parameters.AddWithValue("@Soyisim", textBox3.Text);
                cmd.Parameters.AddWithValue("@TC", textBox2.Text);
                cmd.Parameters.AddWithValue("@Cinsiyet", radioButton1.Checked ? "Erkek" : "Kadın");
                cmd.Parameters.AddWithValue("@Telefon", maskedTextBox1.Text);
                cmd.Parameters.AddWithValue("@Kalkis", cmbKalkis.Text);
                cmd.Parameters.AddWithValue("@Varis", cmbVaris.Text);
                cmd.Parameters.AddWithValue("@Tarih", tarihStr);
                cmd.Parameters.AddWithValue("@Saat", ucusSaat.Text);

                cmd.ExecuteNonQuery();
                MessageBox.Show("Yolcu kaydı başarıyla oluşturuldu!");
            }
        }

        private bool BoslukKontrol()
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text) ||
                string.IsNullOrWhiteSpace(textBox2.Text) ||
                string.IsNullOrWhiteSpace(textBox3.Text) ||
                string.IsNullOrWhiteSpace(maskedTextBox1.Text) ||
                string.IsNullOrWhiteSpace(cmbKalkis.Text) ||
                string.IsNullOrWhiteSpace(cmbVaris.Text) ||
                string.IsNullOrWhiteSpace(ucusSaat.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!");
                return false;
            }
            return true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}