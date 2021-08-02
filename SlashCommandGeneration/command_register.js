const DiscordJS = require("discord.js");
require("dotenv").config();

//const guildId = "735263201612005472";
const client = new DiscordJS.Client();

const getApp = () => {
  const app = client.api.applications(client.user.id);
  // if (guildId) {
  //   app.guilds(guildId);
  // }
  return app.commands;
};

client.on("ready", async () => {
  console.log("Ready");

  // const commands = await getApp(guildId).commands.get();

  // /time
  await getApp().post({
    data: {
      name: "time",
      description: "View the time for a user or yourself",
      options: [
        {
          name: "user",
          description: "The user you want to see the time for",
          required: false,
          type: 6, // 3 = string
        },
      ],
    },
  });

  await getApp().post({
    data: {
      name: "country",
      description: "View the country for a user or yourself",
      options: [
        {
          name: "user",
          description: "The user you want to see the time for",
          required: false,
          type: 6, // 3 = string
        },
      ],
    },
  });

  await getApp().post({
    data: {
      name: "timstats",
      description: "View the stats for the bot",
    },
  });

  await getApp().post({
    data: {
      name: "timeall",
      description: "View the time for everyone",
    },
  });

  await getApp().post({
    data: {
      name: "countryall",
      description: "View the stats for everyone",
    },
  });

  await getApp().post({
    data: {
      name: "timehelp",
      description: "Get help for setting up your time",
    },
  });

  await getApp().post({
    data: {
      name: "timeset",
      description: "Set your time",
      options: [
        {
          name: "time_difference",
          description: "The difference in time (do /timehelp for help)",
          required: true,
          type: 10, // 10 = double
        },
      ],
    },
  });

  await getApp().post({
    data: {
      name: "countryset",
      description: "Set the country for yourself",
      options: [
        {
          name: "country",
          description: "The country you're in",
          required: true,
          type: 3, // 3 = string
        },
      ],
    },
  });
});

client.login(process.env.TOKEN);
