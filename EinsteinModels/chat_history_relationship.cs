﻿using EBBuildClient.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EinsteinModels
{
    public static class chat_history_relationship
    {    
        public static SchemaRelationshipDef GetRelationshipDefinition()
        {
            
            List<string> _childFilters_01 = default(List<string>);
            List<string> _childFilterFunctions_01 = default(List<string>);

            List<string> _childFilters_02 = default(List<string>);
            List<string> _childFilterFunctions_02 = default(List<string>);

            List<string> _childFilters_03 = default(List<string>);
            List<string> _childFilterFunctions_03 = default(List<string>);

            SchemaRelationshipDef _schemaDefinition = new SchemaRelationshipDef()
            {
                RelationshipName = "chat_history-relationship",
                Relationships = new List<SchemaRelationshipDetails>()
                {
                    {
                          new SchemaRelationshipDetails()
                          {
                                TargetSchemaName = typeof(chats).AssemblyQualifiedName.Substring(0,typeof(chats).AssemblyQualifiedName.IndexOf(",")),
                                TargetSchemaType = typeof(chats),
                                Relationship = new SchemaRelationship()
                                {
                                        SourceSchemaKey = "FKchat_id",
                                        TargetSchemaKey = "chat_id",
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
                                TargetSchemaName = typeof(prompts).AssemblyQualifiedName.Substring(0,typeof(prompts).AssemblyQualifiedName.IndexOf(",")),
                                TargetSchemaType = typeof(prompts),
                                Relationship = new SchemaRelationship()
                                {
                                        SourceSchemaKey = "FKprompt_id",
                                        TargetSchemaKey = "id",
                                        Filters= _childFilters_02,
                                        FilterFunctions = _childFilterFunctions_02
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
                                        Filters= _childFilters_03,
                                        FilterFunctions = _childFilterFunctions_03
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
