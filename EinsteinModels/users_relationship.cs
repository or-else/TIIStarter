using EBBuildClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EinsteinModels
{
    public static class users_relationship
    {
     
        public static SchemaRelationshipDef GetRelationshipDefinition()
        {            
            List<string> _childFilters = default(List<string>);
            List<string> _childFilterFunctions = default(List<string>);

            SchemaRelationshipDef _schemaDefinition = new SchemaRelationshipDef()
            {
                RelationshipName = "users-relationship",
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
