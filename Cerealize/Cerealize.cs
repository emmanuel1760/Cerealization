using System;
using System.IO;
using System.Numerics;
namespace Cerealization
{
    public enum Datatype
    {
        _integer = 1,
        _float = 2,
        _char = 3
    }

    public class Cerealize
    {
        byte[] header;
        byte[] body;
        byte[] data;
        byte[] msgBytes;
        Int16 int2;
        Int32 int4;
        Int64 int8;
        float f8;
        String msg;

        public Cerealize()
        {
            //MakeHeader(t,seq);
        }

        public byte[] CerealizeMSG(byte t, Int32 seq, Message obj)
        {
            MakeHeader(t, seq);
            this.body = MSG(obj);
            FinalBytes();
            return getMSG();
        }

        public byte[] CerealizeMSG(byte t, Int32 seq, LoginMsg obj)
        {
            MakeHeader(t, seq);
            this.body = liMSG(obj);
            FinalBytes();
            return getMSG();
        }
        public byte[] CerealizeMSG(byte t, Int32 seq, LogoutMsg obj)
        {
            MakeHeader(t, seq);
            loMSG(obj);
            FinalBytes();
            return getMSG();
        }
        public byte[] CerealizeMSG(byte t, Int32 seq, MoveMsg obj)
        {
            MakeHeader(t, seq);
            mvMSG(obj);
            FinalBytes();
            return getMSG();
        }

        public void IntByte(Int16 num)
        {
            byte[] bytes = BitConverter.GetBytes(num);
        }

        public byte[] IntByte(Int32 num)
        {
            byte[] bytes = BitConverter.GetBytes(num);
            return bytes;
            //Combine(body, bytes);
        }

        public void IntByte(Int64 num)
        {
            byte[] bytes = BitConverter.GetBytes(num);
        }

        public byte[] FByte(float num)
        {
            num = (Int32)(num * 1000);
            byte[] bytes = BitConverter.GetBytes(num);
            //Combine(body, bytes);
            return bytes;
        }

        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }

        public static byte[] Combine(byte[] first, byte[] second, byte[] third)
        {
            byte[] ret = new byte[first.Length + second.Length + third.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length,
                             third.Length);
            return ret;
        }

        public static byte[] Combine(byte[] first, byte[] second, byte[] third, byte[] fourth)
        {
            byte[] ret = new byte[first.Length + second.Length + third.Length + fourth.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length,
                             third.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length + third.Length,
                              fourth.Length);
            return ret;
        }

        private void MakeHeader(byte t, Int32 seq)
        {
            byte[] type = new byte[1];
            type[0] = t;

            this.header = Combine(type, BitConverter.GetBytes(seq));
        }

        private void FinalBytes()
        {
            Int64 msgSize = header.Length + body.Length + 8;
            msgBytes = BitConverter.GetBytes(msgSize);
            data = Combine(msgBytes, header, body);
        }


        //get header info and sends to correct parse function with msg parameters
        public Message ReadMessage(byte[] recMSG)
        {
            Message MSG = new Message();

            int bytesRead = 14;
            byte[] size = new byte[8];
            byte[] header = new byte[5];

            Stream s = new MemoryStream(recMSG);
            s.Position = 0;

            int sz = s.Read(size, 0, 7);

            int bsize = System.BitConverter.ToInt32(size, 0) - 13;

            //Header data: messagetype|
            //int hdr = s.Read(header, 8, 13);
            Array.Copy(recMSG,8,header,0,5);
            
            //message type
            byte[] MSGtype = { header[0],header[1],header[2],header[3] };

            //msgType defines what function to send data to
            switch (BitConverter.ToInt32(MSGtype,0))
            {
                case 1: MSG = parse(s,bytesRead,bsize,4,(int)Datatype._integer);
                    break;
                case 2: MSG = parse(s,bytesRead,bsize,4,(int)Datatype._float);
                    break;
                case 3: MSG = parse(s,bytesRead,bsize,1,(int)Datatype._char);
                    break;
                default: //Debug.Log("ReadMessage: message type error.\n");
                    break;
            }

            return MSG;
        }

        //integer parse function for types using the integer values
        public Message parse(Stream s, int bytesRead, int bsize, int bits, int datatype)
        {
            //Message mesag = new Message();
            int count = 0;
            byte[] data = new byte[bits];
            byte[][] bytes = new byte[bsize][];
            do {
                int dta = s.Read(data, bytesRead, bits);
                bytesRead += dta;
                bytes[count][0] = data[0];
                bytes[count][1] = data[1];
                bytes[count][2] = data[2];
                bytes[count][3] = data[3];
                count += 1;
            } while (bytesRead == bsize);

            switch (datatype)
            {
                case 1:
                case 2:
                    
                    Int32[] i= ProcessMessageInt(data.Length, bytes);
                    LoginMsg limsg = new LoginMsg(i[0]);
                    return limsg;
                case 3:
                    
                    Int32[] j = ProcessMessageInt(bits, bytes);
                    MoveMsg mmsg = new MoveMsg(j[0]);
                    float[] f = ProcessMessageFloat(data.Length, bytes, bits);
                    mmsg.pos = new Vector3(f[0], f[1], f[2]);
                    mmsg.playerRotation = new Quaternion(f[3], f[4], f[5], f[6]);
                    mmsg.cameraRotation = new Quaternion(f[7], f[8], f[9], f[10]);
                    return mmsg;
                default: //Debug.Log("Message parse: datatype error.\n");
                    break;
            }
            return null;
        }

        private Int32[] ProcessMessageInt(Int64 size, byte[][] data)
        {
            Int32[] variables = new Int32[size];
            for (int i = 0; i < size; i++)
            {
                byte[] integ = new byte[4];
                for (int j = 0; j < 4; j++)
                    integ[j] = data[i][j];
                variables[i] = BitConverter.ToInt32(integ, 0);
            }
            return variables;
        }

        private float[] ProcessMessageFloat(Int64 size, byte[][] data, int start)
        {
            float[] variables = new float[size];
            for (int i = start-1; i < size; i++)
            {
                byte[] flo = new byte[4];
                for (int j = 0; j < 4; j++)
                    flo[j] = data[i][j];
                variables[i] = (float)BitConverter.ToInt32(flo, 0) / 100;
            }
            return variables;
        }

        public byte[] MSG(Message obj)
        {
            byte[] from = IntByte(obj.from);
            byte[] pos = Combine(FByte(obj.pos.X), FByte(obj.pos.Y), FByte(obj.pos.X));
            byte[] rot = Combine(FByte(obj.playerRotation.W), FByte(obj.playerRotation.X), FByte(obj.playerRotation.Y), FByte(obj.playerRotation.Z));
            byte[] cameraRotation = Combine(FByte(obj.cameraRotation.W), FByte(obj.cameraRotation.X), FByte(obj.cameraRotation.Y), FByte(obj.cameraRotation.Z));
            byte[] MSG = Combine(from, pos, rot, cameraRotation);
            return MSG;
        }

        public byte[] liMSG(LoginMsg obj)
        {
            byte[] liMSG = IntByte(obj.from);
            return liMSG;
        }

        public byte[] loMSG(LogoutMsg obj)
        {
            byte[] loMSG = IntByte(obj.from);
            return loMSG;
        }

        public byte[] mvMSG(MoveMsg obj)
        {
            byte[] from = IntByte(obj.from);
            byte[] pos = Combine(FByte(obj.pos.X), FByte(obj.pos.Y), FByte(obj.pos.X));
            byte[] rot = Combine(FByte(obj.playerRotation.W), FByte(obj.playerRotation.X), FByte(obj.playerRotation.Y), FByte(obj.playerRotation.Z));
            byte[] cameraRotation = Combine(FByte(obj.cameraRotation.W), FByte(obj.cameraRotation.X), FByte(obj.cameraRotation.Y), FByte(obj.cameraRotation.Z));
            byte[] mvMSG = Combine(from, pos, rot, cameraRotation);
            return mvMSG;
        }

        public byte[] getMSG()
        {
            return data;
        }
    }
}
//Header
//  First 8 bytes - number of bytes in message including these 8 bytes
//  Second 1 byte - message type (0-255 is available)
//  third 4 bytes - number from msg sequence, each type is different
//Body
//  fourth body.Length - the payload, 4 byte variables only for now
//Footer (optional)

//Body format
    //login/logout
        //from
    //move
        //