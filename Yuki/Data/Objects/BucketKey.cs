using Qmmands;
using System;
using Yuki.Commands;

namespace Yuki.Data.Objects
{
    public class BucketKey
    {
        public static object Generate(Command command, object bucketType, ICommandContext context, IServiceProvider provider)
        {
            if (!(context is YukiCommandContext commandContext))
            {
                throw new InvalidOperationException("Invalid command context");
            }

            string data = string.Empty;

            commandContext.Command = commandContext.Command ?? command;

            if (bucketType is CooldownBucketType bucket)
            {
                switch (bucket)
                {
                    case CooldownBucketType.Guild:
                        data += commandContext.Guild.Id;
                        break;
                    case CooldownBucketType.Channel:
                        data += commandContext.Channel.Id;
                        break;
                    case CooldownBucketType.User:
                        data += commandContext.User.Id;
                        break;
                    case CooldownBucketType.Global:
                        data += command;
                        break;
                    default:
                        throw new InvalidOperationException("Unknown bucket type!");
                }

                return data;
            }

            throw new InvalidOperationException("Unknown bucket type!");
        }
    }
}
