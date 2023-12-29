using Microsoft.Extensions.Options;
using TFA.Twitter;
using TFA.Twitter.Config;
using TFA.Utils;

namespace TFA.Presentation.Common.Services;

public class TwitterService(IOptions<TwitterOptions> options) : AbstractTwitterService(options, Env.IsDevelopment());
