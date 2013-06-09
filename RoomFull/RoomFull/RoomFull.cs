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
            // hook to Communicator.sendRequest(Message msg);
            // only hook to the method with the Message parameter, don't care about the String version
            return new MethodDefinition[] { scrollsTypes["Communicator"].Methods.GetMethod("sendRequest", new Type[]{typeof(Message)}) };
        }

        public override void Init()
        {
        }

        public override bool BeforeInvoke(InvocationInfo info, out object returnValue)
        {
            returnValue = null;
            Message m = (Message)info.Arguments()[0];

            // check whether it's a RoomEnterMessage, leave the RoomEnterFreeMessage because
            // we still want to join the first free room upon starting the game
            if (m is RoomEnterMessage && !(m is RoomEnterFreeMessage))
            {
                RoomEnterMessage rem = (RoomEnterMessage)m;

                // construct new RoomEnterMultiMessage with desired room
                RoomEnterMultiMessage remm = new RoomEnterMultiMessage(new String[]{ rem.roomName });
                // and send the request to the server.
                App.Communicator.sendRequest((Message)remm);

                // add the room to the UI, this isn't done automatically after RoomEnterMulti, 
                // only after RoomEnter which this isn't.
                App.ArenaChat.ChatRooms.SetCurrentRoom(rem.roomName);

                // ... and stop the method from executing :)
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
