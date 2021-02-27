using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Classroom.v1;
using Google.Apis.Classroom.v1.Data;
using Google.Apis.Services;
using Bot.Services;

namespace Bot.Modules
{
    public class Main : ModuleBase
    {
        [Command("ping")]
        public async Task Ping()
        {
            await Context.Channel.SendMessageAsync($"Pong in {Context.Channel.Name}!");
        }

        [Command("courses")]
        public async Task Courses()
        {
            List<Course> courseArray = new List<Course>();
            courseArray = ClassroomHandler.GetCourses();

            var builder = new EmbedBuilder()
                .WithTitle("Ваши классы")
                .WithDescription("Убедитесь, что вы вошли в свой аккаунт")
                .WithColor(new Color(0xEDB518))
                .WithTimestamp(DateTimeOffset.FromUnixTimeMilliseconds(1613635022395))
                .WithFooter(footer =>
                {
                    footer
                        .WithText("Discord & Google Classroom")
                        .WithIconUrl($@"https://lh3.googleusercontent.com/proxy/PhqjvfaYuC88ct8urIsJGYasfqfT_ISitoBiUp7GdhdcTseX9N6torDiAUW-H8qqCod5klrJhKis5j58_yRy_FyJc9UUH_NCWH2Ds0X489NiU9NrQkgxDbfnEjTsngvTmYp_WDRRYIpfZTOiVWXxWMoXzklvTXYp0qN0-gx7vCTA");
                })
                .WithThumbnailUrl("https://upload.wikimedia.org/wikipedia/commons/thumb/2/25/Google_Classroom_icon.svg/139px-Google_Classroom_icon.svg.png")
                .WithAuthor(author =>
                {
                    author
                        .WithName("Discord Classroom")
                        .WithUrl($@"https://upload.wikimedia.org/wikipedia/commons/5/53/Google_%22G%22_Logo.svg");

                });
            
            courseArray.ForEach(entry =>
            {
                builder.AddField($@":memo: Название: **{entry.Name}**", $@"**:id: ID:** {entry.Id}, **:door: Комната:** {entry.Room}");
            });


            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }

        [Command("info")]
        public async Task Info(SocketGuildUser user = null)
        {
            if (user == null)
            {
                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())
                    .WithDescription("Информация о вашем аккаунте")
                    .WithColor(0xEDB518)
                    .AddField(":id: **ID**", Context.User.Id)
                    .AddField(":gem: **Имя пользователя:** ", Context.User.Username + "#" + Context.User.Discriminator)
                    .AddField(":calendar_spiral: **Аккаунт создан:**", Context.User.CreatedAt.ToString("dd/MM/yyyy"))
                    .AddField(":inbox_tray: **Присоединился к серверу**", (Context.User as SocketGuildUser).JoinedAt.Value.ToString("dd/MM/yyyy"))
                    .AddField("**Роли**:", string.Join(" ", (Context.User as SocketGuildUser).Roles.Select(x => x.Mention)))
                    .WithCurrentTimestamp();

                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            } 
            else
            {
                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())
                    .WithDescription("Информация о вашем аккаунте")
                    .WithColor(0xEDB518)
                    .AddField(":id: **ID**", user.Id)
                    .AddField(":gem: **Имя пользователя:** ", user.Username + "#" + user.Discriminator)
                    .AddField(":calendar_spiral: **Аккаунт создан:**", user.CreatedAt.ToString("dd/MM/yyyy"))
                    .AddField(":inbox_tray: **Присоединился к серверу**", user.JoinedAt.Value.ToString("dd/MM/yyyy"))
                    .AddField("**Роли**:", string.Join(" ", user.Roles.Select(x => x.Mention)))
                    .WithCurrentTimestamp();

                var embed = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, embed);
            }
            
        }

        [Command("purge")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        }

        [Command("add_user")]
        public async Task Add(string id, SocketGuildUser user)
        {
            await Context.Channel.SendMessageAsync("Привет");
        }

        [Command("remove_user")]
        public async Task Remove(string id, SocketGuildUser user)
        {
            await Context.Channel.SendMessageAsync("Привет");
        }

        [Command("purge_class")]
        public async Task PurgeClass(string id)
        {
            await Context.Channel.SendMessageAsync("Привет");
        }

        [Command("class")]
        public async Task Class(string id)
        {
            Course myCourse = ClassroomHandler.ClassInformation(id);
            var builder = new EmbedBuilder()
                .WithTitle(myCourse.Name)
                .WithDescription(myCourse.Description)
                .WithColor(0x008F00)
                .AddField(":door: **Комната:** ", myCourse.Room)
                .AddField(":egg: **Код:**", myCourse.EnrollmentCode);

            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }

        [Command("connect")]
        public async Task Connect()
        {
            ClassroomHandler.ConnectClassroom();
            await Context.Channel.SendMessageAsync("Подключение обновлено");
        }

        /*[Command("mute")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Mute(SocketGuildUser user, int seconds, [Remainder]string reason = null)
        {
            if (Context.User.Username.Equals(""))
            {
                await Context.Channel.SendMessageAsync("Invalid User");
            }

            var role = (Context.Guild as IGuild).Roles.FirstOrDefault(x => x.Name == "Muted");
            if (role == null)
                role = await Context.Guild.CreateRoleAsync("Заглушен", new GuildPermissions(sendMessages: false), null, false, null);

            
            
        }*/
    }
}
