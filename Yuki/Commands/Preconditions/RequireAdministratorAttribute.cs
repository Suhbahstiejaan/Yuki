﻿using Discord;
using Qmmands;
using System;
using System.Linq;
using System.Threading.Tasks;
using Yuki.Services.Database;

namespace Yuki.Commands.Preconditions
{
    public class RequireAdministratorAttribute : CheckBaseAttribute
    {
        public override Task<CheckResult> CheckAsync(ICommandContext c, IServiceProvider provider)
        {
            YukiCommandContext context = c as YukiCommandContext;

            CheckResult result = CheckResult.Unsuccessful("You must be an administrator to execute this command.");

            foreach (ulong role in GuildSettings.GetGuild(context.Guild.Id).AdministratorRoles)
            {
                if ((context.User as IGuildUser).RoleIds.Contains(role))
                {
                    result = CheckResult.Successful;
                    break;
                }
            }

            if(!result.IsSuccessful)
            {
                if(context.Guild.OwnerId == context.User.Id)
                {
                    result = CheckResult.Successful;
                }
            }

            return Task.FromResult(result);
        }
    }
}