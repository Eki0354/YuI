using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
using System.Runtime.InteropServices;

namespace SaleSystem
{
    /// <summary>
    /// 扫描枪工作类
    /// </summary>
    public class ForceGun
    {
        //串口引用
        SerialPort _serialPort;
        public SerialPort SerialPort
        {
            get
            {
                return _serialPort;
            }
        }

        //存储转换的数据值
        private string _code;
        public string Code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        public ForceGun()
        {
            _serialPort = new SerialPort();
        }

        //串口状态
        public bool IsOpen
        {
            get
            {
                return _serialPort != null && _serialPort.IsOpen;
            }
        }

        //打开串口
        public bool Open()
        {
            if (_serialPort == null)
                return this.IsOpen;

            if (_serialPort.IsOpen)
                this.Close();

            _serialPort.Open();
            _serialPort.DataReceived += _serialPort_DataReceived;

            return this.IsOpen;
        }

        //关闭串口
        public void Close()
        {
            _serialPort.Close();
        }

        //定入数据，这里没有用到
        private void WritePort(byte[] send, int offSet, int count)
        {
            if (IsOpen)
            {
                _serialPort.Write(send, offSet, count);
            }
        }

        //获取可用串口
        public string[] GetComName()
        {
            string[] names = null;
            try
            {
                names = SerialPort.GetPortNames(); // 获取所有可用串口的名字
            }
            catch (Exception)
            {
                return null;
            }
            return names;
        }

        //注册一个串口
        public void SerialPortValue(string portName, int baudRate)
        {
            //串口名
            _serialPort.PortName = portName;
            //波特率
            _serialPort.BaudRate = baudRate;
            //数据位
            _serialPort.DataBits = 8;
            //两个停止位
            _serialPort.StopBits = StopBits.One;
            //无奇偶校验位
            _serialPort.Parity = Parity.None;
            _serialPort.ReadTimeout = 100;
            //commBar._serialPort.WriteTimeout = -1;
        }

        void _serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            // 等待100ms，防止读取不全的情况  
            Thread.Sleep(100);
            byte[] m_recvBytes = new byte[_serialPort.BytesToRead];//定义缓冲区大小  
            int result = _serialPort.Read(m_recvBytes, 0, m_recvBytes.Length);//从串口读取数据  
            if (result <= 0)
                return;
            string strResult = Encoding.ASCII.GetString(m_recvBytes, 0, m_recvBytes.Length);//对数据进行转换  
            _serialPort.DiscardInBuffer();

            this.DataReceived?.Invoke(this, new SerialSortEventArgs() { Code = strResult });
        }

        public event EventHandler<SerialSortEventArgs> DataReceived;

        public class SerialSortEventArgs
        {
            public string Code { get; set; }
        }
    }
}