using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;

namespace GM.Plugins.AmountCheckerPlugin
{
    public class ValidateAmountPlugin : PluginBoilerplate
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
            var requestedAmount = target.GetAttributeValue<Money>("gm_requestedamount");
            var fundingOpportunity = target.GetAttributeValue<EntityReference>("gm_fundingopportunity");

            if (requestedAmount is null || fundingOpportunity is null)
            {
                return;
            }

            var fundingOpportunityRecord = service.Retrieve(
                "gm_fundingopportunity",
                fundingOpportunity.Id,
                new ColumnSet("gm_maximumaward")
                );

            var maximumAward = fundingOpportunityRecord.GetAttributeValue<Money>("gm_maximumaward");
            if (maximumAward is null) return;

            if (requestedAmount.Value > maximumAward.Value)
            {
                throw new InvalidPluginExecutionException(
                    "The requested amount cannot exceed the maximum award for the selected funding opportunity.");
            }
        }
    }
}
