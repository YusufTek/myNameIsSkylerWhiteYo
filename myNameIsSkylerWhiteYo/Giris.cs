using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


namespace myNameIsSkylerWhiteYo
{
    public partial class Giris : Form
    {
        string connString = ConfigurationManager
                            .ConnectionStrings["MyDB"]
                            .ConnectionString;
        public Giris()
        {
            InitializeComponent();
        }

        

        private void Giris_Load(object sender, EventArgs e)
        {

        }

        private void AdminBilgi(string adminAd, string adminSifre)
        {
            bool girisBasarili = false;

            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                string query = "SELECT * FROM AdminBilgi WHERE AdminAd=@user AND AdminSifre=@pass";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn)){
                    cmd.Parameters.AddWithValue("@user", adminAd);
                    cmd.Parameters.AddWithValue("@pass", adminSifre);

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        girisBasarili = dr.Read();
                    }
                }
                
            }
            if (girisBasarili) // Bir satır bulunduysa giriş BAŞARILI
            {
                MessageBox.Show("Giriş başarılı!");

                AdminPanel f1 = new AdminPanel();
                f1.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Hatalı kullanıcı adı veya şifre.");
            }

        }

        private void KullaniciBilgi(string kullaniciIsim, string kullaniciSoyisim)
        {
            bool girisBasarili = false;

            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();

                string query = "SELECT * FROM KullaniciBilgi WHERE kullaniciIsim=@isim AND kullaniciSoyisim=@soyisim";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@isim", kullaniciIsim);
                    cmd.Parameters.AddWithValue("@soyisim", kullaniciSoyisim);

                    using (SQLiteDataReader dr = cmd.ExecuteReader())
                    {
                        girisBasarili = dr.Read();
                    }
                }
            }
            if (girisBasarili)
            {
                MessageBox.Show("Giriş başarılı!");

                KullaniciPanel f2 = new KullaniciPanel();
                f2.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Hatalı kullanıcı adı veya şifre.");
            }
        }

        private void register_Click()
        {
            if(!BoslukKontrol())
            {
                return;
            }

            using (SQLiteConnection conn = new SQLiteConnection(connString))
            {
                conn.Open();
                string query = "INSERT INTO KullaniciBilgi (KullaniciIsim, KullaniciSoyisim, KullaniciSifre, TC, DogumTarihi) VALUES (@isim, @soyisim, @sifre, @TC, @dogumTarih)";
                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@isim", YeniIsim.Text);
                    cmd.Parameters.AddWithValue("@soyisim", YeniSoyisim.Text);
                    cmd.Parameters.AddWithValue("@sifre", YeniSifre.Text);
                    cmd.Parameters.AddWithValue("@TC", YeniTC.Text);
                    cmd.Parameters.AddWithValue("@dogumTarih", YeniDogumTarih.Text);
                    int rowaffected = cmd.ExecuteNonQuery();
                    if (rowaffected > 0)
                    {
                        MessageBox.Show("Kayıt başarılı");
                        tabControl1.SelectedTab = tabPage1;
                    }
                    else
                        MessageBox.Show("Kayıt başarısız");
                }
            }
        }

        private bool BoslukKontrol()
        {
            if (string.IsNullOrWhiteSpace(YeniIsim.Text) ||
                string.IsNullOrWhiteSpace(YeniSoyisim.Text) ||
                string.IsNullOrWhiteSpace(YeniDogumTarih.Text) ||
                string.IsNullOrWhiteSpace(YeniSifre.Text) ||
                string.IsNullOrWhiteSpace(YeniSifre2.Text) ||
                string.IsNullOrWhiteSpace(YeniTC.Text))
            {
                MessageBox.Show("Lütfen tüm alanları doldurunuz!");
                return false;
            }
            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage3;
        }

        private void AdminGiris_Click(object sender, EventArgs e)
        {
            AdminBilgi(adminAd.Text, adminSifre.Text);
        }

        private void registerPanel_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            register_Click();
        }

        private void kullaniciButton_Click_1(object sender, EventArgs e)
        {
            KullaniciBilgi(kullaniciIsim.Text, kullaniciSoyisim.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedTab = tabPage1;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
