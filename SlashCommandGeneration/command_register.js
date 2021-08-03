const DiscordJS = require("discord.js");
require("dotenv").config();

const client = new DiscordJS.Client();

const getApp = () => {
  const app = client.api.applications(client.user.id);
  return app.commands;
};

client.on("ready", async () => {
  console.log("Ready");

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
          type: 6, // 6 = user
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
          type: 6, // 6 = user
        },
      ],
    },
  });

  await getApp().post({
    data: {
      name: "timestats",
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
