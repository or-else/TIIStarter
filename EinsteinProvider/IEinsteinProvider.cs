using EinsteinModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EBBuildClient.Core;

namespace Einstein.Provider
{
    public interface IEinsteinProvider
    {
        public Task<bool> CreateApiKey(Guid key_id, Guid user_id, string api_key, DateTime creation_time, DateTime deleted_time, bool is_active = true);
        public Task<api_keys> FindApiKey(string api_key, bool includeRelationship = false);
        public List<LedgerListResponseDTO> GetLedgerStats();
        public Task<bool> CreateUserIdentity(Guid user_id, string openai_api_key);
        public Task<user_identity> FindUserIdentity(Guid User_id, string Openai_api_key);
        public Task<bool> CreateUser(Guid user_id, string email, string date, Int32 requests_count);
        public Task<users> FindUser(Guid user_id, bool includeRelationship = false);
        public Task<bool> CreateBrainVector(Guid brain_id, Guid vector_id, string file_sha1);
        public Task<brains_vectors> FindBrainVector(Guid brain_id, Guid vector_id, bool includeRelationship = false);
        public Task<bool> CreateBrainUser(Guid brain_id, Guid user_id_id, string rights, Guid FKuser_id, Guid FKbrain_id, bool default_brain = false);
        public Task<brains_users> FindBrainUser(Guid brain_id, Guid user_id_id, bool includeRelationship = false);
        public Task<bool> CreateBrain(Guid brain_id, string name, string status, string description, string model, Int32 max_tokens, float temperature, string openai_api_key, Guid FKprompt_id);
        public Task<brains> FindBrain(Guid brain_id);
        public Task<bool> CreateChatHistory(Guid message_id, Guid chat_id, string user_message, string assistant, DateTime message_time, Guid FKprompt_id, Guid FKbrain_id);
        public Task<chat_history> FindChatHistory(Guid message_id, Guid chat_id, bool includeRelationship = false);
        public Task<bool> CreatePrompt(Guid id, string title, string content, string status = "private");
        public Task<prompts> FindPrompt(Guid id);
        public Task<bool> CreateSummary(long id, Guid document_id, string content, string metadata, Embeddings<EBBuildClient.Core.Vector<Int32>, Int32> embedding);
        public Task<summaries> FindSummary(Guid document_id, bool includeRelationship = false);        
        public Task<bool> CreateStat(long id, DateTime time, bool chat, bool embedding, string details, string metadata);
        public Task<stats> FindStat(long id);
        public Task<bool> CreateVector(Guid id, string content, string metadata, Embeddings<EBBuildClient.Core.Vector<Int32>, Int32> embedding);
        public Task<vectors> FindVector(Guid id);
        public Task<bool> CreateChat(Guid chat_id, Guid user_id, DateTime creation_time, string history, string chat_name);
        public Task<chats> FindChat(Guid chat_id, bool includeRelationship = false);
        public Task<bool> CreateMigration(string name, DateTime executed_at);
        public Task<migrations> FindMigration(string name);
        public Task<bool> CreateSubscriptionInvitations(Guid brain_id, string email, string rights, Guid FKbrain_id);
        public Task<brain_subscription_invitations> FindSubscriptionInvitations(Guid brain_id, bool includeRelationship = false);
        public Task<(AuthStatus, List<summaries>)> FindSummaryByEmbedding(Embeddings<EBBuildClient.Core.Vector<Int32>, Int32> ebmedding, string metaDataString, bool includeRelationship = false);
    }
}
