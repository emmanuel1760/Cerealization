using System;
using Cerealization;
using System.Numerics;
using System.Text;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //Cerealize cc = new Cerealize();
            //Message ms = new Message();

            Mensaje MSG = new Mensaje();
            Translate T = new Translate();

            Console.ReadKey();
            Console.WriteLine("{0} {1} {2}\n",MSG.pos, MSG.word, MSG.letter);

            byte[] msg = T.SerializeMSG(MSG);

            Console.WriteLine(msg.Length);

            Mensaje MSGT = T.DeserializeMSG(msg);

            Console.WriteLine("{0} {1} {2}\n",MSGT.pos, MSGT.word, MSGT.letter);
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
            pos = 99.0f;
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
        public Translate()
        {

        }
        public byte[] SerializeMSG(Mensaje msg)
        {
            float pos = msg.pos;
            String word = msg.word;
            Char letter = msg.letter;

            return BuildMSG(pos,word,letter);
        }
        private byte[] BuildMSG(float pos, String word, Char letter)
        {
            Int32 posi = (Int32)(pos * 1000);

            byte[] posb = BitConverter.GetBytes(posi);
            byte[] wordb = Encoding.ASCII.GetBytes(word);
            byte letterb = (byte)letter;

            return Add_byteTobytes(Combine(posb, wordb), letterb);

        }
        public Mensaje DeserializeMSG(byte[] msg)
        {
            byte[] pos = new byte[4];
            byte[] word = new byte[msg.Length - 5];
            byte[] letter = new byte[1];

            Array.Copy(msg, 0, pos, 0, 4);
            Array.Copy(msg, 4, word, 0, msg.Length - 5);
            Array.Copy(msg, msg.Length-1, letter, 0, 1);

            return GetMSG(pos,word,letter);
        }
        private Mensaje GetMSG(byte[] pos, byte[] word, byte[] letter)
        {
            float posf = (float)BitConverter.ToInt32(pos, 0) / 1000;
            String wordS = Encoding.ASCII.GetString(word);
            Char letterC = Convert.ToChar(letter[0]);

            Mensaje msg = new Mensaje(posf, wordS, letterC);

            return msg;
        }
        public byte[] Add_byteTobytes(byte[] bArray, byte newByte)
        {
            byte[] newArray = new byte[bArray.Length + 1];
            bArray.CopyTo(newArray, 0);
            newArray[newArray.Length-1] = newByte;
            return newArray;
        }
        public static byte[] Combine(byte[] first, byte[] second)
        {
            byte[] ret = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, ret, 0, first.Length);
            Buffer.BlockCopy(second, 0, ret, first.Length, second.Length);
            return ret;
        }
    }
}
