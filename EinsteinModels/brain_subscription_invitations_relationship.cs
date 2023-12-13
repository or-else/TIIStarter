using EBBuildClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EinsteinModels
{
    public static class brain_subscription_invitations_relationship
    {       
        public static SchemaRelationshipDef GetRelationshipDefinition()
        {            
            List<string> _childFilters = default(List<string>);
            List<string> _childFilterFunctions = default(List<string>);


            SchemaRelationshipDef _schemaDefinition = new SchemaRelationshipDef()
            {
                RelationshipName = "brain_subscription_invitation-relationship",
                Relationships = new List<SchemaRelationshipDetails>()
                {
                    {
                          new SchemaRelationshipDetails()
                          {
                                TargetSchemaName = typeof(brains).AssemblyQualifiedName.Substring(0,typeof(brains).AssemblyQualifiedName.IndexOf(",")),
                                TargetSchemaType = typeof(brains),
                                Relationship = new SchemaRelationship()
                                {
                                        SourceSchemaKey = "FKbrain_id",
                                        TargetSchemaKey = "brain_id",
                                        Filters= _childFilters,
                                        FilterFunctions = _childFilterFunctions
                                },
                                ChildRelationship = null

                          }
                     }
                 }
            };

            return _schemaDefinition;
        }

    }
}
