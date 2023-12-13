using EBBuildClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EinsteinModels
{
    public static class brains_users_relationship
    {
    
        public static SchemaRelationshipDef GetRelationshipDefinition()
        {
           
            List<string> _childFilters_01 = default(List<string>);
            List<string> _childFilterFunctions_01 = default(List<string>);

            List<string> _childFilters_02 = default(List<string>);
            List<string> _childFilterFunctions_02 = default(List<string>);

            SchemaRelationshipDef _schemaDefinition = new SchemaRelationshipDef()
            {
                RelationshipName = "brain_users-relationship",
                Relationships = new List<SchemaRelationshipDetails>()
                {
                    {
                         new SchemaRelationshipDetails()
                         {
                                TargetSchemaName = typeof(user_identity).AssemblyQualifiedName.Substring(0,typeof(user_identity).AssemblyQualifiedName.IndexOf(",")),
                                TargetSchemaType = typeof(user_identity),
                                Relationship = new SchemaRelationship()
                                {
                                        SourceSchemaKey = "FKuser_id",
                                        TargetSchemaKey = "user_id",
                                        Filters= _childFilters_01,
                                        FilterFunctions = _childFilterFunctions_01
                                },
                                ChildRelationship = null

                         }
                    }
                    ,
                    {
                          new SchemaRelationshipDetails()
                          {
                                TargetSchemaName = typeof(brains).AssemblyQualifiedName.Substring(0,typeof(brains).AssemblyQualifiedName.IndexOf(",")),
                                TargetSchemaType = typeof(brains),
                                Relationship = new SchemaRelationship()
                                {
                                        SourceSchemaKey = "FKbrain_id",
                                        TargetSchemaKey = "brain_id",
                                        Filters= _childFilters_02,
                                        FilterFunctions = _childFilterFunctions_02
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
