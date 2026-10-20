namespace AzertaNodes.Infrastructure.Nodes
{
    internal class NodeConstants
    {
        //General constants
        public static string ResponseMessageKey { get; } = "ResponseMessage";
        public static string MessageKey { get; } = "Message";

        //LLM node constants
        public static string ModelIdKey { get; } = "ModelId";
        public static string CredentialIdKey { get; } = "CredentialId";
        public static string SystemPromptKey { get; } = "SystemPrompt";
        public static string UserPromptKey { get; } = "UserPrompt";
        public static string LlmNodeResultKey { get; } = "LlmNodeResult";
        public static string EndpointKey { get; } = "Endpoint";
        public static string TemperatureKey { get; } = "Temperature";
        public static string MaxTokensKey { get; } = "MaxTokens";
        public static string TopPKey { get; } = "TopP";
        public static string PresencePenaltyKey { get; } = "PresencePenalty";
        public static string FrequencyPenaltyKey { get; } = "FrequencyPenalty";
        public static string SeedKey { get; } = "Seed";
        public static string ServiceIdKey { get; } = "ServiceId";
        public static string StopSequencesKey { get; } = "StopSequences";

        //HTTP request node constants
        public static string RequestMethodKey { get; } = "RequestMethod";
        public static string UriKey { get; } = "Uri";
        public static string HeadersKey { get; } = "Headers";
        public static string AuthorizationKey { get; } = "Authorization";
        public static string BodyKey { get; } = "Body";
        public static string IsSuccessStatusCodeKey { get; } = "IsSuccessStatusCode";
        public static string StatusCodeKey { get; } = "StatusCode";
        public static string EnablePollingKey { get; } = "EnablePolling";
        public static string MaxPollingAttemptsKey { get; } = "MaxPollingAttempts";
        public static string PollingIntervalMsKey { get; } = "PollingIntervalMs";
        public static string ContentTypeKey { get; } = "ContentType";
        public static string MaxJitterMillisecondsKey { get; } = "MaxJitterMilliseconds";
        public static string MaxDelaySecondsKey { get; } = "MaxDelaySeconds";

        //Logic node constants
        public static string BooleanKey { get; } = "Boolean";
        public static string ExpressionKey { get; } = "Expression";
        public static string TruePathKey { get; } = "TruePath";
        public static string FalsePathKey { get; } = "FalsePath";

        //Discord trigger node constants
        public static string AuthorKey { get; } = "Author";
        public static string ChannelKey { get; } = "Channel";

        //Discord send message node constants
        public static string TargetChannelKey { get; } = "TargetChannel";
    }
}
