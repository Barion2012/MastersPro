using System;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace Agregator
{
    using Services;
    using Data;
    using Models;

    [Authorize(AuthenticationSchemes  = Schemes)]
    public class QLabHub : Hub
    {
        const string Schemes = "Identity.Application," + JwtBearerDefaults.AuthenticationScheme;

        private readonly UserManager<AgregatorUser> _userManager;

#if DESIGN
        private readonly ApplicationDbContext _dbContext;
        public QLabHub(
        ApplicationDbContext dbContext
#else
        private ApplicationStoreContext _dbContext;
        public QLabHub(
            ApplicationStoreContext dbContext
#endif
            ,UserManager<AgregatorUser> userManager)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        [AllowAnonymous]
        public async Task Send(string username, string message)
        {
            await this.Clients.All.SendAsync("Receive", username, message);
        }


        public async Task SendAll(string message)
        {
            var user = await _userManager.GetUserAsync(Context.User);

            await this.Clients.All.SendAsync("Receive", user.Name ?? user.UserName, message);
        }

        public async Task Register()
        {
            var user = await _userManager.GetUserAsync(Context.User);
            user.ClientID = Context.ConnectionId;
            await _userManager.UpdateAsync(user);

        }

        public async Task Notify(string vacancy_id, string message)
        {
            var user_id = (await _dbContext.Vacancies.SingleOrDefaultAsync(f => f.RowId == int.Parse(vacancy_id))).Updater;
            var user = await _userManager.FindByIdAsync(user_id.ToString());
            if (!string.IsNullOrEmpty(user.ClientID))
                await this.Clients.Client(user.ClientID)?.SendAsync("Alert", message);
        }

        public async Task<string> UserName()
        {
            var user = await _userManager.GetUserAsync(Context.User);
            return user.Name??user.UserName;

        }

        static readonly Guid BOT = Guid.Parse(Properties.Resources.BOT);


        public async Task SendEx(string message)
        {
            var user = await _userManager.GetUserAsync(Context.User);

            //            await this.Clients.All.SendAsync("Receive", user.Name ?? user.UserName, message);
            _dbContext.Messages.Add(new CMessage() 
            {
                Text = message
                , SendFromId = user.Id
                , SendToId = Guid.Parse("729EE9F0-8B56-4785-C8F7-08D8EEC6A678")
            });

            await _dbContext.SaveChangesAsync();

            await Clients.Caller.SendAsync("ReceiveEx", new ChatMessage
            {
                who = Sender.Me
                ,
                user = user.Name ?? user.UserName
                ,
                text = message
            });


        }

    }
}