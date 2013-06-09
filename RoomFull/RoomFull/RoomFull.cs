using Mono.Cecil;
using ScrollsModLoader.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RoomFull
{
    public class RoomFull : BaseMod
    {
        public RoomFull()
        {
        }

        public override string GetName()
        {
            return "RoomFull";
        }

        public override int GetVersion()
        {
            return 1;
        }

        public override MethodDefinition[] GetHooks(TypeDefinitionCollection scrollsTypes, int version)
        {
            return new MethodDefinition[] { scrollsTypes["Communicator"].Methods.GetMethod("sendRequest", new Type[]{typeof(Message)}) };
        }

        public override void Init()
        {
        }

        public override bool BeforeInvoke(InvocationInfo info, out object returnValue)
        {
            returnValue = null;
            Message m = (Message)info.Arguments()[0];

            if (m is RoomEnterMessage && !(m is RoomEnterFreeMessage))
            {
                RoomEnterMessage rem = (RoomEnterMessage)m;

                RoomEnterMultiMessage remm = new RoomEnterMultiMessage(new String[]{ rem.roomName });
                App.Communicator.sendRequest((Message)remm);
                App.ArenaChat.ChatRooms.SetCurrentRoom(rem.roomName);
                return true;
            }

            return false;
        }

        public override void AfterInvoke(InvocationInfo info, ref object returnValue)
        {
            return;
        }
    }
}
