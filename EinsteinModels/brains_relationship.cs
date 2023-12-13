using EBBuildClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EinsteinModels
{
    public static class brains_relationship
    {    
        public static SchemaRelationshipDef GetRelationshipDefinition()
        {            
            List<string> _childFilters = default(List<string>);
            List<string> _childFilterFunctions = default(List<string>);

            SchemaRelationshipDef _schemaDefinition = new SchemaRelationshipDef()
            {
                RelationshipName = "brains-relationship",
                Relationships = new List<SchemaRelationshipDetails>()
                {
                    {
                          new SchemaRelationshipDetails()
                          {
                                TargetSchemaName = typeof(prompts).AssemblyQualifiedName.Substring(0,typeof(prompts).AssemblyQualifiedName.IndexOf(",")),
                                TargetSchemaType = typeof(prompts),
                                Relationship = new SchemaRelationship()
                                {
                                        SourceSchemaKey = "FKprompt_id",
                                        TargetSchemaKey = "id",
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
