using EBBuildClient.Core;
using EinsteinModels;
using ServiceStack;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using ConfigurationRegistry;
using Newtonsoft.Json;

namespace Einstein.Provider
{
    public class EinsteinProvider : IEinsteinProvider
    {
        const Int32 TTL_Days = 90;
        const Int32 MAX_TRAN_REQ = 25;
        const Int32 MAX_RECORDS_RETURNED = 10;
        private readonly ConfigurationRegistry.IStartup _context;

        private IConfiguration _configuation { get; set; }
        private readonly EBBuildApiFactory _ebBuildDBApiServiceFactory;
        private readonly string _ebbuildDBApiBaseUri;
        private readonly string _ebbuildDBApiToken;
        private readonly string _ebbuildDBApiRoles;
        private readonly string _ebbuildDBUserEmail;
        private readonly string _ebbuildDBTenantId;
        private readonly string _ebbuildDBLedgerPreface;
        private readonly string _ebbuildDBLedgerEncryption;
        private readonly string _ebbuildDBLedgerName;
        private readonly bool _ebbuildDBUseWebSockets;
        private readonly Int32 _ebbuildDBTimeOut;
        private readonly Int32 _maxConnections;
        private readonly Int32 _maxRecordsReturned;
        private readonly Int32 _asyncScale;
        private readonly string _hostRegistration;



        public EinsteinProvider(List<string> Roles = null)
        {

            _context = new ConfigurationRegistry.StartupContext();
            _configuation = ConfigurationRegistry.StartupContext.Configuration;


            _ebbuildDBApiBaseUri = StartupContext.Configuration?.GetValue<string>("EbbuildApiBaseUri");
            _ebbuildDBApiToken = StartupContext.Configuration?.GetValue<string>("EbbuildApiToken");
            _ebbuildDBApiRoles = (Roles == null || Roles?.Count == 0 ? StartupContext.Configuration?.GetValue<string>("EbbuildApiRoles") : String.Join(", ", Roles?.ToArray()));
            _ebbuildDBUserEmail = StartupContext.Configuration?.GetValue<string>("EbbuildUserEmail");
            _ebbuildDBTenantId = StartupContext.Configuration?.GetValue<string>("EbbuildTenantId");
            _ebbuildDBLedgerEncryption = StartupContext.Configuration?.GetValue<string>("EbbuildLedgerEncryption");
            _ebbuildDBLedgerPreface = StartupContext.Configuration?.GetValue<string>("EbbuildLedgerPreface");
            _ebbuildDBLedgerName = string.Format("{0}", StartupContext.Configuration?.GetValue<string>("EbbuildLedgerName"));
            _ebbuildDBUseWebSockets = StartupContext.Configuration.GetValue<bool>("EbbuildUseWebSockets");
            _ebbuildDBTimeOut = StartupContext.Configuration.GetValue<Int32>("EbbuildTimeOut");
            _maxConnections = StartupContext.Configuration.GetValue<Int32>("EbbuildMaxConncetions");
            _maxRecordsReturned = StartupContext.Configuration.GetValue<Int32>("EbbuildMaxRecordsReturned");
            _hostRegistration = StartupContext.Configuration.GetValue<string>("EbbuildRegistrationHost");
            _asyncScale = StartupContext.Configuration.GetValue<Int32>("EbbuildAsyncScale");



            _ebBuildDBApiServiceFactory = new EBBuildApiFactory(_maxConnections, _configuation, _ebbuildDBApiToken, _ebbuildDBApiRoles, _ebbuildDBUserEmail, _ebbuildDBTenantId, _ebbuildDBLedgerPreface, _ebbuildDBLedgerName, _ebbuildDBApiBaseUri, _maxRecordsReturned, _ebbuildDBUseWebSockets, asyncScale: _asyncScale);


            InitializeAsync();

        }


        public void InitializeAsync()
        {
            /*/
             * Create a client from the factory.
            /*/
            IEBBuildAPIService _client = _ebBuildDBApiServiceFactory.GetApiClient();


            /*/
             * Check if the user credentials exist.
            /*/
            AuthStatus authMessage = _client.IsCredentialsValid().Result;

            if (authMessage != AuthStatus.Success && authMessage != AuthStatus.CredentialsNotFound)
            {
                if (authMessage == AuthStatus.TokenInvalid)
                {
                    Debug.WriteLine(string.Format("Your API Token is invalid!"));
                    throw new Exception(string.Format("Your API Token is invalid!"));
                }
                else
                {
                    if (authMessage == AuthStatus.TokenInvalidForEnvironment)
                    {
                        Debug.WriteLine(string.Format("Your API Token is invalid for this cluster environment!"));
                        throw new Exception(string.Format("Your API Token is invalid for this cluster environment!"));

                    }
                    else
                    {
                        if (authMessage == AuthStatus.Failed)
                        {
                            Debug.WriteLine(string.Format("Authentication failed!  See Administrator immediately."));
                            throw new Exception(string.Format("Authentication failed!  See Administrator immediately."));
                        }
                    }
                }
            }


            /*/
             * Check if the desired table ledger exists.
            /*/
            var _einsteinLedger = _client.GetAllLedgersAsync(_client.GetLedgerName()).Result;

            /*/
             * Check to see if the table ledger exists in the list of available table ledgers.
            /*/
            if (_einsteinLedger.Item2 == null || _einsteinLedger.Item2?.ToList().Count == 0)
            {
                /*/
                 * Create the table ledger if it does not exist.
                /*/
                (string errorMessage, CreateBlockchainLedgerResponseDto ledgerResult) =  _client.CreateLedgerAsync(_client.GetLedgerName()).Result;


                /*/
                 * If we are unable to create the table ledger on the fly then raise an exception.
                /*/
                if (string.IsNullOrEmpty(errorMessage) == false)
                {
                    throw new Exception(errorMessage);

                }

            }
        }

        public List<LedgerListResponseDTO> GetLedgerStats()
        {
            IEBBuildAPIService _client = _ebBuildDBApiServiceFactory.GetApiClient();


            var _einsteinLedger = _client.GetAllLedgersAsync(_client.GetLedgerName()).Result;

          

            return _einsteinLedger.Item2.ToList();
        }


        #region Api Keys Functions

        public async Task<bool> CreateApiKey(Guid key_id, Guid user_id, string api_key, DateTime creation_time, DateTime deleted_time, bool is_active = true)
        {
            bool retVal = true;

            try
            {
                api_keys apiKeyRec = await FindApiKey(api_key);

                if (apiKeyRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    api_keys rec = new api_keys()
                    {
                        api_key = api_key,
                        key_id = key_id,
                        is_active = is_active,
                        creation_time = creation_time,
                        deleted_time = deleted_time,
                        BlockName = blockName
                    };


                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "api_key", FilterOperation.EQ, api_key);
                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<api_keys>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;
            }
            catch (Exception ex)
            {
                retVal = false;
            }

            return retVal;
        }

        public async Task<api_keys> FindApiKey(string api_key, bool includeRelationship = false)
        {

            List<api_keys> resultList = default(List<api_keys>);

            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);



                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "api_key", FilterOperation.EQ, api_key, BooleanOperation.AND);

                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<api_keys>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: (includeRelationship == false ? null : api_keys_relationship.GetRelationshipDefinition()),
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);

            }
            catch (Exception ex)
            {

            }

            return resultList.FirstOrDefault<api_keys>();
        }

        public async Task<api_keys> FindApiKey(Guid key_id)
        {
            List<api_keys> resultList = default(List<api_keys>);

            if (key_id.ToString().IsNullOrEmpty() == true)
                return null;


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);



                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "key_id", FilterOperation.EQ, key_id.ToString(), BooleanOperation.AND);

                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<api_keys>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: null, // api_keys_relationship.GetRelationshipDefinition(),
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }

            return resultList.FirstOrDefault<api_keys>();
        }

        #endregion


        #region User Identity Functions
        public async Task<bool> CreateUserIdentity(Guid user_id, string openai_api_key) 
        {
            bool retVal = true;

                     
            try
            { 
            user_identity userIDRec = await FindUserIdentity(user_id, openai_api_key);

            if (userIDRec == null)
            {
                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                string blockName = String.Format(Guid.NewGuid().ToString());
                List<string> _filters = new List<string>();
                List<List<string>> _queryPatterns = new List<List<string>>();
                List<string> _queryPatternFunctions = default(List<string>);



                user_identity rec = new user_identity()
                {
                    user_id = user_id,
                    openai_api_key = openai_api_key,
                    BlockName = blockName
                };



                _filters = new List<string>();
                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "user_id", FilterOperation.EQ, user_id.ToString());
                _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                /*/
                 * Save parent.
                /*/
                EBBuildAPIService.SaveDataToLedgerWithNoResponse<user_identity>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


            }
            else
                retVal = false;

            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        public async Task<user_identity> FindUserIdentity(Guid User_id, string Openai_api_key)
        {
            List<user_identity> resultList = default(List<user_identity>);


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                if (string.IsNullOrEmpty(Openai_api_key) == false)
                {
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "openai_api_key", FilterOperation.EQ, Openai_api_key, BooleanOperation.AND);
                }
                else
                {
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "user_id", FilterOperation.EQ, User_id.ToString(), BooleanOperation.AND);
                }


                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<user_identity>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: null,
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);

            }
            catch (Exception ex)
            {

            }
            return resultList.FirstOrDefault<user_identity>();
        }


        #endregion


        #region Users Functions

        public async Task<bool> CreateUser(Guid user_id, string email, string date, Int32 requests_count)
        {
            bool retVal = true;


            try
            {
                users userIDRec = await FindUser(user_id);

                if (userIDRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    users rec = new users()
                    {
                        user_id = user_id,
                        email = email,
                        date = DateTime.Parse(date),
                        requests_count = requests_count,
                        BlockName = blockName
                    };



                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "user_id", FilterOperation.EQ, user_id.ToString());
                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<users>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;

            }
            catch (Exception ex)
            {
                retVal = false;
            }

            return retVal;
        }

        public async Task<users> FindUser(Guid user_id, bool includeRelationship = false)
        {
            List<users> resultList = default(List<users>);

            try
            {

                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();

                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "user_id", FilterOperation.EQ, user_id.ToString(), BooleanOperation.AND);



                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<users>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: (includeRelationship == false ? null : users_relationship.GetRelationshipDefinition()),
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);

            }
            catch (Exception ex)
            {

            }

            return resultList.FirstOrDefault<users>();
        }

        #endregion


        #region Brain Vectors Functions

        public async Task<bool> CreateBrainVector(Guid brain_id, Guid vector_id, string file_sha1)
        {
            bool retVal = true;

            try
            {
                brains_vectors brainVectorRec = await FindBrainVector(brain_id, vector_id);

                if (brainVectorRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    brains_vectors rec = new brains_vectors()
                    {                       
                        file_sha1 = file_sha1,
                        FKbrain_id = brain_id,
                        FKvector_id = vector_id,
                        BlockName = blockName
                    };



                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "FKbrain_id", FilterOperation.EQ, brain_id.ToString());
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "FKvector_id", FilterOperation.EQ, vector_id.ToString());
                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<brains_vectors>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;

            }
            catch (Exception ex)
            {
                retVal = false;
            }

            return retVal;
        }


        public async Task<brains_vectors> FindBrainVector(Guid brain_id, Guid vector_id, bool includeRelationship = false)
        {

            List<brains_vectors> resultList = default(List<brains_vectors>);

            if (brain_id.ToString().IsNullOrEmpty() == true && vector_id.ToString().IsNullOrEmpty() == true)
                return null;

            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();

                if (brain_id.ToString().IsNullOrEmpty() == false)
                {
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "FKbrain_id", FilterOperation.EQ, brain_id.ToString(), BooleanOperation.AND);
                }
                else
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "FKvector_id", FilterOperation.EQ, vector_id.ToString(), BooleanOperation.AND);



                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsWithChildrenAsync<brains_vectors>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: (includeRelationship == false ? null : brain_vectors_relationship.GetRelationshipDefinition()),
                servicContext: dBContext,
                refreshCacheResults: true).ConfigureAwait(false);

            }
            catch (Exception ex)
            {

            }

            return resultList.FirstOrDefault<brains_vectors>();
        }


        #endregion


        #region Brain Users Functions


        public async Task<bool> CreateBrainUser(Guid brain_id, Guid user_id, string rights, Guid FKuser_id, Guid FKbrain_id, bool default_brain = false)
        {
            bool retVal = true;

            try
            {
                brains_users brainUserRec = await FindBrainUser(brain_id, user_id);

                if (brainUserRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    brains_users rec = new brains_users()
                    {
                        brain_id = brain_id,
                        user_id = user_id,
                        rights = rights,
                        FKbrain_id = FKbrain_id,
                        FKuser_id = FKuser_id,
                        default_brain = default_brain,
                        BlockName = blockName
                    };



                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "brain_id", FilterOperation.EQ, brain_id.ToString());
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "user_id", FilterOperation.EQ, user_id.ToString());
                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<brains_users>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;


            }
            catch (Exception ex)
            {
                retVal= false;
            }

            return retVal;
        }

        public async Task<brains_users> FindBrainUser(Guid brain_id, Guid user_id, bool includeRelationship = false)
        {
            List<brains_users> resultList = default(List<brains_users>);

            if (brain_id.ToString().IsNullOrEmpty() == true && user_id.ToString().IsNullOrEmpty() == true)
                return null;


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();

                if (brain_id.ToString().IsNullOrEmpty() == false)
                {
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "brain_id", FilterOperation.EQ, brain_id.ToString(), BooleanOperation.AND);
                }
                else
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "user_id", FilterOperation.EQ, user_id.ToString(), BooleanOperation.AND);



                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<brains_users>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: (includeRelationship == false ? null : brains_users_relationship.GetRelationshipDefinition()),
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }

            return resultList.FirstOrDefault<brains_users>();
        }


        #endregion


        #region Brains Functions

        public async Task<bool> CreateBrain(Guid brain_id, string name, string status, string description, string model, Int32 max_tokens, float temperature, string openai_api_key, Guid FKprompt_id)
        {
            bool retVal = true;

            try
            {
                brains brainRec = await FindBrain(brain_id);

                if (brainRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    brains rec = new brains()
                    {
                        brain_id = brain_id,
                        name = name,
                        status = status,
                        description = description,
                        model = model,
                        max_tokens = max_tokens,
                        temperature = temperature,
                        openai_api_key = openai_api_key,
                        FKprompt_id = FKprompt_id,
                        BlockName = blockName
                    };



                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "brain_id", FilterOperation.EQ, brain_id.ToString());
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "name", FilterOperation.EQ, name);
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "openai_api_key", FilterOperation.EQ, openai_api_key);
                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<brains>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;

            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        public async Task<brains> FindBrain(Guid brain_id)
        {
            List<brains> resultList = default(List<brains>);


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "brain_id", FilterOperation.EQ, brain_id.ToString(), BooleanOperation.AND);



                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<brains>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: null,
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);

            }
            catch (Exception ex)
            {

            }
            return resultList.FirstOrDefault<brains>();
        }

        #endregion


        #region Chat History Functions

        public async Task<bool> CreateChatHistory(Guid message_id, Guid chat_id, string user_message, string assistant, DateTime message_time, Guid FKprompt_id, Guid FKbrain_id)
        {
            bool retVal = true;


            try
            {
                chat_history chatHistoryRec = await FindChatHistory(message_id, chat_id);

                if (chatHistoryRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    chat_history rec = new chat_history()
                    {
                        message_id = message_id,
                        FKchat_id = chat_id,
                        user_message = user_message,
                        assistant = assistant,
                        message_time = message_time,
                        FKprompt_id = FKprompt_id,
                        FKbrain_id = FKbrain_id,
                        BlockName = blockName
                    };



                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "message_id", FilterOperation.EQ, message_id.ToString());
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "chat_id", FilterOperation.EQ, chat_id.ToString());

                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<chat_history>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;

            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        public async Task<chat_history> FindChatHistory(Guid message_id, Guid chat_id, bool includeRelationship = false)
        {
            List<chat_history> resultList = default(List<chat_history>);

            if (message_id.ToString().IsNullOrEmpty() == true && chat_id.ToString().IsNullOrEmpty() == false)
                return null;


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();

                if (message_id.ToString().IsNullOrEmpty() == false)
                {
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "message_id", FilterOperation.EQ, message_id.ToString(), BooleanOperation.AND);
                }
                else
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "chat_id", FilterOperation.EQ, chat_id.ToString(), BooleanOperation.AND);



                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<chat_history>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: (includeRelationship == false ? null : chat_history_relationship.GetRelationshipDefinition()),
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return resultList.FirstOrDefault<chat_history>();
        }

        #endregion


        #region Prompts Functions

        public async Task<bool> CreatePrompt(Guid id, string title, string content, string status = "private")
        {
            bool retVal = true;

            try
            {
                prompts promptRec = await FindPrompt(id);

                if (promptRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    prompts rec = new prompts()
                    {
                        id = id,
                        title = title,
                        content = content,
                        status = status,
                        BlockName = blockName
                    };



                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "id", FilterOperation.EQ, id.ToString());
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "title", FilterOperation.EQ, title.ToString());

                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<prompts>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;

            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        public async Task<prompts> FindPrompt(Guid id)
        {
            List<prompts> resultList = default(List<prompts>);


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "id", FilterOperation.EQ, id.ToString(), BooleanOperation.AND);



                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<prompts>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: null,
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);

            }
            catch (Exception ex)
            {

            }
            return resultList.FirstOrDefault<prompts>();
        }

        #endregion


        #region Summaries Functions

        public async Task<bool> CreateSummary(long id, Guid document_id, string content, string metadata, Embeddings<EBBuildClient.Core.Vector<Int32>, Int32> embedding)
        {
            bool retVal = true;

            try
            {
                summaries summariesRec = await FindSummary(document_id);

                if (summariesRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = Guid.NewGuid().ToString();
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    summaries rec = new summaries()
                    {
                        id = id,
                        FKdocument_id = document_id,
                        content = content,
                        metadata = metadata,
                        embedding = embedding.GetEmbeddingString(),
                        BlockName = blockName
                    };


                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "FKdocument_id", FilterOperation.EQ, document_id.ToString());

                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<summaries>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;

            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        public async Task<summaries> FindSummary(Guid document_id, bool includeRelationship = false)
        {
            List<summaries> resultList = default(List<summaries>);


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "FKdocument_id", FilterOperation.EQ, document_id.ToString(), BooleanOperation.AND);



                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<summaries>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: (includeRelationship == false ? null : summaries_relationship.GetRelationshipDefinition()),
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return resultList.FirstOrDefault<summaries>();
        }

        #endregion


        #region Stats Functions

        public async Task<bool> CreateStat(long id, DateTime time, bool chat, bool embedding, string details, string metadata)
        {
            bool retVal = true;

            try
            {
                stats statsRec = await FindStat(id);

                if (statsRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    stats rec = new stats()
                    {
                        id = id,
                        time = time,
                        chat = chat,
                        embedding = embedding,
                        details = details,
                        metadata = metadata,
                        BlockName = blockName
                    };


                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "id", FilterOperation.EQ, id.ToString());

                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<stats>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;

            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        public async Task<stats> FindStat(long id)
        {
            List<stats> resultList = default(List<stats>);


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "id", FilterOperation.EQ, id.ToString(), BooleanOperation.AND);



                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<stats>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: null,
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return resultList.FirstOrDefault<stats>();
        }

        #endregion


        #region Vectors Functions

        public async Task<bool> CreateVector(Guid id, string content, string metadata, Embeddings<EBBuildClient.Core.Vector<Int32>, Int32> embedding)
        {
            bool retVal = true;


            try
            {
                vectors vectorRec = await FindVector(id);

                if (vectorRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    vectors rec = new vectors()
                    {
                        id = id,
                        content = content,
                        metadata = metadata,
                        embedding = embedding.GetEmbeddingString(),
                        BlockName = blockName
                    };


                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "id", FilterOperation.EQ, id.ToString());

                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<vectors>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                   retVal = false;

            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        public async Task<vectors> FindVector(Guid id)
        {
            List<vectors> resultList = default(List<vectors>);


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "id", FilterOperation.EQ, id.ToString(), BooleanOperation.AND);



                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<vectors>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: null,
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);

            }
            catch (Exception ex)
            {
         
            }
            return resultList.FirstOrDefault<vectors>();
        }

        #endregion


        #region Vectors Functions

        public async Task<bool> CreateChat(Guid chat_id, Guid user_id, DateTime creation_time, string history, string chat_name)
        {
            bool retVal = true;


            try
            {
                chats chatRec = await FindChat(chat_id);

                if (chatRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    chats rec = new chats()
                    {
                        chat_id = chat_id,
                        FKuser_id = user_id,
                        creation_time = creation_time,
                        history = history,
                        chat_name = chat_name,
                        BlockName = blockName
                    };


                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "chat_id", FilterOperation.EQ, chat_id.ToString());

                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<chats>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;

            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        public async Task<chats> FindChat(Guid chat_id, bool includeRelationship = false)
        {
            List<chats> resultList = default(List<chats>);


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "chat_id", FilterOperation.EQ, chat_id.ToString(), BooleanOperation.AND);



                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<chats>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: (includeRelationship == false ? null : chats_relationship.GetRelationshipDefinition()),
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return resultList.FirstOrDefault<chats>();
        }

        #endregion


        #region Migrations Functions

        public async Task<bool> CreateMigration(string name, DateTime executed_at)
        {
            bool retVal = true;

            try
            {
                migrations migrationRec = await FindMigration(name);

                if (migrationRec == null)
                {
                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();



                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    migrations rec = new migrations()
                    {
                        name = name,
                        executed_at = executed_at,
                        BlockName = blockName
                    };


                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "name", FilterOperation.EQ, name);

                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<migrations>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;

            }
            catch (Exception ex)
            {
                retVal = false;
            }
            return retVal;
        }

        public async Task<migrations> FindMigration(string name)
        {
            List<migrations> resultList = default(List<migrations>);


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "name", FilterOperation.EQ, name, BooleanOperation.AND);



                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<migrations>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: null,
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return resultList.FirstOrDefault<migrations>();
        }

        #endregion




        #region Subscription Invitation
        public async Task<bool> CreateSubscriptionInvitations(Guid brain_id, string email, string rights, Guid FKbrain_id)
        {
            bool retVal = true;

            try
            {
                brain_subscription_invitations brainSubscriptionInvitationRec = await FindSubscriptionInvitations(brain_id);

                if (brainSubscriptionInvitationRec == null)
                {

                    IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                    string blockName = String.Format(Guid.NewGuid().ToString());
                    List<string> _filters = new List<string>();
                    List<List<string>> _queryPatterns = new List<List<string>>();
                    List<string> _queryPatternFunctions = default(List<string>);



                    brain_subscription_invitations rec = new brain_subscription_invitations()
                    {
                        brain_id = brain_id,
                        FKbrain_id = FKbrain_id,
                        email = email,
                        rights = rights,
                        BlockName = blockName
                    };



                    _filters = new List<string>();
                    _filters = EBIBuildAPIHelper.BuildFilter(_filters, "brain_id", FilterOperation.EQ, brain_id.ToString());
                    _queryPatterns = EBIBuildAPIHelper.BuildQueryPattern(_queryPatterns, _filters);


                    /*/
                     * Save parent.
                    /*/
                    EBBuildAPIService.SaveDataToLedgerWithNoResponse<brain_subscription_invitations>(rec, _ebBuildDBApiServiceFactory.GetAsyncWrapper(), _queryPatterns, _queryPatternFunctions, dBContext, rec.BlockName, String.Empty);


                }
                else
                    retVal = false;
            }
            catch (Exception ex)
            {
                retVal = false;
            }

            return retVal;
        }

        public async Task<brain_subscription_invitations> FindSubscriptionInvitations(Guid brain_id, bool includeRelationship = false)
        {

            List<brain_subscription_invitations> resultList = default(List<brain_subscription_invitations>);

            if (brain_id.ToString().IsNullOrEmpty() == true)
                return null;


            try
            {
                List<string> _filters = new List<string>();
                AuthStatus authStatus = AuthStatus.Success;

                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "brain_id", FilterOperation.EQ, brain_id.ToString(), BooleanOperation.AND);

                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<brain_subscription_invitations>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: (includeRelationship == false ? null : brain_subscription_invitations_relationship.GetRelationshipDefinition()),
                servicContext: dBContext,
                refreshCacheResults: false).ConfigureAwait(false);

            }
            catch (Exception ex)
            {

            }

            return resultList.FirstOrDefault<brain_subscription_invitations>();
        }


        #endregion



        #region Misc Filter Search

        public async Task<(AuthStatus, List<summaries>)> FindSummaryByEmbedding(Embeddings<EBBuildClient.Core.Vector<Int32>, Int32> ebmedding, string metaDataString, bool includeRelationship = false)
        {

            List<summaries> resultList = default(List<summaries>);
            AuthStatus authStatus = AuthStatus.Success;


            try
            {
                List<string> _filters = new List<string>();
                
                /*/
                 * Use the filterFunctions to set groupBy and/or sorting of data
                /*/
                List<string> _filterFunctions = new List<string>();


                PaginationDTO paginationDataList = default(PaginationDTO);


                IEBBuildAPIService dBContext = _ebBuildDBApiServiceFactory?.GetApiClient();


                _filters = EBIBuildAPIHelper.BuildFilter(_filters, "embedding", FilterOperation.VECTORMATCH, ebmedding.GetEmbeddingString(), BooleanOperation.AND);
                

                 
                (authStatus, resultList, paginationDataList) = await EBBuildAPIService.GetLedgerRecordsAsync<summaries>(
                asyncWrapper: _ebBuildDBApiServiceFactory.GetAsyncWrapper(),
                parentToLazyLoadChildren: null,
                filterConditions: _filters,
                filterFunctions: null,
                relationship: (includeRelationship == false ? null : summaries_relationship.GetRelationshipDefinition()),
                servicContext: dBContext,
                refreshCacheResults: true).ConfigureAwait(false);
            }
            catch (Exception ex)
            {

            }
            return (authStatus , resultList);
        }


        #endregion
    }
}