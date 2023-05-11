using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ASCII转换进制转换
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string str = textBox1.Text.Replace(" ", ""); 
            int k = 0;//字节移动偏移量
            byte[] buffer = new byte[str .Length / 2];//存储变量的字节
            for (int i = 0; i < str.Length / 2; i++)
            {
                    //每两位合并成为一个字节
                    buffer[i] = byte.Parse(str.Substring(k, 2), System.Globalization.NumberStyles.HexNumber);
                    k = k + 2;
            }
            //将字节转化成汉字
            textBox2.Text = Encoding.Default.GetString(buffer);

        }//ASCII转字符串

        private void button2_Click(object sender, EventArgs e)
        {

            //这里我们将采用2字节一个汉字的方法来取出汉字的16进制码
            byte[] textbuf = Encoding.Default.GetBytes(textBox1.Text);
            //用来存储转换过后的ASCII码
            string textAscii = string.Empty;
            for (int i = 0; i < textbuf.Length; i++)
            {
                textAscii += textbuf[i].ToString("X");
            }
            textBox2.Text = "";
            int k = 0;//字节移动偏移量
            byte[] buffer = new byte[textAscii.Length / 2];//存储变量的字节
            for (int i = 0; i < textAscii.Length / 2; i++)
            {
                
                //每两位合并成为一个字节
                buffer[i] = byte.Parse(textAscii.Substring(k, 2), System.Globalization.NumberStyles.HexNumber);
                k = k + 2;

                textBox2.Text += Convert.ToString( buffer[i],16).ToUpper()+" ";

            }

        }//字符串转ASCII

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text == "二进制")
                {
                    string str = textBox7.Text;

                    textBox3.Text = Convert.ToString(str);
                    //textBox4.Text = Convert.ToString(int1,8);
                    textBox5.Text = Convert.ToString(Convert.ToInt32(str, 2));
                    textBox6.Text = Convert.ToString(Convert.ToInt32(textBox5.Text), 16).ToUpper();
                }
                //if (comboBox1.Text == "八进制")
                //{
                //    string str = textBox7.Text;
                //}
                if (comboBox1.Text == "十进制")
                {
                    string str = textBox7.Text;
                    textBox5.Text = str;//十进制转十进制数
                                        //textBox4.Text = Convert.ToInt32(str, 8).ToString();//十进制转八进制数
                    textBox3.Text = Convert.ToString(Convert.ToInt32(str), 2);//十进制转二进制数
                    textBox6.Text = Convert.ToString(Convert.ToInt32(str), 16).ToUpper();//十进制转二进制数
                }
                if (comboBox1.Text == "十六进制")
                {
                    string str = textBox7.Text;
                    textBox5.Text = Convert.ToInt32(str, 16).ToString().ToUpper();//十六进制转十进制数
                                                                                  //textBox4.Text = Convert.ToInt32(textBox5.Text,8).ToString();//十进制转八进制数
                    textBox3.Text = Convert.ToString(Convert.ToInt32(textBox5.Text), 2);//十进制转二进制数
                    textBox6.Text = textBox7.Text.ToUpper();//十六进制转十六进制数
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }//进制转换

        private void button4_Click_1(object sender, EventArgs e)
        {
            string dst = "";
            string src = textBox8.Text;
            int len = src.Length / 6;
            for (int i = 0; i <= len - 1; i++)
            {
                string str = "";
                str = src.Substring(0, 6).Substring(2);
                src = src.Substring(6);
                byte[] bytes = new byte[2];
                bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());
                dst += Encoding.Unicode.GetString(bytes);
            }
            textBox9.Text = dst;

        }//UNICODE转字符串

        private void button5_Click(object sender, EventArgs e)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(textBox8.Text);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                 stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            textBox9.Text= stringBuilder.ToString();

        }//字符串转UNICODE
    }
}
