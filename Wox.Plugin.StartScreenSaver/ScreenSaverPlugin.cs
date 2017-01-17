using System;
using System.Collections.Generic;
using System.Linq;

namespace Wox.Plugin.StartScreenSaver
{
    public class ScreenSaverPlugin : IPlugin
    {
        private PluginInitContext _context;

        public void Init(PluginInitContext context)
        {
            _context = context;
        }

        public List<Result> Query(Query query)
        {
            string queryFilter = query.Search;
            IEnumerable<ScreenSaverInfo> screensavers = null;
            if (queryFilter != null && !String.IsNullOrEmpty(queryFilter))
            {
                screensavers = ScreenSaverManager.GetScreenSavers(queryFilter);
            }
            else
            {
                screensavers = ScreenSaverManager.GetScreenSavers("*");
            }

            List<Result> results = screensavers.Select(p => new Result()
            {
                Title = p.Name,
                SubTitle = p.FileName,
                IcoPath = "Images\\ico.png",  // TODO icone du screensaver
                Action = e =>
                {
                    System.Diagnostics.Process.Start(p.FileName);
                    return true;
                }
            }).ToList();

            return results;
        }
    }
}
