using System;
using Cerealization;
using System.Numerics;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Int32 seq_num = 100;

            Console.WriteLine("Hello World!");

            Cerealize cc = new Cerealize();

            Message ms = new Message();
            LoginMsg msg = new LoginMsg(2);
            MoveMsg mmsg = new MoveMsg(1);

            
            byte[] type = BitConverter.GetBytes(2);
            byte[] mesg = cc.CerealizeMSG(type[3], seq_num, msg);
            
            Console.ReadKey();
            Console.WriteLine(ms.byteToString(mesg));
            Console.ReadKey();

            ms.pos = new Vector3(5.0f);
            ms.playerRotation = new Quaternion(mmsg.pos, 5.0f);
            ms.cameraRotation = new Quaternion(mmsg.pos, 5.0f);

            Console.WriteLine(ms.pos);

            type = BitConverter.GetBytes(mmsg.msgType);
            byte[] mmesg = cc.CerealizeMSG(type[3], seq_num, ms);

            Console.ReadKey();
            Console.WriteLine(cc.ReadMessage(mmesg));
            Console.ReadKey();

            Message nuw = cc.ReadMessage(mmesg);

            Console.WriteLine(nuw.pos);
        }
    }
}
