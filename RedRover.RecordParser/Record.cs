using System.Text;

namespace RedRover.RecordParser
{
    public class Record
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public RecordType Type { get; set; } = null!;
        public string ExternalId { get; set; } = null!;

        private Record() { }

        public Record(
            string id,
            string name,
            string email,
            RecordType type,
            string externalId)
        {
            Id = id;
            Name = name;
            Email = email;
            Type = type;
            ExternalId = externalId;
        }

        /// <summary>
        /// Parses raw string data into a <see cref="Record"/>.
        /// </summary>
        /// <remarks>
        /// Assumptions:
        /// - <paramref name="data"/> is in the following format:
        /// (id, name, email, type(id, name, customFields(c1, c2, c3)), externalId)
        /// - Field values do not contain commas or parentheses.
        /// </remarks>
        public static Record Parse(string data)
        {
            var parser = new RecordParser(data, startIndex: 1); // Start at index 1 to skip leading '('
            var record = new Record();

            static void ThrowIf(bool condition, string message)
            {
                if (condition)
                {
                    throw new RecordParserException(message);
                }
            }

            record.Id = parser.ReadValue();
            record.Name = parser.ReadValue();
            record.Email = parser.ReadValue();

            parser.ReadObjectMarker("type");
            string typeId = parser.ReadValue();
            string typeName = parser.ReadValue();

            parser.ReadObjectMarker("customFields");

            var typeCustomFields = new List<string>();
            foreach (string customField in parser.ReadListValues())
            {
                typeCustomFields.Add(customField);
            }

            var recordType = new RecordType(typeId, typeName, typeCustomFields);

            ThrowIf(parser.ReadToObjectEnd() != string.Empty, "Expected end of type object");
            parser.SkipToNextValue();

            record.ExternalId = parser.ReadToObjectEnd();
            ThrowIf(!parser.EndOfData, "Expected to reach end of data");

            record.Type = recordType;
            return record;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"- {Id}");
            builder.AppendLine($"- {Name}");
            builder.AppendLine($"- {Email}");
            builder.AppendLine("- type");

            builder.AppendLine($"  - {Type.Id}");
            builder.AppendLine($"  - {Type.Name}");

            if (Type.CustomFields.Any())
            {
                builder.AppendLine("  - customFields");
                foreach (string customField in Type.CustomFields)
                {
                    builder.AppendLine($"    - {customField}");
                }
            }

            builder.AppendLine($"- {ExternalId}");

            return builder.ToString();
        }

        public string ToAlternateOrderString()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"- {Email}");
            builder.AppendLine($"- {ExternalId}");
            builder.AppendLine($"- {Id}");
            builder.AppendLine($"- {Name}");
            builder.AppendLine("- type");

            if (Type.CustomFields.Any())
            {
                builder.AppendLine("  - customFields");
                foreach (string customField in Type.CustomFields)
                {
                    builder.AppendLine($"    - {customField}");
                }
            }

            builder.AppendLine($"  - {Type.Id}");
            builder.AppendLine($"  - {Type.Name}");

            return builder.ToString();
        }
    }

    public class RecordType
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<string> CustomFields { get; set; } = null!;

        public RecordType(string id, string name,  List<string> customFields)
        {
            Id = id;
            Name = name;
            CustomFields = customFields;
        }
    }
}
