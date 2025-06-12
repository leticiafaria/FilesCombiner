using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using Serilog.Events;
using Serilog.Formatting;

namespace FilesCombiner.Configuration
{
    public class CustomLogFormatter : ITextFormatter
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions;

        public CustomLogFormatter()
        {
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null) throw new ArgumentNullException(nameof(logEvent));
            if (output == null) throw new ArgumentNullException(nameof(output));

            // Mapeia as propriedades para um dicionário de string e object  
            var properties = new Dictionary<string, object>();

            foreach (var property in logEvent.Properties)
            {
                properties[property.Key] = CoerceProperty(property.Value);
            }

            var logEntry = new LogEntry
            {
                Timestamp = logEvent.Timestamp,
                Level = logEvent.Level.ToString(),
                Message = logEvent.RenderMessage().Replace("\"", ""),
                Properties = properties,
                Exception = logEvent.Exception != null ? JsonSerializer.Serialize(logEvent.Exception, _jsonSerializerOptions) : null
            };

            string json = JsonSerializer.Serialize(logEntry, _jsonSerializerOptions);
            output.WriteLine(json);
        }

        // Método auxiliar para converter o LogEventPropertyValue para um tipo serializável  
        private object CoerceProperty(LogEventPropertyValue propertyValue)
        {
            if (propertyValue is ScalarValue scalarValue)
            {
                return scalarValue.Value!;
            }
            else if (propertyValue is DictionaryValue dictionaryValue)
            {
                return dictionaryValue.Elements.ToDictionary(
                    kvp => kvp.Key.Value!.ToString(),
                    kvp => CoerceProperty(kvp.Value)
                );
            }
            else if (propertyValue is SequenceValue sequenceValue)
            {
                return sequenceValue.Elements.Select(CoerceProperty).ToList();
            }
            else if (propertyValue is StructureValue structureValue)
            {
                var structureProperties = new Dictionary<string, object>();
                foreach (var prop in structureValue.Properties)
                {
                    structureProperties[prop.Name] = CoerceProperty(prop.Value);
                }
                return structureProperties;
            }

            return propertyValue.ToString();
        }
    }
}
