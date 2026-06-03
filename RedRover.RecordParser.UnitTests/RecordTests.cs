namespace RedRover.RecordParser.UnitTests
{
    public class RecordTests
    {
        public static readonly IEnumerable<object[]> SuccessTestCases =
        [
            // Basic example
            [
                "(id, name, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId)",
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string> { "c1", "c2", "c3" }),
                    "externalId")
            ],

            // No custom fields
            [
                "(id, name, email, type(type_id, type_name, customFields()), externalId)",
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string>()),
                    "externalId")
            ],

            // Single custom field
            [
                "(id, name, email, type(type_id, type_name, customFields(c1)), externalId)",
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string> { "c1" }),
                    "externalId")
            ],

            // No spaces after commas
            [
                "(id,name,email,type(type_id,type_name,customFields(c1,c2,c3)),externalId)",
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string> { "c1", "c2", "c3" }),
                    "externalId")
            ],

            // Extra spaces
            [
                "  ( id , name , email , type ( type_id , type_name ,  customFields( c1, c2 , c3 )  ),  externalId  )",
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string> { "c1", "c2", "c3" }),
                    "externalId")
            ],

            // Blank data fields
            [
                "(,,,type(,,customFields(,,)),)",
                new Record(
                    "",
                    "",
                    "",
                    new RecordType("", "", new List<string> { "", "", "" }),
                    "")
            ],

            // Whitespace data fields
            [
                "(  ,   ,   , type( ,   ,customFields(  ,   ,   )), )",
                new Record(
                    "",
                    "",
                    "",
                    new RecordType("", "", new List<string> { "", "", "" }),
                    "")
            ],

            // "Real" data
            [
                "(12345, John Doe, john.doe@gmail.com, type(12, Teacher, customFields(Bachelor's degree, 5 years experience, Math expertise)), abcde)",
                new Record(
                    "12345",
                    "John Doe",
                    "john.doe@gmail.com",
                    new RecordType("12", "Teacher", new List<string> { "Bachelor's degree", "5 years experience", "Math expertise" }),
                    "abcde")
            ],

            // Escaped segments
            [
                "(\"i,d(\" , \"(n,a,m,e) \" , \"ema(,)il\", type(\"type)(,_id\", \"type_name(), \" , customFields(\"c,)1 \", \"c)(,2\", \"(((c,3)))\" )), \"e,x,t,ernal()()Id\")",
                new Record(
                    "i,d(",
                    "(n,a,m,e)",
                    "ema(,)il",
                    new RecordType("type)(,_id", "type_name(),", new List<string> { "c,)1", "c)(,2", "(((c,3)))" }),
                    "e,x,t,ernal()()Id")
            ],

            [
                "(12345, \"John Doe, Jr.\", john.doe@gmail.com, type(12, Teacher, customFields(Bachelor's degree, 5 years experience, \"Math expertise, Spanish proficiency (studied 8 years)\")), \"\"\"abcde\"\"\")",
                new Record(
                    "12345",
                    "John Doe, Jr.",
                    "john.doe@gmail.com",
                    new RecordType("12", "Teacher", new List<string> { "Bachelor's degree", "5 years experience", "Math expertise, Spanish proficiency (studied 8 years)" }),
                    "\"abcde\"")
            ]
        ];

        [Theory]
        [MemberData(nameof(SuccessTestCases))]
        public void Record_Parse_Success(string inputData, Record expected)
        {
            // Act
            var actual = Record.Parse(inputData);

            // Assert
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Equal(expected.Type.Id, actual.Type.Id);
            Assert.Equal(expected.Type.Name, actual.Type.Name);
            Assert.Equal(expected.Type.CustomFields, actual.Type.CustomFields);
            Assert.Equal(expected.ExternalId, actual.ExternalId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("()")]
        [InlineData("(")]
        [InlineData(")")]

        // Missing name
        [InlineData("(id, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId)")]

        // Missing type object
        [InlineData("(id, name, email, externalId)")]

        // Missing type.customFields object
        [InlineData("(id, email, type(type_id, type_name), externalId)")]

        // Unexpected fields after type.customFields
        [InlineData("(id, email, type(type_id, type_name, customFields(c1, c2, c3), unexpected_field), externalId)")]

        // Unexpected fields after externalId
        [InlineData("(id, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId, unexpected_field)")]

        // Missing parentheses
        [InlineData("id, name, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId)")]
        [InlineData("(id, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId")]
        [InlineData("(id, email, type type_id, type_name, customFields(c1, c2, c3)), externalId)")]
        [InlineData("(id, email, type(type_id, type_name, customFields(c1, c2, c3), externalId)")]

        // Data cut short
        [InlineData("(id, email, type")]
        [InlineData("(id, email, type(")]
        [InlineData("(id, email, type(type_id, type_nam")]
        [InlineData("(id, name, email, type(type_id, type_name, customFields")]
        [InlineData("(id, name, email, type(type_id, type_name, customFields(")]
        [InlineData("(id, name, email, type(type_id, type_name, customFields(c1, c2")]
        [InlineData("(id, name, email, type(type_id, type_name, customFields(c1, c2,")]

        // Invalid escape segments
        [InlineData("(id, \"na\"me, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId)")]
        [InlineData("(id, \"na\"  me, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId)")]
        [InlineData("(id, name, email, type(type_id, type_name, customFields(c1, c2, \"c3\"")]
        [InlineData("(id, name, email, type(type_id, type_name, customFields(c1, c2, \"c\"3)), externalId)")]
        [InlineData("(id, na,me, email, type(type_id, type_name, customFields(c1, c2, c3)), externalId)")]
        [InlineData("(id, na,me, email, type(type_id, type_name, customFields(c1, c2, (c3))), externalId)")]
        public void Record_Parse_Failure(string inputData)
        {
            // Act & Assert
            Assert.Throws<RecordParserException>(() => Record.Parse(inputData));
        }

        public static readonly IEnumerable<object[]> ToStringTestCases =
        [
            // Basic example
            [
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string> { "c1", "c2", "c3" }),
                    "externalId"),
                $"- id{Environment.NewLine}- name{Environment.NewLine}- email{Environment.NewLine}- type{Environment.NewLine}  - type_id{Environment.NewLine}  - type_name{Environment.NewLine}  - customFields{Environment.NewLine}    - c1{Environment.NewLine}    - c2{Environment.NewLine}    - c3{Environment.NewLine}- externalId{Environment.NewLine}"
            ],

            // No custom fields
            [
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string>()),
                    "externalId"),
                $"- id{Environment.NewLine}- name{Environment.NewLine}- email{Environment.NewLine}- type{Environment.NewLine}  - type_id{Environment.NewLine}  - type_name{Environment.NewLine}- externalId{Environment.NewLine}"
            ],

            // "Real" data
            [
                new Record(
                    "12345",
                    "John Doe",
                    "john.doe@gmail.com",
                    new RecordType("12", "Teacher", new List<string> { "\"Bachelor's degree\"", "5 years experience", "Math expertise" }),
                    "abcde"),
                $"- 12345{Environment.NewLine}- John Doe{Environment.NewLine}- john.doe@gmail.com{Environment.NewLine}- type{Environment.NewLine}  - 12{Environment.NewLine}  - Teacher{Environment.NewLine}  - customFields{Environment.NewLine}    - \"Bachelor's degree\"{Environment.NewLine}    - 5 years experience{Environment.NewLine}    - Math expertise{Environment.NewLine}- abcde{Environment.NewLine}"
            ]
        ];

        [Theory]
        [MemberData(nameof(ToStringTestCases))]
        public void Record_ToString(Record record, string expected)
        {
            // Act
            string actual = record.ToString();

            // Assert
            Assert.Equal(expected, actual);
        }

        public static readonly IEnumerable<object[]> ToAlternateOrderStringTestCases =
        [
            // Basic example
            [
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string> { "c1", "c2", "c3" }),
                    "externalId"),
                $"- email{Environment.NewLine}- externalId{Environment.NewLine}- id{Environment.NewLine}- name{Environment.NewLine}- type{Environment.NewLine}  - customFields{Environment.NewLine}    - c1{Environment.NewLine}    - c2{Environment.NewLine}    - c3{Environment.NewLine}  - type_id{Environment.NewLine}  - type_name{Environment.NewLine}"
            ],

            // No custom fields
            [
                new Record(
                    "id",
                    "name",
                    "email",
                    new RecordType("type_id", "type_name", new List<string>()),
                    "externalId"),
                $"- email{Environment.NewLine}- externalId{Environment.NewLine}- id{Environment.NewLine}- name{Environment.NewLine}- type{Environment.NewLine}  - type_id{Environment.NewLine}  - type_name{Environment.NewLine}"
            ],

            // "Real" data
            [
                new Record(
                    "12345",
                    "John Doe",
                    "john.doe@gmail.com",
                    new RecordType("12", "Teacher", new List<string> { "\"Bachelor's degree\"", "5 years experience", "Math expertise" }),
                    "abcde"),
                $"- john.doe@gmail.com{Environment.NewLine}- abcde{Environment.NewLine}- 12345{Environment.NewLine}- John Doe{Environment.NewLine}- type{Environment.NewLine}  - customFields{Environment.NewLine}    - \"Bachelor's degree\"{Environment.NewLine}    - 5 years experience{Environment.NewLine}    - Math expertise{Environment.NewLine}  - 12{Environment.NewLine}  - Teacher{Environment.NewLine}"
            ]
        ];

        [Theory]
        [MemberData(nameof(ToAlternateOrderStringTestCases))]
        public void Record_ToAlternateOrderString(Record record, string expected)
        {
            // Act
            string actual = record.ToAlternateOrderString();

            // Assert
            Assert.Equal(expected, actual);
        }
    }
}
