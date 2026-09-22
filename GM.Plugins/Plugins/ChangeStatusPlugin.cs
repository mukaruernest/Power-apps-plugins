using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GM.Plugins.AmountCheckerPlugin
{
    public class ChangeStatusPlugin : PluginBoilerplate
    {
        public override void Action(IPluginExecutionContext context, IOrganizationService service, ITracingService trace)
        {
            if (!context.InputParameters.Contains("Target")
                || context.InputParameters["Target"] == null
                || !(context.InputParameters["Target"] is Entity))
            {
                return;
            }
            var target = context.InputParameters["Target"] as Entity;
            var applicationToUpdate = new Entity(target.LogicalName);
            applicationToUpdate.Id = target.Id;

            applicationToUpdate["gm_applicationstatus"] = new OptionSetValue(122780000);
            applicationToUpdate["gm_reviewstatus"] = new OptionSetValue(122780001);
            service.Update(applicationToUpdate);
        }
    }
}
