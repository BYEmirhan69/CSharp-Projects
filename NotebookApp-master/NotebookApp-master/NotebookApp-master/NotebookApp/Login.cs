using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotebookApp
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            var notest = DataStore.Notes;
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {

            /* if (txtPasword.Text!="" && txtPasword.Text!=null && txtUserName.Text!=""&& txtUserName.Text !=null)
             {
                  Aşağıdaki if bloğunun içindeki koşulu bu şekilde de yazabiliriz. 
             }*/
            if (!string.IsNullOrEmpty(txtPasword.Text) && !string.IsNullOrEmpty(txtUserName.Text))
            {
                //kulanıcı girişinde bu şekilde yapmamız gerekiyor kullanıcıya kullanıcı ya da şifre başarısız olduğunu belirtirsek sistemde açık olabilir.
                //belirli yanlış denemeden sonra ne olursa olsun hesabı belirli süre kilitlemek
                //Antiforgery Token

                if (DataStore.AppUser.UserName==txtUserName.Text && DataStore.AppUser.Password==txtPasword.Text)
                {
                    //Form1 açmak için
                    Form1 form1 = new Form1();
                    form1.Show();
                    //
                    //Form1 açıldığında login ekranının kapatılması için
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Kullanıcı adı veya şifre hatalı!");
                }

            }
            else
            {
                MessageBox.Show("Alanları Boş Bırakmayınız.");
            }
        }

        private void txtUserName_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
