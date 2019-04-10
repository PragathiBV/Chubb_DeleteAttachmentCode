using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Workflow;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeleteQuoteAttachment
{
    public class DeleteAttachment : CodeActivity
    {
        [Input("Quote ID")]

        [ReferenceTarget("quote")]

        public InArgument<EntityReference> QuoteID { get; set; }
        protected override void Execute(CodeActivityContext executionContext)
        {

            //Create the context

            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();

            IOrganizationServiceFactory serviceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();

            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);

            EntityReference qoute = QuoteID.Get<EntityReference>(executionContext);

            QueryExpression _noteattachmentQuery = new QueryExpression
            {
                EntityName = "annotation",

                ColumnSet = new ColumnSet("subject", "filename", "notetext", "documentbody"),

                Criteria = new FilterExpression
                {
                    Conditions =
                    {
                        new ConditionExpression
                        {
                            AttributeName = "objectid",
                            Operator = ConditionOperator.Equal,
                            Values = { qoute.Id }
                        },
                        //new ConditionExpression
                        //{
                        //    AttributeName = "isdocument",
                        //    Operator = ConditionOperator.Equal,
                        //    Values = {true}
                        //}
                    }
                }

            };
            var results = service.RetrieveMultiple(_noteattachmentQuery);

            if (results.Entities.Count > 0)
            {
                //we need to fetch Last attachment
                Entity NotesAttachment = results.Entities.Last();

                foreach (Entity act in results.Entities)
                {
                    if (NotesAttachment.Id == act.Id)
                        continue;
                    service.Delete(act.LogicalName, act.Id);

                }
            }
        }
    }
}
