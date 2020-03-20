﻿using System;
using Cerealization;
using System.Numerics;
using System.Text;
using System.Collections.Generic;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Cerealize cc = new Cerealize();
            MoveMsg msg = new MoveMsg(2);

            //Mensaje MSG = new Mensaje();
            Translate T = new Translate();

            Console.ReadKey();

            msg.msgType = 7;
            msg.pos = new Vector3(5.555555f);
            msg.playerRotation = new Quaternion(msg.pos, 7.444444f);
            msg.cameraRotation = new Quaternion(msg.pos, 3.888888f);
            byte[] bmsg = T.SerializeMSG(msg);
            MoveMsg tmsg = T.DeserializeMMSG(bmsg);

            Console.WriteLine("{0} {1} {2} {3:0.####} {4:0.####} {5:0.####} ",
                tmsg.msgType, tmsg.from, tmsg.pos, tmsg.playerRotation, tmsg.cameraRotation);

            //Console.WriteLine("{0:0.####} {1} {2}\n", MSG.pos, MSG.word, MSG.letter);
            //byte[] msg = T.SerializeMSG(MSG);
            //Console.WriteLine(msg.Length);
            //Mensaje MSGT = T.DeserializeMSG(msg);
            //Console.WriteLine("{0:0.####} {1} {2}\n", MSGT.pos, MSGT.word, MSGT.letter);
            Console.ReadKey();
        }
    }

    public class Mensaje
    {
        public float pos;
        public String word;
        public Char letter;
        public byte[] bytes;

        public Mensaje()
        {
            pos = 99.555555f;
            word = "Bird";
            letter = 'a';
        }
        public Mensaje(float pos, String word, Char letter)
        {
            this.pos = pos;
            this.word = word;
            this.letter = letter;
        }
        public Mensaje GetMensaje()
        {
            return this;
        }
    }

    public class Translate
    {
        public int[] sequence = { 0, 0, 0, 0, 0, 0, 0, 0 };
        public Translate()
        {

        }

        // Serialization Functions //////////////////
        //public byte[] SerializeMSG(Mensaje msg)
        //{
        //    float pos = msg.pos;
        //    String word = msg.word;
        //    Char letter = msg.letter;

        //    return BuildMSG(pos, word, letter);
        //}
        public byte[] SerializeMSG(Message msg)
        {
            byte[] header = Header(msg.msgType);
            byte[] body = BuildMSG( msg.from, msg.to);
            return Combine(IntByte((Int64)8+header.Length+body.Length),header,body);
        }
        public byte[] SerializeMSG(LoginMsg msg)
        {
            byte[] header = Header(msg.msgType);
            byte[] body = BuildMSG(msg.from);
            return Combine(IntByte((Int64)8+header.Length + body.Length), header, body);
        }
        public byte[] SerializeMSG(LogoutMsg msg)
        {
            byte[] header = Header(msg.msgType);
            byte[] body = BuildMSG(msg.from);
            return Combine(IntByte((Int64)8 + header.Length + body.Length), header, body);
        }
        public byte[] SerializeMSG(MoveMsg msg)
        {
            byte[] header = Header(msg.msgType);
            byte[] body = BuildMSG(msg.from, msg.pos,
                                    msg.playerRotation,
                                    msg.cameraRotation);
            return Combine(IntByte((Int64)8 + header.Length + body.Length), header, body);
        }
        public byte[] SerializeMSG(MoveVRMsg msg)
        {
            byte[] header = Header(msg.msgType);
            byte[] body = BuildMSG(msg.from);
            return Combine(IntByte((Int64)8 + header.Length + body.Length), header, body);
        }
        public byte[] SerializeMSG(ShootMsg msg)
        {
            byte[] header = Header(msg.msgType);
            byte[] body = BuildMSG(msg.from);
            return Combine(IntByte((Int64)8 + header.Length + body.Length), header, body);
        }
        public byte[] SerializeMSG(SnapshotMsg msg)
        {
            byte[] header = Header(msg.msgType);
            byte[] body = BuildMSG(msg.from, msg.to, msg.positions,
                                    msg.rotation,
                                    msg.camRotation);
            return Combine(IntByte((Int64)8 + header.Length + body.Length), header, body);
        }
        public byte[] SerializeMSG(StructureChangeMsg msg)
        {
            byte[] header = Header(msg.msgType);
            byte[] body = BuildMSG(msg.pos,msg.vertices,msg.triangles);
            return Combine(IntByte((Int64)8 + header.Length + body.Length), header, body);
        }
        public byte[] SerializeMSG(AddPlayer msg)
        {
            byte[] header = Header(msg.msgType);
            byte[] body = BuildMSG(msg.playerType);
            return Combine(IntByte((Int64)8 + header.Length + body.Length), header, body);
        }
        public byte[] SerializeMSG(TestMsg msg)
        {
            byte[] header = Header(msg.msgType);
            byte[] body = BuildMSG(msg.stuff);
            return Combine(IntByte((Int64)8 + header.Length + body.Length), header, body);
        }
        public byte[] SerializeMSG(BigTest msg)
        {
            byte[] header = Header(msg.msgType);
            byte[] body = BuildMSG(msg.userId, msg.positions, msg.rotation);
            return Combine(IntByte((Int64)8 + header.Length + body.Length), header, body);
        }

        // Construction Processing Functions ///////////
        private byte[] Header(Int32 msgType)
        {
            //Console.WriteLine(Convert.ToString(IntByte(msgType)[0], 2).PadLeft(8, '0'));
            //Console.WriteLine(Convert.ToString(IntByte(msgType)[1], 2).PadLeft(8, '0'));
            //Console.WriteLine(Convert.ToString(IntByte(msgType)[2], 2).PadLeft(8, '0'));
            //Console.WriteLine(Convert.ToString(IntByte(msgType)[3], 2).PadLeft(8, '0'));

            return Combine(IntByte(msgType)[0], Sequence(msgType));
        }
        private byte[] Sequence(Int32 msgType)
        {
            return IntByte(sequence[msgType++]);
        }
        //private byte[] BuildMSG(float pos, String word, Char letter)
        //{
        //    Int32 posi = (Int32)(pos * 1000);

        //    byte[] posb = BitConverter.GetBytes(posi);
        //    byte[] wordb = Encoding.ASCII.GetBytes(word);
        //    byte letterb = (byte)letter;

        //    return Combine(Combine(posb, wordb), letterb);
        //}
        private byte[] BuildMSG(Int32 from, Int32 to)
        {
            byte[] fromB = IntByte(from);
            byte[] toB = IntByte(to);
            byte[] body = Combine(fromB,toB);
            return body;
        }
        private byte[] BuildMSG(Int32 from) //also used for AddPlayer
        {
            return IntByte(from);
        }
        private byte[] BuildMSG(Int32 from, Vector3 pos, Quaternion pR, Quaternion cR)
        {
            byte[] fromB = IntByte(from);
            byte[] posB = Combine(FByte(pos.X), FByte(pos.Y), FByte(pos.X));
            byte[] pRB = Combine(FByte(pR.W), FByte(pR.X), FByte(pR.Y), FByte(pR.Z));
            byte[] cRB = Combine(FByte(cR.W), FByte(cR.X), FByte(cR.Y), FByte(cR.Z));
            byte[] body = Combine(fromB, posB, pRB, cRB);
            return body;
        }
        private byte[] BuildMSG(Int32 from, Int32 to, List<Vector3> pos, List<Quaternion> pR, List<Quaternion> cR)
        {
            byte[] fromB = IntByte(from);
            byte[] toB = IntByte(to);
            byte[] posB = Vec3Byte(pos);
            byte[] pRB = Combine(FByte(pR.W), FByte(pR.X), FByte(pR.Y), FByte(pR.Z));
            byte[] cRB = Combine(FByte(cR.W), FByte(cR.X), FByte(cR.Y), FByte(cR.Z));
            byte[] body = Combine(fromB, toB, posB, pRB, cRB);
            return body;
        }
        private byte[] BuildMSG(Vector3 pos, Vector3[] vertices, Int32[] triangles)
        {
            byte[] posB = Combine(FByte(pos.X), FByte(pos.Y), FByte(pos.X));
            byte[] verticiesB = Vec3Byte(vertices);
            byte[] trianglesB = IntByte(triangles);
            byte[] body = Combine(posB, verticiesB, trianglesB);
            return body;
        }
        private byte[] BuildMSG(Int32[] stuff)
        {
            return IntByte(stuff);
        }
        private byte[] BuildMSG(List<int> userID, List<Vector3> position, List<Quaternion> rotation)
        {
            byte[] userIDB = IntByte(userID);
            byte[] positionB = Vec3Byte(position);
            byte[] rotationB = QuatByte(rotation);
            byte[] body = Combine(userIDB, positionB, rotationB);
            return body;
        }

        // Deserialization Functions
        //public Mensaje DeserializeMSG(byte[] msg)
        //{
        //    byte[] pos = new byte[4];
        //    byte[] word = new byte[msg.Length - 5];
        //    byte[] letter = new byte[1];

        //    Array.Copy(msg, 0, pos, 0, 4);
        //    Array.Copy(msg, 4, word, 0, msg.Length - 5);
        //    Array.Copy(msg, msg.Length-1, letter, 0, 1);

        //    return GetMSG(pos,word,letter);
        //}
        public Message DeserializeMSG(byte[] msg)
        {
            Int64 msgSize = checkMSG(msg);
            if (msgSize != 0)
            {
                return GetMSG(msg, msgSize);
            }
            else
                return null;
        }
        public MoveMsg DeserializeMMSG(byte[] msg)
        {
            Int64 msgSize = checkMSG(msg);
            if (msgSize != 0)
            {
                return GetMMSG(msg, msgSize);
            }
            return null;
        }

        // Deconstruction Processing Functions
        //private Mensaje GetMSG(byte[] pos, byte[] word, byte[] letter)
        //{
        //    float posf = (float)BitConverter.ToInt32(pos, 0) / 1000;
        //    String wordS = Encoding.ASCII.GetString(word);
        //    Char letterC = Convert.ToChar(letter[0]);

        //    Mensaje msg = new Mensaje(posf, wordS, letterC);

        //    return msg;
        //}
        private Message GetMSG(byte[] msg, Int64 msgSize)
        {
            Console.WriteLine(msgSize);
            Int32 type = ByteInt32(msg[8]);
            byte[] seq_num = new byte[4];
            byte[] vars = new byte[msgSize-13];

            Array.Copy(msg, 9, seq_num, 0, 4);
            Array.Copy(msg, 13, vars, 0, msgSize - 13);

            Message MSG = MSGVars(vars);
            MSG.msgType = type;
            return MSG;
        }
        private MoveMsg GetMMSG(byte[] msg, Int64 msgSize)
        {
            Int32 type = ByteInt32(msg[8]);
            byte[] seq_num = new byte[4];
            byte[] vars = new byte[msgSize - 13];

            Array.Copy(msg, 9, seq_num, 0, 4);
            Array.Copy(msg, 13, vars, 0, msgSize - 13);

            MoveMsg MMSG = MMSGVars(vars);
            MMSG.msgType = type;
            return MMSG;
        }
        private LoginMsg GetLiMSG(byte[] msg, Int64 msgSize)
        {
            Int32 type = ByteInt32(msg[8]);
            byte[] seq_num = new byte[4];
            byte[] vars = new byte[msgSize - 13];

            Array.Copy(msg, 9, seq_num, 0, 4);
            Array.Copy(msg, 13, vars, 0, msgSize - 13);

            LoginMsg LiMSG = LiMSGVars(vars);
            LiMSG.msgType = type;
            return LiMSG;
        }
        private LogoutMsg GetLoMSG(byte[] msg, Int64 msgSize)
        {
            Int32 type = ByteInt32(msg[8]);
            byte[] seq_num = new byte[4];
            byte[] vars = new byte[msgSize - 13];

            Array.Copy(msg, 9, seq_num, 0, 4);
            Array.Copy(msg, 13, vars, 0, msgSize - 13);

            LogoutMsg LoMSG = LoMSGVars(vars);
            LoMSG.msgType = type;
            return LoMSG;
        }
        private MoveVRMsg GetMVRMSG(byte[] msg, Int64 msgSize)
        {
            Int32 type = ByteInt32(msg[8]);
            byte[] seq_num = new byte[4];
            byte[] vars = new byte[msgSize - 13];

            Array.Copy(msg, 9, seq_num, 0, 4);
            Array.Copy(msg, 13, vars, 0, msgSize - 13);

            MoveMsg MVRMSG = MVRMSGVars(vars);
            MMSG.msgType = type;
            return MVRMSG;
        }
        private Message MSGVars(byte[] vars)
        {
            byte[] from = new byte[4];
            byte[] pos = new byte[12];
            byte[] pR = new byte[16];
            byte[] cR = new byte[16];

            int index = 0;
            Console.WriteLine(vars.Length);
            Array.Copy(vars, index, from, 0, 4);
            index += 4;
            Array.Copy(vars, index, pos, 0, 12);
            index += 12;
            Array.Copy(vars, index, pR, 0, 16);
            index += 16;
            Array.Copy(vars, index, cR, 0, 16);

            MoveMsg msg = new MoveMsg(ByteInt32(from));
            msg.pos = ByteVec3(pos);
            msg.playerRotation = ByteQuat(pR);
            msg.cameraRotation = ByteQuat(cR);
          
            return msg;
        }

        // Check Message For Correct Length
        private Int64 checkMSG(byte[] msg)
        {
            byte[] size = new byte[8];
            Array.Copy(msg, 0, size, 0, 8);
            Int64 msgSize = ByteInt64(size);
            if (msg.Length == msgSize)
                return msgSize;
            else
                return 0;
        }

        // Byte Concatinating Functions ///////////////////
        //
        // Bytes = Bytes . Byte
        public byte[] Combine(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 0);
            newArray[newArray.Length-1] = newByte;
            return newArray;
        }
        // Bytes = Byte . Bytes
        public byte[] Combine(byte newByte, byte[] bArray)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 1);
            newArray[0] = newByte;
            return newArray;
        }
        // Bytes = Bytes . Bytes
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
            Buffer.BlockCopy(fourth, 0, ret, first.Length + second.Length + third.Length,
                              fourth.Length);
            return ret;
        }
        public static byte[] Combine(byte[] first, byte[] second, byte[] third, byte[] fourth, byte[] fifth)
        {
            byte[] ret = new byte[first.Length + second.Length + third.Length + fourth.Length + fifth.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, ret, first.Length + second.Length,
                             third.Length);
            Buffer.BlockCopy(fourth, 0, ret, first.Length + second.Length + third.Length,
                              fourth.Length);
            Buffer.BlockCopy(fifth, 0, ret, first.Length + second.Length + third.Length +
                              fourth.Length, fifth.Length);
            return ret;
        }

        // Byte Convertion Functions
        //
        // VarType to Byte Functions
        public byte[] IntByte(Int16 num)
        {
            return BitConverter.GetBytes(num);
        }
        public byte[] IntByte(Int32 num)
        {
            return BitConverter.GetBytes(num);
        }
        public byte[] IntByte(Int64 num)
        {
            return BitConverter.GetBytes(num);
        }
        public byte[] IntByte(Int32[] num)
        {
            return numB;
        }
        public byte[] IntByte(List<int> num)
        {
            //get list size
            //byte[] numB = new byte[list_size * 4];
            //for (int i=0; i<numB.Length; i + 4;
            //  numB[i,i+1,i+2,i+3] = num.next();
            return numB;
        }
        public byte[] FByte(float num)
        {
            Int32 numi = (Int32)(num * 1000);
            return BitConverter.GetBytes(numi);
        }
        public byte CharByte(Char letter)
        {
            return (byte)letter;
        }
        public byte[] CharByte(Char[] letters)
        {
            return Encoding.ASCII.GetBytes(letters);
        }
        public byte[] Vec3Byte(Vector3[] vec)
        {
            return vecB;
        }
        public byte[] Vec3Byte(List<Vector3> vec)
        {
            return vecB;
        }
        public byte[] QuatByte(List<Quaternion> quat)
        {
            return quatB;
        }

        // Byte to VarType Functions
        public Int32 ByteInt32(byte bite)
        {
            Int32 type = 0x00 << 24 |
                         0x00 << 16 |
                         0x00 << 8 | bite;
            return type;
        }
        public Int32 ByteInt32(byte[] bite)
        {
            return BitConverter.ToInt32(bite, 0);
        }
        public Int64 ByteInt64(byte[] bite)
        {
            return BitConverter.ToInt64(bite, 0);
        }
        public float ByteFloat(byte[] bite)
        {
            Console.WriteLine(bite.Length);
            return (float)BitConverter.ToInt32(bite, 0) / 1000;
        }
        public Char ByteChar(byte bite)
        {
            return (Char)bite;
        }
        public Char[] ByteChar(byte[] bite)
        {
            return Encoding.ASCII.GetString(bite).ToCharArray();
        }
        public Vector3 ByteVec3(byte[] bite)
        {
            Console.WriteLine(bite.Length);
            return new Vector3(ByteFloat(bite.Range(8, 11)),
                               ByteFloat(bite.Range(4, 7)),
                               ByteFloat(bite.Range(0, 3)));
        }
        public Quaternion ByteQuat(byte[] bite)
        {
            return new Quaternion(ByteFloat(bite.Range(12,15)),
                               ByteFloat(bite.Range(8, 11)),
                               ByteFloat(bite.Range(4, 7)),
                               ByteFloat(bite.Range(0, 3)));
        }

        
    }

    //https://www.dotnetperls.com/array-slice
    public static class Extensions
    {
        // Array Range Function
        public static T[] Range<T>(this T[] source, int start, int end)
        {
            if (end < 0)
            {
                end = source.Length + end;
            }
            int len = end - start + 1;

            T[] res = new T[len];

            for (int i = 0; i < len; i++)
            {
                res[i] = source[i + start];
            }
            return res;
        }
    }
}
