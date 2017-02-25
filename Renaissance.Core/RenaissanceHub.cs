using System;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace Renaissance.Core
{
    public class RenaissanceHub : Hub
    {
        [Authorize]
        public class LemonaHub : Hub
        {
            public override async Task OnConnected()
            {
                using (var context = new UserContext())
                {
                    var user = context.Users.Include(u => u.Rooms).SingleOrDefault(u => u.UserName == Context.User.Identity.Name);
                    if (user == null)
                    {
                        context.Users.Add(new User() { UserName = Context.User.Identity.Name });
                        await context.SaveChangesAsync();
                    }
                    else
                    {
                        await Task.WhenAll(user.Rooms.Select(room => Task.Run(() => Groups.Add(Context.ConnectionId, room.RoomName))));
                    }
                }
                await base.OnConnected();
            }

            public async Task JoinRoomAsync(string roomName)
            {
                using (var context = new UserContext())
                {
                    var room = context.Rooms.Find(roomName);
                    if (room != null)
                    {
                        var user = new User() { UserName = Context.User.Identity.Name };
                        context.Users.Attach(user);

                        room.Users.Add(user);
                        await context.SaveChangesAsync();

                        await Groups.Add(Context.ConnectionId, roomName);
                        Clients.OthersInGroup(roomName).addToRoom(Context.User.Identity.Name, roomName);
                    }
                }
            }

            public async Task LeaveRoomAsync(string roomName)
            {
                using (var context = new UserContext())
                {
                    var room = context.Rooms.Find(roomName);
                    if (room != null)
                    {
                        var user = new User() { UserName = Context.User.Identity.Name };
                        context.Users.Attach(user);

                        room.Users.Remove(user);
                        await context.SaveChangesAsync();

                        Clients.OthersInGroup(roomName).removeFromRoom(Context.User.Identity.Name);
                        await Groups.Remove(Context.ConnectionId, roomName);
                    }
                }
            }

            public async Task SendMsgAsync(string roomName, string messege)
            {
                using (var context = new UserContext())
                {
                    var room = context.Rooms.Find(roomName);
                    if (room != null)
                    {
                        room.Messages.Add(messege);
                        await context.SaveChangesAsync();
                        Clients.OthersInGroup(roomName).addChatMsg(Context.User.Identity.Name, roomName, messege);
                    }
                }
            }

            public async Task AddAdvertAsync(int whom, string text)
            {
                using (var context = new UserContext())
                {
                    var user = context.Users.SingleOrDefault(u => u.UserName == Context.User.Identity.Name);
                    if (user != null)
                    {
                        var advert = new Advert() { User = user, Whom = whom, Text = text };
                        user.Adverts.Add(advert);
                        await context.SaveChangesAsync();
                        Clients.Client(Context.ConnectionId).AddAdvertOn(advert.Id);
                    }
                }
            }

            public async Task UpdateAdvertAsync(int advertId, string text)
            {
                using (var context = new UserContext())
                {
                    var advert = context.Adverts.Find(advertId);
                    if (advert != null)
                    {
                        advert.Text = text;
                        await context.SaveChangesAsync();
                        Clients.Client(Context.ConnectionId).UpdateAdvertOn(advert.Id);
                    }
                }
            }

            public async Task RemoveAdvertAsync(int advertId)
            {
                using (var context = new UserContext())
                {
                    var advert = context.Adverts.Find(advertId);
                    if (advert != null)
                    {
                        context.Adverts.Remove(advert);
                        await context.SaveChangesAsync();
                        Clients.Client(Context.ConnectionId).RemoveAdvertOn(advert.Id);
                    }
                }
            }
        }
    }
}