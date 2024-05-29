//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System.Data.SqlClient;
//using System.Diagnostics.CodeAnalysis;

//namespace Edu_Block_dev.Logging
//{
//    public class DbLogger : ILogger
//    {
//        private readonly DbLoggerProvider _dbLoggerProvider;
//        public DbLogger([NotNull] DbLoggerProvider dbLoggerProvider)
//        {
//            _dbLoggerProvider = dbLoggerProvider;
//        }
//        public IDisposable BeginScope<TState>(TState state)
//        {
//            return null;
//        }
//        public bool IsEnabled(LogLevel logLevel)
//        {
//            return logLevel != LogLevel.None;
//        }
//        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
//        {
//            if (!IsEnabled(logLevel))
//            {
//                return;
//            }
//            var threadId = Thread.CurrentThread.ManagedThreadId;
//            using (var connection = new SqlConnection(_dbLoggerProvider.Options.ConnectionString))
//            {
//                connection.Open();
//                var values = new JObject();
//                if (_dbLoggerProvider?.Options?.LogFields?.Any() ?? false)
//                {
//                    foreach (var logField in _dbLoggerProvider.Options.LogFields)
//                    {
//                        switch (logField)
//                        {
//                            case "LogLevel":
//                                if (!string.IsNullOrWhiteSpace(logLevel.ToString()))
//                                {
//                                    values["LogLevel"] = logLevel.ToString();
//                                }
//                                break;
//                            case "ThreadId":
//                                values["ThreadId"] = threadId;
//                                break;
//                            case "EventId":
//                                values["EventId"] = eventId.Id;
//                                break;
//                            case "EventName":
//                                if (!string.IsNullOrWhiteSpace(eventId.Name))
//                                {
//                                    values["EventName"] = eventId.Name;
//                                }
//                                break;
//                            case "Message":
//                                {
//                                    values["Message"] = formatter(state, exception);
//                                }
//                                break;
//                            case "ExceptionMessage":
//                                if (exception != null &&
//                                    !string.IsNullOrWhiteSpace(exception.Message))
//                                {
//                                    values["ExceptionMessage"] = exception?.Message;
//                                }
//                                break;
//                            case "ExceptionStackTrace":
//                                if (exception != null
//                                    && !string.IsNullOrWhiteSpace(exception.StackTrace))
//                                {
//                                    values["ExceptionStackTrace"] = exception?.StackTrace;
//                                }
//                                break;
//                            case "ExceptionSource":
//                                if (exception != null
//                                    && !string.IsNullOrWhiteSpace(exception.Source))
//                                {
//                                    values["ExceptionSource"] = exception?.Source;
//                                }
//                                break;
//                        }
//                    }
//                }
//                string test = JsonConvert.SerializeObject(values, new JsonSerializerSettings
//                {
//                    NullValueHandling = NullValueHandling.Ignore,
//                    DefaultValueHandling = DefaultValueHandling.Ignore,
//                    Formatting = Formatting.None
//                }).ToString();
//                using (var command = new SqlCommand())
//                {
//                    try
//                    {
//                        command.Connection = connection;
//                        command.CommandType = System.Data.CommandType.Text;
//                        command.CommandText = string.Format("INSERT INTO {0} ([Id], [Values], [Created]) " +
//                            "VALUES (@Id, @Values, @Created)",
//                            _dbLoggerProvider.Options.LogTable);

//                        command.Parameters.Add(new SqlParameter("@Id",
//                            Guid.NewGuid()));

//                        command.Parameters.Add(new SqlParameter("@Values",
//                            test));

//                        command.Parameters.Add(new SqlParameter("@Created", DateTimeOffset.Now.ToString()));

//                        command.ExecuteNonQuery();

//                    }
//                    catch (Exception ex)
//                    {
//                    }
//                }
//                connection.Close();
//            }
//        }
//    }
//}

