using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ProActiveBot.Bot.Bots;
using ProActiveBot.Bot.Dialogs;
using System.Collections.Concurrent;

namespace ProActiveBot.Bot
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson();
            services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            // Create a global hashset for our ConversationReferences
            services.AddSingleton<ConcurrentDictionary<string, ConversationReference>>();
            // Create the storage we'll be using for User and Conversation state, as well as Single Sign On.
            // (Memory is great for testing purposes.)
            services.AddSingleton<IStorage, MemoryStorage>();

            // For SSO, use CosmosDbPartitionedStorage

            /* COSMOSDB STORAGE - Uncomment the code in this section to use CosmosDB storage */

            // var cosmosDbStorageOptions = new CosmosDbPartitionedStorageOptions()
            // {
            //     CosmosDbEndpoint = "<endpoint-for-your-cosmosdb-instance>",
            //     AuthKey = "<your-cosmosdb-auth-key>",
            //     DatabaseId = "<your-database-id>",
            //     ContainerId = "<cosmosdb-container-id>"
            // };
            // var storage = new CosmosDbPartitionedStorage(cosmosDbStorageOptions);

            /* END COSMOSDB STORAGE */

            services.AddSingleton<UserState>();
            services.AddSingleton<ConversationState>();
            services.AddSingleton<CheckInDialog>();
            services.AddTransient<IBot, TeamsBot<CheckInDialog>>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                });

        }
    }
}
