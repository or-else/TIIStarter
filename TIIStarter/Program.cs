using EBBuildClient.Core;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using ConfigurationRegistry;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceStack;
using System;
using System.Data;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using static ServiceStack.LicenseUtils;
using System.Runtime.CompilerServices;
using Einstein.Provider;
using System.Xml.Linq;
using EinsteinModels;
using System.Timers;
using System.Collections.Generic;
using System.Collections;

public class Program
{


    /*/
     * Prepared by : EverythingBlockchain, Inc  (C) All Rights Reserved 2023
     * Prepared for: Technology Innovation Institute.
     * License     : General Public License (GNU )
     * 
     * Purpose:     This test application is created to demonstrate the BuildDB vector and document database as a replacement to Superbase database.
     *               
     * Scope:       This test application simulates core dataabse functionality.
     * Approach:    This test application uses a sequential approach to test interaction with each table with parent child relationships.
    /*/

    static string apiKey = "dec-1234567";
    static string openai_api_key = "openApiKey-1234567";

    static string _key_id = "2aae892a-e706-457f-b8aa-38a07dc6c338";
    static Guid key_id = Guid.Parse(_key_id);

    static string _user_id = "5ec99154-4841-4ef5-a6ed-d6bf40c28314";
    static Guid user_id = Guid.Parse(_user_id);
    static string email = "testuser@dcl.com";
    static Int32 user_request_count = 10;

    static string _id = "86a5c6db-9143-4c98-8501-330d4674d1db";
    static Guid id = Guid.Parse(_id);

    static string _brain_id = "a71a4a73-e931-4163-a74e-c5f73cb0abdd";
    static Guid brain_id = Guid.Parse(_brain_id);

    static string _vector_id = "59902a1b-a887-451b-a655-cab3cd5c2719";
    static Guid vector_id = Guid.Parse(_vector_id);

    static string _document_id = "ad587d47-9041-4522-abd3-07d88871b368";
    static Guid document_id = Guid.Parse(_document_id);

    static string _chat_id = "3cdab489-f062-4638-b3a7-839bd0b7431e";
    static Guid chat_id = Guid.Parse(_chat_id);

    static string _message_id = "414891bd-fb5b-444e-a3d5-275fe0431f45";
    static Guid message_id = Guid.Parse(_message_id);


    static string _brain_subscription_id = "544627af-dcc2-46ab-af39-3b0f060defa1";
    static Guid brain_subscription_id = Guid.Parse(_brain_subscription_id);






    static string file_sha1 = "SHA256";
    static string rights = "Admin";
    static bool default_brain = false;
    static string name = "TestBrain_100";
    static string status = "OK";
    static string description = "Teste Brain";
    static string model = "Nov-29-2023-1000";
    static Int32 max_tokens = 10;
    static float temperature = 76.09F;
    static string user_message = "This model is really accurate!";
    static string assistant = "x100";
    static long summaryId = 102;
    static string title = "Technology Innovation Institute Test Harness";
    static string content = "awnevdxfiyvgczmcmwnbgufsncsqktbpelbtlcvebrkvbiyjyvtrovzbbtphafiizwsvwnavereecvdmvhwsyzuclowkhhqxsytojpphezusgqmlopvkpmkmwrauadgg";
    static string metadata = "{\"Department\":\"Finance\",\"Code\":\"A209\"}";
    static bool chat = true;
    static bool isEmbedding = true;
    static string details = "NA";
    static string history = "11-29-2023";
    static string chat_name = "Model-101";



    public static void Main(string[] args)
    {

        Task task = Task.Run(async () =>
        {
            await Begin();
        });

        task.Wait();

    }
    public static async Task Begin()
    {

        bool _result = false;


        IEinsteinProvider provider = new EinsteinProvider();



        /*/
         * Get ledger statistics
        /*/
        List<LedgerListResponseDTO> _ledgerStats = provider.GetLedgerStats();



        /*/
         * Insert API Key         
        /*/
        _result = provider.CreateApiKey(key_id, user_id, apiKey, DateTime.Now, DateTime.Now, true).Result;



        /*/
         * Create User Identity
        /*/
        _result = provider.CreateUserIdentity(user_id, apiKey).Result;


        /*/
         * Create User 
        /*/
        _result = provider.CreateUser(user_id, email, DateTime.Now.ToString(), user_request_count).Result;


        /*/
         * Find User : By default all find operations do not return relationships.
         *             By setting the IncludeRelationship to "true" we are able to demonstrate the ability to define hierarchical relationships (on the fly) without indexes.
        /*/
        users _user = provider.FindUser(user_id, true).Result;


        /*/
         * Create Prompt
        /*/
        _result = provider.CreatePrompt(id, title, content, "private").Result;


        /*/
        * Find Prompt by id
       /*/
        prompts _prompt = provider.FindPrompt(id).Result;


        /*/
         * Create Vector
        /*/
        _result = provider.CreateVector(id, content, metadata, GetVectorEmbeddingData()).Result;

        /*/
         * Find vector by id
        /*/
        vectors _vector = provider.FindVector(id).Result;


        /*/
         * Create Brain
        /*/
        if (_prompt != null)
        {
            _result = provider.CreateBrain(brain_id, name, status, description, model, max_tokens, temperature, openai_api_key, _prompt.id).Result;
        }

        /*/
         * Find Brains by brains_id
        /*/
        brains _brains = provider.FindBrain(brain_id).Result;


        /*/
         * Create Brain Vector by brain_id
        /*/
        if (_vector != null && _brains != null)
        {
            _result = provider.CreateBrainVector(_brains.brain_id, _vector.id, file_sha1).Result;


            /*/
             * Find Brains Vector by brains_id and vector_id
            /*/
            brains_vectors _brainsVector = provider.FindBrainVector(_brains.brain_id, _vector.id, true).Result;

        }



        /*/
         * Create Brain User
        /*/
        if (_user != null && _brains != null)
        {
            _result = provider.CreateBrainUser(brain_id, user_id, rights, _user.user_id, _brains.brain_id, default_brain).Result;

            /*/
             * Find Brains Users by brains_id and user_id
            /*/
            brains_users _brainsUsers = provider.FindBrainUser(brain_id, user_id).Result;
        }



        /*/
         * Create Chat 
        /*/
        _result = provider.CreateChat(chat_id, user_id, DateTime.Now, history, chat_name).Result;

        /*/
         * Find Chat
        /*/
        chats _chats = provider.FindChat(chat_id).Result;


        /*/
         * Create Chat History
        /*/
        if (_prompt != null && _brains != null)
        {
            _result = provider.CreateChatHistory(message_id, chat_id, user_message, assistant, DateTime.Now, _prompt.id, _brains.brain_id).Result;

            /*/
             * Find Chat History
            /*/
            chat_history _chatHistory = provider.FindChatHistory(message_id, chat_id).Result;
        }


        /*/
         * Create Summary
        /*/
        if (_vector != null)
        {
            _result = provider.CreateSummary(summaryId, _vector.id, content, metadata, GetSummaryEmbeddingData()).Result;

            /*/
             * Find Summaries
            /*/
            summaries _summaries = provider.FindSummary(_vector.id).Result;
        }

        /*/
         * Create Stat History
        /*/
        _result = provider.CreateStat(summaryId, DateTime.Now, chat, isEmbedding, details, metadata).Result;

        /*/
         * Find Stats
        /*/
        stats _stats = provider.FindStat(summaryId).Result;



        /*/
         * Create Migration
        /*/
        _result = provider.CreateMigration(name, DateTime.Now).Result;

        /*/
         * Find Migration
        /*/
        migrations _migration = provider.FindMigration(name).Result;



        /*/
         * Create Brain Subscription Invitation
        /*/

        if (_brains != null)
        {
            _result = provider.CreateSubscriptionInvitations(brain_subscription_id, email, rights, _brains.brain_id).Result;

            /*/
             * Find Brain subscription
            /*/
            brain_subscription_invitations _brainSubscription = provider.FindSubscriptionInvitations(_brains.brain_id).Result;


        }


        /*/
         * Misc search by embedding and metadata
        /*/
        (AuthStatus authStatus, List<summaries> summaryDataWithChildren) = provider.FindSummaryByEmbedding(GetVectorEmbeddingData(), "A209", false).Result;



        /*/
         * Demonstrate RBAC where same search is being conducted with different role.
        /*/
        IEinsteinProvider providerRBAC = new EinsteinProvider(new List<string>() { "Guest" });

        (authStatus, summaryDataWithChildren) = provider.FindSummaryByEmbedding(GetVectorEmbeddingData(), "A209", false).Result;



    }



    public static Embeddings<EBBuildClient.Core.Vector<Int32>, Int32> GetVectorEmbeddingData()
    {
        Embeddings<EBBuildClient.Core.Vector<Int32>, Int32> embedding = new Embeddings<Vector<int>, int>();
        embedding.AddEmbedding(new[,]
        {
                { new Vector<Int32>(new Int32[] { 1, 2, 32, 4, 5 }) },
                { new Vector<Int32>(new Int32[] { 5, 6, 3, 8, 7 }) },
                { new Vector<Int32>(new Int32[] { 3, 7, 3, 9, 1 }) }
            }
        );

        return embedding;
    }



    public static Embeddings<EBBuildClient.Core.Vector<Int32>, Int32> GetSummaryEmbeddingData()
    {
        Embeddings<EBBuildClient.Core.Vector<Int32>, Int32> embedding = new Embeddings<Vector<int>, int>();
        embedding.AddEmbedding(new[,]
        {
                { new Vector<Int32>(new Int32[] { 1, 2, 32, 4, 5 }) },
                { new Vector<Int32>(new Int32[] { 5, 6, 3, 8, 7 }) },
                { new Vector<Int32>(new Int32[] { 3, 7, 3, 9, 1 }) }
            }
        );

        return embedding;
    }

}
