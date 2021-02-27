using System;
using System.Collections.Generic;
using System.Text;
using Discord.WebSocket;
using Discord;


namespace Bot.Common
{
    public class Mute
    {
        public SocketGuild guild;
        public SocketGuildUser user;
        public IRole role;
        public DateTime date;
    }
}
