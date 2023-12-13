using EBBuildClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EinsteinModels
{
    public static class summaries_relationship
    {        
        public static SchemaRelationshipDef GetRelationshipDefinition()
        {           
            List<string> _childFilters = default(List<string>);
            List<string> _childFilterFunctions = default(List<string>);

            SchemaRelationshipDef _schemaDefinition = new SchemaRelationshipDef()
            {
                RelationshipName = "summaries-relationship",
                Relationships = new List<SchemaRelationshipDetails>()
                {
                    {
                          new SchemaRelationshipDetails()
                          {
                                TargetSchemaName = typeof(vectors).AssemblyQualifiedName.Substring(0,typeof(vectors).AssemblyQualifiedName.IndexOf(",")),
                                TargetSchemaType = typeof(vectors),
                                Relationship = new SchemaRelationship()
                                {
                                        SourceSchemaKey = "FKdocument_id",
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
