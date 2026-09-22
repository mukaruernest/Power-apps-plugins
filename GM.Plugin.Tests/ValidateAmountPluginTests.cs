using GM.Plugins.AmountCheckerPlugin;
using System.Security.Policy;
using System.Security.Principal;
using FakeXrmEasy;
using Microsoft.Xrm.Sdk;

namespace GM.Plugin.Tests
{
    [TestClass]
    public class ValidateAmountPluginTests
    {
        [TestMethod]
        public void ValidateAmount_Should_NotThrow_When_RequestedAmount_IsWithinMaximum()
        {
            // Arrange
            var test = CreateTestObjects(5000, 10000);

            // Act
            test.Plugin.Action(
                test.PluginContext,
                test.Service,
                test.TracingService);

            // No Assert required.
        }

        [TestMethod]
        public void ValidateAmount_Should_ThrowException_When_RequestedAmount_ExceedsMaximum()
        {
            // Arrange
            var test = CreateTestObjects(15000, 10000);

            // Act & Assert
            try
            {
                test.Plugin.Action(
                    test.PluginContext,
                    test.Service,
                    test.TracingService);

                Assert.Fail("Expected InvalidPluginExecutionException was not thrown.");
            }
            catch (InvalidPluginExecutionException ex)
            {
                Assert.AreEqual(
                    "The requested amount cannot exceed the maximum award for the selected funding opportunity.",
                    ex.Message);
            }
        }

        private (
            ValidateAmountPlugin Plugin,
            XrmFakedPluginExecutionContext PluginContext,
            IOrganizationService Service,
            ITracingService TracingService)
        CreateTestObjects(decimal requestedAmount, decimal maximumAward)
        {
            var context = new XrmFakedContext();

            var fundingOpportunityId = Guid.NewGuid();

            var fundingOpportunity = new Entity("gm_fundingopportunity")
            {
                Id = fundingOpportunityId
            };

            fundingOpportunity["gm_maximumaward"] = new Money(maximumAward);

            context.Initialize(new List<Entity>
            {
                fundingOpportunity
            });

            var target = new Entity("gm_grantapplication");

            target["gm_requestedamount"] = new Money(requestedAmount);

            target["gm_fundingopportunity"] =
                new EntityReference("gm_fundingopportunity", fundingOpportunityId);

            var pluginContext = context.GetDefaultPluginContext();
            pluginContext.InputParameters["Target"] = target;

            var service = context.GetOrganizationService();

            var tracingService = new XrmFakedTracingService();

            var plugin = new ValidateAmountPlugin();

            return (
                plugin,
                pluginContext,
                service,
                tracingService);
        }
    }
}
