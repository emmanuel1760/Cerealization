using System;
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

            Cerealize cc = new Cerealize();
            

            
            
            
            
            
            
            
            
            
            MoveMsg msg = new MoveMsg(2);

            Console.ReadKey();

            msg.msgType = 7;
            msg.pos = new Vector3(5.555555f);
            msg.playerRotation = new Quaternion(msg.pos, 7.444444f);
            msg.cameraRotation = new Quaternion(msg.pos, 3.888888f);

            Console.WriteLine("{0} {1} {2:0.####} {3:0.####} {4:0.####} ",
                msg.msgType, msg.from, msg.pos, msg.playerRotation, msg.cameraRotation);

            byte[] bmsg = cc.SerializeMSG(msg);

            MoveMsg tmsg = cc.DeserializeMMSG(bmsg);

            Console.WriteLine("{0} {1} {2:0.####} {3:0.####} {4:0.####} ",
                tmsg.msgType, tmsg.from, tmsg.pos, tmsg.playerRotation, tmsg.cameraRotation);

            Console.ReadKey();
        }
    }
}