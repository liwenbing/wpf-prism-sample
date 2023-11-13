using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using Modbus.Device;
using Modbus.Extensions.Enron;
using Modbus.Utility;

namespace UI.Component.Communication
{
    public class ModbusHelper
    {
        private TcpClient tcpClient = null;
        private ModbusIpMaster master = null;
        private static Object TcpClientLock = new Object();

        private const int SendTimeout = 1000;
        private const int ReceiveTimeout = 1000;

        private const int SlaveId = 1;
        private const string RemoteIp = "127.0.0.1";
        private const int Ip = 502;

        //private static ModbusHelper _instance = new ModbusHelper();
        //private ModbusHelper()
        //{
        //    tcpClient = new TcpClient(RemoteIp, Ip);
        //    tcpClient.SendTimeout = SendTimeout;
        //    tcpClient.ReceiveTimeout = ReceiveTimeout;

        //    master = ModbusIpMaster.CreateIp(tcpClient);

        //}

        //public static ModbusHelper Instance { get { return _instance; } }

        public ModbusHelper()
        {
            tcpClient = new TcpClient(RemoteIp, Ip);
            tcpClient.SendTimeout = SendTimeout;
            tcpClient.ReceiveTimeout = ReceiveTimeout;

            master = ModbusIpMaster.CreateIp(tcpClient);

        }

        public void Write(ushort startAddress, ushort newValue)
        {
            Connect();

            master.WriteSingleRegister(SlaveId, (ushort)(startAddress - 1), newValue);
        }

        public void Write(ushort startAddress, int bitAddress, int oriByteValue, bool newVaue)
        {
            Connect();

            int mask = 1 << bitAddress;
            if (newVaue)
                oriByteValue |= mask;
            else
                oriByteValue &= ~mask;

            master.WriteSingleRegister(SlaveId, (ushort)(startAddress - 1), (ushort)oriByteValue);
        }

        public void Write(ushort startAddress, float newVaue)
        {
            Connect();

            byte[] bs = BitConverter.GetBytes(newVaue);
            ushort[] ushorts = ToUshortArray(bs);

            master.WriteMultipleRegisters(SlaveId, (ushort)(startAddress - 1), ushorts);
        }

        public ushort[] ReadPlc(ushort startPoint, ushort numberOfPoints)
        {
            Connect();

            return master.ReadHoldingRegisters(SlaveId, (ushort)(startPoint - 1), numberOfPoints);
        }

        public async Task<ushort[]> ReadPlcAsync(ushort startPoint, ushort numberOfPoints)
        {
            Connect();

            return await master.ReadHoldingRegistersAsync(SlaveId, (ushort)(startPoint - 1), numberOfPoints);
        }

        public async Task<List<Tuple<ushort, float>>> ReadPlcFloatAsync(ushort startPoint, ushort numberOfPoints)
        {
            Connect();

            List<Tuple<ushort, float>> tuples = new List<Tuple<ushort, float>>();
            var arrayShorts = await master.ReadHoldingRegistersAsync(SlaveId, (ushort)(startPoint - 1), numberOfPoints);
            for (ushort i = 0; i < arrayShorts.Length; i++)
            {
                if ((i + 1) % 2 == 0)
                {
                    tuples.Add(new Tuple<ushort, float>((ushort)(startPoint + i - 1), ToFloat(arrayShorts[i - 1], arrayShorts[i])));
                }
            }
            return tuples;
        }

        private void Connect()
        {
            if (tcpClient == null || tcpClient.Client == null || !tcpClient.Connected)
            {
                lock (TcpClientLock)
                {
                    if (tcpClient == null || tcpClient.Client == null || !tcpClient.Connected)
                    {
                        tcpClient = new TcpClient(RemoteIp, Ip);
                        tcpClient.SendTimeout = SendTimeout;
                        tcpClient.ReceiveTimeout = ReceiveTimeout;

                        master = ModbusIpMaster.CreateIp(tcpClient);
                    }
                }
            }
        }

        private ushort[] ToUshortArray(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            Array.Reverse(bytes, 0, bytes.Length);

            byte[] param = new byte[bytes.Length];
            Array.Copy(bytes, 0, param, param.Length - bytes.Length, bytes.Length);

            ushort[] ushorts = new ushort[param.Length >> 1];
            for (int n = 0; n < ushorts.Length; n++)
            {
                Array.Reverse(param, 2 * n, 2);
                ushorts[n] = (ushort)BitConverter.ToInt16(param, 2 * n);
            }
            return ushorts;
        }

        //public static float ToFloat(this ushort[] ushorts)
        //{
        //    return ModbusUtility.GetSingle(ushorts[0], ushorts[1]);
        //}

        public static float ToFloat(ushort highOrderValue, ushort lowOrderValue)
        {
            return ModbusUtility.GetSingle(highOrderValue, lowOrderValue);
        }
    }
}
