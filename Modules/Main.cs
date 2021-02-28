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
using Bot.Modules;

namespace Bot.Modules
{
    public class Main : ModuleBase
    {
        [Command("courses")]
        [Alias("cinfo", "courseinfo", "courselist", "list", "classes")]
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
        [Alias("information", "account")]
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
        [Alias("clear", "delete")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        public async Task Purge(int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);
        }

        [Command("add_user")]
        [Alias("a_u", "au", "add_usr", "a_usr", "adduser", "auser", "ausr")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task Add(string id, SocketGuildUser user)
        {
            Course course;
            course = ClassroomHandler.findCourseById(id);
            var role = Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals($"{course.Name}"));
            if (role == null)
            {
                await Context.Channel.SendMessageAsync($":warning: Роль для класса **{course.Name}** не найдена, будет создана новая");
                await Context.Guild.CreateRoleAsync(course.Name, null, null, false, null);
                role = Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals($"{course.Name}"));
            }
            await (user as IGuildUser).AddRoleAsync(role);
            await Context.Channel.SendMessageAsync($":white_check_mark: Роль **{course.Name}** успешно выдана {user.Mention}");
        }

        [Command("remove_user")]
        [Alias("r_u", "ru", "rem_user", "remove_user", "remove_usr", "removeuser", "reomve_user", "remove_usre")]
        [RequireUserPermission(GuildPermission.ManageRoles)]
        public async Task Remove(string id, SocketGuildUser user)
        {
            Course course;
            course = ClassroomHandler.findCourseById(id);
            var removerole = Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals($"{course.Name}")); //bool role = user.Roles.Any(r => r.Name == course.Name);
            if (!(user.Roles.Any(r => r.Name == course.Name)))
            {
                // The check doesn't work but the command works fine
                await Context.Channel.SendMessageAsync($":warning: У пользователя **{user.Mention}** нет роли **{course.Name}**.");
            } 
            else 
            {             
                //var removerole = Context.Guild.Roles.FirstOrDefault(x => x.Name.Equals($"{course.Name}"));
                await user.RemoveRoleAsync(removerole);
                await Context.Channel.SendMessageAsync($":white_check_mark: У пользователя **{user.Nickname}#{user.Discriminator}** удалена роль **{course.Name}**.");
            }
        }

        [Command("create_class")]
        [Alias("c_c", "cc", "cl", "create_c", "c_class")]
        public async Task CreateClass(string name, string section = "Section", string descriptionHeading = "Description heading", string description = "Description", string room = "Room")
        {
            ClassroomHandler.CreateClass(name, section, descriptionHeading, description, room);
            Course thisCourse = ClassroomHandler.findCourseByName(name);
            var builder = new EmbedBuilder()
                .WithTitle($"**:tada: Создан новый класс!**")
                .WithDescription($":teacher: Создана пользователем {Context.Message.Author.Mention}")
                .AddField($":classical_building: Название:", thisCourse.Name)
                .AddField($":trophy: Раздел:", thisCourse.Section)
                .AddField($":placard: Заголовок описания", thisCourse.DescriptionHeading)
                .AddField($":bookmark_tabs: Описание:", thisCourse.Description)
                .AddField($":door: Комната:", thisCourse.Room)
                .AddField($":id: ID:", thisCourse.Id)
                .WithColor(0x008F00)
                .WithCurrentTimestamp();
            var embed = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, embed);
        }

        [Command("class")]
        [Alias("classinfo", "classembed", "classs")]
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
        [Alias("cnnnct", "update", "reconnect")]
        public async Task Connect()
        {
            ClassroomHandler.ConnectClassroom();
            await Context.Channel.SendMessageAsync("Подключение обновлено");
        }

        
    }
}
