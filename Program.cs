﻿/*
 * 由SharpDevelop创建。
 * 用户： rsa
 * 日期: 2016/11/2
 * 时间: 22:27
 * 
 * 要改变这种模板请点击 工具|选项|代码编写|编辑标准头文件
 */
using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Linq;
using System.Data;

namespace SharpLearn
{
	/// <summary>
	/// controller class
	/// </summary>
	public sealed class Program
	{
		static Program p = new Program();
		public static Utils.FormLog logger;
		int port = 8091;
		
		/// <summary>
		/// Program entry point.
		/// </summary>
		[STAThread]
		private static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			logger = new Utils.FormLog();
			Application.Run(new MainForm(p));
		}
		
		public void ForOthers()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			logger = new Utils.FormLog();
			Application.Run(new MainForm(p));
		}
		
		//send message by udp in lan
		public bool SendMessage(String ip, String message) {
			Socket s = null;
			try {
				IPAddress ipa = IPAddress.Parse(ip);
				IPEndPoint ipp = new IPEndPoint(ipa, port);
				s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
				               ProtocolType.Udp);
				byte[] bs = UTF8Encoding.Default.GetBytes(message);
				
				byte[] data = new byte[bs.Length + 1];
				new byte[]{(byte)bs.Length}.CopyTo(data,0);
				bs.CopyTo(data, 1);
				s.Connect(ipp);
				s.Send(data, data.Length, 0);
			} catch (Exception e) {
				MessageBox.Show(e.StackTrace);
			} finally {
				if(s != null) {
					s.Close();
				}
			}
			return true;
		}
		
		//listen port 8091 by udp
		public void StartListen(object textbox) {
			TextBox tb = textbox as TextBox;
			Socket s = null;
			try {
				IPAddress ipa = IPAddress.Parse("192.168.1.102");
				IPEndPoint ipp = new IPEndPoint(IPAddress.Any, port);
				s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram,
				               ProtocolType.Udp);
				s.Bind(ipp);
				while(true) {
					EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
					byte[] data = new byte[1024];
					s.ReceiveFrom(data, ref point);//接收数据报
					int length = data[0];
					byte[] message = new byte[length];
					Buffer.BlockCopy(data, 1, message, 0,message.Length);
					MessageBox.Show(UTF8Encoding.Default.GetString(message));
					tb.AppendText(UTF8Encoding.Default.GetString(message));
				}
			} catch (Exception e) {
				string error = e.StackTrace;
			} finally {
				if(s != null) {
					s.Close();
				}
			}
		}
		
		//receive message
		public void ReceiveMessage(object ss) {
			Socket s = (Socket)ss;
			byte[] data = new byte[1024];
			s.Receive(data, data.Length, 0);
			int length = data[0];
			byte[] message = new byte[length];
			Buffer.BlockCopy(data, 1, message, 0,message.Length);
			MessageBox.Show(UTF8Encoding.Default.GetString(message));
			s.Close();
		}
		
		public String Remove(String str) {
			return str.Replace(" ", "");
		}

        public static void LinqTest() {
            int[] array = { 1, 5, 2, 10, 7 };
            // Select squares of all odd numbers in the array sorted in descending order
            var results = from x in array
                          where x % 2 == 1
                          orderby x descending
                          select x * x;
            foreach (var result in results) {
                log(result.ToString());
            }
        }

        public static void DataColumnTest() {
            DataTable tb = new DataTable();
            tb.Columns.Add(new DataColumn("name", Type.GetType("System.String")));
            tb.Columns.Add(new DataColumn("id", Type.GetType("System.Double")));

            foreach(DataColumn d in tb.Columns) {
                log((d.DataType is System.Double).ToString());
                log(d.DataType.ToString());
                log((d.DataType.ToString() == "System.Double").ToString());
            }
        }

        public static void  ShowTable(DataTable tb) {
            System.IO.StreamWriter sw = new System.IO.StreamWriter("data.txt");
            foreach(DataColumn col in tb.Columns) {
                sw.Write("{0,-20}", col.ColumnName);
            }
            sw.WriteLine();
            foreach(DataRow dr in tb.Rows) {
                foreach(object o in dr.ItemArray) {
                    sw.Write("{0,-20}", o.ToString());
                }
                sw.WriteLine();
            }
            sw.Close();
        }

        public static int GetWeekdayNum(DateTime dt) {
            dt = new DateTime(dt.Year, dt.Month, 1);
            var monthDays = dt.Subtract(dt.AddMonths(1));
            int num = 0;
            for(int i = 1; i <= Math.Abs(monthDays.TotalDays) - 28; i++) {
                var dtTemp = new DateTime(dt.Year, dt.Month, 28 + i);
                if(dtTemp.DayOfWeek != DayOfWeek.Saturday && dtTemp.DayOfWeek != DayOfWeek.Sunday) {
                    num++;
                }
            }
            return 20 + num;
        }
        
		public static void log(String logMessage) {
			logger.appendLog(logMessage);

		}
	}
}
