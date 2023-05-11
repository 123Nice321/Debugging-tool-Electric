using CefSharp;
using CefSharp.WinForms;
using MSTSCLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using 测试工具助手.MyClass;
using 测试工具助手.Properties;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace 测试工具助手
{
    public partial class 测试工具合集 : Form
    {
        public 测试工具合集()
        {
            InitializeComponent();
            //获取当前计算机的串口端口名的数组 并写入到 com串口名的集合（Items）中
            com串口名.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
        }
        private void toolStripStatusLabel2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://wangyan.pip51.cn:13300");
        }



        #region ASCII转换进制转换
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                string str = textBox1.Text.Replace(" ", "");
                int k = 0;//字节移动偏移量
                byte[] buffer = new byte[str.Length / 2];//存储变量的字节
                for (int i = 0; i < str.Length / 2; i++)
                {
                    //每两位合并成为一个字节
                    buffer[i] = byte.Parse(str.Substring(k, 2), System.Globalization.NumberStyles.HexNumber);
                    k = k + 2;
                }
                //将字节转化成汉字
                textBox2.Text = Encoding.Default.GetString(buffer);
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }//ASCII转字符串

        private void button2_Click(object sender, EventArgs e)
        {
            try
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

                    textBox2.Text += Convert.ToString(buffer[i], 16).ToUpper() + " ";

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
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
            try
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }//UNICODE转字符串

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                byte[] bytes = Encoding.Unicode.GetBytes(textBox8.Text);
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i += 2)
                {
                    stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
                }
                textBox9.Text = stringBuilder.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }//字符串转UNICODE
        #endregion

        #region 简单串口调试工具
        private long receive_count = 0; //接收字节计数, 作用相当于全局变量
        private long send_count = 0;//发送字节计数, 作用相当于全局变量
        private StringBuilder sb = new StringBuilder();     //为了避免在接收处理函数中反复调用，依然声明为一个全局变量
        private DateTime current_time = new DateTime();    //为了避免在接收处理函数中反复调用，依然声明为一个全局变量

        private void Form1_Load(object sender, EventArgs e)
        {
            int i;
            for (i = 300; i <= 38400; i = i * 2) //循环
            {
                com波特率.Items.Add(i.ToString());//添加某项到项目的列表
            }

            string[] bund = { "4300", "56000", "57600", "115200", "128000", "230400", "256000", "460800" };
            com波特率.Items.AddRange(bund);//添加数组到项目的列表

            string[] pari = { "无校验", "偶校验", "奇校验", "Mark", "Space" };
            com校验位.Items.AddRange(pari);

            string[] stop = { "1位", "2位" };
            com停止位.Items.AddRange(stop);

            string[] data = { "7", "8" };
            com数据位.Items.AddRange(data);

            //初始化端口参数
            com串口名.Text = "";
            com波特率.Text = "9600";
            com数据位.Text = "8";
            com校验位.Text = "偶校验";
            com停止位.Text = "2位";

            //groupBox8.Enabled = false;
            //groupBox9.Enabled = false;
            groupBox6.Enabled = false; 

            but连接串口按钮.BackColor = Color.ForestGreen;//改变按钮背景色颜色

            label5.Text = "串口已关闭";
            label5.ForeColor = Color.Red;

        }

        private void but连接串口按钮_Click(object sender, EventArgs e)
        {
            try //将可能产生的异常代码放置在try块中
            {
                if (serialPort1.IsOpen)  //根据当前串口属性来判断是否打开
                {
                    serialPort1.Close();//关闭串口
                    but连接串口按钮.Text = "打开串口";
                    but连接串口按钮.BackColor = Color.ForestGreen;//改变按钮背景色颜色
                    //groupBox8.Enabled = false;
                    //groupBox9.Enabled = false;
                    groupBox6.Enabled = false;

                    com串口名.Enabled = true;
                    com波特率.Enabled = true;
                    com数据位.Enabled = true;
                    com校验位.Enabled = true;
                    com停止位.Enabled = true;
                    //textBox_receive.Text = "";  //清空接收区
                    textBox_send.Text = "";     //清空发送区
                    receive_count = 0;          //计数清零
                    label12.Text = "Rx:" + receive_count.ToString() + "Bytes";   //刷新界面
                    label6.Text = "Tx:" + receive_count.ToString() + "Bytes";   //刷新界面
                    label5.Text = "串口已关闭";
                    label5.ForeColor = Color.Red;

                }
                else
                {
                    serialPort1.PortName = com串口名.Text;
                    serialPort1.BaudRate = Convert.ToInt32(com波特率.Text);
                    serialPort1.DataBits = Convert.ToInt32(com数据位.Text);

                    if (com校验位.Equals("无校验"))
                        serialPort1.Parity = System.IO.Ports.Parity.None;//设置为 无校验
                    else if (com校验位.Equals("奇校验"))
                        serialPort1.Parity = System.IO.Ports.Parity.Odd;//设置为 奇校验
                    else if (com校验位.Equals("偶校验"))
                        serialPort1.Parity = System.IO.Ports.Parity.Even;//设置为 偶校验
                    else if (com校验位.Equals("Mark"))
                        serialPort1.Parity = System.IO.Ports.Parity.Mark;//将 奇偶校验设置为 1
                    else if (com校验位.Equals("Space"))
                        serialPort1.Parity = System.IO.Ports.Parity.Space;//将 奇偶校验设置为 0

                    if (com停止位.Equals("1位"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.One;
                    else if (com停止位.Equals("2位"))
                        serialPort1.StopBits = System.IO.Ports.StopBits.Two;

                    serialPort1.Open(); //打开串口
                    but连接串口按钮.Text = "关闭串口";
                    but连接串口按钮.BackColor = Color.Firebrick;////改变按钮背景色颜色
                    //groupBox8.Enabled = true;
                    //groupBox9.Enabled = true;
                    groupBox6.Enabled = true;
                    com串口名.Enabled = false;
                    com波特率.Enabled = false;
                    com数据位.Enabled = false;
                    com校验位.Enabled = false;
                    com停止位.Enabled = false;

                    label5.Text = "串口已打开";
                    label5.ForeColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button7_Click(object sender, EventArgs e)//发送按钮
        {
            byte[] temp = new byte[1];
            try
            {
                //首先判断串口是否开启
                if (serialPort1.IsOpen)
                {
                    int num = 0;   //获取本次发送字节数
                     //串口处于开启状态，将发送区文本发送
                    //判断发送模式
                    if (radioButton3.Checked)
                    {
                        //以HEX模式发送
                        //首先需要用正则表达式将用户输入字符中的十六进制字符匹配出来
                        string buf = textBox_send.Text;
                        string pattern = @"\s";
                        string replacement = "";
                        Regex rgx = new Regex(pattern);
                        string send_data = rgx.Replace(buf, replacement);

                        //不发送新行
                        num = (send_data.Length - send_data.Length % 2) / 2;
                        for (int i = 0; i < num; i++)
                        {
                            temp[0] = Convert.ToByte(send_data.Substring(i * 2, 2), 16);
                            serialPort1.Write(temp, 0, 1);  //循环发送
                        }
                        //如果用户输入的字符是奇数，则单独处理
                        if (send_data.Length % 2 != 0)
                        {
                            temp[0] = Convert.ToByte(send_data.Substring(textBox_send.Text.Length - 1, 1), 16);
                            serialPort1.Write(temp, 0, 1);
                            num++;
                        }
                        //判断是否需要发送新行
                        if (checkBox2.Checked)
                        {
                            //自动发送新行
                            serialPort1.WriteLine("");
                        }
                    }
                    else
                    {
                        //以ASCII模式发送
                        //判断是否需要发送新行
                        if (checkBox2.Checked)
                        {
                            //自动发送新行
                            serialPort1.WriteLine(textBox_send.Text);
                            num = textBox_send.Text.Length + 2; //回车占两个字节
                        }
                        else
                        {
                            //不发送新行
                            serialPort1.Write(textBox_send.Text);
                            num = textBox_send.Text.Length;
                        }
                    }

                    send_count += num;      //计数变量累加
                    label6.Text = "Tx:" + send_count.ToString() + "Bytes";   //刷新界面
                }
            }
            catch (Exception ex)
            {
                //捕获到异常，创建一个新的对象，之前的不可以再用
                serialPort1 = new System.IO.Ports.SerialPort();
                //刷新COM口选项
                //comboBox1.Items.Clear();
                //comboBox1.Items.AddRange(System.IO.Ports.SerialPort.GetPortNames());
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                //button1.Text = "打开串口";
                //button1.BackColor = Color.ForestGreen;
                MessageBox.Show(ex.Message);
                //comboBox1.Enabled = true;
                //comboBox2.Enabled = true;
                //comboBox3.Enabled = true;
                //comboBox4.Enabled = true;
                //comboBox5.Enabled = true;
            }
        }

        private void serialPort1_DataReceived(object sender, SerialDataReceivedEventArgs e) //串口接收事件处理
        {

            int num = serialPort1.BytesToRead;      //获取接收缓冲区中的字节数
            byte[] received_buf = new byte[num];    //声明一个大小为num的字节数据用于存放读出的byte型数据

            receive_count += num;                   //接收字节计数变量增加nun
            serialPort1.Read(received_buf, 0, num);   //读取接收缓冲区中num个字节到byte数组中

            sb.Clear();     //防止出错,首先清空字符串构造器
            //                //遍历数组进行字符串转化及拼接
            //foreach (byte b in received_buf)
            //{
            //    sb.Append(b.ToString());
            //}
            if (radioButton2.Checked)

            {
                //选中HEX模式显示
                foreach (byte b in received_buf)
                {
                    sb.Append(b.ToString("X2") + ' ');    //将byte型数据转化为2位16进制文本显示,用空格隔开
                }
            }
            else
            {
                //选中ASCII模式显示
                sb.Append(Encoding.ASCII.GetString(received_buf));  //将整个数组解码为ASCII数组
            }

            try
            {
                //因为要访问UI资源，所以需要使用invoke方式同步ui
                this.Invoke((EventHandler)(delegate
                {
                    //textBox_receive.AppendText(serialPort1.ReadExisting());
                    if (checkBox1.Checked)
                    {
                        //显示时间
                        current_time = System.DateTime.Now;     //获取当前时间
                        textBox_receive.AppendText("调试时间 "+current_time.ToString("HH:mm:ss") + " 数据 " + sb.ToString()+"\r\n");

                    }
                    else
                    {
                        //不显示时间 
                        textBox_receive.AppendText(" 数据 " + sb.ToString()+"\r\n");
                    }
                    label12.Text = "Rx:" + receive_count.ToString() + "Bytes";
                }
                   )
                );

            }
            catch (Exception ex)
            {
                //响铃并显示异常给用户
                System.Media.SystemSounds.Beep.Play();
                MessageBox.Show(ex.Message);

            }
        }

        private void button6_Click(object sender, EventArgs e)//清空接收按钮
        {
            textBox_receive.Text = "";  //清空接收文本框
            textBox_send.Text = "";     //清空发送文本框
            receive_count = 0;          //计数清零
            label12.Text = "Rx:" + receive_count.ToString() + "Bytes";   //刷新界面
            label6.Text = "Tx:" + receive_count.ToString() + "Bytes";   //刷新界面
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                //自动发送功能选中,开始自动发送
                numericUpDown1.Enabled = false;     //失能时间选择
                timer1.Interval = (int)numericUpDown1.Value;     //定时器赋初值
                timer1.Start();     //启动定时器
                label5.Text = "串口已打开" + " 自动发送中...";
            }
            else
            {
                //自动发送功能未选中,停止自动发送
                numericUpDown1.Enabled = true;     //使能时间选择
                timer1.Stop();     //停止定时器
                label5.Text = "串口已打开";

            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //定时时间到
            button7_Click(button7, new EventArgs());    //调用发送按钮回调函数
        }


        #endregion

        #region CRC16计算器
        private void button8_Click(object sender, EventArgs e)
        {
            textBox10.Text = "";
            try
            {
                //byte[] c2 = CRC16Helper.CRC16("01 06 11 98 00 00");  //计算结果为   0x0D, 0x19
                byte[] c1 = CRC16Helper.CRC16(textBox11.Text);
                //选中HEX模式显示
                foreach (byte b in c1)
                {
                    textBox10.AppendText(b.ToString("X2") + ' ');    //将byte型数据转化为2位16进制文本显示,用空格隔开
                }
                //textBox10.Text = c1[1].ToString() + c1[0].ToString();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion

        #region Ping网络通断测试
        bool IsPingAllow = false;
        int PingCount = 0;
        int Interval = 1000;
        private void btnPing_Click(object sender, EventArgs e)
        {
            if (IsPingAllow == false)
            {
                btnPing.Text = "停止";
                PingCount = (int)nmPingCount.Value;
                Interval = (int)nmInterval.Value;
                IsPingAllow = true;
                Thread ThPing = new Thread(new ParameterizedThreadStart(PingThread));
                ThPing.IsBackground = true;
                ThPing.Start(txtServerIP.Text.Trim());
            }
            else
            {
                IsPingAllow = false;
                btnPing.Enabled = false;
            }
        }
        private void PingThread(object ServerIP)
        {
            txtCmd.Invoke(new MethodInvoker(delegate
            {
                txtCmd.AppendText("正在 Ping " + ServerIP.ToString() + "：\r\n");
            }));
            Ping p = new Ping();
            int pcount = 0;
            long MinRT = -1;
            long MaxRT = -1;
            long AvgRT = -1;
            long TotalRT = 0;
            int SuccessRT = 0;
            while (pcount < PingCount || PingCount == 0)
            {
                PingReply pr = p.Send(ServerIP.ToString());
                txtCmd.Invoke(new MethodInvoker(delegate
                {
                    if (pr.Status == IPStatus.Success)
                    {
                        SuccessRT++;
                        txtCmd.AppendText("来自 " + pr.Address.ToString() + " 的答复：字节=" + pr.Buffer.Length + " 时间=" + pr.RoundtripTime + " TTL=" + pr.Options.Ttl + "\r\n");
                        if (MinRT == -1 || pr.RoundtripTime < MinRT)
                        {
                            MinRT = pr.RoundtripTime;
                        }
                        if (MaxRT == -1 || pr.RoundtripTime > MaxRT)
                        {
                            MaxRT = pr.RoundtripTime;
                        }
                        TotalRT += pr.RoundtripTime;
                        AvgRT = TotalRT / SuccessRT;
                    }
                    else
                    {
                        txtCmd.AppendText(pr.Status.ToString() + "\r\n");
                    }
                }));
                pcount++;
                if (IsPingAllow == false)
                {
                    break;
                }
                Thread.Sleep(Interval);
            }
            this.Invoke(new MethodInvoker(delegate
            {
                txtCmd.AppendText("Ping统计信息：\r\n");
                txtCmd.AppendText("数据包：已发送 = " + pcount + ",已接收 = " + SuccessRT + ",丢失 = " + (pcount - SuccessRT) + " <" + (int)((1 - (float)SuccessRT / (float)pcount) * 100) + "%丢失>\r\n");
                txtCmd.AppendText("往返行程估计时间：最短 = " + MinRT + "ms,最长 = " + MaxRT + "ms,平均 = " + AvgRT + "ms\r\n\r\n");
                btnPing.Enabled = true;
                btnPing.Text = "Ping";
            }));
            IsPingAllow = false;
        }


        #endregion

        #region 局域网远程连接测试
        private void button9_Click(object sender, EventArgs e)
        {


            if (button9.Text == "连接")
            {
                if(tb_server.Text == "")
                {
                    MessageBox.Show("请输入一个远程主机名");
                    return;
                }
                if(tb_user.Text=="")
                {
                    MessageBox.Show("请输入登录用户名");
                    return;
                }
                if (tb_pwd.Text == "")
                {
                    MessageBox.Show("请输入密码");
                    return;
                }
                try
                {
                    this.Height = Screen.PrimaryScreen.Bounds.Height;  //获取屏幕分辨率
                    this.Width = Screen.PrimaryScreen.Bounds.Width;
                    IMsTscNonScriptable securd = (IMsTscNonScriptable)axMsRDPClient.GetOcx();
                    axMsRDPClient.UserName = tb_user.Text;  //远程桌面用户名
                    securd.ClearTextPassword = tb_pwd.Text;  //远程桌面密码
                    /// 等价于axMsRDPClient.AdvancedSettings7.ClearTextPassword = "1";  //远程桌面密码
                    axMsRDPClient.Server = tb_server.Text;  //远程桌面计算机名或IP
                    axMsRDPClient.AdvancedSettings7.RDPPort = 3389;  //远程桌面服务器端口号默认为3389
                    /// 此电脑需要与被远程电脑在同一局域网下
                    axMsRDPClient.FullScreen = true;  //控件全屏显示
                    axMsRDPClient.AdvancedSettings9.SmartSizing = true;  //控件随窗体自适应变化
                    axMsRDPClient.AdvancedSettings9.NegotiateSecurityLayer = true;  //安全检查
                    axMsRDPClient.AdvancedSettings7.EnableCredSspSupport = true;  //允许连接Win11系统
                    axMsRDPClient.ColorDepth = 64;  //显示色彩位数
                    /// 颜色位数可选 8,16,24,32,64等等，网络条件不好建议选择较低的色彩位数保证远程控制的流畅性
                    axMsRDPClient.Connect();  //连接远程桌面


                    button9.Text = "断开连接";
                    button9.BackColor = Color.Red;                
                }
                catch(Exception ex)
                {
                    MessageBox.Show("无法连接远程主机" + ex.Message);
                    return;
                }

            }
            else
            {
                button9.Text = "连接";
                button9.BackColor = Color.PaleGreen;
                try
                {
                    axMsRDPClient.Disconnect();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("断开连接失败" + ex.Message);
                    return;
                }
            }



        }

        #endregion

        #region 数据库测试备份及恢复

        MyMeans MyDataClass = new MyMeans();
        MyModule MyMC = new MyModule();
        public static DataSet MyDS_Grid;//用dataGridView1控件显示信息

        
        string SqlStr = "";//数据库连接信息
        private void but连接数据库_Click(object sender, EventArgs e)
        {
            try
            {
                SqlStr = "Server=" + text数据库名.Text.ToString() + ";User Id =" + text用户名.Text.ToString() + "; Pwd =" + text密码.Text.ToString() + ";DataBase = " + text数据表.Text.ToString() + ";";
                MyDataClass.con_open(SqlStr);
                if (MyMeans.My_con.State == ConnectionState.Open)//判断连接是否打开
                {
                    显示说明1.Text = "SQL server 数据库连接成功";
                    but连接数据库.Enabled = false;
                    but断开数据库.Enabled = true;
                    button2获取表名.Enabled = true;
                    数据库连接Box.Enabled = false;
                    dataGridView1.Enabled = true;
                    Box数据表.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void but断开数据库_Click(object sender, EventArgs e)
        {
            //string SqlStr = "Server = 服务器名 ; User Id = 用户名; Pwd = 用户密码; DataBase = 数据库名;";
            //SqlStr = "Server=" + text数据库名.Text.ToString() + ";User Id =" + text用户名.Text.ToString() + "; Pwd =" + text密码.Text.ToString() + ";DataBase = " + text数据表.Text.ToString() + ";";
            MyDataClass.con_open(SqlStr);//实例化SqlConnection数据库连接对象
            MyDataClass.con_close();
            if (MyMeans.My_con.State == ConnectionState.Closed) //判断连接是否关闭
            {
                显示说明1.Text = "SQL server 数据库已关闭";
                but断开数据库.Enabled = false;
                but连接数据库.Enabled = true;
                group查询功能.Enabled = false;
                group数据更改.Enabled = false;
                Box数据表.Enabled = false;
                dataGridView1.Enabled = false;
                button2获取表名.Enabled = false;
                数据库连接Box.Enabled = true;
                dataGridView1.DataSource = null;
            }
        }

        private void button2获取表名_Click(object sender, EventArgs e)
        {
            try
            {
                //string SqlStr = "Server=" + text数据库名.Text.ToString() + ";User Id =" + text用户名.Text.ToString() + "; Pwd =" + text密码.Text.ToString() + ";DataBase = " + text数据表.Text.ToString() + ";";

                MyDataClass.con_open(SqlStr);//实例化SqlConnection数据库连接对象

                //用dataGridView1控件显示全部信息                                                                   
                MyDS_Grid = MyDataClass.getDataSet(MyMeans.AllSql1, text数据表.Text.ToString());
                dataGridView1.DataSource = MyDS_Grid.Tables[0];
                dataGridView1.Enabled = true;
                dataGridView1.AutoGenerateColumns = true;  //是否自动创建列



            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button1查询内容_Click(object sender, EventArgs e)
        {
            try
            {
                MyDataClass.con_open(SqlStr);//实例化SqlConnection数据库连接对象

                //用dataGridView1控件显示全部信息                                                                   
                MyDS_Grid = MyDataClass.getDataSet(MyMeans.AllSql2+ textBox表名.Text.ToString(), textBox表名.Text.ToString());
                dataGridView1.DataSource = MyDS_Grid.Tables[0];
                dataGridView1.AutoGenerateColumns = true;  //是否自动创建列

            }


            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void butt查询单一数据_Click(object sender, EventArgs e)
        {
            try
            {
                MyDataClass.con_open(SqlStr);//实例化SqlConnection数据库连接对象

                //查找列数据数据，可能会出现问题，增加单引号
                string sq1 = "select * from " + textBox表名.Text + " Where " + text数据列名.Text + "='" + text实际数据.Text.Trim() + "'";
                //用dataGridView1控件显示全部信息                                                                   
                MyDS_Grid = MyDataClass.getDataSet(sq1, textBox表名.Text.ToString());
                dataGridView1.DataSource = MyDS_Grid.Tables[0];
                dataGridView1.AutoGenerateColumns = true;  //是否自动创建列

 
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void butt数据删除_Click(object sender, EventArgs e)
        {
            try
            {
                MyDataClass.con_open(SqlStr);//实例化SqlConnection数据库连接对象

                //列数据数据，可能会出现问题，增加单引号
                string sq1 = "delete from " + textBox表名.Text.ToString() + " Where " + text数据列名.Text.ToString() + "='" + text实际数据.Text.Trim() + "'";
                //用dataGridView1控件显示全部信息                                                                   
                MyDS_Grid = MyDataClass.getDataSet(sq1, textBox表名.Text.ToString());
                //dataGridView1.DataSource = MyDS_Grid.Tables[0];
                //dataGridView1.AutoGenerateColumns = true;  //是否自动创建列

                //调用按钮回调函数
                butt查询单一数据_Click(butt查询单一数据, new EventArgs());

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void but帮助说明_Click(object sender, EventArgs e)
        {
            MessageBox.Show("说明\n" +
               "1.输入服务器名称【打开Microsoft SQL Server Management Studio查看服务器名】\n" +
               "2.输入用户名【默认】 例如： sa\n" +
               "3.输入密码【默认】 例如：数字\n" +
               "4.输入数据表名【默认】 例如：SSPECM\n" +
               "5.点击【连接数据库】\n" +
               "6.连接成功后点击【显示全部表名】\n" +
               "7.下方表格出现所有表名，选择自己需要的双击复制表名到【数据表名文本框中】\n" +
               "8.点击【查询表内容】\n" +
               "9.输入关键字【数据表列名】+【数据】\n" +
               "10.点击【查询】可以查询目标数据；【删除】可以删除目标数据\n" +
               "关于：后期功能阶段性增加\n" +
               "WangYan 2022-09-09 V1.0");
        }
        private void button12_Click(object sender, EventArgs e)//备份按钮
        {
            string path = Path.GetDirectoryName(Application.StartupPath);//获取当前程序路径
            string folder = System.Environment.CurrentDirectory + "\\bar\\";
            string FilePath = Path.Combine(path, folder);//拼接路径
            string Str_dar = "";
            if (radioButton6.Checked == true)
            if (!Directory.Exists(FilePath))//判断当前路径下文件夹是否存在
            {
                Directory.CreateDirectory(FilePath);//如果不存在就创建目录                  
            }
            Str_dar = System.Environment.CurrentDirectory + "\\bar\\";

            if (radioButton5.Checked == true)
                Str_dar = textBox12.Text + "\\";
            if (textBox12.Text == "" & radioButton5.Checked == true)
            {
                MessageBox.Show("请选择备份数据库文件的路径。");
                return;
            }
            try
            {
                Str_dar = "backup database "+text数据表.Text.Trim()+" to disk='" + Str_dar + "数据库备份" + System.DateTime.Now.ToShortDateString().Replace("/", ".")  + MyMC.Time_Format(System.DateTime.Now.ToString()) + ".bak" + "'";
                MyDataClass.getsqlcom(Str_dar);
                MessageBox.Show("数据备份成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private void button11_Click(object sender, EventArgs e)//备份选择保存的文件夹目录按钮
        {
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
            {
                textBox12.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button15_Click(object sender, EventArgs e)////还原选择保存的文件夹目录按钮
        {
            openFileDialog1.Filter = "*.bak|*.bak";
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                textBox14.Text = openFileDialog1.FileName;
            }
        }

        private void button14_Click(object sender, EventArgs e)//还原按钮
        {
            if (textBox14.Text == "")
            {
                MessageBox.Show("请选择备份数据库文件的路径。");
                return;
            }
            try
            {
                if (MyMeans.My_con.State == ConnectionState.Open)
                {
                    MyMeans.My_con.Close();
                }
                SqlConnection Tem_con = new SqlConnection(MyMeans.M_str_sqlcon);
                Tem_con.Open();
                SqlCommand SQLcom = new SqlCommand("use master restore database " + textBox16.Text.Trim() + " from disk='"
                    + textBox14.Text.Trim() + "' with replace", Tem_con);
                SQLcom.ExecuteNonQuery();
                SQLcom.Dispose();
                Tem_con.Close();
                Tem_con.Dispose();
                MessageBox.Show("数据还原成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MyDataClass.con_open(MyMeans.M_str_sqlcon);
                MyDataClass.con_close();
                MessageBox.Show("为了避免数据丢失，在数据库还原后将关闭整个系统。");
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        #endregion


    }
}
